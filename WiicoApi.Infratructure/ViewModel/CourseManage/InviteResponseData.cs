using WiicoApi.Infrastructure.ValueObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class InviteResponseData
    {
        /// <summary>
        /// 邀請狀態
        /// </summary>
        [JsonProperty("inviteStatus")]
        public InviteStatusEnum InviteStatus { get; set; }
        /// <summary>
        /// 邀請課碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        /// <summary>
        /// 組織名稱
        /// </summary>
        [JsonProperty("orgName")]
        public string OrgName { get; set; }
        /// <summary>
        /// 課程名稱
        /// </summary>
        [JsonProperty("circleName")]
        public string CircleName { get; set; }
        /// <summary>
        /// 是否可註冊
        /// </summary>
        [JsonProperty("isOrgRegister")]
        public bool IsOrgRegister { get; set; }
    }
}
