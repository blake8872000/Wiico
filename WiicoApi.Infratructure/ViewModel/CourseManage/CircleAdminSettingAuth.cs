using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class CircleAdminSettingAuth : CircleInfoSettingAuth
    {
        [JsonProperty("deleteCircleAdmin")]
        public bool DeleteCircleAdmin { get; set; }
    }
}
