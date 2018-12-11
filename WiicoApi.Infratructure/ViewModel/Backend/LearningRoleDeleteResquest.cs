using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class LearningRoleDeleteResquest : Infrastructure.ViewModel.Base.BackendBaseRequest
    {
        [JsonProperty("ids")]
        public IEnumerable<int> Ids { get; set; }
    }
}
