using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class ICan5Course
    {
        /// <summary>
        /// 流水號
        /// </summary>
        //[JsonProperty("id")]
        // public int Id { get; set; }
        /// <summary>
        /// 課程代碼
        /// </summary>
        [JsonProperty("course_no")]
        public string Course_No { get; set; }
        /// <summary>
        /// 課程名稱 - 中文
        /// </summary>
        [JsonProperty("cour_name_1000")]
        public string Cour_Name_1000 { get; set; }
        /// <summary>
        /// 課程名稱 - 英文
        /// </summary>
        [JsonProperty("cour_name_2000")]
        public string Cour_Name_2000 { get; set; }
        /// <summary>
        /// 學年度
        /// </summary>
        [JsonProperty("cour_year")]
        public string Cour_Year { get; set; }
        /// <summary>
        /// 學院代碼
        /// </summary>
        [JsonProperty("coll_no")]
        public string Coll_No { get; set; }
        /// <summary>
        /// 學分數
        /// </summary>
        //[JsonProperty("cour_grade")]
        //public int Cour_Grade { get; set; }
        /// <summary>
        /// 摘要 - 中文
        /// </summary>
        [JsonProperty("cour_comment_1000")]
        public string Cour_Comment_1000 { get; set; }
        /// <summary>
        /// 摘要 - 英文
        /// </summary>
        [JsonProperty("cour_comment_2000")]
        public string Cour_Comment_2000 { get; set; }
        /// <summary>
        /// 教材
        /// </summary>
        [JsonProperty("cour_teachitem")]
        public string Cour_Teachitem { get; set; }
        /// <summary>
        /// 其他教材
        /// </summary>
        [JsonProperty("cour_otheritem")]
        public string Cour_Otheritem { get; set; }
        /// <summary>
        /// 開始日期
        /// </summary>
        [JsonProperty("startdate")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("enddate")]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        [JsonProperty("createdate")]
        public DateTime? CreateDate { get; set; }
    }
}
