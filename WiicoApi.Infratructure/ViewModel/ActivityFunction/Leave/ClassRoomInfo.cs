using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class ClassRoomInfo
    {
        /// <summary>
        /// 教室代碼
        /// </summary>
        [JsonProperty("roomId")]
        public string RoomId { get; set; }
        /// <summary>
        /// 教室名稱
        /// </summary>
        [JsonProperty("roomName")]
        public string RoomName { get; set; }
        /// <summary>
        /// 課程開始時間
        /// </summary>
        [JsonProperty("ClassStart")]
        public DateTime? classStart { get; set; }
        /// <summary>
        /// 課程結束時間
        /// </summary>
        [JsonProperty("classEnd")]
        public DateTime? ClassEnd { get; set; }
        /// <summary>
        /// 星期幾的對應中文名稱
        /// </summary>
        [JsonProperty("NameOfWeekDay")]
        public string NameOfWeekDay { get; set; }
    }
}
