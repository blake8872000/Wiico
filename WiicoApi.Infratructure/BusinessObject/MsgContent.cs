using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    /// <summary>
    /// 留言詳細資訊
    /// </summary>
    public class MsgContent
    {
        /// <summary>
        /// 留言詳細資訊
        /// </summary>
        [JsonProperty("msgInfo")]
        public ActModuleMessage MsgInfo { get; set; }
        /// <summary>
        /// 留言點讚資訊
        /// </summary>
        [JsonProperty("like")]
        public LikeCount Like { get; set; }
        /// <summary>
        /// 成員資訊
        /// </summary>
        [JsonProperty("memberInfo")]
        public ValueObject.LearningCircleMemberInfo MemberInfo { get; set; }
        /// <summary>
        /// 是否為自己留言的
        /// </summary>
        [JsonProperty("isMy")]
        public bool IsMy { get; set; }

    }
}
