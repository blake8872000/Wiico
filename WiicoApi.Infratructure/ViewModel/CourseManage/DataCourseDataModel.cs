using WiicoApi.Infrastructure.ViewModel.MemberManage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class DataCourseDataModel
    {
        /// <summary>
        /// 課程編號
        /// </summary>
    //    [JsonProperty("classId")]
        public string ClassId { get; set; }
        /// <summary>
        /// 課程名稱
        /// </summary>
     //   [JsonProperty("className")]
        public string ClassName { get; set; }
        /// <summary>
        /// 教師姓名
        /// </summary>
   //     [JsonProperty("classTeacher")]
        public string ClassTeacher { get; set; }
        /// <summary>
        /// 課程教師照片連結位置(課程中可能有多位教師，以陣列方式回傳)
        /// </summary>
       // [JsonProperty("teacherPhoto")]
        public TeacherPhotoInfo[] TeacherPhoto { get; set; }
        /// <summary>
        /// 課程公告尚未讀取的項目數
        /// </summary>
     //   [JsonProperty("bulletinUnreadCount")]
        public int BulletinUnreadCount { get; set; }
        /// <summary>
        /// 課程參考資料尚未讀取的項目數
        /// </summary>
    //    [JsonProperty("materialUnreadCount")]
        public int MaterialUnreadCount { get; set; }
        /// <summary>
        /// 模組ID
        /// </summary>
   //     [JsonProperty("classDomainId")]
        public string ClassDomainId { get; set; }
        /// <summary>
        /// 模組名稱
        /// </summary>
   //     [JsonProperty("classDomainName")]
        public string ClassDomainName { get; set; }
        /// <summary>
        /// 課程主旨
        /// </summary>
     //   [JsonProperty("classSubjectName")]
        public string ClassSubjectName { get; set; }
        /// <summary>
        /// 系級(eg:進學資管20)
        /// </summary>
   //     [JsonProperty("collInfo")]
        public string CollInfo { get; set; }

        /// <summary>
        /// 上課人數
        /// </summary>
        public int MemberCount { get; set; }
        /// <summary>
        /// 開始上課日
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 最後上課日
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 上課時間表
        /// </summary>
        public List<GetAllMyCourseWeekTable> WeekTable { get; set; }
    }
}
