using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 組織編輯API用
    /// </summary>
    public class OrganizationPutRequest : OrganizationPostRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
