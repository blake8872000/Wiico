using WiicoApi.Infrastructure.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School.SignInSynchronize
{
    public class SignSyncMemberListRequest
    {
        public string StudID { get; set; }
        public AttendanceStateEnum Status { get; set; }
    }
}
