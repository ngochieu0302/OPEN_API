using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.ExceptionHandlers
{
    public class OracleDbException : Exception
    {
        public OracleDbException(Exception ex) : base(GetMessage(ex))
        {

        }
        private static string GetMessage(Exception ex)
        {
            string errorCode = ex.Message;
            int firstIndex = errorCode.ToLower().IndexOf("loi:");
            int lastIndex = errorCode.ToLower().LastIndexOf(":loi");
            if (firstIndex == -1 || lastIndex == -1)
            {
                return errorCode;
            }
            string code = errorCode.Substring(firstIndex + 4, lastIndex - firstIndex - 4);
            return code;
        }
    }
}
