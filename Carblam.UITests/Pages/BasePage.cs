using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace Carblam.UITests.Pages
{
    public abstract class BasePage
    {
        //protected BasePage(IApp app, string pageTitle)
        protected BasePage(IApp app, string pageTitle)
        {
            this.App = app;
            this.PageTitle = pageTitle;

            var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                             .Where(f => f.FieldType == typeof(Query))
                             .ToArray();

            Query MakeQueryByMark(string mark)
            {
                return x => x.Marked(mark);
            }

            foreach (var field in fields)
            {
                field.SetValue(this, MakeQueryByMark(field.Name.Trim('_')));
            }
        }

        public string PageTitle { get; }
        protected IApp App { get; }

        readonly Query _lblPopupText, _btnPopupClose;

        public static TimeSpan Timeout { get; } = TimeSpan.FromMinutes(1);

        public virtual void WaitForPageToLoad()
        {
            this.App.WaitForElement(x => x.Class("AppCompatTextView").Text(this.PageTitle), timeout: Timeout);
        }

        protected void EnterText(in Query textEntryQuery, in string text, in bool shouldDismissKeyboard = true)
        {
            this.App.WaitForElement(textEntryQuery, timeout: Timeout);

            //try
            //{
            this.App.ClearText(textEntryQuery);
            //}
            //catch (Exception ex)
            //{
            //    try
            //    {
            //        this.App.ClearText(textEntryQuery);
            //    }
            //    catch (Exception ex2)
            //    {
            //        this.App.Repl();
            //    }
            //}

            //try
            //{
            this.App.EnterText(textEntryQuery, text);
            //}
            //catch (Exception ex)
            //{
            //    try
            //    {
            //        this.App.EnterText(textEntryQuery, text);
            //    }
            //    catch (Exception ex2)
            //    {
            //        this.App.Repl();
            //    }
            //}

            if (shouldDismissKeyboard)
                this.App.DismissKeyboard();
        }

        protected void Tap(Query btn)
        {
            this.App.WaitForElement(btn, timeout: Timeout);
            this.App.Tap(btn);
        }

        protected string GetText(Query btn)
        {
            return this.App.WaitForElement(btn, timeout: Timeout).FirstOrDefault()?.Text;
        }

        public void ValidatePopupContainsAndClose(string text)
        {
            var allText = string.Join(Environment.NewLine, this.App.WaitForElement(_lblPopupText, timeout: Timeout).Select(r => r.Text));
            Assert.IsTrue(allText.Contains(text));
            this.Tap(_btnPopupClose);
        }
    }

    public abstract class HomeBasePage : BasePage
    {
        public HomePageMenu Menu { get; private set; }

        public HomeBasePage(IApp app, string title) : base(app, title)
        {
            this.Menu = new HomePageMenu(app);
        }
    }

    public abstract class RootBasePage : BasePage
    {
        public RootPageMenu Menu { get; private set; }

        public RootBasePage(IApp app, string title) : base(app, title)
        {
            this.Menu = new RootPageMenu(app);
        }
    }

    public class LoginPage : HomeBasePage
    {
        readonly Query _txtLogin, _txtPassword, _btnLogin;

        public LoginPage(IApp app) : base(app, "Login")
        {
        }

        public void InputLogin(string text) { this.EnterText(_txtLogin, text); }
        public void InputPassword(string text) { this.EnterText(_txtPassword, text); }

        public void ClickLogin() { this.App.Tap(_btnLogin); }
    }

    public class RestorePage : HomeBasePage
    {
        readonly Query txtResLogin, txtResEmail1, txtResEmail2, btnRestore;

        public RestorePage(IApp app) : base(app, "Restore")
        {
        }

        public void InputLogin(string text) { this.EnterText(txtResLogin, text); }
        public void InputEmail1(string text) { this.EnterText(txtResEmail1, text); }
        public void InputEmail2(string text) { this.EnterText(txtResEmail2, text); }

        public void ClickRestore() { this.App.Tap(btnRestore); }
    }

    public class RegisterPage : HomeBasePage
    {
        readonly Query _txtRegLogin,
                        _txtRegEmail,
                        _txtRegPwd1,
                        _txtRegPwd2,
                        _btnRegister;


        public RegisterPage(IApp app) : base(app, "Register")
        {
        }

        public void InputLogin(string text) { this.EnterText(_txtRegLogin, text); }
        public void InputEmail(string text) { this.EnterText(_txtRegEmail, text); }
        public void InputPassword1(string text) { this.EnterText(_txtRegPwd1, text); }
        public void InputPassword2(string text) { this.EnterText(_txtRegPwd2, text); }

        public void ClickRegister() { this.App.Tap(_btnRegister); }
    }

    public class CreateDeliveryPage : RootBasePage
    {
        readonly Query _btnLookupCancel,
                        _btnLookupTake,
                        _txtFromAddr,
                        _btnLookupFrom,
                        _txtToAddr,
                        _btnLookupTo,
                        _txtRecipient,
                        _txtDescr,
                        _txtMsg,
                        _btnCreate;

        public CreateDeliveryPage(IApp app) : base(app, "New delivery")
        {

        }

        public void InputFrom(string text) { this.EnterText(_txtFromAddr, text); }
        public void InputTo(string text) { this.EnterText(_txtToAddr, text); }
        public void InputRecipient(string text) { this.EnterText(_txtRecipient, text); }
        public void InputDescr(string text) { this.EnterText(_txtDescr, text); }
        public void InputMsg(string text) { this.EnterText(_txtMsg, text); }

        public void ClickLookupFrom() { this.Tap(_btnLookupFrom); }
        public void ClickLookupTo() { this.Tap(_btnLookupTo); }
        public void ClickLookupTake() { this.Tap(_btnLookupTake); }
        public void ClickLookupCancel() { this.Tap(_btnLookupCancel); }
        public void ClickCreate() { this.Tap(_btnCreate); }
    }

    public class OrdersListPage : RootBasePage
    {
        public OrdersListPage(IApp app) : base(app, "My deliveries")
        {
        }

        public void ClickDetails(int n) { this.Tap(x => x.Marked("btnOrdLstDetails").Index(n)); }
    }

    public class OrderDetailsPage : RootBasePage
    {
        readonly Query _btnOrdConfirm,
                        _btnOrdCancel,
                        _btnOrdAccept,
                        _btnOrdLoading,
                        _btnOrdLoaded,
                        _btnOrdUnloading,
                        _btnOrdDone;

        public OrderDetailsPage(IApp app) : base(app, "Order details")
        {
        }

        public void ClickConfirm() { this.Tap(_btnOrdConfirm); }
        public void ClickCancel() { this.Tap(_btnOrdCancel); }
        public void ClickAccept() { this.Tap(_btnOrdAccept); }
        public void ClickLoading() { this.Tap(_btnOrdLoading); }
        public void ClickLoaded() { this.Tap(_btnOrdLoaded); }
        public void ClickUnloading() { this.Tap(_btnOrdUnloading); }
        public void ClickDone() { this.Tap(_btnOrdDone); }
    }

    public class DriverPage : RootBasePage
    {
        readonly Query _txtDrvDesign,
                        _txtDrvDescr,
                        _btnDrvStart,
                        _btnDrvStop,
                        _btnDrvDetails;

        public DriverPage(IApp app) : base(app, "Driver mode")
        {
        }

        public void InputDesign(string text) { this.EnterText(_txtDrvDesign, text); }
        public void InputDescr(string text) { this.EnterText(_txtDrvDescr, text); }

        public void ClickStart() { this.Tap(_btnDrvStart); }
        public void ClickStop() { this.Tap(_btnDrvStop); }
        public void ClickAccept(int n) { this.Tap(x => x.Marked("btnDrvAccept").Index(n)); }
        public void ClickDetails() { this.Tap(_btnDrvDetails); }
    }

    public class LogoutPage : RootBasePage
    {
        readonly Query _btnLogout;

        public LogoutPage(IApp app) : base(app, "Logout")
        {

        }
        public void ClickLogout() { this.Tap(_btnLogout); }
    }

    public class PropfilePage : RootBasePage
    {
        readonly Query _lblMailOptions,
                        _txtMailOld,
                        _txtMailNew1,
                        _txtMailNew2,
                        _txtMailPwd,
                        _btnMailChangeMail,
                        _lblPwdOptions,
                        _txtPwdOldPass,
                        _txtPwdNewPass1,
                        _txtPwdNewPass2,
                        _txtPwdEmail,
                        _btnPwdChangePass,
                        _txtActivationEmail,
                        _btnActivateProfile;


        public PropfilePage(IApp app) : base(app, "Profile")
        {

        }

        public void ClickMailOpts() { this.Tap(_lblMailOptions); }
        public void InputMailOld(string text) { this.EnterText(_txtMailOld, text); }
        public void InputMailNew1(string text) { this.EnterText(_txtMailNew1, text); }
        public void InputMailNew2(string text) { this.EnterText(_txtMailNew2, text); }
        public void InputMailPwd(string text) { this.EnterText(_txtMailPwd, text); }
        public void ClickMailChange() { this.Tap(_btnMailChangeMail); }

        public void ClickPwdOpts() { this.Tap(_lblPwdOptions); }
        // public void InputPwdOld(string text) { this.EnterText(_txtPwdOldPass, text); }
        public void InputPwdNew1(string text) { this.EnterText(_txtPwdNewPass1, text); }
        public void InputPwdNew2(string text) { this.EnterText(_txtPwdNewPass2, text); }
        public void InputPwdEmail(string text) { this.EnterText(_txtPwdEmail, text); }
        public void ClickPwdChange() { this.Tap(_btnPwdChangePass); }

        public void InputActivationEmail(string text) { this.EnterText(_txtActivationEmail, text); }
        public void ClickRequestActivation() { this.Tap(_btnActivateProfile); }
    }
}
