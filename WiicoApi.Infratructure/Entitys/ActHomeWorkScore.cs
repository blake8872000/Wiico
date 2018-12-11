using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActHomeWorkScore : Base.ChangeTimeBase
    {
        /// <summary>
        /// 打分數Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 老師當下打分數成績
        /// </summary>
        public int? Score { get; set; }
        /// <summary>
        /// 學生上傳logId
        /// </summary>
        public int HomeWorkLogId { get; set; }
        /// <summary>
        /// 發布成績
        /// </summary>
        public int? SendScore { get; set; }
        /// <summary>
        /// 退回成績
        /// </summary>
        public int? BackScore { get; set; }

    }
}

