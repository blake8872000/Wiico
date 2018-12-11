using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 擴充欄位的值
    /// </summary>
    public class ExtensionValue
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
        /// 資料來源的代碼
        /// </summary>
        public int DataId { get; set; }
    }
}
