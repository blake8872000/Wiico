using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather.OpenWeather
{
    /// <summary>
    /// 回傳時間
    /// </summary>
    public class OpenWeatherElementTime
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public OpenWeatherElementParameter Parameter { get; set; }
    }
}
