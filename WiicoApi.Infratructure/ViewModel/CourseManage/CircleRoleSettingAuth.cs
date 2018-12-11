using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class CircleRoleSettingAuth : CircleInfoSettingAuth
    {
        [JsonProperty("addCircleRole")]
        public bool AddCircleRole { get; set; }
        [JsonProperty("addLevelOne")]
        public bool AddLevelOne { get; set; }
        [JsonProperty("deleteCircleRole")]
        public bool DeleteCircleRole { get; set; }
    }
}