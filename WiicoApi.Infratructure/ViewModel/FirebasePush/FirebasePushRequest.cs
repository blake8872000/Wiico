using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    public class FirebasePushRequest
    {
        public string[] registration_ids { get; set; }
        public string to { get; set; }
        public FirebaseNotification notification { get; set; }
        public JObject  data { get; set; }
        public FirebaseAndroid android { get; set; }

        public Firebaseios apns { get; set; }

        public FirebaseWebPush webpush { get; set; }
    }
}
