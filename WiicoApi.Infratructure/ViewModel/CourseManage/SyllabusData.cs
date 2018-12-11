using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    /// <summary>
    /// Client來的資料
    /// </summary>
    public class SyllabusData
    {
        [JsonProperty("id")]
        public int? Id { get; set; }
        [JsonProperty("syllTitle")]
        public string Title { get; set; }
        /// <summary>
        /// 進度課綱說明
        /// </summary>
        [JsonProperty("syllNote")]
        public string Note { get; set; }
        [JsonProperty("syll_date")]
        public DateTime Date { get; set; }
        [JsonProperty("syllSort")]
        public int Sort { get; set; }
        [JsonProperty("syll_id")]
        public Guid SyllGuid{ get; set; }
    }
}
