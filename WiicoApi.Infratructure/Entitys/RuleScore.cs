using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class RuleScore
    {
        public int Id { get; set; }

        public int LearningId { get; set; }

        public int RuleId { get; set; }

        public decimal Score { get; set; }
    }
}
