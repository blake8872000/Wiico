using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public class GetPdsFlowsRequest: Base.BackendBaseRequest
    {
        public int Coll_SemeGrade { get; set; }
        public string ID_coll { get; set; }
    }
}
