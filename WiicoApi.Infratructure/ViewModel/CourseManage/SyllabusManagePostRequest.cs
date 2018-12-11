using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class SyllabusManagePostRequest
    {
        /// <summary>
        /// 進度課綱編號
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// 進度課綱名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 進度課綱所屬學習圈代碼
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        /// <summary>
        /// 進度課綱說明
        /// </summary>
        [JsonProperty("note")]
        public string Note{ get; set; }
        /// <summary>
        /// 進度課綱日期
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        /// <summary>
        /// 進度課綱排序
        /// </summary>
        [JsonProperty("sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool? Enable { get; set; }
        /// <summary>
        /// 建立者代碼
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// 課程章節列表
        /// </summary>
        [JsonProperty("syllabuses")]
        public List<SyllabusData>Syllabuses { get; set; }

    }
}
