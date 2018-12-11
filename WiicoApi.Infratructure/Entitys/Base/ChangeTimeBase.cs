using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WiicoApi.Infrastructure.Entity.Base
{
    public class ChangeTimeBase : TimeMarkBase
    {
        /// <summary>
        /// 建立者 MemberId
        /// </summary>
        [JsonIgnore]
        public int? CreateUser { get; set; }

        /// <summary>
        /// 最後修改者 MemberId
        /// </summary>
        [JsonIgnore]
        public int? UpdateUser { get; set; }

        /// <summary>
        /// 刪除者 MemberId
        /// </summary>
        [JsonIgnore]
        public int? DeleteUser { get; set; }
    }
}
