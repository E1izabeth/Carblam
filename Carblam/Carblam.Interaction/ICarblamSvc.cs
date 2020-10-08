using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Carblam.Interaction
{
    [ServiceContract(Namespace = "MicroLearningSvc")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document)]
    public interface ICarblamSvc
    {
        //[OperationContract, WebInvoke(UriTemplate = "/{*path}", Method = "OPTIONS")]
        //void CorsHandler(string path);

        [OperationContract, WebInvoke(UriTemplate = "/profile?action=register", Method = "POST")]
        void Register(RegisterSpecType registerSpec);
        [OperationContract, WebInvoke(UriTemplate = "/profile?action=restore", Method = "POST")]
        void RequestAccess(ResetPasswordSpecType spec);
        [OperationContract, WebGet(UriTemplate = "/profile?action=activate&key={key}")]
        OkType Activate(string key);
        [OperationContract, WebGet(UriTemplate = "/profile?action=restore&key={key}")]
        OkType RestoreAccess(string key);
        [OperationContract, WebInvoke(UriTemplate = "/profile?action=delete", Method = "POST")]
        void DeleteProfile();

        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=login")]
        void Login(LoginSpecType loginSpec);
        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=activate")]
        void RequestActivation(RequestActivationSpecType spec);
        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=set-email")]
        void SetEmail(ChangeEmailSpecType spec);
        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=set-password")]
        void SetPassword(ChangePasswordSpecType spec);
        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=logout")]
        void Logout();
        [OperationContract, WebGet(UriTemplate = "/profile")]
        ProfileFootprintInfoType GetProfileFootprint();

        [OperationContract, WebGet(UriTemplate = "/car")]
        CarInfoType GetCarInfo();
        [OperationContract, WebGet(UriTemplate = "/cars?lat={lat}&lon={lon}")]
        CarInfoListType GetFreeCarsNear(string lat, string lon);
        [OperationContract, WebInvoke(UriTemplate = "/car?action=start", Method = "POST")]
        void StartWork(StartWorkSpecType spec);
        [OperationContract, WebInvoke(UriTemplate = "/car?action=stop", Method = "POST")]
        void StopWork();
        [OperationContract, WebInvoke(UriTemplate = "/car?action=location", Method = "POST")]
        void UpdateLocation(LocationType spec);

        [OperationContract, WebGet(UriTemplate = "/orders?filter=outcome")]
        OrderInfoListType GetMyOutcomeOrders();
        [OperationContract, WebGet(UriTemplate = "/orders?filter=income")]
        OrderInfoListType GetMyIncomeOrders();
        [OperationContract, WebGet(UriTemplate = "/orders?filter=unaccepted&lat={lat}&lon={lon}")]
        OrderInfoListType GetUnacceptedOrdersNear(string lat, string lon);
        [OperationContract, WebInvoke(UriTemplate = "/orders", Method = "POST")]
        OrderInfoType CreateOrder(OrderSpecType spec);
        [OperationContract, WebGet(UriTemplate = "/orders/{ordId}")]
        OrderInfoType GetOrder(string ordId);
        [OperationContract, WebInvoke(UriTemplate = "/orders/{ordId}?action=confirm", Method = "POST")]
        void ConfirmOrder(string ordId);
        [OperationContract, WebInvoke(UriTemplate = "/orders/{ordId}?action=cancel", Method = "POST")]
        void CancelOrder(string ordId);
        [OperationContract, WebInvoke(UriTemplate = "/orders/{ordId}?action=accept", Method = "POST")]
        void AcceptOrder(string ordId);
        [OperationContract, WebInvoke(UriTemplate = "/orders/{ordId}?action=loading", Method = "POST")]
        void ConfirmOrderLoading(string ordId);
        [OperationContract, WebInvoke(UriTemplate = "/orders/{ordId}?action=moving", Method = "POST")]
        void ConfirmOrderMoving(string ordId);
        [OperationContract, WebInvoke(UriTemplate = "/orders/{ordId}?action=unloading", Method = "POST")]
        void ConfirmOrderUnloading(string ordId);
        [OperationContract, WebInvoke(UriTemplate = "/orders/{ordId}?action=done", Method = "POST")]
        void ConfirmOrderDone(string ordId);

        [OperationContract, WebGet(UriTemplate = "/notify?timeout={secs}&from={from}&order={ordId}")]
        StampInfoType GetNotify(string secs, string from, string ordId);

        [OperationContract, WebInvoke(UriTemplate = "/error-report")]
        void PushErrorReport(ErrorInfoType errorInfo);
    }
}
