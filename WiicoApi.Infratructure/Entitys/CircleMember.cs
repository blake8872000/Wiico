using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// Member與Learningcircle的關聯表
    /// </summary>
    public class CircleMember : Base.ChangeTimeBase
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 學習圈代碼
        /// </summary>
        public int CircleId { get; set; }
        /// <summary>
        /// 人物代碼
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 該成員所屬系所
        /// </summary>
        public int? DeptId { get; set; }
        /// <summary>
        /// 該成員在校年度
        /// </summary>
        public string MemberGrade { get; set; }
        /// <summary>
        /// 該成員在校班級
        /// </summary>
        public string MemberGroup { get; set; }
        /// <summary>
        /// 是否為外部人員 - 有值代表是外部
        /// </summary>
        public int? ExternalRid { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 異動原因
        /// </summary>
        public string UpdateReason { get; set; }
        /// <summary>
        /// 院系年班組 - 人員當時修課狀態
        /// </summary>
        public string MemberInfo{ get; set; }

    }
}
