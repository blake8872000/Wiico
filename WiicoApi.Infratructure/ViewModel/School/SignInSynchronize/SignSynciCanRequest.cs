using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WiicoApi.Infrastructure.ViewModel.School.SignInSynchronize
{
    public class SignSynciCanRequest
    {
        /// <summary>
        /// 課程代碼
        /// </summary>
        [JsonProperty("course_no")]
        public string Course_no { get; set; }
        /// <summary>
        /// 大綱(進度)ID
        /// </summary>
        [JsonProperty("syll_id")]
        public string Syll_id { get; set; }
        /// <summary>
        /// 第幾次點名
        /// </summary>
        public string Times { get; set; }
        /// <summary>
        /// 教師或助教的token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
        /// <summary>
        /// 課程成員出勤狀態
        /// </summary>
        [JsonProperty("classmatesStatus")]
        public string ClassmatesStatus { get; set; }

    }
}
