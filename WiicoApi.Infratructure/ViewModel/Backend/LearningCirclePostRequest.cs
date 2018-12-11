using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 建立學習圈參數
    /// </summary>
    public class LearningCirclePostRequest : BackendBaseRequest
    {
        /// <summary>
        /// 學習圈名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 學習圈說明
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }


        /// <summary>
        /// 學習圈開始日期
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 學習圈結束日期
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 上課地點
        /// </summary>
        [JsonProperty("place")]
        public string Place { get; set; }
        /// <summary>
        /// 傳統課表日 [0:週日 | 1:周一 | 2:周二 | 3:週三 | 4:周四 | 5:周五 | 6:周六]
        /// </summary>
        [JsonProperty("weeks")]
        public List<int> Weeks { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// 要修改的上課方式
        /// </summary>
        [JsonProperty("classWeekType")]
        public int ClassWeekType { get; set; }

        /// <summary>
        /// 組織編號
        /// </summary>
        [JsonProperty("orgId")]
        public int? OrgId { get; set; }

        /// <summary>
        /// 目標
        /// </summary>
        [JsonProperty("objective")]
        public string Objective { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        [JsonProperty("remark")]
        public string Remark { get; set; }
    }
}
