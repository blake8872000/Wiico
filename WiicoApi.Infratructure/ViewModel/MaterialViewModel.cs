using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class MaterialViewModel : ValueObject.ActivityBase
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
        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }


        /// <summary>
        /// 檔案的fileId
        /// </summary>
        [JsonProperty("googleDriveFileId")]
        public string GoogleDriveFileId { get; set; }
        /// <summary>
        /// 檔案類型
        /// </summary>
        [JsonProperty("fileType")]
        public string FileType { get; set; }

        /// <summary>
        /// 檔案大小
        /// </summary>
        [JsonProperty("fileLength")]
        public int FileLength { get; set; }

        /// <summary>
        /// 檔案縮圖
        /// </summary>
        [JsonProperty("fileImgUrl")]
        public string FileImgUrl { get; set; }
        /// <summary>
        /// 檔案網頁瀏覽
        /// </summary>
        [JsonProperty("fileWebViewUrl")]
        public string FileWebViewUrl { get; set; }
        /// <summary>
        /// 檔案下載
        /// </summary>
        [JsonProperty("fileDownLoadUrl")]
        public string FileDownLoadUrl { get; set; }
        /// <summary>
        /// 資料夾位置
        /// </summary>
        [JsonProperty("folderId")]
        public string FolderId { get; set; }

        /// <summary>
        /// 要顯示的頁數
        /// </summary>
        [JsonProperty("pages")]
        public int Pages { get; set; }
        /// <summary>
        /// 要顯示的數量
        /// </summary>
        [JsonProperty("rows")]
        public int Rows { get; set; }

    }
}
