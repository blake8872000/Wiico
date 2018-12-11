using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.uDollar
{

    public class OnlineBalanceResult
    {
        /// <summary>
        /// uDollar剩餘金額(判斷餘額是否足夠時請以此金額進行判斷)
        /// </summary>
        public decimal uDollarAmount { get; set; }
        /// <summary>
        /// 已經預購的商品金額總數
        /// </summary>
        public int orderAmount { get; set; }
        /// <summary>
        /// 查詢的帳號
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 系統估算uPurse剩餘金額
        /// </summary>
        public decimal uPursePerhapsBlance { get; set; }
    }
}
