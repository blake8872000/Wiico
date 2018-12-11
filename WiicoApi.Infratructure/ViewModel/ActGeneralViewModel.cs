using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class ActGeneralViewModel : ValueObject.ActivityBase
    {
        /// <summary>
        /// 公版活動名稱
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// 活動摘要
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// 所屬活動類型
        /// </summary>
        [JsonProperty("activity_type")]
        public string ActType { get; set; }

        /// <summary>
        /// 貼圖
        /// </summary>
        [JsonProperty("icon_url")]
        public string Icon { get; set; }

        /// <summary>
        /// 預覽圖
        /// </summary>
        [JsonProperty("picture_url")]
        public string Picture { get; set; }

        /// <summary>
        /// 類型 [WebView: 0 / Browser: 1]
        /// </summary>
        [JsonProperty("target_type")]
        public int Target_Type { get; set; }

        /// <summary>
        /// 顯示類型
        /// </summary>
        [JsonProperty("target_type_name")]
        public string Target_Type_Name
        {
            get
            {
                switch (Target_Type)
                {
                    case 0:
                        return "開啟webView";
                    case 1:
                        return "開啟browser";
                    case 2:
                        return "開啟file";
                    default:
                        return "啥都不開";
                }
            }
        }

        /// <summary>
        /// 導入頁
        /// </summary>
        [JsonProperty("target_url")]
        public string Target_Url { get; set; }
        /// <summary>
        /// 建立者
        /// </summary>
        [JsonProperty("creator")]
        public int Creator { get; set; }

        /// <summary>
        /// 是否顯示
        /// </summary>
        [JsonProperty("cardisshow")]
        public bool? CardIsShow { get; set; }
    }
}
