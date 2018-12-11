using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public  class GetCourseDetailRequest : Base.BackendBaseRequest
    {
        /// <summary>
        /// 課程代碼
        /// </summary>
        [JsonProperty("classID")]
        public string ClassID { get; set; }
        /// <summary>
        /// 要查詢的日期(如沒有帶入API自動填入呼叫時間)
        /// </summary>
        [JsonProperty("checkDate")]
        public DateTime? CheckDate { get; set; }
    }
}
