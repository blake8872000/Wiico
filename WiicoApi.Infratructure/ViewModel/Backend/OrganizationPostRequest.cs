using WiicoApi.Infrastructure.ViewModel.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 建立組織參數
    /// </summary>
    public class OrganizationPostRequest : BackendBaseRequest
    {
        /// <summary>
        /// 組織名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }


        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("semesterLength")]
        public int? SemesterLength { get; set; }
    }
}
