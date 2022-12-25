using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.Http
{
    public class NetworkCredentials
    {
        public static List<NetworkCredentialItem> Items { get; set; }
        public static NetworkCredentialItem GetItem(string key)
        {
            return Items.Where(n => n.Code == key).Select(n=> {
                n.FullPath = @"\\" + n.IpRemote + @"\" + n.BaseFolderName;
                return n;
            }).FirstOrDefault();
        }
    }
    public class NetworkCredentialItem
    {
        public  string Code { get; set; }
        public  string IpRemote { get; set; }
        public  string BaseFolderName { get; set; }
        public  string UserName { get; set; }
        public  string Password { get; set; }
        public  string FullPath { get; set; }
        public bool IsLocal { get; set; }
    }
}
