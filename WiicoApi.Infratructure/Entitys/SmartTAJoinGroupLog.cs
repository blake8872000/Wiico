using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// SmartTA joinGroup 的紀錄
    /// </summary>
    public class SmartTAJoinGroupLog
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        ///  SmartTA的代號-帳號
        /// </summary>
        [JsonProperty("smartTAName")]
        public string SmartTAName { get; set; }

        /// <summary>
        ///  SmartTA 在signalR的connectionid
        /// </summary>
        [JsonProperty("connectionId")]
        public string ConnectionId{ get; set; }

        /// <summary>
        ///  SmartTA 目前所joinGroup的學習圈
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }

        /// <summary>
        ///  建立日期
        /// </summary>
        [JsonProperty("createUtcDate")]
        public DateTime CreateUtcDate { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}
