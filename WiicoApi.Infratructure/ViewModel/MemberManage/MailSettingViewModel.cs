using WiicoApi.Infrastructure.ViewModel.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.MemberManage
{
    public class MailSettingViewModel : Base.BackendBaseRequest
    {
        /// <summary>
        /// 是否公開個人email
        /// </summary>
        public bool ShowMail { get; set; }
        /// <summary>
        /// 個人email位置
        /// </summary>
        public string EmailAddress { get; set; }
    }
}
