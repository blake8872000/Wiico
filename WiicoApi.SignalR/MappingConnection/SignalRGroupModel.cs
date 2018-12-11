using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WiicoApi.SignalR.MappingConnection
{
    /// <summary>
    /// 屬於SignalR的Group物件
    /// </summary>
    public class SignalRGroupModel
    {
        /// <summary>
        /// gorup名稱
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 連線物件列表
        /// </summary>
        public List<SignalRConnectionModel> ConnectionList { get; set; }

        /// <summary>
        /// 連線列表
        /// </summary>
        public List<string> Connections{ get; set; }

        /// <summary>
        /// 版號列表 - 方便處理不同版號的結果
        /// </summary>
        public List<int> Versions { get; set; }
    }
}