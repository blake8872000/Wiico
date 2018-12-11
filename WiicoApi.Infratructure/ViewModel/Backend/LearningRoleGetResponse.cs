using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class LearningRoleGetResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name{ get; set; }
        [JsonProperty("level")]
        public int? Level { get; set; }
        [JsonProperty("roleCode")]
        public string RoleCode { get; set; }
        [JsonProperty("isEdit")]
        public bool IsEdit { get; set; }
        [JsonProperty("isFixed")]
        public bool IsFixed { get; set; }
        [JsonIgnore]
        public int? ExternalRid { get; set; }
    }
}
