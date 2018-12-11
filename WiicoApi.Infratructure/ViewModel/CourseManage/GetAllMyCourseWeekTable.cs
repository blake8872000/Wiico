using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.CourseManage
{
    public class GetAllMyCourseWeekTable
    {
        public string Place { get; set; }

        public string Week { get; set; }

        public int StartPeriod { get; set; }

        public int EndPeriod { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}
