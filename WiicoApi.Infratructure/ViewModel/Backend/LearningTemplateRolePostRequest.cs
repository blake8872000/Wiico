using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class LearningTemplateRolePostRequest :BackendBaseRequest
    {

        [JsonProperty("level")]
        public int Level { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("roleCode")]
        public string RoleCode { get; set; }
        [JsonProperty("courseRoles")]
        public List<LearningTemplateRoles> CourseRoles { get; set; }
    }
}
