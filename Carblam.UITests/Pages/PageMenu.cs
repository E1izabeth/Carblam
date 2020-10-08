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
    public abstract class PageMenu
    {
        protected IApp _app;

        public Query MenuButton { get; }

        public PageMenu(IApp app)
        {
            _app = app;

            var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                            .Where(p => p.PropertyType == typeof(Query) && p.DeclaringType != typeof(PageMenu))
                            .ToArray();

            this.MenuButton = c => c.Class("AppCompatImageButton").Marked("OK");

            Query MakeQueryMenuByText(string mark)
            {
                return x => x.Marked("menuListItem").Text(mark);
            }

            foreach (var prop in props)
            {
                prop.SetValue(this, MakeQueryMenuByText(prop.Name.Replace('_', ' ')));
            }
        }

        public void GoTo(Query menuEntry)
        {
            _app.WaitForElement(this.MenuButton, timeout: BasePage.Timeout);
            _app.Tap(this.MenuButton);
            _app.WaitForElement(menuEntry, timeout: BasePage.Timeout);
            _app.Tap(menuEntry);
            _app.WaitForNoElement(menuEntry, timeout: BasePage.Timeout);
        }
    }

    public class HomePageMenu : PageMenu
    {
        public Query Login { get; private set; }
        public Query Register { get; private set; }
        public Query Restore { get; private set; }
        public Query About { get; private set; }

        public HomePageMenu(IApp app)
            : base(app)
        {

        }
    }

    public class RootPageMenu : PageMenu
    {
        public Query New_delivery { get; private set; }
        public Query My_deliveries { get; private set; }
        public Query Driver_mode { get; private set; }
        public Query Profile { get; private set; }
        public Query About { get; private set; }
        public Query Logout { get; private set; }

        public RootPageMenu(IApp app)
            : base(app)
        {

        }
    }
}
