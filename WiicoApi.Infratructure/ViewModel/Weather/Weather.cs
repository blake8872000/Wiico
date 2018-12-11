using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Weather
{
        public class Weather
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 目前天氣大類
            /// </summary>
            public string main { get; set; }
            /// <summary>
            /// 天氣說明文字
            /// </summary>
            public string description { get; set; }
            /// <summary>
            /// 原始資料來源的icon種類
            /// </summary>
            public string icon { get; set; }
            /// <summary>
            /// 對應的icon名稱
            /// </summary>
            public string icon_group { get; set; }
        }
    }

