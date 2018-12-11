using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class ReviewDataModel
    {
        [JsonProperty("signInOuterKey")]
        public string SignInOuterKey { get; set; }
        [JsonProperty("signInDateTime")]
        public DateTime SignInDateTime { get; set; }
    }
}
