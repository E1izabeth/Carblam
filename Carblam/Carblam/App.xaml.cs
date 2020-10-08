using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Carblam.Views;
using Carblam.ViewModels;

namespace Carblam
{
    public partial class App : Application
    {
        public AppViewModel AppModel { get; }

        public App()
        {
            this.InitializeComponent();

            var mainPage = new MainPage();

            this.MainPage = mainPage;
            this.AppModel = mainPage.AppModel;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

    }
}
