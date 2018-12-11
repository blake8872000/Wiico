using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    public class FirebasePushResponse
    {
        public Int64 multicast_id { get; set; }

        public bool success { get; set; }

        public bool failure { get; set; }
        public int canonical_ids { get; set; }

        public JArray results { get; set; }
     
    }
}
