using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 用於登入帳號時token與帳號綁訂的資料表
    /// </summary>
    [Description("用於登入帳號時token與帳號綁訂的資料表")]
    public class UserToken : Base.ChangeTimeBase
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Description("流水號")]
        public int Id { get; set; }

        /// <summary>
        /// 登入人員帳號編號 - 流水號
        /// </summary>
        [Description("登入人員帳號編號 - 流水號")]
        public int MemberId { get; set; }

        /// <summary>
        /// token
        /// </summary>
        [Description("token"), MaxLength(500)]
        public string Token { get; set; }

        /// <summary>
        /// 是否使用過
        /// </summary>
        [Description("是否使用過")]
        public bool TokenMark { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Description("是否有效")]
        public bool Enable { get; set; }

        /// <summary>
        /// 所屬組織代碼
        /// </summary>
        [Description("所屬組織代碼")]
        public int OrgId { get; set; }

        /// <summary>
        /// 模擬帳號編號
        /// </summary>
        [Description("模擬帳號")]
        public int? SimulationMember { get; set; }

        /// <summary>
        /// 裝置類型與裝置版號
        /// </summary>
        [Description("裝置類型與裝置版號"), MaxLength(100)]
        public string RequestSystem { get; set; }

        /// <summary>
        /// 裝置代碼
        /// </summary>
        [Description("裝置代碼"), MaxLength(128)]
        public string DeviceKey { get; set; }
        public bool IsOrgAdmin { get; set; }
        /// <summary>
        /// 推播代碼
        /// </summary>
        [Description("推播代碼"), MaxLength(500)]
        public string PushToken { get; set; }

    }
}
