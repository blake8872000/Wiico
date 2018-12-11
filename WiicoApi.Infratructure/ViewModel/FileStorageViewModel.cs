using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;

namespace WiicoApi.Infrastructure.ViewModel
{
    [NotMapped]
    public class FileStorageViewModel : FileStorage
    {
        /// <summary>
        /// 刪除日期
        /// </summary>
        [JsonProperty("deleteTime")]

        public DateTime? DeleteTime { get { if (DeleteUtcDate.HasValue) return DeleteUtcDate.Value.ToLocalTime(); else return null; } }
        /// <summary>
        /// 檔案縮圖網址
        /// </summary>
       // [JsonProperty("fileImageUrl")]
        [JsonProperty("fileImageUrl")]
        public string FileImageUrl { get; set; }
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
        /// 建立日期
        /// </summary>
        [JsonProperty("createTime")]

        public DateTime CreateTime { get { return CreateUtcDate.ToLocalTime(); } }

    }
}
