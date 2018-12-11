using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class SignInLogViewModel
    {
        public Dictionary<string, Infrastructure.ValueObject.SignInLog> OuterKeySignInLog { get; set; }

        /// <summary>
        /// 請假單事件Id
        /// </summary>
        public Guid EventId { get; set; }
    }
}
