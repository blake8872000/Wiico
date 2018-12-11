using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class GoogleFile
    {
        /// <summary>
        /// 檔案編號
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 所屬檔案代碼GoogleDriveFileId
        /// </summary>
        [JsonProperty("fileId")]
        public string FileId { get; set; }
        /// <summary>
        /// 所屬資料夾GoogleDriveFolderId
        /// </summary>
        [JsonProperty("parentFileId")]
        public string ParentFileId { get; set; }
        /// <summary>
        /// 檔案名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 預覽網址
        /// </summary>
        [JsonProperty("imgUrl")]
        public string ImgUrl { get; set; }
        /// <summary>
        /// 網站顯示網址
        /// </summary>
        [JsonProperty("webViewUrl")]
        public string WebViewUrl { get; set; }
        /// <summary>
        /// 下載網址
        /// </summary>
        [JsonProperty("downLoadUrl")]
        public string DownLoadUrl { get; set; }
        /// <summary>
        /// 檔案類型
        /// </summary>
        [JsonProperty("fileType")]
        public string FileType { get; set; }
        /// <summary>
        /// 檔案大小
        /// </summary>
        [JsonProperty("size")]
        public long? Size { get; set; }
        /// <summary>
        /// 建立者
        /// </summary>
        [JsonProperty("create_User")]
        public int Create_User { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("create_Utc")]
        public DateTime Create_Utc { get; set; }


    }
}
