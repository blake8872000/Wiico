using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MemberManage
{
    public class MemberPhotoRequest : Base.BackendBaseRequest
    {
        /// <summary>
        /// 使用者照片資料(base64)
        /// </summary>
        public string Photo { get; set; }
    }
}
