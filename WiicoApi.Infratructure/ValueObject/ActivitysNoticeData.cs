using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject 
{
    public class ActivitysNoticeData : Infrastructure.ViewModel.Base.BaseViewModel
    {
        [JsonIgnore]
        public int UnreadCount { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        private string _toRoomId;
        [JsonProperty("circleKey")]
        public string ToRoomId { get { return _toRoomId?.ToLower(); } set { _toRoomId = value; } }

        [JsonProperty("circleId")]
        public int CircleId { get; set; }

        [JsonProperty("circleName")]
        public string CircleName { get; set; }

        [JsonProperty("moduleKey")]
        public string ModuleKey { get; set; }

        [JsonIgnore]
        public string NoticeContent { get; set; }

        [JsonIgnore]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text
        {
            get
            {
                return string.Format("{0}{1}", Title, NoticeContent);
            }
        }

        [JsonProperty("readMark")]
        public bool HasClick { get; set; }

        [JsonIgnore]
        public DateTime CreateTime { get; set; }

        [JsonProperty("createTime")]
        public DateTime Date_Local { get { return CreateTime.ToLocalTime(); } }
        
        [JsonIgnore]
        public DateTime? DeleteTime { get; set; }

        [JsonProperty("isDelete")]
        public bool IsDelete { get; set; }
    }
}
