using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MemberManage
{
    public class MemberPhotoResponse
    {
        /// <summary>
        /// 上傳照片的使用者
        /// </summary>
     //   [JsonProperty("acpdId")]
        public string AcpdId { get; set; }
        /// <summary>
        /// 上傳後的照片連結位置
        /// </summary>
      //  [JsonProperty("photo")]
        public string Photo { get; set; }
    }
}
