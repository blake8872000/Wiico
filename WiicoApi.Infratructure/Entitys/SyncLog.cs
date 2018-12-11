using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 用於同步iCan5的比對資料表
    /// </summary>
    public class SyncLog
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 要匯入的iCan5的table
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 目前更新時間 - 比對iCan5資料表是否需要更新用
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
