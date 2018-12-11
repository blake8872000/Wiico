using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather.OpenWeather
{
    /// <summary>
    /// 回傳地點時間結構
    /// </summary>
    public class OpenWeatherLocation
    {
        public string LocationName { get; set; }

        public List<OpenWeatherElement> WeatherElement { get; set; }
    }
}
