using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class LearningRolePostRole
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("level")]
        public int Level{ get; set; }
        [JsonProperty("id")]
        public int RoleId { get; set; }
    }
}
