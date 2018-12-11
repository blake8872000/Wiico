using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class CircleMemberSettingAuth : CircleInfoSettingAuth
    {
        [JsonProperty("addCircleMember")]
        public bool AddCircleMember { get; set; }
        [JsonProperty("deleteCircleMember")]
        public bool DeleteCircleMember { get; set; }
        [JsonProperty("editLevelOne")]
        public bool EditLevelOne { get; set; }
    }
}
