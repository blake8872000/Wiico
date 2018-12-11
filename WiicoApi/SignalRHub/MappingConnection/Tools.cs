using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WiicoApi.SignalR.MappingConnection
{
    /// <summary>
    /// 處理SignalRConnection的工具
    /// </summary>
    public class Tools
    {

        /// <summary>
        /// 處理連線列表 - 組成一組相同版號的connecitons列表
        /// </summary>
        public static List<string> ConnectionsProcess(SignalRGroupModel groups, int connectionVersion)
        {
            var result = new List<string>();
            //查出version的connections
            var tempConnections = groups.ConnectionList.Where(t => t.Version == connectionVersion);
            //根據版號塞人
            foreach (var conneciton in tempConnections)
                result.Add(conneciton.Connection);

            return result;
        }
    }
}