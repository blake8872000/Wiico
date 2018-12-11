using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using WiicoApi.Infrastructure.ViewModel.MQTT;
using WiicoApi.Service.SignalRService;

namespace WiicoApi.Service.MQTT
{
    public class MotionDeviceService
    {
        private string accessKey = ConfigurationManager.AppSettings["MQTTMotionAccessKey"].ToString();
        private string testAccessKey = ConfigurationManager.AppSettings["MQTTTestAccessKey"].ToString();
        private string iotUrl = ConfigurationManager.AppSettings["IOTUrl"].ToString();
        private string mqttConnectUrl = ConfigurationManager.AppSettings["ConnectMQTTUrl"].ToString();
        private MqttClient mqttClient;
        private MQTTDeviceService mqttDeviceService = new MQTTDeviceService();
        /// <summary>
        /// 取得情感資訊
        /// </summary>
        public IOTProjectMoctionViewModel<IOTProjectAvgDataResponse> GetMotionData(string circleKey)
        {
            var sensors = mqttDeviceService.GetDeviceSensors(Convert.ToInt64(DeviceEnum.EMotion).ToString(), accessKey).Result;
            var avgResponse = SetCourseStatusInitData();
            var circleMemberRoleService = new Service.Backend.CircleMemberService();
            var circleMemberDatas = circleMemberRoleService.GetCircleMemberRoleListByCircleKey(circleKey.ToLower());
            var memberCount = circleMemberDatas.FirstOrDefault() != null ? circleMemberDatas.Count() : 0;
            var voteService = new VoteService();
            var list = voteService.GetList(circleKey);
            if (list != null)
            {
                var index = 0;
                var avgPresentCount = 0;
                var avgParticipateCount = 0;
                var itemList = voteService.GetItemListByCircleKey(circleKey);
                //計算參與平均數
                foreach (var vote in list)
                {
                    avgPresentCount += vote.PresentCount.HasValue ? vote.PresentCount.Value : 0;
                    var items = itemList.Where(t => t.ActVoteId == vote.VoteId).ToList();
                    foreach (var item in items)
                    {
                        avgParticipateCount += item.ChooseCount;
                    }
                    index++;
                }
                avgPresentCount = avgPresentCount / index;
                avgParticipateCount = avgParticipateCount / index;

                avgResponse.ParticipateCount = avgParticipateCount;
                avgResponse.PresentCount = avgPresentCount;
                //計算參與率
                avgResponse.ParticipateRate = (avgParticipateCount != 0 && avgPresentCount != 0) ? Math.Round((Convert.ToDouble(avgParticipateCount) / Convert.ToDouble(avgPresentCount)) * 100, 0) : 0;
            }

            foreach (var sensor in sensors)
            {
                //情緒平均資料
                if (sensor.Id == "FeelingAvg")
                {
                    var sensorData = sensor.Value.FirstOrDefault();
                    if (sensorData == null || sensorData.Count() <= 0)
                        continue;
                    try
                    {
                        var apiData = JsonConvert.DeserializeObject<IOTProjectViewModel<IOTProjectAverageData>>(sensorData);
                        var statusIndex = 0;
                        var sensorMemberCount = 0.00;
                        foreach (var data in apiData.Data)
                        {
                            var _data = new IOTProjectAvgDataResponse()
                            {
                                StatusCountAvg = Math.Round(data.ChooseCountAvg, 0),
                                StatusID = data.ChooseID
                            };
                            sensorMemberCount += _data.StatusCountAvg;
                            avgResponse.Status[statusIndex] = _data;
                            statusIndex++;
                        }
                        foreach (var responseData in avgResponse.Status)
                        {
                            responseData.Percentage = Math.Round((responseData.StatusCountAvg / sensorMemberCount) * 100, 0);
                        }

                        avgResponse.RecordTime = apiData.RecordTime;
                    }
                    catch (Exception ex)
                    {
                        return avgResponse;
                    }
                }
            }
            return avgResponse;
        }

        /// <summary>
        /// 取得舉手資訊
        /// </summary>
        public IOTProjectViewModel<IOTProjectRecordData> GetInteractiveData()
        {
            var sensors = mqttDeviceService.GetDeviceSensors(Convert.ToInt64(DeviceEnum.Interactive).ToString(), accessKey).Result;
            var response = new IOTProjectViewModel<IOTProjectRecordData>();
            foreach (var sensor in sensors)
            {
                if (sensor.Id == "vote")
                {
                    var sensorData = sensor.Value.FirstOrDefault();
                    try
                    {
                        if (sensorData != null && sensorData.Count() > 0)
                            response = JsonConvert.DeserializeObject<IOTProjectViewModel<IOTProjectRecordData>>(sensorData);

                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
            };
            return response;
        }
        /// <summary>
        /// 設定情緒初始資料
        /// </summary>
        /// <returns></returns>
        private IOTProjectMoctionViewModel<IOTProjectAvgDataResponse> SetCourseStatusInitData()
        {
            var response = new IOTProjectMoctionViewModel<IOTProjectAvgDataResponse>();
            response.Status = new List<IOTProjectAvgDataResponse>();
            for (var i = 0; i <= 3; i++)
            {

                var _data = new IOTProjectAvgDataResponse()
                {
                    Percentage = 0,
                    StatusCountAvg = 0,
                    StatusID = i
                };
                response.Status.Add(_data);
            }
            response.ParticipateCount = 0;
            response.ParticipateRate = 0;
            response.PresentCount = 0;
            response.RecordTime = DateTime.Now;
            return response;
        }

    }

}
