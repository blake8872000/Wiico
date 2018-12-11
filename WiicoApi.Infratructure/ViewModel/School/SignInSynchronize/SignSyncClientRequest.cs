using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School.SignInSynchronize
{
    public class SignSyncClientRequest : Infrastructure.ViewModel.Base.BackendBaseRequest
    {
        /// <summary>
        /// 課程代碼
        /// </summary>
        public string ClassID { get; set; }
        /// <summary>
        /// 第幾次點名
        /// </summary>
        public int Times { get; set; }
        /// <summary>
        /// 大綱(進度)ID
        /// </summary>
        public string syll_id { get; set; }
        /// <summary>
        /// 課程成員清單
        /// </summary>
        public List<SignSyncMemberListRequest> MemberList { get; set; } 
    }
}
