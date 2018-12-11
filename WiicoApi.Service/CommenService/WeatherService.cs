using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    public class WeatherService
    {
        private readonly GenericUnitOfWork _uow;
        static DateTime lastCall = DateTime.Parse("2010/1/1");

        static Infrastructure.ViewModel.Weather.WeatherDataModel mWeatherDataModel;
        public WeatherService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 取得天氣資訊
        /// </summary>
        /// <param name="data">驗證資訊</param>
        /// <returns></returns>
        public async Task<Infrastructure.ViewModel.Weather.WeatherDataModel> GetData(Infrastructure.ViewModel.Base.BackendBaseRequest data)
        {
            var db = _uow.DbContext;
            var loginService = new LoginService();
            var memberInfo = (from m in db.Members
                              join ut in db.UserToken on m.Id equals ut.MemberId
                              where m.Account == data.Account && ut.Token == data.ICanToken
                              select m).FirstOrDefault();

            if (memberInfo == null)
                return null;

            var result = new Infrastructure.ViewModel.Weather.WeatherDataModel()
            {
                name = "Taipei",
                id = (int)(Math.Round(99999.0, 0)),
                main = new Infrastructure.ViewModel.Weather.Main() { humidity = 73, pressure = 1016, temp = (float)23.5, temp_max = 24, temp_min = 23 },
                weather = new List<Infrastructure.ViewModel.Weather.Weather>()

            };
            //判斷呼叫時間間隔
            if ((DateTime.Now - lastCall).TotalMinutes <= 5)
                if (mWeatherDataModel != null)
                    return mWeatherDataModel;


            lastCall = DateTime.Now;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var urlparameters = "q=Taipei&units=metric&APPID=7d6d2eba18223d69ab300e873183e8bc";
                var weatherUrl = ConfigurationManager.AppSettings["weatherAPIUrl"].ToString();
                var url = string.Format("{0}?{1}", weatherUrl, urlparameters);
                // var response = await client.GetAsync(url);
                //  var content = await response.Content.ReadAsStringAsync();
                try
                {
                    //   result = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Weather.WeatherDataModel>(content);
                    if (result == null)
                        return null;
                    var cloneWeather = new Infrastructure.ViewModel.Weather.Weather()
                    {
                        id = 803,
                        main = "Clouds",
                        description = "broken clouds",
                        icon = "04d",
                        icon_group = "80x"
                    };
                    result.weather.Add(cloneWeather);
                    //處理icon的對應
                    if (result.weather != null && result.weather.Count > 0)
                    {
                        var d = result.weather.First();
                        if (d.id == 800)
                        {
                            if (DateTime.Now.Hour >= 18)
                            {
                                d.icon_group = "800_n";
                            }
                            else
                            {
                                d.icon_group = "800_d";
                            }
                        }
                        else
                        {
                            var sid = d.id.ToString().Substring(0, 1);
                            switch (sid)
                            {
                                case "2":
                                    d.icon_group = "2xx";
                                    break;
                                case "3":
                                    d.icon_group = "3xx";
                                    break;
                                case "5":
                                    d.icon_group = "5xx";
                                    break;
                                case "6":
                                    d.icon_group = "6xx";
                                    break;
                                case "7":
                                    d.icon_group = "7xx";
                                    break;
                                case "8":
                                    d.icon_group = "80x";
                                    break;
                                default:
                                    d.icon_group = "80x";
                                    break;
                            }
                        }
                    }
                    result.currentDate = DateTime.Now;

                    var morning = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " 05:00:00");
                    var afternoon = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " 11:00:00");
                    var night = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " 18:00:00");
                    if (result.currentDate >= morning && result.currentDate < afternoon)
                        result.greetText = "早安！";
                    else if (result.currentDate >= afternoon && result.currentDate < night)
                        result.greetText = "午安！";
                    else
                        result.greetText = "晚安！";

                    mWeatherDataModel = result;
                    return result;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private Infrastructure.ViewModel.Weather.WeatherDataModel MakeGerrtText(Infrastructure.ViewModel.Weather.WeatherDataModel data)
        {
            data.currentDate = DateTime.Now;
            DateTime morning = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " 05:00:00");
            DateTime afternoon = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " 11:00:00");
            DateTime night = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " 18:00:00");
            if (data.currentDate >= morning && data.currentDate < afternoon)
                data.greetText = "早安！";
            else if (data.currentDate >= afternoon && data.currentDate < night)
                data.greetText = "午安！";
            else
                data.greetText = "晚安！";
            return data;
        }
    }
}
