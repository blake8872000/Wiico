using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class ActivitySyllabusResponseData
    {
        /// <summary>
        /// 活動代碼 
        /// </summary>
        [JsonIgnore]
        public Guid EventId { get; set; }
        /// <summary>
        /// 活動代碼 - client查詢用
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
        /// <summary>
        /// 顯示標題
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// 所屬活動類型
        /// </summary>
        [JsonProperty("moduleKey")]
        public string ModuleKey { get; set; }
    }
}
