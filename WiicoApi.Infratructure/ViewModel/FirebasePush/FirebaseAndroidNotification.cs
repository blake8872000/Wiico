using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.FirebasePush
{
    public class FirebaseAndroidNotification : FirebaseNotification
    {
        public string icon { get; set; }
        public string color { get; set; }
        public string tag { get; set; }
    }
}
