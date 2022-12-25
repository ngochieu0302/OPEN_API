using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Http
{
    public class MailOpenIdConfig
    {
        public MailServer server { get; set; }
        public MailInfo from { get; set; }
        public List<MailInfo> to { get; set; }
        public List<MailInfo> bcc { get; set; }
        public List<MailInfo> cc { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public List<FilesAttach> attach { get; set; }
    }
    public class MailServer
    {
        public string smtp_server { get; set; }
        public string smtp_username { get; set; }
        public string smtp_password { get; set; }
        public int smtp_port { get; set; }
    }
    public class MailInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public string alias { get; set; }
    }
    public class FilesAttach
    {
        public string base64 { get; set; }
        public string file_name { get; set; }
        public string extension { get; set; }
    }
}
