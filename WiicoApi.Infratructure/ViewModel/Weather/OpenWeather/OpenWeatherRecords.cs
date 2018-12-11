using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather.OpenWeather
{
    /// <summary>
    /// 回傳完整報告
    /// </summary>
    public class OpenWeatherRecords
    {
        public string DatasetDescription { get; set; }

        public List<OpenWeatherLocation> Location { get; set; }
    }
}
