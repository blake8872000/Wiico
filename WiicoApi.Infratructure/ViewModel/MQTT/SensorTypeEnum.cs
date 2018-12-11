using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public enum SensorTypeEnum
    {
        /// <summary>
        /// 就是開跟關
        /// </summary>
        開關 = 0,
        /// <summary>
        /// 強中弱、冷氣暖氣送風
        /// </summary>
        運轉模式 = 1,
        /// <summary>
        /// 溫度、濕度
        /// </summary>
        可量刻度 = 2
    }
}
