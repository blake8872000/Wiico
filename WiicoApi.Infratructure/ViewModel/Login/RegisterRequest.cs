using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Login
{
    /// <summary>
    /// 註冊要求Model
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 註冊帳號
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 註冊密碼 - 3DES加密過
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 註冊信箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 註冊學校 - 代碼[註冊頁面提供]
        /// </summary>
        public string  OrgCode{ get; set; }

        /// <summary>
        /// 註冊姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色代碼 1:系統管理者[開發者] | 2:一般使用者 [老師、學生、教務帳號]
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 使用者編號 - 人員管理用
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 是否有效 - 人員管理用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        ///使用者代碼 - 驗證使用者是否有權限建立帳號
        /// </summary>
        public string Token { get; set; }
    }
}
