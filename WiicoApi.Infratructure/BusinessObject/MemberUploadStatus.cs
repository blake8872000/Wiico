using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    /// <summary>
    /// 每位成員上傳資訊
    /// </summary>
    public class MemberUploadStatus
    {
        /// <summary>
        /// 成員 memberId
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 成員姓名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 成員照片
        /// </summary>
        [JsonProperty("pic")]
        public string Pic { get; set; }

        /// <summary>
        /// 上傳狀態 int.toString()
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("score")]
        public int? Score { get; set; }

        /// <summary>
        /// 發布成績
        /// </summary>
        [JsonProperty("releaseScore")]
        public int? ReleaseScore { get; set; }

        /// <summary>
        /// 退回成績
        /// </summary>
        [JsonProperty("backScore")]
        public int? BackScore { get; set; }


        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("fileCount")]
        public int fileCount { get; set; }

        [JsonProperty("googleDriveParentFileId")]
        public string GoogleDriveParentFileId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// 學生上傳檔案 紀錄的Id
        /// </summary>
        [JsonProperty("logId")]
        public int LogId { get; set; }

        /// <summary>
        /// 學生上傳作業草稿區
        /// </summary>
        [JsonProperty("tempGoogleDriveFolder")]
        public string TempGoogleDriveFolder { get; set; }


        /// <summary>
        /// 是否允許上傳
        /// </summary>
        [JsonProperty("allowSend")]
        public bool AllowSend { get; set; }


        /// <summary>
        /// 是否為組長
        /// </summary>
        [JsonProperty("isLeader")]
        public bool IsLeader { get; set; }

        /// <summary>
        /// 最後編輯時間
        /// </summary>
        [JsonProperty("updateDate")]
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 最後編輯時間
        /// </summary>        
        public double jUpdateDate
        {
            get
            {
                if (UpdateDate == null)
                    return -1;
                else
                    return UpdateDate.Value.ToLocalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalMilliseconds;
            }
        }
    }
}
