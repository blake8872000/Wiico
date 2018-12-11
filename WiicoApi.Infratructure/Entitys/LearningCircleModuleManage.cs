using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class LearningCircleModuleManage : Base.ChangeTimeBase
    {
        /// <summary>
        /// 管理編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 所屬學習圈
        /// </summary>
        public string CircleKey { get; set; }
        /// <summary>
        /// 模組代碼
        /// </summary>
        public string ModuleKey { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 是否顯示
        /// </summary>
        public bool Visibility { get; set; }
    }
}
