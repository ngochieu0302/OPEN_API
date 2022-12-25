using ESCS.COMMON.Common;
using ESCS.COMMON.ExtensionMethods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ESCS.COMMON.Request
{
    public class JWTHelper
    {
        public static string GetTokenPublicHeader(string typ = "JWT", string alg = "HS256", string cty = "escs-moblie-api;v=1")
        {
            if (string.IsNullOrEmpty(typ) || string.IsNullOrEmpty(alg) || string.IsNullOrEmpty(cty))
            {
                throw new Exception("Thiếu thông tin header");
            }
            string header =  @"{}".AddPropertyStringJson("typ", typ)
                        .AddPropertyStringJson("alg", alg)
                        .AddPropertyStringJson("cty", cty);
            return Base64UrlEncode(header);
        }
        public static string GetTokenPublicPayload<T>(T payload)
        {
            return Base64UrlEncode(Utilities.EncryptByKey(JsonConvert.SerializeObject(payload), OpenIDConfig.KeyHashPayloadToken));
        }
        public static string GetToken(string header, string payload, string secret_key)
        {
            return header + "." + payload + "." + Utilities.HMACSHA256(header + "." + payload, secret_key);
        }
        public static bool VerifySignature(string jwt,string secret_key)
        {
            string[] parts = jwt.Split(".".ToCharArray());
            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];
            byte[] bytesToSign = getBytes(string.Join(".", header, payload));
            byte[] secret = getBytes(secret_key);
            var alg =  new HMACSHA256(secret);
            var hash = alg.ComputeHash(bytesToSign);
            var computedSignature = Base64UrlEncode(hash);
            return signature == computedSignature;
        }
        private static byte[] getBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            //output = output.Replace("=", "");
            output = output.Split('=')[0];
            output = output.Replace('+', '-'); 
            output = output.Replace('/', '_'); 
            return output;
        }
        public static string Base64UrlEncode(string input)
        {
            return Base64UrlEncode(getBytes(input));
        }
    }
    public class TokenPayload
    {
        /// <summary>
        /// Mã đối tác
        /// </summary>
        public string partner_code { get; set; }
        /// <summary>
        /// Môi trường
        /// </summary>
        public string envcode { get; set; }
        /// <summary>
        /// Tài khoản đăng nhập api
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// Mật khẩu đã được mã hóa
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// Thời gian sống
        /// </summary>
        public long? time_exprive { get; set; }
        /// <summary>
        /// Thời gian bắt đầu phiên làm việc
        /// </summary>
        public long? time_begin_session { get; set; }
        /// <summary>
        /// Loại token
        /// </summary>
        public string type { get; set; }
        public TokenPayload()
        {
            type = "access_token";
        }
    }
    public class TokenHeader
    {
        public string alg { get; set; }
        public string typ { get; set; }
        public string cty { get; set; }
        public string cat { get; set; }
        public string partner { get; set; }
        public string user { get; set; }
        public string envcode { get; set; }
    }
}
