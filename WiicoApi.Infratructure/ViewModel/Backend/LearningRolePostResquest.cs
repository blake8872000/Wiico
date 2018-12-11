using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class LearningRolePostResquest : Base.BackendBaseRequest
    {
        [JsonProperty("roles")]
        public List<LearningRolePostRole> Roles { get; set; }
    }
}
