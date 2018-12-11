using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 公版活動資料表
    /// </summary>
    public class ActGeneral
    {
        /// <summary>
        /// 流水編號
        /// </summary>
        [Key,JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// 公版活動名稱
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 活動摘要
        /// </summary>
        [JsonProperty("content")]
        public string Content{ get; set; }

        /// <summary>
        /// 公版活動代碼
        /// </summary>
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        /// <summary>
        /// 所屬學習圈編號
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId  { get; set; }

        /// <summary>
        /// 所屬活動類型
        /// </summary>
        [JsonProperty("actType")]
        public string ActType { get; set; }

        /// <summary>
        /// 發布日期
        /// </summary>
        [JsonProperty("publish_utc")]
        public DateTime? Publish_Utc  { get; set; }

        /// <summary>
        /// 貼圖
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 預覽圖
        /// </summary>
        [JsonProperty("picture")]
        public string Picture { get; set; }

        /// <summary>
        /// 是否直接導入連結 - 是的話直接開url 不是就導入webView
        /// </summary>
        [JsonProperty("isWebView")]
        public bool IsWebView { get; set; }

        /// <summary>
        /// 類型 [WebView / Browser]
        /// </summary>
        [JsonProperty("target_type")]
        public int Target_Type { get; set; }

        /// <summary>
        /// 導入頁
        /// </summary>
        [JsonProperty("target_url")]
        public string Target_Url { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        [JsonProperty("creator")]
        public int? Creator { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        [JsonProperty("updater")]
        public int? Updater { get; set; }

        /// <summary>
        /// 刪除者
        /// </summary>
        [JsonProperty("deleter")]
        public int? Deleter { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [JsonIgnore]
        public DateTime? CreateDate_Utc { get; set; }

        /// <summary>
        /// 顯示建立日期
        /// </summary>
        [JsonProperty("createDate"), NotMapped]
        public DateTime? CreateDate
        {
            get
            {
                if (CreateDate_Utc.HasValue)
                {
                    return CreateDate_Utc.Value.ToLocalTime();
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 更新日期
        /// </summary>
        [JsonIgnore]
        public DateTime? UpdateDate_Utc { get; set; }

        /// <summary>
        /// 顯示更新日期
        /// </summary>
        [JsonProperty("updateDate"), NotMapped]
        public DateTime? UpdateDate
        {
            get
            {
                if (UpdateDate_Utc.HasValue)
                {
                    return UpdateDate_Utc.Value.ToLocalTime();
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 刪除日期
        /// </summary>
        [JsonIgnore]
        public DateTime? DeleteDate_Utc { get; set; }

        /// <summary>
        /// 顯示刪除日期
        /// </summary>
        [JsonProperty("deleteDate"), NotMapped]
        public DateTime? DeleteDate { get {
                if (DeleteDate_Utc.HasValue)
                {
                    return DeleteDate_Utc.Value.ToLocalTime();
                }
                else
                    return null;
            }
        }
    }
}
