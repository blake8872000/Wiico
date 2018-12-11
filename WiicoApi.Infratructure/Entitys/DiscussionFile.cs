using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class DiscussionFile
    {

        /// <summary>
        /// 主題討論檔案流水號
        /// </summary>
        [Display(Name = "主題討論檔案流水號")]
        public int Id { get; set; }

        /// <summary>
        /// 主題討論編號
        /// </summary>
        [Display(Name = "主題討論編號")]
        public int DiscussionId { get; set; }

        /// <summary>
        /// 留言編號
        /// </summary>
        [Display(Name = "留言編號")]
        public int? MessageId { get; set; }

        /// <summary>
        /// 檔案編號
        /// </summary>
        [Display(Name = "檔案編號")]
        public int FileId { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間"), JsonIgnore]
        public DateTime CreateUtcDate { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [NotMapped]
        public DateTime CreateTime { get { return CreateUtcDate.ToLocalTime(); } }

        /// <summary>
        /// 建立者編號
        /// </summary>
        [Display(Name = "建立者編號")]
        public int Creator { get; set; }
    }
}
