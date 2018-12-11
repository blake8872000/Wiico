using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WiicoApi.SignalR.MappingConnection
{
    /// <summary>
    /// SignalRConnection資訊
    /// </summary>
    public class SignalRConnectionModel
    {
        /// <summary>
        /// 連線代碼
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// 欲使用的版號
        /// </summary>
        public int Version { get; set; }
    }
}