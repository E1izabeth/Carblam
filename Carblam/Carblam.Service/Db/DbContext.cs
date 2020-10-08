using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Carblam.Service.Db
{
    interface IDbContext : IDisposable
    {
        Table<DbUserInfo> Users { get; }
        Table<DbCarInfo> Cars { get; }
        Table<DbOrderInfo> Orders { get; }

        IDbConnection Connection { get; }

        bool DatabaseExists();
        void CreateDatabase();
        void CreateTables();
    }

    class DbContext : DataContext, IDbContext
    {
        public Table<DbUserInfo> Users { get; }
        public Table<DbCarInfo> Cars { get; }
        public Table<DbOrderInfo> Orders { get; }

        IDbConnection IDbContext.Connection { get { return base.Connection; } }

        public DbContext(IDbConnection cnn)
            : base(cnn)
        {
            this.Users = base.GetTable<DbUserInfo>();
            this.Cars = base.GetTable<DbCarInfo>();
            this.Orders = base.GetTable<DbOrderInfo>();
        }

        public void CreateTables()
        {
            foreach (var metaTable in this.Mapping.GetTables())
            {
                var cmd = this.Connection.CreateCommand();
                cmd.CommandText = $@"
    IF EXISTS (SELECT * FROM sys.tables WHERE name = '{metaTable.TableName}') 
        SELECT 1
    ELSE
        SELECT 0
    ";
                var n = (int)cmd.ExecuteScalar();
                if (n < 1)
                {
                    // var metaTable = this.Mapping.GetTable(linqTableClass);
                    var typeName = "System.Data.Linq.SqlClient.SqlBuilder";
                    var type = typeof(DataContext).Assembly.GetType(typeName);
                    var bf = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
                    var sql = type.InvokeMember("GetCreateTableCommand", bf, null, null, new[] { metaTable });
                    var sqlAsString = sql.ToString();
                    this.ExecuteCommand(sqlAsString);
                }
            }
        }
    }

    [Table]
    class DbUserInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
        [Column(DbType = "nvarchar(150)")]
        public string Login { get; set; }
        [Column(DbType = "nvarchar(150)")]
        public string LoginKey { get; set; }
        [Column]
        public DateTime RegistrationStamp { get; set; }
        [Column]
        public DateTime LastLoginStamp { get; set; }

        [Column(DbType = "nvarchar(150)")]
        public string PasswordHash { get; set; }
        [Column(DbType = "nvarchar(150)")]
        public string HashSalt { get; set; }

        [Column(DbType = "nvarchar(150)")]
        public string Email { get; set; }
        [Column]
        public bool Activated { get; set; }

        [Column(DbType = "nvarchar(150)")]
        public string LastToken { get; set; }
        [Column]
        public DateTime LastTokenStamp { get; set; }
        [Column]
        public DbUserTokenKind LastTokenKind { get; set; }

        [Column]
        public bool IsDeleted { get; set; }
    }

    [Table]
    class DbCarInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
        [Column]
        public long DriverUserId { get; set; }
        [Column]
        public string Designation { get; set; }
        [Column]
        public string Description { get; set; }
        [Column]
        public double LocationLat { get; set; }
        [Column]
        public double LocationLon { get; set; }
        [Column]
        public long CurrOrderId { get; set; }
        [Column]
        public double Width { get; set; }
        [Column]
        public double Length { get; set; }
        [Column]
        public double Height { get; set; }
        [Column]
        public double WeightLimit { get; set; }
        [Column]
        public CarStatus Status { get; set; }
    }

    enum CarStatus
    {
        NotWorking,
        Busy,
        Free
    }

    [Table]
    internal class DbOrderInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
        [Column]
        public long CustomerId { get; set; }
        [Column]
        public long RecieverId { get; set; }
        [Column]
        public long DriverId { get; set; }
        [Column]
        public string SourceAddress { get; set; }
        [Column]
        public string DestinationAddress { get; set; }
        [Column]
        public double SourceLocationLat { get; set; }
        [Column]
        public double SourceLocationLon { get; set; }
        [Column]
        public double DestinationLocationLat { get; set; }
        [Column]
        public double DestinationLocationLon { get; set; }
        [Column]
        public double Width { get; set; }
        [Column]
        public double Length { get; set; }
        [Column]
        public double Height { get; set; }
        [Column]
        public double Weight { get; set; }
        [Column]
        public OrderStatus Status { get; set; }
        [Column]
        public string MessageForReciever { get; set; }
        [Column]
        public string Description { get; set; }
        [Column]
        public DateTime CreatedStamp { get; set; }
        [Column]
        public DateTime ConfirmedStamp { get; set; }
        [Column]
        public DateTime AcceptedStamp { get; set; }
        [Column]
        public DateTime LoadingStamp { get; set; }
        [Column]
        public DateTime LoadedStamp { get; set; }
        [Column]
        public DateTime DeliveredStamp { get; set; }
        [Column]
        public DateTime FinishedStamp { get; set; }
        [Column]
        public DateTime CancelledStamp { get; set; }
    }

    public enum OrderStatus
    {
        Created = 0,
        Confirmed = 1,
        Accepted = 2,
        Loading = 3,
        InProgress = 4,
        Unloading = 5,
        Done = 6,
        Canceled = 100
    }

    public enum DbUserTokenKind
    {
        Activation,
        AccessRestore
    }
}
