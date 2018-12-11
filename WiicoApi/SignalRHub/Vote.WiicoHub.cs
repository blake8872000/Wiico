using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote;
using WiicoApi.Infrastructure.ViewModel.MQTT;
using WiicoApi.Service.SignalRService;
using WiicoApi.SignalR;

namespace WiicoApi.SignalRHub
{
    /// <summary>
    /// SignalRHub
    /// </summary>
    public partial class WiicoHub : WiicoHubBase
    {
        private string sensorDevice = string.Format("/v1/device/{0}/sensor/{1}/rawdata", Convert.ToInt64(DeviceEnum.Interactive), "vote");

        private string testSensorDevice = string.Format("/v1/device/{0}/sensor/{1}/rawdata", Convert.ToInt64(DeviceEnum.TestInteractive), "testSensorInfo");
        private MqttClient mqttTestClient;
        private string voteOuterKey = string.Empty;
        private static bool voteIsStart;
        /// <summary>
        /// 取得投票列表
        /// </summary>
        /// <param name="circleKey"></param>
        public void GetVoteList(string circleKey)
        {
            var service = new VoteService();
            var list = service.GetList(circleKey);
            Clients.Group(circleKey.ToLower()).getList(list);
        }

        /// <summary>
        /// 建立 / 編輯投票活動資訊
        /// </summary>
        /// <param name="outerKey">活動代碼[編輯用]</param>
        /// <param name="userToken">登入代碼</param>
        /// <param name="groupId">所屬學習圈</param>
        /// <param name="title">投票標題</param>
        /// <param name="description">投票描述</param>
        /// <param name="voteItems">投票項目</param>
        /// <param name="strAccess">投票項目</param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Base.BaseResponse<string> votePost(string strAccess)
        {

            var result = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            var requestData = JsonConvert.DeserializeObject<VotePostRequest>(strAccess);
            if (requestData.UserToken == null ||
                requestData.GroupId == null ||
                requestData.Title == null)
            {
                result.Success = false;
                result.Message = "遺漏參數";
                result.State = Infrastructure.ViewModel.Base.LogState.DataNotModified;
                return result;
            }
            var checkToken = CheckToken(requestData.UserToken.ToString());
            if (checkToken == null)
            {
                result.Success = false;
                result.Message = "已登出";
                result.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return result;
            }
            var voteService = new VoteService();
            //新增
            if (requestData.OuterKey == null || (requestData.OuterKey == string.Empty && requestData.OuterKey == ""))
            {
                try
                {
                    //開始建立
                    var response = voteService.VoteCreate(checkToken.MemberId, requestData.GroupId, requestData.Title, requestData.Description, requestData.VoteItems);

                    if (response != null)
                    {
                        result.Success = true;
                        result.Message = "建立成功";
                        var rtn = activityService.SignalrResponse(requestData.GroupId, response.OuterKey, Service.Utility.ParaCondition.ModuleType.Vote, checkToken.MemberId, true);
                        Clients.Group(requestData.GroupId.ToLower()).appendActivity(rtn, "");
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "建立失敗";
                    }

                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.Message;
                    return result;
                }

            }
            //編輯
            else
            {
                var response = voteService.VoteUpdate(requestData.OuterKey, checkToken.MemberId, requestData.Title, requestData.Description, requestData.VoteItems);
                if (response != null)
                {
                    result.Success = true;
                    result.Message = "修改成功";
                    var rtn = voteService.GetDetail(response.OuterKey.ToString());
                    Clients.Group(requestData.GroupId.ToLower()).updateDetail(Service.Utility.ParaCondition.ModuleType.Vote, rtn);
                }
            }

            return result;
        }

        /// <summary>
        /// 變更投票活動狀態
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="groupId"></param>
        /// <param name="outerKey"></param>
        /// <param name="isStart"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.Base.BaseResponse<string> voteChangeStart(Guid userToken, string groupId, string outerKey, bool isStart)
        {
            var result = new Infrastructure.ViewModel.Base.BaseResponse<string>();

            if (userToken == null ||
                groupId == null ||
                outerKey == null)
            {
                result.Success = false;
                result.Message = "遺漏參數";
                result.State = Infrastructure.ViewModel.Base.LogState.DataNotModified;
                return result;
            }
            var checkToken = CheckToken(userToken.ToString());
            if (checkToken == null)
            {
                result.Success = false;
                result.Message = "已登出";
                result.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return result;
            }
            var voteService = new VoteService();
            mqttCircleKey = groupId;
            var response = voteService.VoteChangeStart(groupId, outerKey, checkToken.MemberId, isStart);
            if (response != VoteStateEnum.OtherStart && response != VoteStateEnum.NotFound)
            {
                result.Success = true;
                result.Message = "變更成功";
                voteOuterKey = outerKey;

                //結束監聽+中斷連線
                if (response == VoteStateEnum.Stop)
                {
                    voteIsStart = false;
                    StopConnectionSensor();
                    //儲存最後投票資訊
                    var lastVote = voteService.SaveLastVoteData(outerKey);
                    if (lastVote != null)
                        Clients.Group(groupId.ToLower()).updateVoteInfo(lastVote, "結束連線");
                }
                Clients.Group(groupId.ToLower()).voteStartOrEnd(outerKey, isStart);
                //開始監聽+連線
                if (response == VoteStateEnum.Start)
                {
                    voteIsStart = true;
                    ConnectionSensor(groupId, outerKey);
                }

            }
            else
            {
                result.Success = false;

                result.Message = response.ToString();
                if (response == VoteStateEnum.OtherStart)
                    result.Message = "目前仍有投票活動正在進行中，請先結束後，才能開始本活動。";
            }
            return result;
        }
        /// <summary>
        /// 開始連線監聽
        /// </summary>
        public void ConnectionSensor(string circlekey, string outerKey)
        {
            voteIsStart = true;
            voteOuterKey = outerKey;
            mqttCircleKey = circlekey;
            mqttTestClient = new MqttClient(mqttConnectUrl);
            mqttTestClient.Connect("interactive", motionAccessKey, motionAccessKey);
            var qoslevels = new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            //訂閱儀器
            mqttTestClient.Subscribe(new string[] { sensorDevice }, qoslevels);
            //監聽
            mqttTestClient.MqttMsgPublishReceived += MqttClientInteractiveReceived;
        }

        /// <summary>
        /// 結束監聽
        /// </summary>
        public void StopConnectionSensor()
        {
            voteIsStart = false;
            //    mqttClient = new MqttClient(mqttConnectUrl);
            mqttTestClient = new MqttClient(mqttConnectUrl);
            mqttTestClient.Connect("interactive", motionAccessKey, motionAccessKey);
            mqttTestClient.Unsubscribe(new string[] { sensorDevice });
            mqttTestClient.Disconnect();
        }

        private void MqttClientInteractiveReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var motionResponse = new IOTProjectViewModel<IOTProjectRecordData>();
            var apiResponse = Encoding.UTF8.GetString(e.Message);
            var sensorStatus = JsonConvert.DeserializeObject<IOTSensorResponse>(apiResponse);
            var responseValue = sensorStatus.Value.Length > 0 ? sensorStatus.Value[0] : "0";
            if (responseValue != "0")
            {
                var voteService = new VoteService();
                var rtn = voteService.GetDetail(voteOuterKey);
                motionResponse = JsonConvert.DeserializeObject<IOTProjectViewModel<IOTProjectRecordData>>(responseValue);
                if (rtn.StartDate.HasValue && (rtn.StartDate.Value <= motionResponse.RecordTime))
                    rtn = voteService.ItemProxy(rtn, motionResponse);
                if (rtn.IsStart)
                    Clients.Group(mqttCircleKey.ToLower()).updateVoteInfo(rtn, "成功");
            }
        }
    }
}