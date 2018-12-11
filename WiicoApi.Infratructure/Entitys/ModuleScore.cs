using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ModuleScore
    {
        public int Id { get; set; }
        public string ModuleKey { get; set; }
        public int LearningId { get; set; }
        public int ActivityId { get; set; }
        public decimal Score { get; set; }
        public int MemberId { get; set; }
    }
}
