using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Base
{
        /// <summary>
        /// API呼叫的回應狀態
        /// </summary>
        public enum LogState : int
        {
            /// <summary>
            /// 成功
            /// </summary>
            [Description("成功")]
            Suscess = 0,
            /// <summary>
            /// 沒有帳號
            /// </summary>
            [Description("沒有帳號")]
            NoAccount = 1,
            /// <summary>
            /// 密碼錯誤
            /// </summary>
            [Description("密碼錯誤")]
            PasswordError = 2,
            /// <summary>
            /// 查詢錯誤
            /// </summary>
            [Description("查詢錯誤")]
            Error = 3,
            /// <summary>
            /// 登出
            /// </summary>
            [Description("登出")]
            Logout = 4,
            /// <summary>
            /// PhoneID為空值
            /// </summary>
            [Description("PhoneID為空值")]
            PhoneIDError = 5,
            /// <summary>
            /// 資料沒有變更
            /// </summary>
            [Description("資料沒有變更")]
            DataNotModified = 6,
            /// <summary>
            /// 參數資料缺少必要欄位內容
            /// </summary>
            [Description("參數資料缺少必要欄位內容")]
            RequestDataError = 7,
            /// <summary>
            /// uDollar帳號異常
            /// </summary>
            [Description("uDollar帳號異常")]
            uDollarAccountError = 8


        }
    }

