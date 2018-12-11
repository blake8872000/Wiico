﻿using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class TimeTableGetRequest : Base.BackendBaseRequest
    {
        /// <summary>
        /// 開始日期
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 結束日期
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("weekTable")]
        public List<WeekTable> WeekTable { get; set; }
    }
}
