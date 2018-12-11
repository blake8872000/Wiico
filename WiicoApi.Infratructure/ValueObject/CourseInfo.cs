using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class CourseInfo
    {
        public int Id { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }

        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string CourseOutline { get; set; }
        public string CourseCode { get; set; }
        [MaxLength(10)]
        public string ClassRoom { get; set; }
        public DateTime ClassTime { get; set; }
        public string GoogleDriveFileId { get; set; }
        /// <summary>
        /// 用於查詢身分用
        /// </summary>
        public int CircleId { get; set; }

        public int HomeWorkCount { get; set; }

        public int SignCount { get; set; }
        /// <summary>
        /// 存上課老師
        /// </summary>
        public List<ValueObject.LearningCircleMemberInfo> Teachers { get; set; }
        /// <summary>
        /// 存擴充欄位
        /// </summary>
        public List<ValueObject.ExtensionColumnValue> ExtensionColumnInfo { get; set; }
        /// <summary>
        /// 存上課地點跟時間
        /// </summary>
        public List<Entity.TimeTable> TimeTable { get; set; }
        /// <summary>
        /// 存目前課程的進度
        /// </summary>
        public List<Entity.Syllabus> Syllabus { get; set; }
    }
}
