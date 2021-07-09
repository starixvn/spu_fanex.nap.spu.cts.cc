using System;
using System.Text.Json.Serialization;

namespace Fanex.NAP.SPU.CTS.Features.Transform.Accounts.Entities
{
    public class AccountEntity
    {
        private DateTime lastLoginTime;

        [JsonIgnore]
        public int BatchId { get; set; }

        public int AccountId { get; set; }

        public string LoginName { get; set; }

        public int SubscriberId { get; set; }

        public DateTime LastLoginTime
        {
            get
            {
                return Convert.ToDateTime(lastLoginTime.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
            }

            set
            {
                lastLoginTime = value;
            }
        }
    }
}
