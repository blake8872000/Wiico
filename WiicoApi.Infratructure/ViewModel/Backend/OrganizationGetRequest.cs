using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class OrganizationGetRequest : BackendBaseRequest
    {
        /// <summary>
        /// 查詢的條件 : 名稱或代碼
        /// </summary>
        [JsonProperty("search")]
        public string Search { get; set; }

        /// <summary>
        /// 查詢頁數
        /// </summary>
        [JsonProperty("pages")]
        public int? Pages { get; set; }
        /// <summary>
        /// 查詢數量
        /// </summary>
        [JsonProperty("rows")]
        public int? Rows { get; set; }
    }
}
