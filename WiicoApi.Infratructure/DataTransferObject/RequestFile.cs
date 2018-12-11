using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.DataTransferObject
{
    public class RequestFile
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int Size { get; set; }
        public Stream Stream { get; set; }
        public byte[] ByteStream { get; set; }
    }
}
