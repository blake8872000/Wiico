using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 錯誤訊息紀錄表
    /// </summary>
    public class SystemErrorLog
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 錯誤類型編號
        /// </summary>
        [JsonProperty("errorType")]
        public int ErrorType { get; set; }
        /// <summary>
        /// 錯誤描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        ///  發生日期
        /// </summary>
        [JsonProperty("createUtcDate")]
        public DateTime CreateUtcDate { get; set; }
        /// <summary>
        /// 是否修正
        /// </summary>
        [JsonProperty("isFix")]
        public bool IsFix { get; set; }
    }
}
