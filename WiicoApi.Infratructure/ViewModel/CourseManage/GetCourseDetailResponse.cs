using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class GetCourseDetailResponse
    {
        /// <summary>
        /// 課程人員總數
        /// </summary>
     //   [JsonProperty("memberCount")]
        public int MemberCount { get; set; }
        /// <summary>
        /// 是否可以進行點名的功能(條件等同於身分為教師/助教,以及課程時間與現在時間比對後是可以進行點名動作的)
        /// </summary>
      //  [JsonProperty("edit")]
        public bool Edit { get; set; }
        /// <summary>
        /// 是否可以修改印象分數(條件等同身分為教師/助教)
        /// </summary>
    //    [JsonProperty("editImpression")]
        public bool EditImpression { get; set; }
        /// <summary>
        /// 課程ID(eg:1033CBNCE1CH39200)
        /// </summary>
    //    [JsonProperty("classId")]
        public string ClassId { get; set; }
        /// <summary>
        /// 課程名稱(eg:【人文公民素養】 世界文明巡禮)
        /// </summary>
    //    [JsonProperty("className")]
        public string ClassName { get; set; }
        /// <summary>
        /// 模組ID
        /// </summary>
    //    [JsonProperty("classDomainId")]
        public string ClassDomainId { get; set; }
        /// <summary>
        /// 模組名稱
        /// </summary>
     //   [JsonProperty("classDomainName")]
        public string ClassDomainName { get; set; }
        /// <summary>
        /// 課程主旨
        /// </summary>
    //    [JsonProperty("classSubjectName")]
        public string ClassSubjectName { get; set; }
        /// <summary>
        /// 系級(eg:進學資管20)
        /// </summary>
     //   [JsonProperty("collInfo")]
        public string CollInfo { get; set; }

        /// <summary>
        /// 上課老師
        /// </summary>
        public string ClassTeachers { get; set; }

        /// <summary>
        /// 上課地點與時間
        /// </summary>
     //   public List<string>ClassPlaceTime { get; set; }

        /// <summary>
        /// 上課時間表
        /// </summary>
        public List<GetAllMyCourseWeekTable> WeekTable { get; set; }

        /// <summary>
        /// 上課期間
        /// </summary>
        public string ClassPeriod { get; set; }

        /// <summary>
        /// 簡介
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 教學目標
        /// </summary>
        public string ClassTarget { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Note{ get; set; }
        /// <summary>
        /// 開始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
