using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WiicoApi.SignalRHub
{
    /// <summary>
    /// 發通知
    /// </summary>
    public class SignalrClientHelper
    {
        private static readonly IHubContext objHub = GlobalHost.ConnectionManager.GetHubContext<WiicoHubBase>();

        /// <summary>
        /// 依照 ConnectID 推播相關訊息
        /// </summary>
        /// <param name="connectIdAndData"></param>
        public static void ShowRecordListById(Dictionary<List<string>, dynamic> connectIdAndData)
        {
            // 所有歸屬在這個id下的connection都會收到
            foreach (KeyValuePair<List<string>, dynamic> item in connectIdAndData)
            {
                foreach (var connectId in item.Key)
                {
                    objHub.Clients.Client(connectId).showRecordList(item.Value);
                }
            }
        }

        /// <summary>
        /// 廣播發送訊息
        /// </summary>
        /// <param name="connectIdAndData"></param>
        public static void SendNotice(Dictionary<List<string>, dynamic> connectIdAndData)
        {

            // 所有歸屬在這個id下的connection都會收到
            foreach (KeyValuePair<List<string>, dynamic> item in connectIdAndData)
            {
                foreach (var connectId in item.Key)
                {
                    objHub.Clients.Client(connectId).showNoticeList(item.Value);
                    objHub.Clients.Client(connectId).appendNotice(item.Value);
                }
            }
        }

        public static void onError(string connectId, string type, string message)
        {
            objHub.Clients.Client(connectId).onError(type, message);
        }

        /// <summary>
        /// 依照 Group 推播主題討論活動
        /// </summary>
        /// <param name="data"></param>
        /// <param name="circleKey"></param>
        public static void AppendActivityByGroup(string circleKey, dynamic data)
        {
            objHub.Clients.Group(circleKey.ToLower()).appendActivity(data, "");
        }

        public static void AppendActivityByGroup(string circleKey, dynamic data, string key)
        {
            objHub.Clients.Group(circleKey.ToLower()).appendActivity(data, key.ToLower());
        }

        public static void UpdateDetail(string circleKey, string type, dynamic data)
        {
            objHub.Clients.Group(circleKey.ToLower()).updateDetail("discussion", data);
        }

        /// <summary>
        /// 上傳作業
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="outerKey"></param>
        /// <param name="record"></param>
        public static void ShowUploadCountByGroup(string circleKey, string outerKey, Infrastructure.BusinessObject.StuHomeworkRecord record)
        {
            objHub.Clients.Group(circleKey.ToLower()).showuploadcount(record.TotalCount, record.EventLogId, outerKey, 1, record.HomeworkRecord, record.HomeworkRecordByPerson);
        }

        public static void AppendComment(string circleKey, dynamic newMsg)
        {
            objHub.Clients.Group(circleKey.ToLower()).appendComment(newMsg);

        }

        /// <summary>
        /// 刪除活動卡
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="moduleType"></param>
        /// <param name="eventId"></param>
        public static void DeleteActivity(string circleKey, string moduleType, Guid eventId)
        {
            objHub.Clients.Group(circleKey.ToLower()).deleteActivity(moduleType, Service.Utility.OuterKeyHelper.GuidToPageToken(eventId));

        }

        /// <summary>
        /// 重新載入活動卡
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="moduleType"></param>
        /// <param name="updateResult"></param>
        public static void RenderDetails(string circleKey, string moduleType, dynamic updateResult)
        {
            objHub.Clients.Group(circleKey.ToLower()).renderDetails(moduleType, updateResult);
        }

        public static void AddLeave(string circleKey, bool check)
        {
            objHub.Clients.Group(circleKey.ToLower()).addLeave(check);
        }

        public static void SignIn_StatusChanged(string circleKey, string outerKey, Infrastructure.ValueObject.SignInLog signInLog)
        {
            objHub.Clients.Group(circleKey.ToLower()).signIn_StatusChanged(outerKey, signInLog);
        }
    }
}