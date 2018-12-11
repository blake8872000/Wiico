using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class GetAbSenceFormDetailRequest :  Base.BackendBaseRequest
    {
        /// <summary>
        /// 課程代碼
        /// </summary>
        [JsonProperty("classID")]
        public string ClassID { get; set; }
        /// <summary>
        /// 操作的項目代碼
        /// </summary>
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
    }
}
