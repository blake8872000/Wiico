using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class DeptManagePostRequest : BackendBaseRequest
    {
        /// <summary>
        /// 欲建立的分類名稱
        /// </summary>
        [JsonProperty("deptName")]
        public string DeptName { get; set; }
        /// <summary>
        /// 欲建立的分類代碼
        /// </summary>
        [JsonProperty("deptCode")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 分類簡寫
        /// </summary>
        [JsonProperty("shortName")]
        public string ShortName { get; set; }
        /// <summary>
        /// 所屬分類編號
        /// </summary>
        [JsonProperty("parentId")]
        public int? ParentId { get; set; }

    }
}
