using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class GetCourseSyllabusRequest : Base.BackendBaseRequest
    {
        /// <summary>
        /// 課程代碼
        /// </summary>
        [JsonProperty("classID")]
        public string ClassID { get; set; }
    }
}
