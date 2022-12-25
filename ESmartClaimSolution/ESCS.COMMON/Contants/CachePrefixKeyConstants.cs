using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ESCS.COMMON.Contants
{
    /// <summary>
    /// CachePrefixConstants - Quản lý các prefix key cache 
    /// </summary>
    public class CachePrefixKeyConstants
    {
        /// <summary>
        /// Cache lưu thông tin đối tác (PUBLIC) và cấu hình đối tác
        /// </summary>
        private const string PARTNER_PUBLIC = "PARTNER:{PARTNER_CODE}.{USERNAME}";
        public static string GetKeyCachePartnerPublic(string partner_code, string username)
        {
            return PARTNER_PUBLIC.Replace("{PARTNER_CODE}", partner_code.ToUpper()).Replace("{USERNAME}", username.ToUpper());
        }
        /// <summary>
        /// Cache lưu thông tin đối tác (PRIVATE) và cấu hình đối tác
        /// </summary>
        private const string PARTNER_PRIVATE = "PARTNER:{PARTNER_CODE}.{TOKEN}";
        public static string GetKeyCachePartnerPrivate(string partner_code, string tọken)
        {
            return PARTNER_PRIVATE.Replace("{PARTNER_CODE}", partner_code.ToUpper()).Replace("{TOKEN}", tọken);
        }
        /// <summary>
        /// Cache những tài khoản bị khóa 5p, 10p, 15p
        /// </summary>
        private const string ACCOUNT_LOCK = "ACCOUNT.LOCK:{PARTNER_CODE}:{USERNAME}";
        public static string GetKeyCacheAccountLock(string partner_code, string username)
        {
            return ACCOUNT_LOCK.Replace("{PARTNER_CODE}", partner_code.ToUpper()).Replace("{USERNAME}", username.ToUpper());
        }
        /// <summary>
        /// Cache số lần đăng nhập sai của 1 tài khoản để check kiểm tra nếu vượt quá thì sẽ tạm khóa tài khoản
        /// </summary>
        private const string LOGIN_ERROR_COUNT = "LOGIN.ERROR.COUNT.{PARTNER_CODE}.{USERNAME}";
        public static string GetKeyCacheLoginErrorCount(string partner_code, string username)
        {
            return LOGIN_ERROR_COUNT.Replace("{PARTNER_CODE}", partner_code.ToUpper()).Replace("{USERNAME}", username.ToUpper());
        }
        /// <summary>
        /// Cache lưu thông tin response của Action
        /// </summary>
        private const string CACHE_DATA_ACTION = "CACHE.RESPONSE.DATA:{ENVCODE}:{PARTNER_CODE}:{DBNAME}.{SCHEMA}:{ACTIONCODE}.{PREFIX_KEY}.*";
        public static string GetKeyCacheResponseAction(string envcode, string partner_code, string dbname, string schema, string actioncode, string prefix, string search = null)
        {
            if (string.IsNullOrEmpty(search)|| search =="")
            {
                return CACHE_DATA_ACTION.Replace("{ENVCODE}", envcode.ToUpper())
                                        .Replace("{PARTNER_CODE}", partner_code.ToUpper())
                                        .Replace("{DBNAME}", dbname.ToUpper())
                                        .Replace("{SCHEMA}", schema.ToUpper())
                                        .Replace("{ACTIONCODE}", actioncode.ToUpper())
                                        .Replace("{PREFIX_KEY}", prefix.ToUpper())
                                        .Replace(".*", "")
                                        .Replace("#", "");
            }
            return CACHE_DATA_ACTION.Replace("{ENVCODE}", envcode.ToUpper())
                                        .Replace("{PARTNER_CODE}", partner_code.ToUpper())
                                        .Replace("{DBNAME}", dbname.ToUpper())
                                        .Replace("{SCHEMA}", schema.ToUpper())
                                        .Replace("{ACTIONCODE}", actioncode.ToUpper())
                                        .Replace("{PREFIX_KEY}", prefix.ToUpper())
                                        .Replace("*", search)
                                        .Replace("#", "");
        }
        /// <summary>
        /// Cache thông tin action
        /// </summary>
        private const string CACHE_ACTION = "CACHE.ACTION:{PARTNER_CODE}:{ENVCODE}:{ACTIONCODE}";
        public static string GetKeyCacheAction(string partner_code, string envcode, string actioncode)
        {
            return CACHE_ACTION.Replace("{PARTNER_CODE}", partner_code.ToUpper()).Replace("{ENVCODE}", envcode.ToUpper()).Replace("{ACTIONCODE}", actioncode.ToUpper());
        }
        /// <summary>
        /// Lưu tất cả thông tin param của 1 Action
        /// </summary>
        private const string CACHE_ACTION_PARAM = "ACTION.PARAM:{DBNAME}.{SCHEMA}:{PKG_NAME?}.{STORED_NAME}";
        public static string GetKeyCacheParamStored(string dbname, string schema, string storedname, string packagename = null)
        {
            if (string.IsNullOrEmpty(packagename) || packagename.Trim()=="")
            {
                return CACHE_ACTION_PARAM.Replace("{DBNAME}", dbname.ToUpper()).Replace("{SCHEMA}", schema.ToUpper()).Replace("{PKG_NAME?}.", "").Replace("{STORED_NAME}", storedname.ToUpper()).Replace("#","");
            }
            return CACHE_ACTION_PARAM.Replace("{DBNAME}", dbname.ToUpper()).Replace("{SCHEMA}", schema.ToUpper()).Replace("{PKG_NAME?}", packagename.ToUpper()).Replace("{STORED_NAME}", storedname.ToUpper()).Replace("#", "");
        }
        /// <summary>
        /// Key cache check ddos
        /// </summary>
        private const string CACHE_DDOS = "DDOS:{KEY_CACHE_PARTNER}.{ACTION}.{IPv4}.{IPv6}";
        public static string GetKeyCacheDDos(string keyCachePartner, string actionCode, string ip_remote_ipv4, string ip_remote_ipv6)
        {
            keyCachePartner = keyCachePartner.Replace(":", "@");
            return CACHE_DDOS.Replace("{KEY_CACHE_PARTNER}", keyCachePartner).Replace("{ACTION}", actionCode).Replace("{IPv4}", ip_remote_ipv4).Replace("{IPv6}", ip_remote_ipv6);
        }
        /// <summary>
        /// Key cache action none authen
        /// </summary>
        private const string CACHE_ACTION_NONE_AUTHEN = "CACHE.ACTION.NONE.AUTHEN";
        public static string GetKeyCacheActionNoneAuthen()
        {
            return CACHE_ACTION_NONE_AUTHEN;
        }
    }
}
