using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class GetAbSenceFormDetailResponse : LeaveData
    {
        /// <summary>
        /// 課程名稱
        /// </summary>
        [JsonProperty("courseName")]
        public string CourseName { get; set; }
        /// <summary>
        /// 請假人大頭照位置
        /// </summary>
        [JsonProperty("memberImg")]
        public string MemberImg { get; set; }
        /// <summary>
        /// 上課地點資訊
        /// </summary>
        [JsonProperty("rooms")]
        public List<ClassRoomInfo> Rooms { get; set; }
    }
}
