using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class MaterialEvent
    {
        /// <summary>
        /// 活動id
        /// </summary>
        [JsonProperty("outerKey")]
        public Guid OuterKey { get; set; }

        /// <summary>
        /// 建立帳號
        /// </summary>
        [JsonProperty("createAccount")]
        public string CreateAccount { get; set; }

        /// <summary>
        /// 課id
        /// </summary>
        [JsonProperty("classId")]
        public string ClassId { get; set; }

        /// <summary>
        /// 建立者id
        /// </summary>
        [JsonProperty("creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }



        Dictionary<string, int> _uploadSheet;
        /// <summary>
        /// 上傳表 {檔案類型 ,檔案大小}
        /// </summary>
        public Dictionary<string, int> UploadSheet
        {
            get
            {
                if (_uploadSheet == null)
                    _uploadSheet = new Dictionary<string, int>();
                return _uploadSheet;
            }
            set { _uploadSheet = value; }
        }
    }
}
