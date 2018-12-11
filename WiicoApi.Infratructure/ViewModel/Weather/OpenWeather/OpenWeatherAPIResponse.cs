using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather.OpenWeather
{
    /// <summary>
    /// 氣象局回傳結構
    /// </summary>
    public class OpenWeatherAPIResponse
    {
        public string Success { get; set; }
        public OpenWeatherResult Result { get; set; }
        public OpenWeatherRecords Records { get; set; }
    }
}
