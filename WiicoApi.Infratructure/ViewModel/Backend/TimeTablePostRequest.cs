using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class TimeTablePostRequest : Base.BackendBaseRequest
    {
        /// <summary>
        /// 設定的上課時間資料
        /// </summary>
        [JsonProperty("timeTable")]
        public List<TimeTable> TimeTable { get; set; }
    }
}
