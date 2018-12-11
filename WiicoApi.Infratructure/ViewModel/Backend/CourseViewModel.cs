using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class CourseViewModel
    {
        /// <summary>
        /// 課程基本欄位
        /// </summary>
        public Course CourseInfo { get; set; }

        /// <summary>
        /// 課程擴充欄位
        /// </summary>
        public List<CourseExt> ExtensionInfo { get; set; }

        /// <summary>
        /// 擴充欄位詳細資料
        /// </summary>
        public List<ExtensionColumn> ExtColumn { get; set; }
    }
}
