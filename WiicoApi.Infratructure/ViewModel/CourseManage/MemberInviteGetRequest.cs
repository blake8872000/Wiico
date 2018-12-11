using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class MemberInviteGetRequest : Infrastructure.ViewModel.Base.BackendBaseRequest
    {
        /// <summary>
        /// 是否為前台查詢課程邀請碼 true : 前台查詢 | false : 後台查詢
        /// </summary>
        [JsonProperty("isMainCode")]
        public bool IsMainCode { get; set; }
    }
}
