using WiicoApi.Infrastructure.ViewModel.CourseManage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class LearningCircleGetResponse
    {
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("domainId")]
        public string DomainId { get; set; }
        [JsonProperty("domainName")]
        public string DomainName { get; set; }
        [JsonProperty("teachers")]
        public List<string> Teachers{ get; set; }
        [JsonProperty("weekTable")]
        public List<GetAllMyCourseWeekTable> WeekTable { get; set; }
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("collInfo")]
        public string CollInfo { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("objective")]
        public string Objective { get; set; }
        [JsonProperty("remark")]
        public string Remark { get; set; }
    }
}
