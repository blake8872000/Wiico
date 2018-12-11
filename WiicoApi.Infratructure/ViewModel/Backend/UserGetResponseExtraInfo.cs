using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class UserGetResponseExtraInfo
    {
        /// <summary>
        /// 目前年級
        /// </summary>
        [NotMapped]
        public int? Grade { get; set; }
        /// <summary>
        /// 需要修習的年份
        /// </summary>
        [NotMapped]
        public int? SemesterGrade { get; set; }
        /// <summary>
        /// 畢業狀態
        /// </summary>
        [NotMapped]
        public int? GraduationStatus { get; set; }
        /// <summary>
        /// 入學年
        /// </summary>
        [NotMapped]
        public string SchoolRoll { get; set; }
    }
}
