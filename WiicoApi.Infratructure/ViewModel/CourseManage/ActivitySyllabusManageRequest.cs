using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class ActivitySyllabusManageRequest : Base.BackendBaseRequest
    {

        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
        [JsonProperty("syllabusId")]
        public int SyllabusId { get; set; }
    }
}
