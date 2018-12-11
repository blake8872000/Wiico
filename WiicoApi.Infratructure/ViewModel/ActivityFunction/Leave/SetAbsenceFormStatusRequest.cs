using WiicoApi.Infrastructure.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class SetAbsenceFormStatusRequest : Base.BackendBaseRequest
    {
        /// <summary>
        /// 課程代碼
        /// </summary>
        public string ClassID { get; set; }
        /// <summary>
        /// 操作的項目代碼
        /// </summary>
        public List<string> OuterKey { get; set; }
        /// <summary>
        /// 請假單狀態
        /// </summary>
        public enumAbsenceFormStatus status { get; set; }
        /// <summary>
        /// 備註文字
        /// </summary>
        public string reason { get; set; }
    }
}
