using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class LeaveListViewModel
    {
        [JsonProperty("isManager")]
        public bool IsManager { get; set; }
        [JsonProperty("leaveList")]
        public List<LeaveViewModel> LeaveList { get; set; }
    }
}
