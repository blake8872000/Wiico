using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    /// <summary>
    /// 分組情形
    /// </summary>
    public class GroupInfo
    {
        /// <summary>
        /// 組別代碼
        /// </summary>
        [JsonProperty("groupId")]
        public int GroupId { get; set; }
        /// <summary>
        /// 組別名稱
        /// </summary>
        [JsonProperty("groupName")]
        public string GroupName { get; set; }
        /// <summary>
        /// 上傳表[學生id, 姓名, 大頭照URL, 上傳狀態]
        /// </summary>
        [JsonProperty("memberUploadStatus")]
        public List<MemberUploadStatus> MemberUploadStatus { get; set; }
    }
}
