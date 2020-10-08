using Carblam.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;

namespace Carblam.UITests.Util
{
    public sealed class ServiceContext : IDisposable
    {
        public const string TestDbConnectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=carblamdb2test;Integrated Security=True;MultipleActiveResultSets=True;TransparentNetworkIPResolution=False";
        public const string TestServiceHostUrl = @"http://0.0.0.0:8181/mysvc/";

        private static readonly CarblamServiceConfiguration _testServiceConfiguration = new CarblamServiceConfiguration()
        {
            DbConnectionString = TestDbConnectionString,
            ServiceHostUrl = TestServiceHostUrl,
            SessionTimeout = TimeSpan.FromHours(1),
            TokenTimeout = TimeSpan.FromDays(7),

            //SmtpServerHost = "smtp.live.com",
            //SmtpServerPort = 587,
            //SmtpLogin = "ipm.local@hotmail.com",
            //SmtpPassword = "ifmo2020",
            //SmtpUseSsl = true,
            //SmtpUseDefaultCredentials = false,
            //SmtpDeliveryMethod = SmtpDeliveryMethod.Network
            SmtpLogin = "carblam@localhost",
            SmtpServerHost = "127.0.0.1",
            SmtpServerPort = 25,
            SmtpUseSsl = false,
            SmtpUseDefaultCredentials = true,
            SmtpDeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            SmtpPickupDirectoryLocation = @"D:\Temp\"
        };

        private readonly DisposableList _disposables = new DisposableList();

        public ServiceContext()
        {
            _disposables.Add(new CarblamService(_testServiceConfiguration));
        }

        public void Dispose()
        {
            _disposables.SafeDispose();
        }
    }
}
