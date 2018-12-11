using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class CourseManagerDeleteRequest : Infrastructure.ViewModel.Base.BackendBaseRequest
    {
        [JsonProperty("accounts")]
        public List<string> Accounts { get; set; }
    }
}
