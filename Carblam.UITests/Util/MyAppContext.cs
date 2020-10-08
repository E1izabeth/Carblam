using Carblam.UITests.Pages;
using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Carblam.UITests.Util
{
    public class MyAppContext : IDisposable
    {
        private static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                Environment.SetEnvironmentVariable("JAVA_HOME", @"C:\Program Files\Android\Jdk\microsoft_dist_openjdk_1.8.0.25", EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("ANDROID_HOME", @"C:\Program Files (x86)\Android\android-sdk", EnvironmentVariableTarget.Process);
                return ConfigureApp.Android
                                   .ApkFile(@"D:\portable-project.ru\elizabeth-va\Testing\Carblam\Carblam\Carblam.Android\bin\Debug\com.companyname.carblam.apk")
                                   .StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }

        readonly DisposableList _disposables = new DisposableList();

        public static MyAppContext Open(Platform platform, bool resetDb = true)
        {
            return new MyAppContext(StartApp(platform), resetDb);
        }

        public IApp App { get; }
        public AppPages Pages { get; }

        private MyAppContext(IApp app, bool resetDb)
        {
            this.App = app;
            this.Pages = new AppPages(app);

            _disposables.Add(new DbContext(resetDb));
            _disposables.Add(new ServiceContext());
        }

        public void Dispose()
        {
            _disposables.SafeDispose();
        }
    }
}