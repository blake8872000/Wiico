using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class UploadEvent
    {
        /// <summary>
        /// 活動id
        /// </summary>
        [JsonProperty("outerKey")]
        public Guid OuterKey { get; set; }

        /// <summary>
        /// 建立帳號
        /// </summary>
        [JsonProperty("createAccount")]
        public string CreateAccount { get; set; }

        /// <summary>
        /// 課id
        /// </summary>
        [JsonProperty("classId")]
        public string ClassId { get; set; }

        /// <summary>
        /// 建立者id
        /// </summary>
        [JsonProperty("creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 遲交日期
        /// </summary>
        [JsonProperty("overdueDate")]
        public DateTime? OverdueDate { get; set; }

        Dictionary<string, int> _uploadSheet;
        /// <summary>
        /// 上傳表 {學生id, 上傳狀態}
        /// </summary>
        public Dictionary<string, int> UploadSheet
        {
            get
            {
                if (_uploadSheet == null)
                    _uploadSheet = new Dictionary<string, int>();
                return _uploadSheet;
            }
            set { _uploadSheet = value; }
        }
    }
}
