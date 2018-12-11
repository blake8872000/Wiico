using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class LearningCircleManageViewModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 學年期
        /// </summary>
        [JsonProperty("section")]
        public string Section { get; set; }

        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("learningOuterKey")]
        public string LearningOuterKey { get; set; }
        /// <summary>
        /// 學習圈描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public string Enable { get; set; }
        /// <summary>
        /// 開始日期
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 組織編號
        /// </summary>
        [JsonProperty("orgId")]
        public string OrgId { get; set; }

        /// <summary>
        /// 上課地點
        /// </summary>
        [JsonProperty("place")]
        public string Place { get; set; }

        /// <summary>
        /// 上課星期數 - [0:周日 , 1:周一 ...以此類推]
        /// </summary>
        [JsonProperty("weeks")]
        public List<int> Weeks { get; set; }
    }
}
