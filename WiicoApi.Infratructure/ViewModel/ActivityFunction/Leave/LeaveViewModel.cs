using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class LeaveViewModel
    {
        /// <summary>
        /// 資訊
        /// </summary>
        [JsonProperty("info")]
        public WiicoApi.Infrastructure.Entity.AttendanceLeave Info { get; set; }
        /// <summary>
        /// 檔案資訊
        /// </summary>
        [JsonProperty("fileList")]
        public List<WiicoApi.Infrastructure.Entity.GoogleFile> FileList { get; set; }

        /// <summary>
        /// 是老師
        /// </summary>
        [JsonProperty("isManager")]
        public bool IsManager { get; set; }

        /// <summary>
        /// 請假單代碼
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
        /// <summary>
        /// 請假人姓名
        /// </summary>
        [JsonProperty("memberName")]
        public string MemberName { get; set; }
        /// <summary>
        /// 請假人帳號
        /// </summary>
        [JsonProperty("memberAccount")]
        public string MemberAccount { get; set; }
        /// <summary>
        /// 狀態名稱
        /// </summary>
        [JsonProperty("statusName")]
        public string StatusName { get; set; }
        /// <summary>
        /// 課程名稱
        /// </summary>
        [JsonProperty("courseName")]
        public string CourseName { get; set; }
    }
}
