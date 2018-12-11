using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class LearningAuth : Base.ChangeTimeBase
    {
        /// <summary>
        /// 學習圈角色功能編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 學習圈成員角色關聯的編號[LearningRole的Id] - 流水號
        /// </summary>
        public int LearningRoleId { get; set; }

        /// <summary>
        /// 模組"功能"的編號 - 流水號
        /// </summary>
        public int FunctionId { get; set; }
       
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }
    }
}
