using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather.OpenWeather
{
    /// <summary>
    /// 顯示結果
    /// </summary>
    public class OpenWeatherElementParameter
    {
        public string ParameterName { get; set; }

        public string ParameterValue { get; set; }

        public string ParameterUnit { get; set; }
    }
}
