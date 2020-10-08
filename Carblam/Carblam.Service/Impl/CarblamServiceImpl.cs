using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Carblam.Service.Db;
using Carblam.Interaction;
using Carblam.Service.Util;
using System.Security.Policy;
using System.Data.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace Carblam.Service.Impl
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    [XmlSerializerFormat]
    class CarblamServiceImpl : ICarblamSvc
    {
        readonly ICarblamServiceContext _ctx;

        public CarblamServiceImpl(ICarblamServiceContext ctx)
        {
            _ctx = ctx;
        }

        #region profile management

        public void Register(RegisterSpecType registerSpec)
        {
            if (registerSpec.Login.IsEmpty())
                throw new ApplicationException("Login cannot be empty");

            if (registerSpec.Password.IsEmpty() || registerSpec.Password.Length < 10)
                throw new ApplicationException("Password should be of length >10 characters");

            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var loginKey = registerSpec.Login.ToLower();
                if (ctx.Db.Users.FindUserByLoginKey(loginKey) != null)
                {
                    throw new ApplicationException("User " + registerSpec.Login + " already exists");
                }
                else
                {
                    var salt = Convert.ToBase64String(_ctx.SecureRandom.GenerateRandomBytes(64));

                    var user = new DbUserInfo()
                    {
                        Activated = false,
                        HashSalt = salt,
                        Email = registerSpec.Email,
                        IsDeleted = false,
                        RegistrationStamp = DateTime.UtcNow,
                        Login = registerSpec.Login,
                        LoginKey = registerSpec.Login.ToLower(),
                        PasswordHash = registerSpec.Password.ComputeSha256Hash(salt),
                        LastLoginStamp = SqlDateTime.MinValue.Value,
                        LastTokenStamp = SqlDateTime.MinValue.Value
                    };
                    ctx.Db.Users.AddUser(user);
                    ctx.Db.SubmitChanges();

                    this.RequestActivationImpl(ctx, user, registerSpec.Email);
                    ctx.Db.SubmitChanges();
                }
            }
        }

        public void RequestActivation(RequestActivationSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized(false);
                this.RequestActivationImpl(ctx, ctx.Db.Users.GetUserById(ctx.Session.UserId), spec.Email);
                ctx.Db.SubmitChanges();
            }
        }

        private string MakeToken()
        {
            return new[] { "=", "/", "+" }.Aggregate(Convert.ToBase64String(_ctx.SecureRandom.GenerateRandomBytes(64)), (s, c) => s.Replace(c, string.Empty));
        }

        private void RequestActivationImpl(ICarblamRequestContext ctx, DbUserInfo user, string email)
        {
            if (user.Activated)
                throw new ApplicationException("Already activated");

            var activationToken = this.MakeToken();

            user.LastToken = activationToken;
            user.LastTokenStamp = DateTime.UtcNow;
            user.LastTokenKind = DbUserTokenKind.Activation;

            _ctx.SendMail(
                email, "Registration activation",
                $"To confirm your registration follow this link: " + this.MakeSeriveLink(ctx, "/profile?action=activate&key=" + activationToken)
            );
        }

        string MakeSeriveLink(ICarblamRequestContext ctx, string relLink)
        {
            // /profile?action=activate&key={key}

            var urlBuilder = _ctx.Configuration.GetServiceUrl();
            var pair = ctx.RequestHostName.Split(new[] { ':' }, 2);
            urlBuilder.Host = pair[0];
            if (pair.Length > 1 && ushort.TryParse(pair[1], out var port))
                urlBuilder.Port = port;

            var linkUri = new Uri(urlBuilder.Uri, relLink.TrimStart('/'));
            return @"<a href=""" + linkUri + @""">" + linkUri + "</a>";
        }

        public void RequestAccess(ResetPasswordSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var loginKey = spec.Login.ToLower();

                var user = ctx.Db.Users.FindUserByLoginKey(loginKey);
                if (user != null && user.Email == spec.Email && !user.IsDeleted)
                {
                    var accessRestoreToken = this.MakeToken();

                    user.LastToken = accessRestoreToken;
                    user.LastTokenStamp = DateTime.UtcNow;
                    user.LastTokenKind = DbUserTokenKind.AccessRestore;
                    ctx.Db.SubmitChanges();

                    _ctx.SendMail(
                        spec.Email, "Access restore",
                        $"To regain access to your profile follow this link: " + this.MakeSeriveLink(ctx, "/profile?action=restore&key=" + accessRestoreToken)
                    );
                }
                else
                {
                    throw new ApplicationException("User not found or incorrect email");
                }
            }
        }

        public OkType Activate(string key)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var user = ctx.Db.Users.FindUserByTokenKey(key);
                if (user != null && user.LastToken != null && user.LastTokenKind == DbUserTokenKind.Activation)
                {
                    if (user.Activated)
                        throw new ApplicationException("Already activated");

                    if (user.LastTokenStamp + _ctx.Configuration.TokenTimeout >= DateTime.UtcNow)
                    {
                        user.LastLoginStamp = DateTime.UtcNow;
                        user.LastToken = null;
                        user.Activated = true;
                        ctx.Db.SubmitChanges();
                        ctx.Session.SetUserContext(user);
                    }
                    else
                    {
                        throw new ApplicationException("Acivation token expired");
                    }
                }
                else
                {
                    throw new ApplicationException("Invalid activation token");
                }
            }

            return new OkType();
        }

        public OkType RestoreAccess(string key)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var user = ctx.Db.Users.FindUserByTokenKey(key);
                if (user != null && user.LastToken != null && user.LastTokenKind == DbUserTokenKind.AccessRestore)
                {
                    if (user.LastTokenStamp + _ctx.Configuration.TokenTimeout >= DateTime.UtcNow)
                    {
                        user.LastLoginStamp = DateTime.UtcNow;
                        user.LastToken = null;
                        ctx.Db.SubmitChanges();
                        ctx.Session.SetUserContext(user);
                    }
                    else
                    {
                        throw new ApplicationException("Acivation token expired");
                    }
                }
                else
                {
                    throw new ApplicationException("Invalid activation token");
                }
            }

            return new OkType();
        }

        public void SetEmail(ChangeEmailSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized(false);

                var user = ctx.Db.Users.GetUserById(ctx.Session.UserId);
                if (user.Email == spec.OldEmail &&
                    user.PasswordHash == spec.Password.ComputeSha256Hash(user.HashSalt))
                {
                    user.Email = spec.NewEmail;
                    ctx.Db.SubmitChanges();
                }
                else
                {
                    throw new ApplicationException("Invalid old email or password");
                }
            }
        }

        public void SetPassword(ChangePasswordSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized(false);

                var user = ctx.Db.Users.GetUserById(ctx.Session.UserId);
                if (user.Email == spec.Email)
                // user.PasswordHash == spec.OldPassword.ComputeSha256Hash(user.HashSalt))
                {
                    user.PasswordHash = spec.NewPassword.ComputeSha256Hash(user.HashSalt);
                    ctx.Db.SubmitChanges();

                    _ctx.SendMail(spec.Email, "Password was changed", "Dear " + user.Login + ", your password was changed!");
                }
                else
                {
                    throw new ApplicationException("Invalid old email");
                }
            }
        }

        public void Login(LoginSpecType loginSpec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var loginKey = loginSpec.Login;
                var user = ctx.Db.Users.FindUserByLoginKey(loginKey);
                if (user != null && user.PasswordHash == loginSpec.Password.ComputeSha256Hash(user.HashSalt) && !user.IsDeleted)
                {
                    user.LastLoginStamp = DateTime.UtcNow;
                    ctx.Db.SubmitChanges();

                    ctx.Session.SetUserContext(user);
                }
                else
                {
                    throw new ApplicationException("Invalid credentials");
                }
            }
        }

        public void Logout()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.Session.SetUserContext(null);
            }
        }

        public void DeleteProfile()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var user = ctx.Db.Users.GetUserById(ctx.Session.UserId);
                user.IsDeleted = true;
                user.LastToken = null;

                _ctx.SessionsManager.DropUserSessions(user.Id);
                ctx.Db.SubmitChanges();
            }
        }

        public ProfileFootprintInfoType GetProfileFootprint()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized(false);

                var user = ctx.Db.Users.GetUserById(ctx.Session.UserId);

                var parts = user.Email.Split('@');
                var leading = parts[0].Substring(0, Math.Min(2, parts[0].Length));
                var suffixDotPos = parts[1].LastIndexOf('.');
                var ending = suffixDotPos > 0 ? parts[1].Substring(suffixDotPos) : parts[1].Substring(parts[1].Length - Math.Min(2, parts[1].Length));
                var emailFootprint = leading + "***@***" + ending;

                return new ProfileFootprintInfoType()
                {
                    Login = user.Login,
                    EmailFootprint = emailFootprint,
                    IsActivated = user.Activated
                };
            }
        }

        #endregion

        #region orders

        public CarInfoType GetCarInfo()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var carInfo = ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == ctx.Session.UserId);
                if (carInfo == null)
                {
                    return null;
                }
                else
                {
                    return carInfo.Translate();
                }
            }
        }

        public CarInfoListType GetFreeCarsNear(string latStr, string lonStr)
        {
            var lat = double.Parse(latStr);
            var lon = double.Parse(lonStr);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var carInfo = ctx.Db.Raw.Cars.Where(c => c.Status == CarStatus.Free).Select(c => new
                {
                    c,
                    d = (lat - c.LocationLat) * (lat - c.LocationLat) + (lon - c.LocationLon) * (lon - c.LocationLon)
                }).OrderBy(c => c.d).Take(10).Select(c => c.c).ToArray();
                return carInfo.Translate();
            }
        }

        public void StartWork(StartWorkSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var carInfo = ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == ctx.Session.UserId);
                if (carInfo == null)
                {
                    carInfo = new DbCarInfo()
                    {
                        DriverUserId = ctx.Session.UserId
                    };
                    ctx.Db.Raw.Cars.InsertOnSubmit(carInfo);
                }

                if (carInfo.Status != CarStatus.Busy && carInfo.CurrOrderId == -1)
                {
                    carInfo.CurrOrderId = -1;
                    carInfo.Description = spec.Description;
                    carInfo.Designation = spec.Designation;
                    carInfo.Height = spec.Height;
                    carInfo.Length = spec.Length;
                    carInfo.Width = spec.Width;
                    carInfo.WeightLimit = spec.WeightLimit;
                    carInfo.Status = CarStatus.Free;
                }

                ctx.Db.SubmitChanges();
            }
        }

        public void UpdateLocation(LocationType spec)
        {
            var orderId = new Nullable<long>();
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var carInfo = ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == ctx.Session.UserId);

                if (carInfo != null)
                {
                    carInfo.LocationLat = spec.Latitude;
                    carInfo.LocationLon = spec.Longitude;
                    orderId = carInfo.CurrOrderId >= 0 ? carInfo.CurrOrderId : new Nullable<long>();
                }
                ctx.Db.SubmitChanges();
            }

            if (orderId.HasValue)
            {
                lock (_notifiersLock)
                {
                    if (this.TryGetNotificationEntry(orderId.Value, out var entry))
                        entry.Update(DateTime.UtcNow);
                }
            }
        }

        public void StopWork()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var carInfo = ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == ctx.Session.UserId);
                if (carInfo != null)
                {
                    carInfo.Status = CarStatus.NotWorking;
                }
                ctx.Db.SubmitChanges();
            }
        }

        public OrderInfoListType GetMyOutcomeOrders()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                return ctx.Db.Raw.Orders.Where(o => o.CustomerId == ctx.Session.UserId)
                          .Select(o => new
                          {
                              o,
                              d = ctx.Db.Raw.Users.FirstOrDefault(c => c.Id == o.DriverId),
                              r = ctx.Db.Raw.Users.FirstOrDefault(c => c.Id == o.RecieverId)
                          })
                          .Select(o => new OrderInfo(o.o, o.r.Login, o.d.Login, ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == o.d.Id)))
                          .Translate(ctx.Session.UserId);
            }
        }

        public OrderInfoListType GetMyIncomeOrders()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                return ctx.Db.Raw.Orders.Where(o => o.RecieverId == ctx.Session.UserId)
                          .Select(o => new
                          {
                              o,
                              d = ctx.Db.Raw.Users.FirstOrDefault(c => c.Id == o.DriverId),
                              r = ctx.Db.Raw.Users.FirstOrDefault(c => c.Id == o.RecieverId)
                          })
                          .Select(o => new OrderInfo(o.o, o.r.Login, o.d.Login, ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == o.d.Id)))
                          .Translate(ctx.Session.UserId);
            }
        }

        public OrderInfoListType GetUnacceptedOrdersNear(string latStr, string lonStr)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            var lat = double.Parse(latStr, System.Globalization.CultureInfo.InvariantCulture);
            var lon = double.Parse(lonStr, System.Globalization.CultureInfo.InvariantCulture);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var carInfo = ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == ctx.Session.UserId);
                if (carInfo != null && carInfo.Status == CarStatus.Busy)
                {
                    var orders = ctx.Db.Raw.Orders.Where(o => o.Id == carInfo.CurrOrderId)
                                    .Select(o => new OrderInfo(o, null, null, null))
                                    .ToArray();

                    return orders.Translate(ctx.Session.UserId);
                }
                else if (carInfo == null || carInfo.Status != CarStatus.Free)
                {
                    return new OrderInfoListType() { Items = new OrderInfoType[0] };
                }
                else
                {
                    var orders = ctx.Db.Raw.Orders.Where(o => o.Status == OrderStatus.Confirmed && carInfo.Length >= o.Length && carInfo.Width >= o.Width && carInfo.Height >= o.Height && carInfo.WeightLimit >= o.Weight)
                        .Select(ord => new
                        {
                            ord,
                            d = (lat - ord.SourceLocationLat) * (lat - ord.SourceLocationLat) + (lon - ord.SourceLocationLon) * (lon - ord.SourceLocationLon)
                        }).OrderBy(o => o.d).Take(10).Select(o => new OrderInfo(o.ord, null, null, null)).ToArray();
                    return orders.Translate(ctx.Session.UserId);
                }
            }
        }

        public OrderInfoType CreateOrder(OrderSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var receiverLoginKey = spec.ReceiverLogin.ToLower();
                var receiverUser = ctx.Db.Raw.Users.FirstOrDefault(u => u.LoginKey == receiverLoginKey);
                if (receiverUser == null)
                    throw new ApplicationException($"Recipient {spec.ReceiverLogin} not found");

                var ord = new DbOrderInfo()
                {
                    SourceAddress = spec.FromAddress,
                    SourceLocationLat = spec.FromLocation.Latitude,
                    SourceLocationLon = spec.FromLocation.Longitude,
                    DestinationAddress = spec.ToAddress,
                    DestinationLocationLat = spec.ToLocation.Latitude,
                    DestinationLocationLon = spec.ToLocation.Longitude,
                    CustomerId = ctx.Session.UserId,
                    RecieverId = receiverUser.Id,
                    DriverId = -1,
                    MessageForReciever = spec.ConfirmMessage,
                    Description = spec.Parcel.Description,
                    Height = spec.Parcel.Height,
                    Width = spec.Parcel.Width,
                    Length = spec.Parcel.Length,
                    Weight = spec.Parcel.Weight,
                    Status = OrderStatus.Created,
                    CreatedStamp = DateTime.UtcNow,
                    ConfirmedStamp = SqlDateTime.MinValue.Value,
                    AcceptedStamp = SqlDateTime.MinValue.Value,
                    LoadingStamp = SqlDateTime.MinValue.Value,
                    LoadedStamp = SqlDateTime.MinValue.Value,
                    DeliveredStamp = SqlDateTime.MinValue.Value,
                    FinishedStamp = SqlDateTime.MinValue.Value,
                    CancelledStamp = SqlDateTime.MinValue.Value,
                };

                ctx.Db.Raw.Orders.InsertOnSubmit(ord);
                ctx.Db.SubmitChanges();

                return new OrderInfo(ord, receiverUser.Login, null, null).Translate(ctx.Session.UserId);
            }
        }

        public OrderInfoType GetOrder(string ordIdStr)
        {
            var ordId = long.Parse(ordIdStr);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var userId = ctx.Session.UserId;
                var ord = ctx.Db.Raw.Orders.Select(o => new
                {
                    o,
                    d = ctx.Db.Raw.Users.FirstOrDefault(c => c.Id == o.DriverId),
                    r = ctx.Db.Raw.Users.FirstOrDefault(c => c.Id == o.RecieverId)
                })
                             .FirstOrDefault(o => o.o.Id == ordId && (o.o.RecieverId == userId || o.o.CustomerId == userId || o.o.DriverId == userId));

                if (ord == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);

                var ordInfo = new OrderInfo(ord.o, ord.r.Login, ord.d?.Login, ord.d == null ? null : ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == ord.d.Id));
                return ordInfo.Translate(ctx.Session.UserId);
            }
        }

        class OrderNotificationEntry
        {
            public long OrderId { get; }
            public DateTime Stamp { get; private set; }

            private readonly object _lock = new object();

            public OrderNotificationEntry(long orderId)
            {
                _lock = new object();
            }

            public DateTime Wait(TimeSpan timeout, DateTime from)
            {
                lock (_lock)
                {
                    if (this.Stamp.Ticks <= from.Ticks)
                        Monitor.Wait(_lock, timeout);

                    return this.Stamp;
                }
            }

            public void Update(DateTime now)
            {
                lock (_lock)
                {
                    this.Stamp = now;
                    Monitor.PulseAll(_lock);
                }
            }
        }

        readonly object _notifiersLock = new object();
        readonly Dictionary<long, OrderNotificationEntry> _notifiersByOrderId = new Dictionary<long, OrderNotificationEntry>();

        private bool TryGetNotificationEntry(long orderId, out OrderNotificationEntry entry)
        {
            var ordInfo = this.GetOrder(orderId.ToString());
            switch (ordInfo.Status)
            {
                case OrderStatusType.Created:
                case OrderStatusType.Confirmed:
                case OrderStatusType.Accepted:
                case OrderStatusType.Loading:
                case OrderStatusType.InProgress:
                case OrderStatusType.Unloading:
                    {
                        if (!_notifiersByOrderId.TryGetValue(orderId, out entry))
                            _notifiersByOrderId.Add(orderId, entry = new OrderNotificationEntry(orderId));
                    }
                    break;
                case OrderStatusType.Done:
                case OrderStatusType.Canceled:
                    {
                        if (_notifiersByOrderId.TryGetValue(orderId, out entry))
                            _notifiersByOrderId.Remove(orderId);
                        else
                            entry = null;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return entry != null;
        }

        public StampInfoType GetNotify(string secsStr, string fromStr, string ordIdStr)
        {
            var timeout = TimeSpan.FromSeconds(long.Parse(secsStr));
            var from = new DateTime(long.Parse(fromStr));
            var ordId = long.Parse(ordIdStr);

            OrderNotificationEntry entry;
            lock (_notifiersLock)
            {
                if (!this.TryGetNotificationEntry(ordId, out entry))
                    throw new WebFaultException(HttpStatusCode.Forbidden);
            }

            var value = entry.Wait(timeout, from);

            return new StampInfoType() { Value = value.Ticks };
        }

        private void UpdateOrderStatus(string ordIdStr, OrderStatus status)
        {
            lock (_notifiersLock)
            {
                var now = DateTime.UtcNow;
                var orderId = long.Parse(ordIdStr);

                using (var ctx = _ctx.OpenWebRequestContext())
                {
                    ctx.ValidateAuthorized();

                    var userId = ctx.Session.UserId;
                    var ord = ctx.Db.Raw.Orders.FirstOrDefault(o => o.Id == orderId);
                    if (ord == null)
                        throw new WebFaultException(HttpStatusCode.NotFound);

                    var car = ctx.Db.Raw.Cars.FirstOrDefault(c => c.DriverUserId == userId);

                    switch (status)
                    {
                        case OrderStatus.Confirmed when ord.Status == OrderStatus.Created && ord.RecieverId == userId:
                            ord.ConfirmedStamp = now;
                            break;
                        case OrderStatus.Accepted when ord.Status == OrderStatus.Confirmed && ord.DriverId == -1
                                                    && car != null && car.Status == CarStatus.Free:
                            ord.AcceptedStamp = now;
                            ord.DriverId = userId;
                            car.CurrOrderId = ord.Id;
                            car.Status = CarStatus.Busy;
                            break;
                        case OrderStatus.Loading when ord.Status == OrderStatus.Accepted && ord.DriverId == userId:
                            ord.LoadingStamp = now;
                            break;
                        case OrderStatus.InProgress when ord.Status == OrderStatus.Loading && ord.DriverId == userId:
                            ord.LoadedStamp = now;
                            break;
                        case OrderStatus.Unloading when ord.Status == OrderStatus.InProgress && ord.DriverId == userId:
                            ord.DeliveredStamp = now;
                            break;
                        case OrderStatus.Done when ord.Status == OrderStatus.Unloading && ord.DriverId == userId:
                            ord.FinishedStamp = now;
                            car.CurrOrderId = -1;
                            car.Status = CarStatus.Free;
                            break;
                        case OrderStatus.Canceled when ord.Status == OrderStatus.Created && ord.CustomerId == userId:
                            ord.CancelledStamp = now;
                            break;
                        default:
                            throw new WebFaultException(HttpStatusCode.Forbidden);
                    }

                    ord.Status = status;
                    ctx.Db.SubmitChanges();
                }

                if (this.TryGetNotificationEntry(orderId, out var entry))
                    entry.Update(now);
            }
        }

        public void ConfirmOrder(string ordId)
        {
            this.UpdateOrderStatus(ordId, OrderStatus.Confirmed);
        }

        public void CancelOrder(string ordId)
        {
            this.UpdateOrderStatus(ordId, OrderStatus.Canceled);
        }

        public void AcceptOrder(string ordId)
        {
            this.UpdateOrderStatus(ordId, OrderStatus.Accepted);
        }

        public void ConfirmOrderLoading(string ordId)
        {
            this.UpdateOrderStatus(ordId, OrderStatus.Loading);
        }

        public void ConfirmOrderMoving(string ordId)
        {
            this.UpdateOrderStatus(ordId, OrderStatus.InProgress);
        }

        public void ConfirmOrderUnloading(string ordId)
        {
            this.UpdateOrderStatus(ordId, OrderStatus.Unloading);
        }

        public void ConfirmOrderDone(string ordId)
        {
            this.UpdateOrderStatus(ordId, OrderStatus.Done);
        }

        public void PushErrorReport(ErrorInfoType errorInfo)
        {
            System.Diagnostics.Debug.Print("Remote error info received: " + Environment.NewLine + errorInfo.FormatErrorInfo());
        }

        #endregion
    }
}

