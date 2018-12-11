using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 擴充欄位客製化
    /// </summary>
    public class ExtensionColumnCustomization 
    {
        /// <summary>
        /// 擴充欄位客製化編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 資料繫結代碼 - 資料代碼來源
        /// </summary>
        public int DataId { get; set; }

        /// <summary>
        /// 擴充欄位代碼
        /// </summary>
        public int ColumnId { get; set; }

        /// <summary>
        /// 是否顯示
        /// </summary>
        public bool Display { get; set; }

        /// <summary>
        /// 客製化排序
        /// </summary>
        public int Sort { get; set; }

    }
}
