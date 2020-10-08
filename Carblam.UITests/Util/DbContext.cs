#pragma warning disable CS0618 // Type or member is obsolete
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Carblam.UITests.Util
{
    public sealed class DbContext : IDisposable
    {
        static DbContext()
        {
            var asm1 = Assembly.LoadFile(@"C:\Windows\Microsoft.NET\assembly\GAC_64\Microsoft.SqlServer.BatchParser\v4.0_15.0.0.0__89845dcd8080cc91\microsoft.sqlserver.batchparser.dll");
            var asm2 = Assembly.LoadFile(@"c:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.SqlServer.BatchParserClient\v4.0_15.0.0.0__89845dcd8080cc91\Microsoft.SqlServer.BatchParserClient.dll");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, ea) =>
            {
                if (ea.Name.Contains("BatchParserClient"))
                {
                    return asm2;
                }
                if (ea.Name == asm1.FullName)
                    return asm1;
                if (ea.Name == asm2.FullName)
                    return asm2;

                return null;
            };
        }

        public DbContext(bool resetDb)
        {
            if (resetDb)
            {
                using (var cnn = new SqlConnection(ServiceContext.TestDbConnectionString))
                {
                    Server db = new Server(new ServerConnection(cnn));
                    db.ConnectionContext.ExecuteNonQuery(Properties.Resources.DbCleanupScript);
                    db.ConnectionContext.ExecuteNonQuery(Properties.Resources.DbPreparationScript);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
