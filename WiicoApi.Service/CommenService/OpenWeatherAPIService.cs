using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Weather.OpenWeather;

namespace WiicoApi.Service.CommenService
{
    /// <summary>
    /// 查詢氣象局API
    /// </summary>
    public class OpenWeatherAPIService
    {
        private readonly string authorization = ConfigurationManager.AppSettings["weatherTokenKey"].ToString();
        private readonly string apiUrl = "http://opendata.cwb.gov.tw/api/v1/rest/datastore/";
        public OpenWeatherAPIService() {

        }
        /// <summary>
        /// 查詢氣象局資料
        /// </summary>
        /// <param name="searchMethod">F-C0032-001:一般天氣預報-今明 36 小時天氣預報 | F-D0047-061:鄉鎮天氣預報-臺北市未來2天天氣預報 | F-D0047-063: 鄉鎮天氣預報-臺北市未來1週天氣預報</param>
        /// <param name="parameter">format=json&locationName=臺北市</param>
        /// <returns></returns>
        public OpenWeatherAPIResponse GetDatas(string searchMethod,string parameter) {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var weatherUrl = string.Format("{0}{1}",apiUrl,searchMethod);
                var url = string.Format("{0}?{1}", weatherUrl, parameter);
                client.DefaultRequestHeaders.Add("Authorization", authorization);
                try
                {
                    var response = client.GetAsync(url).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    var apiResult = JsonConvert.DeserializeObject<OpenWeatherAPIResponse>(content);
                    return apiResult;
                }
                catch (Exception ex)
                {
                    return null;
                    throw ex;
                }
            }
        }
    }
}
