using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MQTT
{
    public enum DeviceEnum : long
    {
        Device = 5418391279, //教室設備裝置編號 [冷氣儀器、電燈儀器、投影機儀器]
        Environment = 5418464279, //教室儀器設備編號 [溫度、濕度、二氧化碳濃度]
        EMotion = 5609364233, //情緒設備編號
        Interactive = 5609491626, //舉手設備編號
        TestInteractive = 5616212491, //測試舉手設備編號
        TestEMotion = 5618251599, //測試情緒設備編號
    }
}
