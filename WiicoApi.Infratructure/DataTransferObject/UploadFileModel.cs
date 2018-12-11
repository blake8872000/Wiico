using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.DataTransferObject
{
    public class UploadFileModel
    {
        [JsonProperty("token")]
        public Guid Token { get; set; }
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("endDate")]
        public string EndDate { get; set; }
        [JsonProperty("overdueDate")]
        public string OverdueDate { get; set; }
        [JsonProperty("outerKey")]
        public string OuterKey { get; set; }
        [JsonProperty("clientKey")]
        public string ClientKey { get; set; }

        public DateTime LeaveDate { get; set; }

        public int LeaveCategoryId { get; set; }
    }
}
