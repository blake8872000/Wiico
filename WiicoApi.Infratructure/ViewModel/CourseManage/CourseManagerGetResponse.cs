using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class CourseManagerGetResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("resType")]
        public int ResType { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
