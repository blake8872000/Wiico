
using WiicoApi.Infrastructure.ValueObject;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WiicoApi.Infrastructure.Property;

namespace WiicoApi.Infrastructure.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivitysViewModel : ActivityBase, IPagingData
    {
        /// <summary>
        /// 標記 - 已讀:true，未讀:false
        /// </summary>
        [NotMapped, JsonProperty("readMark")]
        public virtual bool ReadMark { get; set; }

        /// <summary>
        /// 已讀數量
        /// </summary>
        [JsonProperty("readCount")]
        public virtual int ReadCount { get; set; }

        /// <summary>
        /// 是否為起始位置
        /// </summary>
        [JsonProperty("positionMark")]
        public virtual bool PositionMark { get; set; }

        [JsonIgnore]
        public long RowNum { get; set; }

        [JsonIgnore]
        public int RollCallId { get; set; }
    }
}