//using iTextSharp.text.xml.xmp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESCS.COMMON.SignaturePDF
{
    public class MetaData
    {
        private IDictionary<string, string> info = new Dictionary<string, string>();
        public IDictionary<string, string> Info
        {
            get { return info; }
            set { info = value; }
        }
        public string Author
        {
            get { return (string)info["Author"]; }
            set { info.Add("Author", value); }
        }
        public string Title
        {
            get { return (string)info["Title"]; }
            set { info.Add("Title", value); }
        }
        public string Subject
        {
            get { return (string)info["Subject"]; }
            set { info.Add("Subject", value); }
        }
        public string Keywords
        {
            get { return (string)info["Keywords"]; }
            set { info.Add("Keywords", value); }
        }
        public string Producer
        {
            get { return (string)info["Producer"]; }
            set { info.Add("Producer", value); }
        }

        public string Creator
        {
            get { return (string)info["Creator"]; }
            set { info.Add("Creator", value); }
        }

        public IDictionary<string, string> getMetaData()
        {
            return this.info;
        }
        //public byte[] getStreamedMetaData()
        //{
        //    MemoryStream os = new System.IO.MemoryStream();
        //    XmpWriter xmp = new XmpWriter(os, this.info);
        //    xmp.Close();
        //    return os.ToArray();
        //}
    }
}
