using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carblam.Service.Db
{
    interface ICarblamDbContext
    {
        IUsersRepository Users { get; }         

        IDbContext Raw { get; }

        void SubmitChanges();
    }

    interface IUsersRepository
    {
        void AddUser(DbUserInfo user);
        DbUserInfo GetUserById(long userId);
        DbUserInfo FindUserByLoginKey(string loginKey);
        DbUserInfo FindUserByTokenKey(string key);
    }

    //interface ICarsRepository
    //{
    //    void AddCar(DbCarInfo car);
    //    DbCarInfo GetCarById(long carId);
    //    List<DbCarInfo> GetCarsByStatus(CarStatus status);
    //}

    //interface IOrdersRepository
    //{
    //    void AddCarOrder(DbOrderInfo order);
    //    DbOrderInfo GetOrderById(long orderId);
    //    List<DbOrderInfo> GetOrdersByCustomerId(long customerId);
    //    List<DbOrderInfo> GetOrdersByDriverId(long driverId);
    //    List<DbOrderInfo> GetOrdersByCustomerWithStatus(long customerId, OrderStatus status);
    //}
}
