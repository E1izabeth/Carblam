﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Javax.Net.Ssl;
using Org.Apache.Http.Params;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Conn.Schemes;
using Org.Apache.Http.Impl.Conn;
using Org.Apache.Http.Conn.Ssl;
using Android.Content;
using Carblam.ViewModels;

namespace Carblam.Droid
{
    [Activity(Label = "Carblam", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, 
              Name = "Carblam.Droid.MainActivity", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, 
                  DataScheme = "http", DataHost = "172.16.100.47")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public AppViewModel AppModel { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.SetFlags("Expander_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);

            var app = new App();
            this.LoadApplication(app);
            this.AppModel = app.AppModel;

            var data = this.Intent?.Data;
            if (data != null)
            {
                var actionName = data.GetQueryParameter("action");
                var keyStr = data.GetQueryParameter("key");
                this.AppModel.ApplyAction(actionName, keyStr);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            var data = intent?.Data;
            if (data != null)
            {
                System.Diagnostics.Debug.Print(data.ToString());
            }
        }

        public override void OnBackPressed()
        {
            var prev = this.AppModel.CurrentStateModel.CurrentPage.PreviousPage;
            if (prev != null)
                this.AppModel.CurrentStateModel.CurrentPage = prev;
        }
    }
}