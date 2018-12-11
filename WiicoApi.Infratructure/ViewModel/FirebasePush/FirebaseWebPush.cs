using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    public class FirebaseWebPush
    {
        /// <summary>
        /// https://tools.ietf.org/html/rfc8030#section-5
        /// </summary>
        public Dictionary<string,string> headers { get; set; }

        public JObject data { get; set; }

        public FirebaseWebNotification notification { get; set; }

    }
}
