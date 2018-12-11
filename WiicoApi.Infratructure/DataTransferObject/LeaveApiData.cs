using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.DataTransferObject
{
    public class LeaveApiData
    {
        public int? LeaveId { get; set; }
        public int LearningId { get; set; }
        public Guid Token { get; set; }
        public DateTime LeaveDate { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string LeaveType { get; set; }
        public string Comment { get; set; }
    }
}
