using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.MQTT;

namespace WiicoApi.Service.MQTT
{
    public class MQTTDeviceService
    {
        private string mottAccessKey = ConfigurationManager.AppSettings["MQTTAccessKey"].ToString();
        private string iotUrl = ConfigurationManager.AppSettings["IOTUrl"].ToString();
        public async Task<RoomDeviceViewModel> GetRoomDevice(string circlekey = "", IOTCallApiMethodsEnum? method = IOTCallApiMethodsEnum.rawdata)
        {
            var result = new RoomDeviceViewModel();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("ck", mottAccessKey);
                var apiResponse = await httpClient.GetAsync(iotUrl);
                var iotResponse = await apiResponse.Content.ReadAsStringAsync();
                if (iotResponse == "NULL" || iotResponse == null || iotResponse == string.Empty || iotResponse == "")
                    return null;
                var responseDatas = JsonConvert.DeserializeObject<List<IOTDeviceResponse>>(iotResponse);
                foreach (var responseData in responseDatas)
                {
                    var sensorDatas = await GetDeviceSensors(responseData.Id, mottAccessKey);
                    var devideId = Convert.ToDouble(responseData.Id);
                    switch (devideId)
                    {
                        case (double)DeviceEnum.Device:
                            result = SetDevice(sensorDatas, result);
                            break;
                        case (double)DeviceEnum.Environment:
                            result = SetEnvironment(sensorDatas, result);
                            break;
                        default:
                            break;
                    }
                }
            }
            var timeTableService = new Service.Backend.TimeTableService();
            var classInfo = timeTableService.Get(circlekey);
            if (classInfo != null)
                result.RoomName = classInfo.ClassRoom;
            return result;
        }

        /// <summary>
        /// 回傳IOTSensor結果
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<List<IOTSensorResponse>> GetDeviceSensors(string deviceId, string accessKey, IOTCallApiMethodsEnum? method = IOTCallApiMethodsEnum.rawdata)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("ck", accessKey);
                var url = string.Format("{0}/{1}/{2}", iotUrl, deviceId, method.ToString());
                var apiResponse = httpClient.GetAsync(url).Result;
                var iotResponse = await apiResponse.Content.ReadAsStringAsync();
                if (iotResponse == "NULL" || iotResponse == null || iotResponse == string.Empty || iotResponse == "")
                    return null;
                var responseDatas = JsonConvert.DeserializeObject<List<IOTSensorResponse>>(iotResponse);
                return responseDatas;
            }
        }

        /// <summary>
        /// 變更設備遙控狀態
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIOTData(string deviceId, string sensorId, string value)
        {
            var data = new SensorSave()
            {
                Id = sensorId,
                Time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                Save = true
            };
            var checkValue = value.Split(',');
            data.Value = new string[checkValue.Count()];
            if (checkValue.Count() > 1)
            {
                var index = 0;
                foreach (var dataValue in checkValue)
                {
                    data.Value.SetValue(dataValue, index);
                    index++;
                }
            }
            else
            {
                data.Value.SetValue(checkValue[0], 0);
            }
            var postData = new SensorSave[1] { data };
            var sendBody = JsonConvert.SerializeObject(postData);
            using (var content = new StringContent(sendBody))
            {
                using (var httpClient = new HttpClient())
                {
                    content.Headers.Add("ck", mottAccessKey);
                    //content.Headers.Add("Content-Type", "application/json; charset=UTF-8");
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    var url = string.Format("{0}/{1}/{2}", iotUrl, deviceId, IOTCallApiMethodsEnum.rawdata.ToString());
                    var responseMessage = await httpClient.PostAsync(url, content);
                    var responseString = await responseMessage.Content.ReadAsStringAsync();

                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                        return true;
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// 設定室內狀態資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private RoomDeviceViewModel SetEnvironment(List<IOTSensorResponse> datas, RoomDeviceViewModel response)
        {
            foreach (var data in datas)
            {
                var setData = Math.Round(Convert.ToDouble(data.Value.FirstOrDefault()), 2);
                switch (data.Id)
                {
                    case "co2":
                        response.Co2 = setData;
                        break;
                    case "humidity":
                        response.Humidity = setData;
                        break;
                    case "temperature":
                        response.Temperature = setData;

                        break;
                    default:
                        break;
                }
            }
            return response;
        }

        /// <summary>
        /// 設定設備資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private RoomDeviceViewModel SetDevice(List<IOTSensorResponse> datas, RoomDeviceViewModel response)
        {
            response.SensorGroups = new List<DeviceViewModel>();
            //查詢冷氣資訊
            var airConditioning = SetAirConditioning(datas);
            //設定燈光資訊
            var lights = SetLight(datas);
            //設定投影機資訊
            var projector = SetProjector(datas);
            //設定情境資訊
            var sceNarioControl = SetSceNarioControl(datas);
            response.SensorGroups.Add(airConditioning);
            response.SensorGroups.Add(lights);
            response.SensorGroups.Add(projector);
            response.SensorGroups.Add(sceNarioControl);
            return response;
        }

        /// <summary>
        /// 設定冷氣資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private DeviceViewModel SetAirConditioning(List<IOTSensorResponse> datas)
        {
            var response = new DeviceViewModel();
            response.GroupID = 0;
            response.GroupName = "冷氣";
            response.Sensors = new List<Sensor>();
            var AirConditioningIO = datas.FirstOrDefault(t => t.Id == "do1");
            var mainSensor = new Sensor()
            {
                DeviceId = AirConditioningIO.DeviceId,
                Name = "冷氣開關",
                SensorId = AirConditioningIO.Id,
                Value = AirConditioningIO.Value.FirstOrDefault(),
                Type = (int)SensorTypeEnum.開關,
                Sort = 0,
                ChildSensor = new List<SubSensor>(),
                SetValues = new List<SensorSetValues>() {
                    new SensorSetValues(){   ValueName  ="關",  Value= "0" },
                    new SensorSetValues(){   ValueName  ="開",  Value= "1" },
                }
            };

            foreach (var data in datas)
            {
                var subSensor = new SubSensor();
                var dataValue = data.Value.FirstOrDefault();
                subSensor.DeviceId = data.DeviceId;
                subSensor.SensorId = data.Id;
                subSensor.Value = dataValue;
                switch (data.Id)
                {
                    case "ao1":
                        subSensor.Name = "運轉模式";

                        subSensor.SetValues = new List<SensorSetValues>() {
                            new SensorSetValues(){ ValueName="冷氣" , Value="0"},
                            new SensorSetValues(){ ValueName="除溼" , Value="1"},
                            new SensorSetValues(){ ValueName="送風" , Value="2"},
                            new SensorSetValues(){ ValueName="自動溫感" , Value="3"},
                            new SensorSetValues(){ ValueName="暖氣" , Value="4"}
                        };
                        subSensor.Type = (int)SensorTypeEnum.運轉模式;
                        subSensor.Sort = 0;
                        mainSensor.ChildSensor.Add(subSensor);
                        break;
                    case "ao2":
                        subSensor.Name = "目標溫度";
                        subSensor.SetValues = new List<SensorSetValues>() {
                            new SensorSetValues(){ ValueName="增加幅度" , Value="1"}
                        };
                        subSensor.Type = (int)SensorTypeEnum.可量刻度;
                        subSensor.Sort = 1;
                        subSensor.Unit = "°c";
                        mainSensor.ChildSensor.Add(subSensor);
                        break;
                    case "ao3":
                        subSensor.Name = "目前風量";
                        subSensor.SetValues = new List<SensorSetValues>() {
                            new SensorSetValues(){ ValueName="微風" , Value="0"},
                            new SensorSetValues(){ ValueName="弱風" , Value="1"},
                            new SensorSetValues(){ ValueName="強風" , Value="2"},
                            new SensorSetValues(){ ValueName="自動" , Value="3"}
                        };
                        subSensor.Type = (int)SensorTypeEnum.運轉模式;
                        subSensor.Sort = 2;
                        mainSensor.ChildSensor.Add(subSensor);
                        break;
                    default:
                        break;
                }
            }
            response.Sensors.Add(mainSensor);
            return response;
        }

        /// <summary>
        /// 設定燈光資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private DeviceViewModel SetLight(List<IOTSensorResponse> datas)
        {
            var response = new DeviceViewModel();
            response.GroupID = 1;
            response.GroupName = "燈";
            response.Sensors = new List<Sensor>();

            foreach (var data in datas)
            {
                var sensor = new Sensor();
                var dataValue = data.Value.FirstOrDefault();
                sensor.DeviceId = data.DeviceId;
                sensor.SensorId = data.Id;
                sensor.Value = dataValue;
                sensor.Type = (int)SensorTypeEnum.開關;
                sensor.SetValues = new List<SensorSetValues>() {
                     new SensorSetValues(){   ValueName  ="關",  Value= "0" },
                     new SensorSetValues(){   ValueName  ="開",  Value= "1" }
                        };
                switch (data.Id)
                {
                    case "lighting2":
                        sensor.Name = "室內光源-前";
                        sensor.Sort = 0;
                        response.Sensors.Add(sensor);
                        break;
                    case "lighting1":
                        sensor.Name = "室內光源-後";
                        sensor.Sort = 1;
                        response.Sensors.Add(sensor);
                        break;
                    case "lamp1":
                        sensor.Name = "室內光源-兩側";
                        sensor.Sort = 2;
                        response.Sensors.Add(sensor);
                        break;
                    default:
                        break;
                }

            }
            return response;
        }

        /// <summary>
        /// 設定投影機資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private DeviceViewModel SetProjector(List<IOTSensorResponse> datas)
        {
            var response = new DeviceViewModel();
            response.GroupID = 2;
            response.GroupName = "投影機";
            response.Sensors = new List<Sensor>();

            foreach (var data in datas)
            {
                var sensor = new Sensor();
                var dataValue = data.Value.FirstOrDefault();
                sensor.DeviceId = data.DeviceId;
                sensor.SensorId = data.Id;
                sensor.Value = dataValue;
                sensor.Type = (int)SensorTypeEnum.開關;
                sensor.SetValues = new List<SensorSetValues>() {
                        new SensorSetValues(){   ValueName  ="關",  Value= "0" },
                        new SensorSetValues(){   ValueName  ="開",  Value= "1" }
                        };
                switch (data.Id)
                {
                    case "projector":
                        sensor.Name = "投影機";
                        sensor.Sort = 0;
                        response.Sensors.Add(sensor);
                        break;
                    case "screen":
                        sensor.Name = "投影布幕";
                        sensor.Sort = 1;
                        response.Sensors.Add(sensor);
                        break;

                    default:
                        break;
                }

            }
            return response;
        }

        /// <summary>
        /// 設定情境資訊
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private DeviceViewModel SetSceNarioControl(List<IOTSensorResponse> datas)
        {
            var response = new DeviceViewModel();
            response.GroupID = 3;
            response.GroupName = "情境控制";
            response.Sensors = new List<Sensor>();

            foreach (var data in datas)
            {
                var sensor = new Sensor();
                var dataValue = data.Value.FirstOrDefault();
                sensor.DeviceId = data.DeviceId;
                sensor.SensorId = data.Id;
                sensor.Value = dataValue;
                sensor.Type = (int)SensorTypeEnum.運轉模式;
                sensor.SetValues = new List<SensorSetValues>() {
                        new SensorSetValues(){   ValueName  ="全開模式",  Value= "1" },
                        new SensorSetValues(){   ValueName  ="全關模式",  Value= "0" },
                        new SensorSetValues(){   ValueName  ="簡報模式",  Value= "2" },
                        new SensorSetValues(){   ValueName  ="解除鎖定",  Value= "5" }
                        };
                switch (data.Id)
                {
                    case "scenarioControl":
                        sensor.Name = "情境控制";
                        sensor.Sort = 0;
                        response.Sensors.Add(sensor);
                        break;

                    default:
                        break;
                }

            }
            return response;
        }
    }
}
