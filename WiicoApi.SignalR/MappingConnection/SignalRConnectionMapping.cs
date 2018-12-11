using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WiicoApi.SignalR.MappingConnection
{
    /// <summary>
    /// 暫存連線
    /// </summary>
    /// <typeparam name="T">groupName</typeparam>
    public class SignalRConnectionMapping<T>
    {
        /// <summary>
        /// 儲存group與連線
        /// </summary>
        public readonly List<SignalRGroupModel> _groups = new List<SignalRGroupModel>();       
        /// <summary>
        /// 加入group 連線
        /// </summary>
        /// <param name="key">groupName</param>
        /// <param name="connectionId">連線者代碼</param>
        /// <param name="version">欲使用的版號，可NULL[代表最舊的版號]</param>
        public void Add(T key,string connectionId,int version){
            lock (_groups)
            {
                var oldGroup = new SignalRGroupModel();
                var newGroup = new SignalRGroupModel();
                var connectionList = new List<SignalRConnectionModel>();
                lock (connectionList)
                {
                    var addConnectionModel = new SignalRConnectionModel() { Connection = connectionId,Version=version};
                    oldGroup = _groups.Where(t => t.GroupName == key.ToString().ToLower()).FirstOrDefault();
                    newGroup  = _groups.Where(t => t.GroupName == key.ToString().ToLower()).FirstOrDefault();
         
                    if (oldGroup == null)
                    {
                        newGroup = new SignalRGroupModel();
                        newGroup.ConnectionList = new List<SignalRConnectionModel>();
                        newGroup.Connections = new List<string>();
                        newGroup.GroupName = key.ToString().ToLower();
                        newGroup.Versions = new List<int>();
                    }

                    if(newGroup.ConnectionList.FirstOrDefault(t=>t.Connection==connectionId)==null)
                        newGroup.ConnectionList.Add(addConnectionModel);

                    if(newGroup.Connections.FirstOrDefault(t=>t==connectionId)==null)
                        newGroup.Connections.Add(connectionId);

                    if (newGroup.Versions.Count() == 0)
                        newGroup.Versions.Add(version);
                    else if(!newGroup.Versions.Where(t=>t==version).Any())
                        newGroup.Versions.Add(version);

                    //再塞入新的資訊
                    _groups.Add(newGroup);
                    _groups.Remove(oldGroup);
                }
            }
        }
        /// <summary>
        /// 取得所屬group的連線群
        /// </summary>
        /// <param name="key">groupName</param>
        /// <returns></returns>
        public SignalRGroupModel GetConnections(T key) {
            return _groups.Where(t=>t.GroupName==key.ToString().ToLower()).FirstOrDefault();
        }
        /// <summary>
        /// 剔除連線者
        /// </summary>
        /// <param name="key">groupName</param>
        /// <param name="connectionId">連線者代碼</param>
        public void Remove(T key, string connectionId)
        {
            lock (_groups)
            {
                var oldGroup = _groups.Where(t => t.GroupName == key.ToString().ToLower()).FirstOrDefault();
                var newGroup = _groups.Where(t => t.GroupName == key.ToString().ToLower()).FirstOrDefault();

                //暫存connections列表
                var connectionVersions = new List<Dictionary<string, int>>();
                var connectionList = new List<SignalRConnectionModel>();
                var deleteConnectionModel = new SignalRConnectionModel();
                if (oldGroup == null)
                    return;
                else
                    connectionList = oldGroup.ConnectionList;

                lock (connectionList)
                {
                    var checkHasConnection = false;
                    foreach (var cnId in connectionList)
                    {
                            if (cnId.Connection == connectionId)
                            {
                                checkHasConnection = true;
                                deleteConnectionModel.Connection = cnId.Connection;
                                deleteConnectionModel.Version = cnId.Version;
                            }
                    }
                    //確定有找到欲刪除的connection
                    if (checkHasConnection)
                    {
                        newGroup.Connections.Remove(connectionId);
                        newGroup.ConnectionList.Remove(deleteConnectionModel);
                        newGroup.GroupName = key.ToString().ToLower();
                        //補回group資訊
                        _groups.Add(newGroup);
                        _groups.Remove(oldGroup);
                    }
                }
            }
        }
    }
}