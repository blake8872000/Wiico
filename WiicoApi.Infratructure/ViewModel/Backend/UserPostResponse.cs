using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 註冊帳號回傳model
    /// </summary>
    public class UserPostResponse
    {
        /// <summary>
        /// 是否公開信箱
        /// </summary>
        public bool IsShowMail { get; set; }
        /// <summary>
        /// 是否為匯入 通常為0 不是匯入的
        /// </summary>
        public int? ExternalRid { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 是否認證
        /// </summary>
        public bool Verified { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 建立者帳號
        /// </summary>
        public string CreateAccount { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
