using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Carblam.Models
{
    class CarblamSvcApi
    {
        readonly WebApiHelper _helper;

        public CarblamSvcApi(string rootUrl, WebSvcMode mode)
        {
            _helper = new WebApiHelper(rootUrl, mode);
        }

        public async Task Login(string login, string password)
        {
            await _helper.Post("/profile?action=login", new LoginSpecType() { Login = login, Password = password });
        }

        public async Task Register(string login, string email, string password, string password2)
        {
            if (password != password2)
                throw new WebApiException("Passwords are not matched");
            else if (!Regex.Match(email, @"(\S)*@(\S)*.(\S)*").Success)
            {
                throw new WebApiException("Invalid email");
            }

            await _helper.Post("/profile?action=register", new RegisterSpecType() { Login = login, Email = email, Password = password });
        }

        public async Task Logout()
        {
            await _helper.Post("/profile?action=logout");
        }

        internal async Task Restore(string login, string email, string email2)
        {
            if (email != email2)
                throw new WebApiException("Emails are not matched");
            else if (!Regex.Match(email, @"(\S)*@(\S)*.(\S)*").Success)
            {
                throw new WebApiException("Invalid email");
            }

            await _helper.Post("/profile?action=restore", new ResetPasswordSpecType() { Login = login, Email = email });
        }

        internal async Task<OkType> Activate(string key)
        {
            return await _helper.Get<OkType>("/profile?action=activate&key=" + key);
        }

        internal async Task<OkType> RestoreAccess(string key)
        {
            return await _helper.Get<OkType>("/profile?action=restore&key=" + key);
        }

        internal async Task<ProfileFootprintInfoType> GetProfileFootprint()
        {
            return await _helper.Get<ProfileFootprintInfoType>("/profile");
        }

        internal async Task RequestActivation(string oldEmail)
        {
            await _helper.Post("/profile?action=activate",
                new RequestActivationSpecType() { Email = oldEmail }
            );
        }

        internal async Task SetEmail(string password, string oldEmail, string newEmail)
        {
            await _helper.Post("/profile?action=set-email",
                new ChangeEmailSpecType() { Password = password, NewEmail = newEmail, OldEmail = oldEmail }
            );
        }

        internal async Task SetPassword(string newPassword, string email)
        {
            await _helper.Post("/profile?action=set-password",
                new ChangePasswordSpecType() { Email = email, NewPassword = newPassword }
            );
        }

        internal async Task<CarInfoType> GetCarInfo()
        {
            return await _helper.GetOrDefault<CarInfoType>("/car");
        }

        internal async Task<CarInfoListType> GetFreeCarsNear(string lat, string lon)
        {
            return await _helper.Get<CarInfoListType>($"/cars?lat={lat}&lon={lon}");
        }

        internal async Task StartWork(StartWorkSpecType spec)
        {
            await _helper.Post("/car?action=start", spec);
        }

        internal async Task StopWork()
        {
            await _helper.Post("/car?action=stop");
        }

        internal async Task UpdateLocation(LocationType spec)
        {
            await _helper.Post("/car?action=location", spec);
        }

        internal async Task<OrderInfoListType> GetMyOutcomeOrders()
        {
            return await _helper.Get<OrderInfoListType>("/orders?filter=outcome");
        }

        internal async Task<OrderInfoListType> GetMyIncomeOrders()
        {
            return await _helper.Get<OrderInfoListType>("/orders?filter=income");
        }

        internal async Task<OrderInfoListType> GetUnacceptedOrdersNear(double lat, double lon)
        {
            return await _helper.Get<OrderInfoListType>($"/orders?filter=unaccepted&lat={lat}&lon={lon}");
        }

        internal async Task<OrderInfoType> CreateOrder(OrderSpecType spec)
        {
            return await _helper.PostAndParse<OrderInfoType>("/orders", spec);
        }

        internal async Task<OrderInfoType> GetOrder(long ordId)
        {
            return await _helper.Get<OrderInfoType>($"/orders/{ordId}");
        }

        internal async Task ConfirmOrder(long ordId)
        {
            await _helper.Post($"/orders/{ordId}?action=confirm");
        }

        internal async Task CancelOrder(long ordId)
        {
            await _helper.Post($"/orders/{ordId}?action=cancel");
        }

        internal async Task AcceptOrder(long ordId)
        {
            await _helper.Post($"/orders/{ordId}?action=accept");
        }

        internal async Task ConfirmOrderLoading(long ordId)
        {
            await _helper.Post($"/orders/{ordId}?action=loading");
        }

        internal async Task ConfirmOrderMoving(long ordId)
        {
            await _helper.Post($"/orders/{ordId}?action=moving");
        }

        internal async Task ConfirmOrderUnloading(long ordId)
        {
            await _helper.Post($"/orders/{ordId}?action=unloading");
        }

        internal async Task ConfirmOrderDone(long ordId)
        {
            await _helper.Post($"/orders/{ordId}?action=done");
        }

        internal async Task<StampInfoType> WaitForNotify(long orderId, DateTime from, TimeSpan timeout)
        {
            return await _helper.Get<StampInfoType>($"/notify?timeout={timeout.TotalSeconds}&from={from.Ticks}&order={orderId}");
        }

        internal async Task PushErrorReport(Exception ex)
        {
            await _helper.Post("/error-report", ex.MakeErrorInfo());
        }
    }
}
