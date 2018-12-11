using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ExternalResType : Base.EntityBase
    {
        [JsonProperty("name")]
        public override string Name { get; set; }

        /// <summary>
        /// 同步的類型代碼
        /// </summary>
        public string AsyncTypeCode { get; set; }
        /// <summary>
        /// 優先同步排序
        /// </summary>
        public int? Sort { get; set; }
        public virtual ICollection<ExternalResource> ExternalResources { get; set; }
    }
}
