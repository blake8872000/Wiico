using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class SignInData : SignInBase
    {
        public DateTime? Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int StuId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StudId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StudName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StudPhoto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LogCreator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LogUpdateDate { get; set; }
        /// <summary>
        /// 請假狀態
        /// </summary>
        [JsonProperty("leaveStatus")]
        public string LeaveStatus { get; set; }

        /// <summary>
        /// 請假代碼
        /// </summary>
        [JsonProperty("leaveEventId")]
        public Guid? LeaveEventId { get; set; }
    }
}
