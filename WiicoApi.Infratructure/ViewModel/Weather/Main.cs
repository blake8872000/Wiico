using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather
{
    public class Main
    {
        /// <summary>
        /// 目前溫度
        /// </summary>
        public float temp { get; set; }
        /// <summary>
        /// 大氣壓力
        /// </summary>
        public decimal pressure { get; set; }
        /// <summary>
        /// 濕度
        /// </summary>
        public int humidity { get; set; }
        /// <summary>
        /// 最低溫
        /// </summary>
        public decimal temp_min { get; set; }
        /// <summary>
        /// 最高溫
        /// </summary>
        public decimal temp_max { get; set; }
    }
}
