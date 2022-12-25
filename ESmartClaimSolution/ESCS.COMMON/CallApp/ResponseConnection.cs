using Nancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.CallApp
{
    public class ResponseConnection
    {
        public decimal? count { get; set; }
        public decimal? pageSize { get; set; }
        public List<UserConnect> users { get; set; }
}
    public class UserConnect
    {
        public decimal? loginTime { get; set; }
        public string userId { get; set; }
        public bool? chatCustomer { get; set; }
        public bool? canCallout { get; set; }
    }
}
