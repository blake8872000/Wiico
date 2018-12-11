using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 分組-第二層組別
    /// </summary>
    public class ActGroup : Base.EntityBase
    {
        /// <summary>
        /// 分組主表代碼
        /// </summary>
        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [JsonProperty("sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 作為signalR的呼叫GroupId
        /// </summary>
        public string GroupId { get; set; }


    }
}
