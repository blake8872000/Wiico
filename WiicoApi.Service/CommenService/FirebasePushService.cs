using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.FirebasePush;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    public class FirebasePushService
    {
        private readonly GenericUnitOfWork _uow;
        private string firebasePushUrl = ConfigurationManager.AppSettings["firebasePushUrl"].ToString();
        private string firebasePushApiKey = ConfigurationManager.AppSettings["firebasePushApiKey"].ToString();
        private string firebasePushSenderId = ConfigurationManager.AppSettings["firebasePushSenderId"].ToString();
        private string firebasePushSound = ConfigurationManager.AppSettings["firebaseIosPushSound"].ToString();
        public FirebasePushService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 發送多個Device推播
        /// </summary>
        /// <param name="json">要傳送的data資訊</param>
        /// <param name="circleKey">所屬學習圈</param>
        /// <param name="accounts">要傳送的對象</param>
        /// <param name="pushDataId">要推送的資訊</param>
        /// <param name="pushMsg">要推播顯示在手機上的訊息</param>
        /// <param name="publishDate">發布日期</param>
        /// <param name="deviceIdList">測試用</param>
        /// <returns></returns>
        public bool SendMutiplePushNotification(string json, string circleKey, List<string> accounts, int pushDataId, string pushMsg, DateTime? publishDate = null, List<string> deviceIdList = null)
        {
            var db = _uow.DbContext;
            try
            {
                if (deviceIdList == null)
                {
                    var userTokens = (accounts != null && accounts.Count() > 0) ?
                                           (from ut in db.UserToken
                                            join m in db.Members on ut.MemberId equals m.Id
                                            join a in accounts on m.Account equals a
                                            orderby ut.Id descending
                                            group ut.MemberId by new { ut } into g
                                            where g.Key.ut.PushToken != null && g.Key.ut.PushToken != "" && g.Key.ut.PushToken != string.Empty
                                            select g.Key.ut
                                          ).ToList() :
                                       (from ut in db.UserToken
                                        join cmr in db.CircleMemberRoleplay on ut.MemberId equals cmr.MemberId
                                        join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                                        where lc.LearningOuterKey == circleKey && ut.PushToken != null && ut.PushToken != "" && ut.PushToken != string.Empty
                                        orderby ut.Id descending
                                        group ut.MemberId by new { ut } into g
                                        select g.Key.ut
                                         ).ToList();
                    var iosDevices = userTokens.Where(t => t.RequestSystem.ToLower().StartsWith("ios")).ToList();
                    var otherDevice = userTokens.SkipWhile(t => t.RequestSystem.ToLower().StartsWith("ios")).ToList();
                    //為了針對不同device計算ios badge ,才使用foreach
                    foreach (var iosDevice in iosDevices)
                    {
                        var iosDeviceBadge = db.PushLog.Where(t => t.DeviceId.ToLower() == iosDevice.PushToken.ToLower() && t.Enable == false).Count();
                        SendPushNotification(json, new string[1] { iosDevice.PushToken }, pushDataId, pushMsg, publishDate, true, iosDeviceBadge);
                    }
                    //推送android&&web訊息
                    var response = SendPushNotification(json, otherDevice.Select(t => t.PushToken).ToArray(), pushDataId, pushMsg, publishDate);
                }
                else //測試用
                    SendPushNotification(json, deviceIdList.ToArray(), pushDataId, pushMsg, publishDate);

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        /// <summary>
        /// 發送推播資訊
        /// </summary>
        public bool SendPushNotification(string json, string[] deviceIds, int pushDataId, string pushMsg, DateTime? publishDate = null, bool? isIOS = false, int? badge = 1)
        {
            var db = _uow.DbContext;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //charset=UTF-8
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + firebasePushApiKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sender", "id=" + firebasePushSenderId);
                var utf8json = Encoding.UTF8.GetString(Encoding.Default.GetBytes(json));
                var pushJsonObject = JObject.Parse(json);
                /*var pushiosJson = "{\"aps\":{\"alert\":{" + string.Format("\"title\":{0},\"body\":{0}", json) + "}},badge: 42,}";
                var iosPushJson = JObject.Parse(pushiosJson);*/
                var data = new FirebasePushRequest()
                {
                    registration_ids = deviceIds,
                    data = pushJsonObject
                };
                if (isIOS.Value)
                {
                    data.notification = new FirebaseNotification()
                    {
                        body = pushMsg,
                        badge = badge.Value,
                        sound = firebasePushSound
                    };
                }
                if (publishDate.HasValue)
                {
                    var ttl = Convert.ToInt32(publishDate.Value.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds) * 1000;
                    data.android.ttl = string.Format("{0}s", ttl);
                    data.apns.headers = new Dictionary<string, string>();
                    data.apns.headers.Add("apns-expiration", ttl.ToString());
                    data.webpush.headers = new Dictionary<string, string>();
                    data.webpush.headers.Add("TTL", ttl.ToString());
                }
                var firebaseJson = JsonConvert.SerializeObject(data);
                var request = new HttpRequestMessage(HttpMethod.Post, firebasePushUrl);
                request.Content = new StringContent(firebaseJson,
                                                    Encoding.UTF8,
                                                    "application/json");
                var response = httpClient.SendAsync(request);
                var responseEntity = JsonConvert.DeserializeObject<FirebasePushResponse>(response.Result.Content.ReadAsStringAsync().Result);
                if (responseEntity.success)
                    CreateMutiplePushLog(pushDataId, deviceIds.ToList(), publishDate); //建立log
                return true;
            }
        }



        /// <summary>
        /// 建立推播資訊
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="systemId"></param>
        /// <param name="gaEvent"></param>
        /// <param name="circleKey"></param>
        /// <param name="eventOuterKey"></param>
        /// <returns></returns>
        public PushData CreatePushData(string title, string content, string systemId, string gaEvent, string circleKey, string eventOuterKey)
        {
            var db = _uow.DbContext;
            try
            {
                var pushData = new PushData()
                {
                    SystemId = systemId,
                    CircleKey = circleKey,
                    Content = content,
                    EventOuterKey = eventOuterKey,
                    GaEvent = gaEvent,
                    Title = title
                };
                db.PushData.Add(pushData);
                db.SaveChanges();
                return pushData;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }
        /// <summary>
        /// 建立log
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="deviceid"></param>
        /// <param name="publishDate"></param>
        /// <returns></returns>
        public PushLog CreatePushLog(int pushDataId, string deviceid, DateTime? publishDate = null)
        {
            var db = _uow.DbContext;
            try
            {
                var pushLog = new PushLog()
                {
                    PushDataId = pushDataId,
                    CreateDate = DateTime.UtcNow,
                    DeviceId = deviceid
                };
                if (publishDate.HasValue)
                    pushLog.PublishDate = publishDate.Value.ToUniversalTime();

                db.PushLog.Add(pushLog);

                return pushLog;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }
        /// <summary>
        /// 建立log
        /// </summary>
        /// <param name="pushDataId"></param>
        /// <param name="deviceid"></param>
        /// <param name="publishDate"></param>
        /// <returns></returns>
        public bool CreateMutiplePushLog(int pushDataId, List<string> deviceids, DateTime? publishDate = null)
        {
            var db = _uow.DbContext;
            try
            {
                foreach (var deviceid in deviceids)
                {
                    var pushLog = new PushLog()
                    {
                        PushDataId = pushDataId,
                        CreateDate = DateTime.UtcNow,
                        DeviceId = deviceid,
                        Enable = false
                    };
                    if (publishDate.HasValue)
                        pushLog.PublishDate = publishDate.Value.ToUniversalTime();

                    db.PushLog.Add(pushLog);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
    }
}
