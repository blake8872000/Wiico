using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class ModuleDetailViewModel
    {

        /// <summary>
        /// 目前要顯示的moduleKey
        /// </summary>
        [JsonProperty("moduleKey")]
        public string ModuleKey { get; set; }
        /// <summary>
        /// 顯示要回去的learningId
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId { get; set; }
        /// <summary>
        /// 學習圈的key
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        /// <summary>
        /// 學習圈的名稱
        /// </summary>
        [JsonProperty("circleName")]
        public string CircleName { get; set; }
        /// <summary>
        /// 是否為模組管理者角色
        /// </summary>
        [JsonProperty("isAdminRole")]
        public bool IsAdminRole { get; set; }
        /// <summary>
        /// 登入者 id
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; set; }
        /// <summary>
        /// 登入者 account
        /// </summary>
        [JsonProperty("memberAccount")]
        public string MemberAccount { get; set; }
        /// <summary>
        /// 登入者 Name
        /// </summary>
        [JsonProperty("memberName")]
        public string MemberName { get; set; }
        /// <summary>
        /// Google Drive 參數
        /// </summary>
        [JsonProperty("googleDriveFileId")]
        public string GoogleDriveFileId { get; set; }
        /// <summary>
        /// 作業細節用
        /// </summary>
        [JsonProperty("homeWorkDetail")]
        public HomeWorkViewModel HomeWorkDetail { get; set; }
        /// <summary>
        /// 點名細節用
        /// </summary>
        [JsonProperty("signInDetail")]
        public SignInDetailViewModel SignInDetail { get; set; }
        /// <summary>
        /// 教材細節用
        /// </summary>
        [JsonProperty("materialDetail")]
        public MaterialViewModel MaterialDetail { get; set; }

        /// <summary>
        /// 主題討論細節用
        /// </summary>
        [JsonProperty("discussionDetail")]
        public DiscussionViewModel DiscussionDetail { get; set; }


        /// <summary>
        /// 分組細節用
        /// </summary>
        [JsonProperty("GroupDetail")]
        public GroupCategoryViewModel GroupDetail { get; set; }
    }
}
