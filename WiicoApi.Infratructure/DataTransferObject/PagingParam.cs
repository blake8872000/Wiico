using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Repository
{
    public class SortParam
    {
        public int MaxResult { get; set; }
        public int MemberId { get; set; }
        public string CircleKey { get; set; }
        public bool Goback { get; set; }
        /// <summary>
        /// 用於查詢特定日期
        /// </summary>
        public DateTime QueryDateTime { get; set; }
    }
}
