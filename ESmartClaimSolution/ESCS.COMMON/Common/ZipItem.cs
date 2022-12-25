using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class ZipItem
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public ZipItem(string fileName, byte[] content)
        {
            this.FileName = fileName;
            this.Content = content;
        }
    }
}
