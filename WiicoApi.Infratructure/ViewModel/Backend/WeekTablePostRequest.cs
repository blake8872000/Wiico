using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class WeekTablePostRequest :Base.BackendBaseRequest
    {
        [JsonProperty("weekTableData")]
        public WeekTableViewModel WeekTableData { get; set; }
    }
}
