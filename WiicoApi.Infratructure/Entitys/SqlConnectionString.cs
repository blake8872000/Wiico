using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 根據不同學校綁不同資料庫的資料表
    /// </summary>
    public class SqlConnectionString : Base.EntityBase
    {
        [MaxLength(50)]
        public override string Name
        {
            get; set;
        }
        public string DBName { get; set; }
        public string DBUserName { get; set; }
        public string DBPwd { get; set; }
    }
}
