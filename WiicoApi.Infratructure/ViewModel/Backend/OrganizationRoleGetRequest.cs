using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class OrganizationRoleGetRequest : BackendBaseRequest
    {

        /// <summary>
        /// 查詢條件 - 編號 | 名稱 | 代碼
        /// </summary>
        [JsonProperty("search")]
        public string Search { get; set; }

    }
}
