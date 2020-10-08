using Carblam.Service.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carblam.Service
{
    interface ICarblamServiceContext
    {
        CarblamServiceConfiguration Configuration { get; }

        string DbConnectionString { get; }
        ICarblamSessionsManager SessionsManager { get; }
        ISecureRandom SecureRandom { get; }

        ICarblamDbContext OpenDb();
        void SendMail(string targetEmail, string subject, string text);
        byte[] DownloadContent(string resourceUrl);

        IBasicOperationContext OpenLocalContext();
        ICarblamRequestContext OpenWebRequestContext();

        DateTime UtcNow { get; }
    }

    interface IWordNormalizer : IDisposable
    {
        string NormalizeWord(string word);
    }

    interface ICarblamSessionsManager : IDisposable
    {
        TimeSpan SessionCleanupTimeout { get; set; }
        
        ICarblamSessionContext CreateSession();
        
        ICarblamSessionContext GetSession(Guid id);
        bool TryGetSession(Guid sessionId, out ICarblamSessionContext session);
     
        void DeleteSession(Guid sessionId);
        void DropUserSessions(long userId);
        void CleanupSessions();
    }

    interface ISecureRandom : IDisposable
    {
        int Next(int minValue, int maxExclusiveValue);
        byte[] GenerateRandomBytes(int bytesNumber);
    }

    interface IBasicOperationContext : IDisposable
    {
        ICarblamDbContext Db { get; }
    }

    interface ICarblamRequestContext : IBasicOperationContext
    {
        string RequestHostName { get; }
        ICarblamSessionContext Session { get; }
        void ValidateAuthorized();
        void ValidateAuthorized(bool requireActivated = true);
    }

    interface ICarblamSessionContext
    {
        Guid Id { get; }
        DateTime LastActivity { get; }
        long UserId { get; }
        bool IsActivated { get; }

        event Action<long> OnUserContextChanging;

        void Renew();
        void SetUserContext(DbUserInfo user);
    }
}
