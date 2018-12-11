using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School.SignInSynchronize
{
    public class SignSynciCanMemberListRequest
    {
        /// <summary>
        /// 學號
        /// </summary>
        [JsonProperty("manno")]
        public string Manno { get; set; }
        /// <summary>
        /// 出勤狀態
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }
    }
}
