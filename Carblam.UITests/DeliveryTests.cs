using System;
using System.IO;
using System.Linq;
using System.Threading;
using Carblam.UITests.Pages;
using Carblam.UITests.Util;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Carblam.UITests
{
    [TestFixture(Platform.Android)]
    public class DeliveryTests
    {
        Platform platform;

        public DeliveryTests(Platform platform)
        {
            this.platform = platform;
        }

        [Test]
        [Order(1)]
        public void S1_CreateDeliveryTest()
        {
            using (var app = MyAppContext.Open(platform, true))
            {
                var p = app.Pages;

                p.Login.InputLogin("login");
                p.Login.InputPassword("1234567890-");
                p.Login.ClickLogin();
                p.OrdersList.WaitForPageToLoad();

                p.CreateDelivery.Menu.GoTo(p.CreateDelivery.Menu.New_delivery);
                p.CreateDelivery.WaitForPageToLoad();

                p.CreateDelivery.InputFrom("Бассейная 41 СПБ");
                p.CreateDelivery.ClickLookupFrom();
                p.CreateDelivery.ClickLookupTake();
                p.CreateDelivery.InputTo("Гражданский 73 СПБ");
                p.CreateDelivery.ClickLookupTo();
                p.CreateDelivery.ClickLookupTake();
                p.CreateDelivery.InputMsg("some secret message");
                p.CreateDelivery.InputRecipient("e1izabeth");
                p.CreateDelivery.InputDescr("my testful parcel");
                p.CreateDelivery.ClickCreate();

                p.Logout.Menu.GoTo(p.Logout.Menu.Logout);
                p.Logout.WaitForPageToLoad();
                p.Logout.ClickLogout();
            }
        }

        [Test]
        [Order(2)]
        public void S2_ConfirmDeliveryTest()
        {
            using (var app = MyAppContext.Open(platform, false))
            {
                var p = app.Pages;

                p.Login.InputLogin("e1izabeth");
                p.Login.InputPassword("1234567890-");
                p.Login.ClickLogin();

                p.OrdersList.WaitForPageToLoad();

                p.OrdersList.Menu.GoTo(p.OrdersList.Menu.My_deliveries);
                p.OrdersList.WaitForPageToLoad();
                p.OrdersList.ClickDetails(0);

                p.OrderDetails.WaitForPageToLoad();
                p.OrderDetails.ClickConfirm();

                p.Logout.Menu.GoTo(p.Logout.Menu.Logout);
                p.Logout.WaitForPageToLoad();
                p.Logout.ClickLogout();
            }
        }

        [Test]
        [Order(3)]
        public void S3_DeliverDeliveryTest()
        {
            using (var app = MyAppContext.Open(platform, false))
            {
                var p = app.Pages;

                p.Login.InputLogin("login");
                p.Login.InputPassword("1234567890-");
                p.Login.ClickLogin();

                p.OrdersList.WaitForPageToLoad();

                p.Driver.Menu.GoTo(p.Driver.Menu.Driver_mode);
                p.Driver.WaitForPageToLoad();
                p.Driver.InputDescr("shevrolet lanos");
                p.Driver.InputDesign("к128ве");
                p.Driver.ClickStart();
                p.Driver.ClickAccept(0);
                p.OrderDetails.WaitForPageToLoad();

                Thread.Sleep(5);
                p.OrderDetails.ClickLoading();
                Thread.Sleep(5);
                p.OrderDetails.ClickLoaded();
                Thread.Sleep(5);
                p.OrderDetails.ClickUnloading();
                Thread.Sleep(5);
                p.OrderDetails.ClickDone();

                p.Driver.Menu.GoTo(p.Driver.Menu.Driver_mode);
                p.Driver.WaitForPageToLoad();
                p.Driver.ClickStop();

                p.Logout.Menu.GoTo(p.Logout.Menu.Logout);
                p.Logout.WaitForPageToLoad();
                p.Logout.ClickLogout();
            }
        }


        //[Test]
        //public void Repl()
        //{
        //    using (var app = MyAppContext.Open(platform, false))
        //    {
        //        app.App.Repl();
        //    }
        //}

        //[Test]
        //[Order(1)]
        //public void Repl()
        //{
        //    using (var app = MyAppContext.Open(platform, false))
        //    {
        //        app.App.Repl();
        //    }

        //    ////    app.WaitForElement(c => c.Class("AppCompatImageButton").Marked("OK")); // menu button

        //    ////Thread.Sleep(5);
        //    ////app.Tap(c => c.Class("AppCompatImageButton").Marked("OK"));

        //    ////app.Repl();

        //    ////Thread.Sleep(5);
        //    ////app.Tap(x => x.Marked("menuListItem").Text("About")); // menu entry

        //    ////Thread.Sleep(5);
        //    ////app.Tap(c => c.Id("menuListItem").Index(4));

        //    ////Thread.Sleep(5);
        //    ////app.Tap(c => c.Class("ImageButton").Marked("OK"));

        //    ////Thread.Sleep(5);
        //    ////app.Tap(c => c.Id("menuListItem").Index(1));

        //    ////app.Tap(c => c.Button("Log-in!"));

        //    ////Thread.Sleep(5);

        //    // AppResult[] results = app.WaitForElement(c => c.Marked("Welcome to Xamarin.Forms!"));
        //    // app.Screenshot("Welcome screen.");

        //    // Assert.IsTrue(results.Any());
        //}
    }
}
