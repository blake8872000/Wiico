using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Login
{
    public class LoginResponse
    {
        /// <summary>
        /// 使用者學/帳號
        /// </summary>
       //[JsonProperty("acpdId")]
        public string AcpdId { get; set; }
        /// <summary>
        /// 使用者姓名
        /// </summary>
        //[JsonProperty("acpdName")]
        public string AcpdName { get; set; }
        /// <summary>
        /// 使用者系統存取權杖(for iCan service)
        /// </summary>
        //[JsonProperty("iCanToken")]
        public string ICanToken { get; set; }
        /// <summary>
        /// 人員身分類型(>=2000代表身分是老師或是助教)
        /// </summary>
       //[JsonProperty("manType")]
        public int? ManType { get; set; }
        /// <summary>
        /// 照片位置(無照片時回應null)
        /// </summary>
       //[JsonProperty("photo")]
        public string Photo { get; set; }
        /// <summary>
        /// 使用者email位置
        /// </summary>
        ///[JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// 系所編號
        /// </summary>
        [JsonProperty("manColl")]
        public string ManColl { get; set; }
        /// <summary>
        /// 系所名稱
        /// </summary>
        //[JsonProperty("collName")]
        public string CollName { get; set; }
        /// <summary>
        /// 班別名稱以及年級
        /// </summary>
        //[JsonProperty("collBrief")]
        public string CollBrief { get; set; }
        /// <summary>
        /// 是否公開email位置
        /// </summary>
        //[JsonProperty("showMail")]
        public bool ShowMail { get; set; }
        /// <summary>
        /// 加密後的裝置代碼(除login動作外，API呼叫每次需要帶入的值)
        /// </summary>
       //[JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// 帳號身分是否為老師或是助教
        /// </summary>
       // [JsonProperty("isTeacher")]
        public bool IsTeacher { get; set; }

        /// <summary>
        /// 是否為組織管理者
        /// </summary>
       // [JsonProperty("isOrgAdmin")]
        public bool IsOrgAdmin { get; set; }

        /// <summary>
        /// 組織編號
        /// </summary>
        [JsonProperty("orgId")]
        public int? OrgId{get; set; }

        /// <summary>
        /// 學習地圖中，成員的基本資訊
        /// </summary>
        [JsonProperty("extraInfo")]
        public LoginLearningMapBasic ExtraInfo { get; set; }
    }
}
