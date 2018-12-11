using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WiicoApi.Infrastructure;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 課程資訊
    /// </summary>
    public partial class Course : Base.EntityBase
    {
        /// <summary>
        /// 課程名稱
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "CourseName", ResourceType = typeof(Localization))]
        public override string Name { get; set; }

        /// <summary>
        /// 開課部門/系所編號
        /// </summary>
        public int? DeptId { get; set; }

        /// <summary>
        /// 課程大綱
        /// </summary>
        public string CourseOutline { get; set; }

        /// <summary>
        /// 課程代碼
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "CourseCode", ResourceType = typeof(Localization))]
        public string CourseCode { get; set; }
        /// <summary>
        /// 是否為外部人員 - 有值代表是外部
        /// </summary>
        public int? ExternalRid { get; set; }

        public virtual Dept Dept { get; set; }

        //public virtual ICollection<Section> Sections { get; set; }

        //public Course()
        //{
        //    this.Created = TimeData.Create(DateTime.UtcNow);
        //    this.Updated = TimeData.Create(null);
        //    this.Deleted = TimeData.Create(null);
        //}
    }
}
