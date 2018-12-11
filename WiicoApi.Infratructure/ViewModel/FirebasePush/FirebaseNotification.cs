using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    /// <summary>
    /// 直接推播內容
    /// </summary>
    public class FirebaseNotification
    {
        public string title { get; set; }
        public string body { get; set; }

        public int badge { get; set; }
        /// <summary>
        /// 推播聲音 : Enabled
        /// </summary>
        public string sound { get; set; }
    }
}
