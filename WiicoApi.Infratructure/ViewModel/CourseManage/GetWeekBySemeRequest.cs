using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class GetWeekBySemeResponse
    {
        [JsonProperty("start_date")]
        public DateTime Start_date { get; set; }
        [JsonProperty("end_date")]
        public DateTime End_date { get; set; }
    }
}
