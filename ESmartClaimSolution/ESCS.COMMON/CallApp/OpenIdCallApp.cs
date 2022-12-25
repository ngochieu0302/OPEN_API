using ESCS.COMMON.Common;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.CallApp
{
    public interface IOpenIdCallApp
    {
        void SetHeader(string typ = "JWT", string alg = "HS256", string cty = "stringee-api;v=1");
        void SetPayloadJson(ht_nsd_call_id call_id, long? exp, string userId);
        string GetSignatureVerify(ht_nsd_call_id call_id);
        string GetSignatureVerify(ht_nsd_call_id call_id, string userId, long? exp);
        string GetTokenRestApi(ht_nsd_call_id call_id, long? exp);
    }
    public class OpenIdCallApp : IOpenIdCallApp
    {
        private readonly ILogMongoService<LogContent> _logContent;
        public OpenIdCallApp(ILogMongoService<LogContent> logContent)
        {
            _logContent = logContent;
        }
        private string header { get; set; }
        private string payload { get; set; }
        public void SetHeader(string typ = "JWT", string alg = "HS256", string cty = "stringee-api;v=1")
        {
            if (string.IsNullOrEmpty(typ) || string.IsNullOrEmpty(alg) || string.IsNullOrEmpty(cty))
            {
                throw new Exception("Thiếu thông tin header Video Call");
            }
            this.header = @"{}".AddPropertyStringJson("typ", typ)
                                         .AddPropertyStringJson("alg", alg)
                                         .AddPropertyStringJson("cty", cty);
        }
        public void SetPayloadJson(ht_nsd_call_id call_id, long? exp, string userId)
        {
            if (exp == null || exp.Value <= 0 || string.IsNullOrEmpty(userId))
            {
                throw new Exception("Thiếu hoặc sai thông tin payload Video Call");
            }
            this.payload = @"{}".AddPropertyStringJson("jti", call_id.sid + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff"))
                                         .AddPropertyStringJson("iss", call_id.sid)
                                         .AddPropertyStringJson("exp", exp)
                                         .AddPropertyStringJson("userId", userId);
        }
        public void SetPayloadJsonRestApi(ht_nsd_call_id call_id, long? exp)
        {
            if (exp == null || exp.Value <= 0)
            {
                throw new Exception("Thiếu hoặc sai thông tin payload Video Call");
            }
            this.payload = @"{}".AddPropertyStringJson("jti", call_id.sid + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff"))
                                         .AddPropertyStringJson("iss", call_id.sid)
                                         .AddPropertyStringJson("exp", exp)
                                         .AddPropertyStringJson("rest_api", true);
        }
        public string GetSignatureVerify(ht_nsd_call_id call_id)
        {
            SetHeader();
            if (string.IsNullOrEmpty(header))
            {
                throw new Exception("Chưa gán thông tin header Video Call");
            }
            if (string.IsNullOrEmpty(payload))
            {
                throw new Exception("Chưa gán thông tin payload Video Call");
            }
            if (string.IsNullOrEmpty(call_id.secret))
            {
                throw new Exception("Chưa nhập apiKeySecret Video Call");
            }
            string str = Utilities.Base64UrlEncode(header) + "." + Utilities.Base64UrlEncode(payload);
            return Utilities.HMACSHA256Default(str, call_id.secret);
        }
        public string GetSignatureVerify(ht_nsd_call_id call_id, string userId, long? exp)
        {
            SetHeader();
            SetPayloadJson(call_id, exp, userId);
            if (string.IsNullOrEmpty(header))
            {
                throw new Exception("Chưa gán thông tin header Video Call");
            }
            if (string.IsNullOrEmpty(payload))
            {
                throw new Exception("Chưa gán thông tin payload Video Call");
            }
            if (string.IsNullOrEmpty(call_id.secret))
            {
                throw new Exception("Chưa nhập apiKeySecret Video Call");
            }
            string str = Utilities.Base64UrlEncode(header) + "." + Utilities.Base64UrlEncode(payload);
            return str + "." + Utilities.HMACSHA256Default(str, call_id.secret);
        }
        public string GetTokenRestApi(ht_nsd_call_id call_id, long? exp)
        {
            SetHeader();
            SetPayloadJsonRestApi(call_id, exp);
            if (string.IsNullOrEmpty(header))
            {
                throw new Exception("Chưa gán thông tin header Video Call");
            }
            if (string.IsNullOrEmpty(payload))
            {
                throw new Exception("Chưa gán thông tin payload Video Call");
            }
            if (string.IsNullOrEmpty(call_id.secret))
            {
                throw new Exception("Chưa nhập apiKeySecret Video Call");
            }
            
            string str = Utilities.Base64UrlEncode(header) + "." + Utilities.Base64UrlEncode(payload);
            string token = str + "." + Utilities.HMACSHA256Default(str, call_id.secret);
            return token;
        }
    }
}
