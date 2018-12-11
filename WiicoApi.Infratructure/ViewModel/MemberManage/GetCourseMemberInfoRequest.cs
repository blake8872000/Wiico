using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MemberManage
{
    public class GetCourseMemberInfoRequest : Base.BackendBaseRequest
    {
        /// <summary>
        /// 要查詢的帳號
        /// </summary>
        [JsonProperty("queryAccount")]
        public string QueryAccount { get; set; }
    }
}
