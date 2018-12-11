using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 公告資料表
    /// </summary>
    public class ActBulletin
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public int Creator { get; set; }

        public DateTime CreateDate { get; set; }

        public int? FileId { get; set; }


    }
}
