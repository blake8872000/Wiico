using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.SignalR.MappingConnection;

namespace WiicoApi.SignalRHub
{

    public partial class WiicoHubBase : Hub
    {

        /// <summary>
        /// 用於管理目前線上group的連線
        /// </summary>
        public readonly SignalRConnectionMapping<string> _connections = new SignalRConnectionMapping<string>();

        private string testAccessKey = ConfigurationManager.AppSettings["MQTTTestAccessKey"].ToString();
        readonly string connCacheName = "ConnectionData";
        public string accessKey = ConfigurationManager.AppSettings["MQTTAccessKey"].ToString();
        public string iotUrl = ConfigurationManager.AppSettings["IOTUrl"].ToString();
        public string mqttConnectUrl = ConfigurationManager.AppSettings["ConnectMQTTUrl"].ToString();
        public string motionAccessKey = ConfigurationManager.AppSettings["MQTTMotionAccessKey"].ToString();
        private MqttClient mqttClient;
        public string mqttCircleKey;
        public TokenService tokenService = new TokenService();
        /// <summary>
        /// 驗證token資訊
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Infrastructure.Entity.UserToken CheckToken(string token)
        {
            var response = tokenService.GetTokenInfo(token).Result;
            if (response == null)
                return null;

            return response;
        }

        /// <summary>
        /// 建立SignalR連線
        /// </summary>
        /// <param name="token"></param>
        public BaseResponse<string> connection(string token)
        {
            string error = string.Empty;
            var response = new BaseResponse<string>();
            var version = (Context.QueryString!=null) ? Context.QueryString["version"] : "1"; 
            try
            {
                var memberService = new MemberService();
                var memberInfo = memberService.TokenToMember(token).Result;
                //更新connectionId
                memberService.SetConnectionId(Context.ConnectionId, memberInfo.Id);
                var isMember = CheckToken(token);
                if (isMember != null)
                {
                    // 紀錄使用者 memberId 與 connectionId 關聯，即使未進入學習圈也可以收到通知
                    response.Success = true;
                    response.State = LogState.Suscess;
                    response.Message = "成功登入" + token + "]";
                    Clients.Caller.showversion("目前version:" + version);
                    Clients.Caller.onSysConnected(token, Context.ConnectionId);
                    SetConnData(isMember.MemberId.ToString());
                }
                else
                {
                    response.Message = "身分驗證失敗，請重新登入! token:[" + token + "]";
                    Clients.Caller.onError("Connection", "身分驗證失敗，請重新登入! token:[" + token + "]");
                }
                return response;
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("Connection", "連接發生意外: " + ex.Message + error);
                response.Message = ex.Message;
                return response;
            }
        }

        /// <summary>
        /// 加入學習圈
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        public BaseResponse<JObject> joinGroup(string circleKey, Guid token)
        {
            var result = new BaseResponse<JObject>();
            string error = string.Empty;
            string groupKey = circleKey.ToLower();
            var version = 0;
            if (Context.QueryString != null && !string.IsNullOrEmpty(Context.QueryString["version"]))
                version = Convert.ToInt32(Context.QueryString["version"]);
            //塞人至相對應的group
            _connections.Add(groupKey, Context.ConnectionId, version);
            try
            {
                var memberService = new MemberService();
                var memberInfo = memberService.TokenToMember(token.ToString()).Result;
                if (memberInfo == null)
                    return null;

                //如果是smartTA登入
                if (memberInfo.RoleName == "3") {
                    var smartTAService = new Service.Backend.SmartTAService();
                    //先把學習圈內的smartTA都踢掉
                    var smartTAs = smartTAService.GetSmartTAs(groupKey);
                    if (smartTAs != null) {
                        foreach (var smartTA in smartTAs) {
                            Groups.Remove(smartTA.ConnectionId, groupKey);
                        }
                    }
                    var setSmartTAValue = new SmartTAPostRequest() {
                        CircleKeys = new List<string>(),
                        ClassRoomId = memberInfo.Account
                    };
                    setSmartTAValue.CircleKeys.Add(groupKey);
                    //建立smartTA關聯
                    var setSmartTA = smartTAService.InsertRelation(setSmartTAValue);
                }
                var learningCircleService = new LearningCircleService();
                var learningCircleInfo = learningCircleService.GetDetailByOuterKey(groupKey);
                var activityService = new ActivityService();
                var authList = activityService.CircleAuth(memberInfo.Id, learningCircleInfo.Id);
                // 將 連接ID 與 群組代碼 放入SignalR的Groups，之後就能透過Groups(群組代碼)將訊息廣播給使用者
                error = "Groups.Add";
                Groups.Add(Context.ConnectionId, groupKey);
                mqttCircleKey = groupKey;
                // 呼叫前端的Method(發送連接成功的Request)
                error = "joinGroup Clients.Caller.onConnected token:[" + token + "]";
                Clients.Caller.onConnected(groupKey, token, Context.ConnectionId, authList);
                result.Data = authList;
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("joinGroup", "連接學習圈發生意外: token:[" + token + "]" + ex.Message + error);
                result.Success = false;
                return result;
            }
        }

        /// <summary>
        /// 離開學習圈 - 只根據學習圈代碼
        /// </summary>
        /// <param name="circleKey"></param>
        public BaseResponse<string> LeaveGroup(string circleKey)
        {
            var groupKey = circleKey.ToLower();
            var result = new BaseResponse<string>();
            try
            {
                var connId = Context.ConnectionId;

                Groups.Remove(connId, groupKey.ToLower());
                //_connections.Remove(circleKey, connId);
                result.Success = true;
                Clients.Caller.leaveChatroomCompleted(groupKey);
                //var connections = _connections.GetConnections(circleKey.ToLower());
                //if (connections.Connections.Count <= 0)
                //    unSubscribe();
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                Clients.Caller.onError("LeaveGroup", " 退出學習圈發生意外: circleKey:[" + groupKey + "]" + ex.Message);
                return result;
            }
        }
        /// <summary>
        ///  離開學習圈 - ban某個連線狀態
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="connId"></param>
        public void LeaveGroup(string circleKey, string connId)
        {
            var groupKey = circleKey.ToLower();
            try
            {

                /* var connId = Context.ConnectionId;*/
                Groups.Remove(connId, groupKey);
                Clients.Caller.leaveChatroomCompleted(groupKey);
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("LeaveGroup", " 退出學習圈發生意外: circleKey:[" + groupKey + "]" + ex.Message);
            }
        }

        /// <summary>
        /// 連接紀錄
        /// </summary>
        /// <param name="memberId"></param>
        private void SetConnData(string memberId)
        {
            var connId = Context.ConnectionId;
            #region // conn 與 member 的關係

            if (HttpContext.Current == null)//for unitTest
                return;
            var conn = HttpContext.Current.Cache.Get(connCacheName) as Dictionary<string, string>;
            if (conn == null)
            {
                conn = new Dictionary<string, string>();
                conn.Add(connId, memberId);
                HttpContext.Current.Cache.Insert(connCacheName, conn, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }
            else
            {
                if (!conn.ContainsKey(connId))
                {
                    conn.Add(connId, memberId);
                    HttpContext.Current.Cache[connCacheName] = conn;
                }
            }
            #endregion
            #region // member 與 conn 的關係
            var myConn = HttpContext.Current.Cache.Get(memberId) as List<string>;
            if (myConn == null)
            {
                myConn = new List<string>();
                myConn.Add(connId);
                HttpContext.Current.Cache.Insert(memberId, myConn, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }
            else
            {
                if (!myConn.Contains(connId))
                {
                    myConn.Add(connId);
                    HttpContext.Current.Cache[memberId] = myConn;
                }
            }
            #endregion
        }

        /// <summary>
        /// mqtt連線
        /// </summary>
        private void mqttConnection()
        {
            mqttClient = new MqttClient(mqttConnectUrl);
            mqttClient.Connect("startMQTTConnection", accessKey, accessKey);
            var qoslevels = new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            //投影機螢幕
            var screenDevice = "/v1/device/5418391279/sensor/screen/rawdata";
            //冷氣機開關
            var airCondictioningIODevice = "/v1/device/5418391279/sensor/do1/rawdata";
            //冷氣機模式
            var airCondictioningModelDevice = "/v1/device/5418391279/sensor/ao1/rawdata";
            //冷氣機溫度
            var airCondictioningtemperatureDevice = "/v1/device/5418391279/sensor/ao2/rawdata";
            //冷氣機風量
            var airCondictioningWindDevice = "/v1/device/5418391279/sensor/ao3/rawdata";
            //投影機開關
            var projectorIODevice = "/v1/device/5418391279/sensor/projector/rawdata";
            //電燈1開關
            var lighting1IODevice = "/v1/device/5418391279/sensor/lighting1/rawdata";
            //電燈2開關
            var lighting2IODevice = "/v1/device/5418391279/sensor/lighting2/rawdata";
            //氣氛燈開關
            var lamp1IODevice = "/v1/device/5418391279/sensor/lamp1/rawdata";
            //情境控制
            var scenarioControlDevice = "/v1/device/5418391279/sensor/scenarioControl/rawdata";
            //訂閱儀器
            mqttClient.Subscribe(new string[] { screenDevice }, qoslevels);
            mqttClient.Subscribe(new string[] { airCondictioningIODevice }, qoslevels);
            mqttClient.Subscribe(new string[] { projectorIODevice }, qoslevels);
            mqttClient.Subscribe(new string[] { lighting1IODevice }, qoslevels);
            mqttClient.Subscribe(new string[] { lighting2IODevice }, qoslevels);
            mqttClient.Subscribe(new string[] { lamp1IODevice }, qoslevels);
            mqttClient.Subscribe(new string[] { airCondictioningModelDevice }, qoslevels);
            mqttClient.Subscribe(new string[] { airCondictioningtemperatureDevice }, qoslevels);
            mqttClient.Subscribe(new string[] { airCondictioningWindDevice }, qoslevels);
            mqttClient.Subscribe(new string[] { scenarioControlDevice }, qoslevels);
            //監聽
            mqttClient.MqttMsgPublishReceived += MqttClientReceived;
        }

        private void MqttClientReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var apiResponse = Encoding.UTF8.GetString(e.Message);
            var now = DateTime.Now;
            var sensorStatus = JsonConvert.DeserializeObject<Infrastructure.ViewModel.MQTT.IOTSensorResponse>(apiResponse);
            var responseTime = Convert.ToDateTime(sensorStatus.Time).ToString("yyyy-MM-ddTHH:mm:ss");
            var responseValue = sensorStatus.Value.Length > 0 ? sensorStatus.Value[0] : "0";
            Clients.All.mqttresponse(responseTime, sensorStatus.Id, responseValue);
        }

        /// <summary>
        /// 取消訂閱+中斷連線
        /// </summary>
        private void unSubscribe()
        {
            //投影機螢幕
            var screenDevice = "/v1/device/5418391279/sensor/screen/rawdata";
            //冷氣機開關
            var airCondictioningIODevice = "/v1/device/5418391279/sensor/do1/rawdata";
            //冷氣機模式
            var airCondictioningModelDevice = "/v1/device/5418391279/sensor/ao1/rawdata";
            //冷氣機溫度
            var airCondictioningtemperatureDevice = "/v1/device/5418391279/sensor/ao2/rawdata";
            //冷氣機風量
            var airCondictioningWindDevice = "/v1/device/5418391279/sensor/ao3/rawdata";
            //投影機開關
            var projectorIODevice = "/v1/device/5418391279/sensor/projector/rawdata";
            //電燈1開關
            var lighting1IODevice = "/v1/device/5418391279/sensor/lighting1/rawdata";
            //電燈2開關
            var lighting2IODevice = "/v1/device/5418391279/sensor/lighting2/rawdata";
            //氣氛燈開關
            var lamp1IODevice = "/v1/device/5418391279/sensor/lamp1/rawdata";
            //情境控制
            var scenarioControlDevice = "/v1/device/5418391279/sensor/scenarioControl/rawdata";
            mqttClient.Unsubscribe(new string[] { screenDevice });
            mqttClient.Unsubscribe(new string[] { airCondictioningIODevice });
            mqttClient.Unsubscribe(new string[] { projectorIODevice });
            mqttClient.Unsubscribe(new string[] { lighting1IODevice });
            mqttClient.Unsubscribe(new string[] { lighting2IODevice });
            mqttClient.Unsubscribe(new string[] { lamp1IODevice });
            mqttClient.Unsubscribe(new string[] { airCondictioningModelDevice });
            mqttClient.Unsubscribe(new string[] { airCondictioningtemperatureDevice });
            mqttClient.Unsubscribe(new string[] { airCondictioningWindDevice });
            mqttClient.Unsubscribe(new string[] { scenarioControlDevice });
            mqttClient.Disconnect();
        }
        /// <summary>
        /// 中斷連線時
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                Console.WriteLine(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
                Clients.Caller.onError(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
            }
            else
            {
                Console.WriteLine(String.Format("Client {0} timed out .", Context.ConnectionId));
                Clients.Caller.onError(String.Format("Client {0} timed out .", Context.ConnectionId));
            }
            if (UserHandler.ConnectedIds.Count == 0)
                unSubscribe();
            return base.OnDisconnected(stopCalled);
        }
        public override Task OnConnected()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            //  if (UserHandler.ConnectedIds.Count == 1)
            //mqttConnection();
            return base.OnConnected();
        }
    }
}
