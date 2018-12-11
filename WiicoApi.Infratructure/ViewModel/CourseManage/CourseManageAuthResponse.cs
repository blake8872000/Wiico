using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    /// <summary>
    /// 課程權限
    /// </summary>
    public class CourseManageAuthResponse
    {
        /// <summary>
        /// 課程設定權限
        /// </summary>
        [JsonProperty("circleInfoSetting")]
        public CircleInfoSettingAuth CircleInfoSetting { get; set; }
        /// <summary>
        /// 課程管理者列表權限
        /// </summary>
        [JsonProperty("circleAdminSetting")]
        public CircleAdminSettingAuth CircleAdminSetting { get; set; }
        /// <summary>
        /// 課程上課時間表權限
        /// </summary>
        [JsonProperty("circleTimelistSetting")]
        public CircleTimelistSettingAuth CircleTimelistSetting { get; set; }
        /// <summary>
        /// 課程章節權限
        /// </summary>
        [JsonProperty("circleScheduleSetting")]
        public CircleScheduleSettingAuth CircleScheduleSetting{ get; set; }
        /// <summary>
        /// 課程角色權限
        /// </summary>
        [JsonProperty("circleRoleSetting")]
        public CircleRoleSettingAuth CircleRoleSetting { get; set; }
        /// <summary>
        /// 課程成員權限
        /// </summary>
        [JsonProperty("circleMemberSetting")]
        public CircleMemberSettingAuth CircleMemberSetting { get; set; }
    }
}
