using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class LCExtensionValue 
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 擴充欄位的值
        /// </summary>
        public string TextValue { get; set; }

        /// <summary>
        /// 欄位代碼
        /// </summary>
        public int ColumnId { get; set; }

        /// <summary>
        /// 學習圈的代碼
        /// </summary>
        public int DataId { get; set; }
        /// <summary>
        /// 是否為外部人員 - 有值代表是外部
        /// </summary>
        public int? ExternalRid { get; set; }

    }
}
