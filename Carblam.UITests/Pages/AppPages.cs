using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;

namespace Carblam.UITests.Pages
{
    public class AppPages
    {
        public LoginPage Login { get; }
        public RegisterPage Register { get; }
        public RestorePage Restore { get; }

        public CreateDeliveryPage CreateDelivery { get; }
        public OrdersListPage OrdersList { get; }
        public OrderDetailsPage OrderDetails { get; }
        public DriverPage Driver { get; }
        public PropfilePage Profile { get; }
        public LogoutPage Logout { get; }


        public AppPages(IApp app)
        {
            this.Login = new LoginPage(app);
            this.Register = new RegisterPage(app);
            this.Restore = new RestorePage(app);

            this.CreateDelivery = new CreateDeliveryPage(app);
            this.OrdersList = new OrdersListPage(app);
            this.OrderDetails = new OrderDetailsPage(app);
            this.Driver = new DriverPage(app);
            this.Profile = new PropfilePage(app);
            this.Logout = new LogoutPage(app);
        }
    }
}
