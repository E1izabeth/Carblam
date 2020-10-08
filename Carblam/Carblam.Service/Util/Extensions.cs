using Carblam.Service.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Carblam.Service.Util
{
    static class Extensions
    {
        public static CarInfoType Translate(this DbCarInfo carInfo)
        {
            return new CarInfoType()
            {
                Spec = new StartWorkSpecType()
                {
                    Width = carInfo.Width,
                    Length = carInfo.Length,
                    Height = carInfo.Height,
                    Description = carInfo.Description,
                    Designation = carInfo.Designation,
                    WeightLimit = carInfo.WeightLimit,
                },
                Location = new LocationType() { Latitude = carInfo.LocationLat, Longitude = carInfo.LocationLon }
            };
        }

        public static CarInfoListType Translate(this IEnumerable<DbCarInfo> cars)
        {
            return new CarInfoListType()
            {
                Items = cars.Select(carInfo => carInfo.Translate()).ToArray()
            };
        }

        public static OrderInfoType Translate(this OrderInfo order, long userId)
        {
            var limit = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            var ord = order.Order;
            return new OrderInfoType()
            {
                Spec = new OrderSpecType()
                {
                    Parcel = new ParcelInfoType()
                    {
                        Width = ord.Width,
                        Length = ord.Length,
                        Height = ord.Height,
                        Description = ord.Description,
                        Weight = ord.Weight
                    },
                    ConfirmMessage = userId == ord.CustomerId || userId == ord.RecieverId ? ord.MessageForReciever : "[Hidden]",
                    FromAddress = ord.SourceAddress,
                    ToAddress = ord.DestinationAddress,
                    FromLocation = new LocationType() { Latitude = ord.SourceLocationLat, Longitude = ord.SourceLocationLon },
                    ToLocation = new LocationType() { Latitude = ord.DestinationLocationLat, Longitude = ord.DestinationLocationLon },
                    ReceiverLogin = order.ReceiverLogin
                },
                Status = ord.Status.ToString().Parse<OrderStatusType>(),
                Id = ord.Id,
                Car = order.Car != null && ord.Status != OrderStatus.Done ? order.Car.Translate() : null,
                IsMyOutcome = ord.CustomerId == userId,
                IsMyIncome = ord.RecieverId == userId,
                IsMyBaggage = ord.DriverId == userId,
                History = new OrderHistoryInfoType()
                {
                    Created = ord.CreatedStamp > limit ? ord.CreatedStamp : new Nullable<DateTime>(),
                    Confirmed = ord.ConfirmedStamp > limit ? ord.ConfirmedStamp : new Nullable<DateTime>(),
                    Accepted = ord.AcceptedStamp > limit ? ord.AcceptedStamp : new Nullable<DateTime>(),
                    Loading = ord.LoadingStamp > limit ? ord.LoadingStamp : new Nullable<DateTime>(),
                    Loaded = ord.LoadedStamp > limit ? ord.LoadedStamp : new Nullable<DateTime>(),
                    Delivered = ord.DeliveredStamp > limit ? ord.DeliveredStamp : new Nullable<DateTime>(),
                    Finished = ord.FinishedStamp > limit ? ord.FinishedStamp : new Nullable<DateTime>(),
                    Cancelled = ord.CancelledStamp > limit ? ord.CancelledStamp : new Nullable<DateTime>(),
                }
            };
        }

        public static OrderInfoListType Translate(this IEnumerable<OrderInfo> ordds, long userId)
        {
            return new OrderInfoListType()
            {
                Items = ordds.Select(ord => ord.Translate(userId)).ToArray()
            };
        }
    }

    class OrderInfo
    {
        public DbOrderInfo Order { get; }
        public string ReceiverLogin { get; }
        public string DriverLogin { get; }
        public DbCarInfo Car { get; }

        public OrderInfo(DbOrderInfo order, string receiverLogin, string driverLogn, DbCarInfo car)
        {
            this.Order = order;
            this.ReceiverLogin = receiverLogin;
            this.DriverLogin = driverLogn;
            this.Car = car;
        }
    }
}
