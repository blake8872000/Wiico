using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Service.Utility
{
    public class PagesRowsTools
    {
        /// <summary>
        /// 下一頁
        /// </summary>
        /// <param name="activePage">目前頁數</param>
        /// <param name="count">總數</param>
        /// <param name="rows">每頁筆數</param>
        /// <returns></returns>
        public int NextPage(int activePage, int count, int rows)
        {
            var activeRows = activePage * rows;
            if (activeRows >= count)
                return 0;
            else if (activeRows < count)
                return activePage + 1;
            else
                return 0;
        }

        /// <summary>
        /// 取得總共頁數
        /// </summary>
        /// <param name="count"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public int CountPage(decimal count, decimal rows)
        {
            return Convert.ToInt32(Math.Ceiling(count / rows));
        }
    }
}
