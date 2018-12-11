using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    /// <summary>
    /// 
    /// </summary>
    public class CircleCacheData
    {
        /// <summary>
        /// 課程代號 (Learning Id)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 課程代碼
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 課程名稱
        /// </summary>
        public string Name { get; set; }
    }
}
