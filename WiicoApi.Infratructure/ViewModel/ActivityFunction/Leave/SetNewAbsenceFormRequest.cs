using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class SetNewAbsenceFormRequest : ActivityFunction.AcitivtyBaseRequest
    {
        [JsonProperty("leaveDate")]
        public DateTime LeaveDate { get; set; }
        /// <summary>
        /// 請假類型 {1:病假 | 2:事假 | 3:公假 | 4:其他}
        /// </summary>
        [JsonProperty("leaveCategoryId")]
        public int LeaveCategoryId { get; set; }
    }
}
