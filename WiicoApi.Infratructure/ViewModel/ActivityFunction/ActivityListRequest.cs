using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction
{
    public class ActivityListRequest :Base.BackendBaseRequest
    {
        /// <summary>
        /// 要求資料類型
        /// </summary>
        [JsonProperty("id")]
        public Infrastructure.ValueObject.activityEnum? Id { get; set; }
        /// <summary>
        /// 資料筆數(不篩選時,直接傳入null)
        /// </summary>
        [JsonProperty("rows")]
        public int? Rows { get; set; }
        /// <summary>
        /// 頁碼(不篩選時,直接傳入null)
        /// </summary>
        [JsonProperty("pages")]
        public int? Pages { get; set; }
    }
}
