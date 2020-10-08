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
    public class ProfileTests
    {
        Platform platform;

        public ProfileTests(Platform platform)
        {
            this.platform = platform;
        }

        [Test]
        [Order(0)]
        public void S0_RegisterTest()
        {
            using (var app = MyAppContext.Open(platform))
            {
                var pages = app.Pages;

                pages.Register.Menu.GoTo(pages.Register.Menu.Register);
                pages.Register.WaitForPageToLoad();
                pages.Register.InputLogin("login2");
                pages.Register.InputEmail("register@mail.ru");
                pages.Register.InputPassword1("1234567890-");
                pages.Register.InputPassword2("1234567890-");
                pages.Register.ClickRegister();

                Thread.Sleep(10000);
            }
        }

        [Test]
        [Order(1)]
        public void S1_ActivationRequest()
        {
            using (var app = MyAppContext.Open(platform, false))
            {
                var pages = app.Pages;

                pages.Login.InputLogin("login2");
                pages.Login.InputPassword("1234567890-");
                pages.Login.ClickLogin();

                pages.OrdersList.WaitForPageToLoad();
                pages.Profile.Menu.GoTo(pages.Profile.Menu.Profile);
                pages.Profile.WaitForPageToLoad();
                pages.Profile.InputActivationEmail("register@mail.ru");
                pages.Profile.ClickRequestActivation();
                pages.Profile.ValidatePopupContainsAndClose("Activation link sent to your email");

                pages.Logout.Menu.GoTo(pages.Logout.Menu.Logout);
                pages.Logout.WaitForPageToLoad();
                pages.Logout.ClickLogout();
            }
        }

        [Test]
        [Order(2)]
        public void S2_LoginLogoutTest()
        {
            using (var app = MyAppContext.Open(platform, false))
            {
                var pages = app.Pages;

                pages.Login.InputLogin("login");
                pages.Login.InputPassword("1234567890-");
                pages.Login.ClickLogin();
                pages.OrdersList.WaitForPageToLoad();

                pages.Logout.Menu.GoTo(pages.Logout.Menu.Logout);
                pages.Logout.WaitForPageToLoad();
                pages.Logout.ClickLogout();
            }
        }

        [Test]
        [Order(3)]
        public void S3_RestoreTest()
        {
            using (var app = MyAppContext.Open(platform))
            {
                var page = app.Pages;
                page.Register.Menu.GoTo(page.Register.Menu.Restore);
                page.Restore.WaitForPageToLoad();
                page.Restore.InputLogin("e1izabeth");
                page.Restore.InputEmail1("restore@mail.ru");
                page.Restore.InputEmail2("restore@mail.ru");
                page.Restore.ClickRestore();
         
                Thread.Sleep(3000);
            }
        }

        [Test]
        [Order(4)]
        public void S4_ChangePasswordTest()
        {
            using (var app = MyAppContext.Open(platform))
            {
                var page = app.Pages;

                page.Login.InputLogin("tester");
                page.Login.InputPassword("qwertyuiop");
                page.Login.ClickLogin();
                page.OrdersList.WaitForPageToLoad();
                page.Profile.Menu.GoTo(page.Profile.Menu.Profile);
                page.Profile.WaitForPageToLoad();
                page.Profile.ClickPwdOpts();
                page.Profile.InputPwdNew1("1234567890-");
                page.Profile.InputPwdNew2("1234567890-");
                page.Profile.InputPwdEmail("kuzenkova.el@yandex.ru");
                page.Profile.ClickPwdChange();

                page.Logout.Menu.GoTo(page.Logout.Menu.Logout);
                page.Logout.WaitForPageToLoad();
                page.Logout.ClickLogout();
            }
        }

        [Test]
        [Order(5)]
        public void S5_ChangePasswordFailTest()
        {
            using (var app = MyAppContext.Open(platform))
            {
                var page = app.Pages;

                page.Login.InputLogin("tester");
                page.Login.InputPassword("qwertyuiop");
                page.Login.ClickLogin();
                page.OrdersList.WaitForPageToLoad();
                page.Profile.Menu.GoTo(page.Profile.Menu.Profile);
                page.Profile.WaitForPageToLoad();
                page.Profile.ClickPwdOpts();
                page.Profile.InputPwdNew1("1234567890-");
                page.Profile.InputPwdNew2("1234-");
                page.Profile.InputPwdEmail("kuzenkova.el@yandex.ru");
                page.Profile.ClickPwdChange();
                page.Profile.ValidatePopupContainsAndClose("Passwords are not match");
                page.Logout.Menu.GoTo(page.Logout.Menu.Logout);
                page.Logout.WaitForPageToLoad();
                page.Logout.ClickLogout();
            }
        }

        [Test]
        [Order(6)]
        public void S6_ChangeEmalTest()
        {
            using (var app = MyAppContext.Open(platform))
            {
                var page = app.Pages;

                page.Login.InputLogin("nokia");
                page.Login.InputPassword("qwertyuiop");
                page.Login.ClickLogin();
                page.OrdersList.WaitForPageToLoad();
                page.Profile.Menu.GoTo(page.Profile.Menu.Profile);
                page.Profile.WaitForPageToLoad();
                page.Profile.ClickMailOpts();
                page.Profile.InputMailOld("kuzenkova.el@yandex.ru");
                page.Profile.InputMailNew1("kuzenkova@e1izabeth.online");
                page.Profile.InputMailNew2("kuzenkova@e1izabeth.online");
                page.Profile.InputMailPwd("1234567890-");
                page.Profile.ClickMailChange();

                page.Logout.Menu.GoTo(page.Logout.Menu.Logout);
                page.Logout.WaitForPageToLoad();
                page.Logout.ClickLogout();
            }
        }

        [Test]
        [Order(7)]
        public void S7_ChangeEmalFailTest()
        {
            using (var app = MyAppContext.Open(platform))
            {
                var page = app.Pages;

                page.Login.InputLogin("nokia");
                page.Login.InputPassword("qwertyuiop");
                page.Login.ClickLogin();
                page.OrdersList.WaitForPageToLoad();
                page.Profile.Menu.GoTo(page.Profile.Menu.Profile);
                page.Profile.WaitForPageToLoad();
                page.Profile.ClickMailOpts();
                page.Profile.InputMailOld("kuzenkova.el@yandex.ru");
                page.Profile.InputMailNew1("rtyui");
                page.Profile.InputMailNew2("kuzenkova@e1izabeth.online");
                page.Profile.InputMailPwd("1234567890-");
                page.Profile.ClickMailChange();

                page.Profile.ValidatePopupContainsAndClose("Emails are not match");
                page.Logout.Menu.GoTo(page.Logout.Menu.Logout);
                page.Logout.WaitForPageToLoad();
                page.Logout.ClickLogout();
            }
        }


    }
}
