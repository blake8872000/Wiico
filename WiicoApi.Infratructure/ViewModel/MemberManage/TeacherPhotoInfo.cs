using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MemberManage
{
    public class TeacherPhotoInfo
    {
        /// <summary>
        /// 連結位置
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// 教師姓名
        /// </summary>
        [JsonProperty("manName")]
        public string ManName { get; set; }
        /// <summary>
        /// 教師email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
