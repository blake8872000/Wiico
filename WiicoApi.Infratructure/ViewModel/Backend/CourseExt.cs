using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 課程擴充欄位資訊
    /// </summary>
    public class CourseExt
    {
        public int ColumnsId { get; set; }

        public string ColumnsName { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public int Sort { get; set; }

        public int EditorMultiLine { get; set; }

        public bool DisplayMultiLine { get; set; }

        public int EditorMaxLength { get; set; }

        public bool Enable { get; set; }

        public bool Editable { get; set; }
    }
}
