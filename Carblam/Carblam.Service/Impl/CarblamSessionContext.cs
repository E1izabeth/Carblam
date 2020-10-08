using Carblam.Service.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carblam.Service.Impl
{
    class CarblamSessionContext : ICarblamSessionContext
    {
        public LinkedListNode<CarblamSessionContext> ListNode { get; private set; }

        public Guid Id { get; private set; }
        public DateTime LastActivity { get; private set; }
        public long UserId { get; private set; }
        public bool IsActivated { get; private set; }

        public event Action<long> OnUserContextChanging = delegate { };

        public CarblamSessionContext()
        {
            this.Id = Guid.NewGuid();
            this.Renew();

            this.ListNode = new LinkedListNode<CarblamSessionContext>(this);
        }

        public void Renew()
        {
            this.LastActivity = DateTime.UtcNow;
        }

        public void SetUserContext(DbUserInfo user)
        {
            var userId = user?.Id ?? 0;
            this.OnUserContextChanging(userId);
            this.UserId = userId;

            this.IsActivated = user?.Activated ?? false;
        }
    }
}
