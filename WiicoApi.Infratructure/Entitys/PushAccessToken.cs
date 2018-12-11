using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class PushAccessToken
    {
        public int Id { get; set; }

        public string token { get; set; }

        public Property.TimeData Created { get; set; }
    }
}
