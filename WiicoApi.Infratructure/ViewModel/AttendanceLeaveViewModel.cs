using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class AttendanceLeaveViewModel
    {
        /// <summary>
        /// 顯示要回去的學習圈learningId
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId { get; set; }

        /// <summary>
        /// 是否可以審核請假單
        /// </summary>
        [JsonProperty("canReview")]
        public bool CanReview { get; set; }

        /// <summary>
        /// 是否可以新增請假單
        /// </summary>
        [JsonProperty("canCreate")]
        public bool CanCreate { get; set; }

        /// <summary>
        /// 列表資料
        /// </summary>
        [JsonProperty("list")]
        public ValueObject.AttendanceLeaveViewData[] List { get; set; }
    }
}
