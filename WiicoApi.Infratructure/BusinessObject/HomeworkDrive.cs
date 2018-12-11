using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class HomeworkDrive
    {
        public string Description { get; set; }
        public string OverdueDate { get; set; }
        public string EndDate { get; set; }
        public string GoogleDriveFolder { get; set; }
    }
}
