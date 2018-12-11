using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class GroupInfoViewModel
    {
        /// <summary>
        /// 組別編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonProperty("sortNumber")]
        public int SortNumber { get; set; }

        /// <summary>
        /// 分組名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 刪除日期
        /// </summary>
        [JsonIgnore]
        public DateTime? DeleteDateUtc { get; set; }

        /// <summary>
        /// 是否刪除
        /// </summary>
        [JsonProperty("isDelete")]
        public bool IsDelete
        {
            get
            {
                if (DeleteDateUtc.HasValue)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 分組成員
        /// </summary>
        [JsonProperty("groupMembers")]
        public List<ValueObject.GroupMember> GroupMembers { get; set; }
    }
}
