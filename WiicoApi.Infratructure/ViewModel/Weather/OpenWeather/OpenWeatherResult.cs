using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather.OpenWeather
{
    /// <summary>
    /// 回傳欄位
    /// </summary>
    public class OpenWeatherResult
    {
        public string Resource_id { get; set; }

        public List<OpenWeatherField> Fields { get; set; }
    }
}
