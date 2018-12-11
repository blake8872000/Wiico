using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class OrganizationDeleteRequest : BackendBaseRequest
    {
        /// <summary>
        /// 組織編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

    }
}
