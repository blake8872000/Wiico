﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.BusinessObject
{
    public class TokenCircleAuth
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string[] Data { get; set; }
    }
}
