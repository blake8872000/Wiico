using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather.OpenWeather
{
    /// <summary>
    /// 回傳時間結構
    /// </summary>
    public class OpenWeatherElement
    {
        public string ElementName { get; set; }

        public List<OpenWeatherElementTime> Time { get; set; }
    }
}
