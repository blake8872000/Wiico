using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    /// <summary>
    /// 學習圈詳細資訊
    /// </summary>
    public class LearningCircleViewModel
    {
        /// <summary>
        /// 學習圈基本欄位
        /// </summary>
        public LearningCircle LearningInfo { get; set; }

        /// <summary>
        /// 學習圈擴充欄位
        /// </summary>
        public List<LearningCircleExt> ExtensionInfo { get; set; }
    }
}
