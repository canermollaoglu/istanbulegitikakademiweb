using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.MongoOptions
{
    public static class MongoCollectionNames
    {
        public const string TransactionLog = "ut_log";
        public const string ExceptionLog = "exception_log";
        public const string BlogViewLog = "blog_view_log";
        public const string CampaignLog = "campaign_log";
    }
}
