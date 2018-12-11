using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 學習圈的擴充欄位
    /// </summary>
    public class LearningCircleExt
    {
        public ExtensionColumn Column { get; set; }

        public LCExtensionValue Value { get; set; }

    }
}
