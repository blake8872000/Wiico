using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public class PdsFlowModel
    {
        /// <summary>
        /// 修課規定
        /// </summary>
        public string Credmemo { get; set; }
        /// <summary>
        /// 畢業流程
        /// </summary>
        public List<FlowData> PDS_Flows { get; set; }

    }
}
