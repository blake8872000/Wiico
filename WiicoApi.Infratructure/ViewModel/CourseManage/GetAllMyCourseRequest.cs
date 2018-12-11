using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class GetAllMyCourseRequest : Base.BackendBaseRequest
    {
        /// <summary>
        /// 上次資料同步時間
        /// 當讀取校園大小事件/校園行事曆/個人歷年課程/個人目前課程/beacon對照表,此欄位比對資料有無更新
        /// </summary>
        [JsonProperty("lastSync")]
        public DateTime? LastSync { get; set; }
    }
}
