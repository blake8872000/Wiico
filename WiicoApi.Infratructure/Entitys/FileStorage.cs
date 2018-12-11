using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 檔案倉庫資料表-用於存放檔案專區
    /// </summary>
    public class FileStorage
    {
        /// <summary>
        /// 檔案儲存流水號
        /// </summary>
        [ JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 檔案名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 檔案代碼
        /// </summary>
        [JsonProperty("fileGuid")]
        public Guid FileGuid { get; set; }

        /// <summary>
        /// 檔案大小
        /// </summary>
        [ JsonProperty("fileSize")]
        public int FileSize { get; set; }

        /// <summary>
        /// 檔案類型
        /// </summary>
        [ JsonProperty("fileContentType")]
        public string FileContentType { get; set; }

        /// <summary>
        /// 檔案圖片寬度 - 可NULL，代表非圖片
        /// </summary>
        [ JsonProperty("fileImageWidth")]
        public int? FileImageWidth { get; set; }

        /// <summary>
        /// 檔案圖片高度 - 可NULL，代表非圖片
        /// </summary>
        [ JsonProperty("fileImageHeight")]
        public int? FileImageHeight { get; set; }

        /// <summary>
        ///  建立者編號
        /// </summary>
        [JsonIgnore]
        public int Creator { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [JsonIgnore]
        public DateTime CreateUtcDate { get; set; }

        /// <summary>
        /// 下載與預覽檔案網址
        /// </summary>
        [ JsonProperty("fileUrl")]
        public string FileUrl { get; set; }



        /// <summary>
        /// 刪除日期
        /// </summary>
        [JsonIgnore]
        public DateTime? DeleteUtcDate { get; set; }

    
        /// <summary>
        /// 刪除者
        /// </summary>
        [ JsonIgnore]
        public int? Deleter { get; set; }
    }
}
