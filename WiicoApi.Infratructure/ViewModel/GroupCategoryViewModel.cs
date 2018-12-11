using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    /// <summary>
    /// 分組關聯表
    /// </summary>
    public class GroupCategoryViewModel
    {
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }

        /// <summary>
        /// 人員登入代碼
        /// </summary>
        [JsonProperty("token")]
        public Guid Token { get; set; }

        /// <summary>
        /// 分組標題 - ActGroupCategory
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 分組描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 分組編譯後代碼
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }

        /// <summary>
        /// 分組代碼
        /// </summary>
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        /// <summary>
        /// 要顯示的頁數
        /// </summary>
        [JsonIgnore]
        public int? Pages { get; set; }
        /// <summary>
        /// 要顯示的數量
        /// </summary>
        [JsonIgnore]
        public int? Rows { get; set; }


        /// <summary>
        /// 組別列表詳細資訊
        /// </summary>
        [JsonProperty("groups")]
        public List<GroupInfoViewModel> Groups { get; set; }
    }
}
