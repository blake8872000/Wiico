
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    /// <summary>
    /// 給APP的上傳資料物件
    /// </summary>
    public class HomeWorkViewModel : ValueObject.ActivityBase
    {
        /// <summary>
        /// 活動編號
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId { get; set; }

        /// <summary>
        /// 活動 Guid
        /// </summary>
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        /// <summary>
        /// 作業活動名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 建立者id
        /// </summary>
        [JsonIgnore]
        public string Creator { get; set; }


        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 作業活動說明描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 作業空間
        /// </summary>
        [JsonProperty("googleDriveFolder")]
        public string GoogleDriveFolder { get; set; }

        /// <summary>
        /// 講義總數
        /// </summary>
        [JsonProperty("lectureCount")]
        public int LectureCount { get; set; }

        /// <summary>
        /// 結束日期
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 遲交日期
        /// </summary>
        [JsonProperty("overdueDate")]
        public DateTime? OverdueDate { get; set; }

        /// <summary>
        /// 允許發布成績
        /// </summary>
        [JsonProperty("allowRelease")]
        public bool AllowRelease { get; set; }

        /// <summary>
        /// 允許遲交
        /// </summary>
        [JsonProperty("allowOverDue")]
        public bool AllowOverDue { get; set; }

        /// <summary>
        /// 已經發布過
        /// </summary>
        [JsonProperty("released")]
        public bool Released { get; set; }

        /// <summary>
        /// 是否為分組作業
        /// </summary>

        [JsonProperty("isGroup")]
        public bool IsGroup { get; set; }
        /// <summary>
        /// 上傳表 [學生id, 上傳狀態]
        /// </summary>
        [JsonProperty("me")]
        public BusinessObject.MemberUploadStatus Me { get; set; }

        /// <summary>
        /// 上傳表[學生id, 姓名, 大頭照URL, 上傳狀態]
        /// </summary>
        [JsonProperty("memberUploadStatus")]
        public List<BusinessObject.MemberUploadStatus> MemberUploadStatus { get; set; }

        /// <summary>
        /// 上傳狀態統計 [上傳狀態(Name 中文), 人數]
        /// </summary>
        [JsonProperty("uploadCount")]
        public List<BusinessObject.UploadCount> UploadCount { get; set; }

        /// <summary>
        /// 分組情形
        /// </summary>
        [JsonProperty("groupInfo")]
        public List<BusinessObject.GroupInfo> GroupInfo { get; set; }


        /// <summary>
        /// 要顯示的頁數
        /// </summary>
        public int? Pages { get; set; }
        /// <summary>
        /// 要顯示的數量
        /// </summary>
        public int? Rows { get; set; }
    }
}
