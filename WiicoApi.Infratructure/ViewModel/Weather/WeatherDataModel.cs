using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather
{
    public class WeatherDataModel
    {
        /// <summary>
        /// 目前天氣狀況
        /// </summary>
        public List<Weather> weather { get; set; }
        /// <summary>
        /// 溫溼度等資訊
        /// </summary>
        public Main main { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 當下日期
        /// </summary>
        public DateTime currentDate { get; set; }
        /// <summary>
        /// 招呼語(早安/午安)
        /// </summary>
        public string greetText { get; set; }
    }
}
