using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.Auth;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Request;
using ESCS.API.Hubs;
using ESCS.MODEL.OpenID.ModelView;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using ESCS.MODEL.OpenID;

namespace ESCS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public const string STATUS_OK = "OK";
        public const string SUCCESS = "ESCS00";
        public const string STATUS_NOTOK = "NotOK";
        public const string INTERNAL_SERVER = "500";
        public const string NOTFOUND = "404";

        protected ICacheServer _cacheServer;
        protected IDynamicService _dynamicService;
        protected IOpenIdService _openIdService;
        protected ILogMongoService<LogException> _logRequestService;
        protected readonly IHubContext<PartnerNotifyHub> _partnerNotifyHub;
        protected readonly IErrorCodeService _errorCodeService;
        protected readonly IDataProtector _protector;
        /// <summary>
        /// BaseController
        /// </summary>
        /// <param name="cacheServer"></param>
        /// <param name="cacheManager"></param>
        public BaseController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> LogMongoService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider)
        {
            _cacheServer = cacheServer;
            _dynamicService = dynamicService;
            _openIdService = openIdService;
            _logRequestService = LogMongoService;
            _partnerNotifyHub = partnerNotifyHub;
            _errorCodeService = errorCodeService;
            _protector = provider.CreateProtector("ESCS.API.Controllers");
        }
        /// <summary>
        /// GetEnvironment - Hàm lấy thông tin môi trường
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        [NonAction]
        public string VetifyToken(HeaderRequest header, out openid_access_token tokenInfo, out string keyCachePartner, bool checkExpriveTime = true, bool checkSignature = true)
        {
            tokenInfo = null;
            keyCachePartner = null;
            string keyCacheActionNoneAuThen = CachePrefixKeyConstants.GetKeyCacheActionNoneAuthen();
            string strJson = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCacheActionNoneAuThen, RedisCacheMaster.DatabaseIndex);
            List<string> actionNoneCheck = null;
            if (!string.IsNullOrEmpty(strJson))
            {
                actionNoneCheck = JsonConvert.DeserializeObject<List<string>>(strJson);
            }
            else
            {
                BaseRequest rq = new BaseRequest();
                rq.data_info = new Dictionary<string, string>();
                var obj = _openIdService.GetActionNoneAuthen(rq).Result;
                if (obj != null)
                {
                    string jsonRes = JsonConvert.SerializeObject(obj);
                    var action = JsonConvert.DeserializeObject<List<sys_action_common>>(jsonRes);
                    actionNoneCheck = action.Select(n => n.actioncode).ToList();
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, 
                        keyCacheActionNoneAuThen, JsonConvert.SerializeObject(actionNoneCheck), 
                        DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, 
                        RedisCacheMaster.DatabaseIndex);
                }
            }

            #region Kiểm tra acction có thuộc nhóm cần phải check token không
            if (actionNoneCheck!=null && actionNoneCheck.Contains(header.action))
            {
                if (string.IsNullOrEmpty(header.envcode))
                {
                    return "Thiếu thông tin môi trường truy cập";
                }
                tokenInfo = new openid_access_token() { evncode = header.envcode };
                return null;
            }
            #endregion;
            if (header.check_ip_backlist && (string.IsNullOrEmpty(header.ip_remote_ipv4) && string.IsNullOrEmpty(header.ip_remote_ipv6)))
            {
                return "Thiếu thông tin IP truy cập";
            }
            try
            {
                string keyCache = "";
                long timeNow = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                #region Token private
                if (header.token.Length <= 64)
                {
                    keyCache = CachePrefixKeyConstants.GetKeyCachePartnerPrivate(header.partner_code, header.token);
                    keyCachePartner = keyCache;
                    string jsonPrivate = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
                    sys_partner_cache userdbPrivate = null;
                    if (string.IsNullOrEmpty(jsonPrivate))
                    {
                        BaseRequest rq = new BaseRequest();
                        rq.data_info = new Dictionary<string, string>();
                        rq.data_info.Add("partner_code", header.partner_code);
                        rq.data_info.Add("api_username", "");
                        rq.data_info.Add("api_password", "");
                        rq.data_info.Add("authen", "authen");
                        rq.data_info.Add("token", header.token);
                        rq.data_info.Add("cat_partner", "PRIVATE");
                        var obj = _openIdService.GetPartnerWithConfig(rq).Result;
                        if (obj == null)
                        {
                            return "Thông tin access token không hợp lệ hoặc thông tin tài khoản đã bị thay đổi.";
                        }
                        string jsonRes = JsonConvert.SerializeObject(obj);
                        userdbPrivate = JsonConvert.DeserializeObject<sys_partner_cache>(jsonRes);
                        _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(userdbPrivate), DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                    }
                    else
                    {
                        userdbPrivate = JsonConvert.DeserializeObject<sys_partner_cache>(jsonPrivate);
                    }
                    if (userdbPrivate== null || string.IsNullOrEmpty(userdbPrivate.config_envcode))
                    {
                        return "Thông tin access token không hợp lệ.";
                    }
                    string signaturePrivate = Utilities.Sha256Hash(header.payload + "." + userdbPrivate.config_secret_key);
                    if (checkSignature && header.signature != signaturePrivate)
                    {
                        return "Chữ ký dữ liệu không hợp lệ.";
                    }
                    if (header.check_ip_backlist && !string.IsNullOrEmpty(userdbPrivate.config_blacklist_ip))
                    {
                        var arr = userdbPrivate.config_blacklist_ip.Split(';');
                        if (arr.Contains(header.ip_remote_ipv4.Trim()) || arr.Contains(header.ip_remote_ipv6.Trim()))
                        {
                            return "IP truy cập tồn tại trong danh sách bị hạn chế.";
                        }
                    }
                    tokenInfo = new openid_access_token();
                    tokenInfo.evncode = userdbPrivate.config_envcode;
                    return null;
                }
                #endregion
                #region Token public
                var arrToken = header.token.Split(".");
                if (arrToken == null || arrToken.Length != 3)
                {
                    return "Token không hợp lệ";
                }
                var headerToken = arrToken[0];
                var payloadToken = arrToken[1];
                var signatureToken = arrToken[2];
                string jsonPayload = ESCS.COMMON.Common.Utilities.DecryptByKey(ESCS.COMMON.Common.Utilities.Base64UrlDecode(payloadToken), OpenIDConfig.KeyHashPayloadToken);
                TokenPayload tokenPayload = JsonConvert.DeserializeObject<TokenPayload>(jsonPayload);
                keyCache = CachePrefixKeyConstants.GetKeyCachePartnerPublic(header.partner_code, tokenPayload.username);
                keyCachePartner = keyCache;
                string json = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
                sys_partner_cache userdb = null;
                if (string.IsNullOrEmpty(json))
                {
                    BaseRequest rq = new BaseRequest();
                    rq.data_info = new Dictionary<string, string>();
                    rq.data_info.Add("partner_code", header.partner_code);
                    rq.data_info.Add("authen", "noauthen");
                    rq.data_info.Add("api_username", "");
                    rq.data_info.Add("api_password", "");
                    rq.data_info.Add("token", "");
                    rq.data_info.Add("envcode", tokenPayload.envcode);
                    rq.data_info.Add("cat_partner", "PUBLIC");
                    var obj = _openIdService.GetPartnerWithConfig(rq).Result;
                    if (obj == null)
                    {
                        return "Thông tin access token không hợp lệ hoặc thông tin tài khoản đã bị thay đổi.";
                    }
                    string jsonRes = JsonConvert.SerializeObject(obj);
                    userdb = JsonConvert.DeserializeObject<sys_partner_cache>(jsonRes);
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, jsonRes, DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                }
                else
                {
                    userdb = JsonConvert.DeserializeObject<sys_partner_cache>(json);
                }
                string signaturePublic = Utilities.Sha256Hash(header.payload + "." + userdb.config_secret_key);
                if (checkSignature && header.signature != signaturePublic)
                {
                    return "Chữ ký dữ liệu không hợp lệ.";
                }
                var arrPublic = userdb.config_blacklist_ip?.Split(';');
                if (header.check_ip_backlist && !string.IsNullOrEmpty(userdb.config_blacklist_ip)&& arrPublic!=null && (arrPublic.Contains(header.ip_remote_ipv4.Trim()) || arrPublic.Contains(header.ip_remote_ipv6.Trim())))
                {
                    return "IP truy cập tồn tại trong danh sách bị hạn chế.";
                }
                var tokenServer = headerToken + "." + payloadToken + "." + Utilities.HMACSHA256(headerToken + "." + payloadToken, userdb.config_secret_key);
                if (header.token != tokenServer)
                {
                    return "Thông tin access token không hợp lệ.";
                }
                if (tokenPayload.time_exprive <= timeNow && checkExpriveTime)
                {
                    return "Access Token đã hết hạn.";
                }
                if (header.partner_code.ToUpper() != tokenPayload.partner_code.ToUpper() || header.partner_code.ToUpper()!=userdb.partner_code.ToUpper())
                {
                    return "Không được sử dụng access token của đối tác khác.";
                }
                tokenInfo = new openid_access_token() { evncode = userdb.config_envcode };
                #endregion
                return null;
            }
            catch
            {
                return "Lỗi trong quá trình xác thực access token do cấu trúc đã bị thay đổi.";
            }
        }
        [NonAction]
        public string VetifyTokenTest(HeaderRequest header, out openid_access_token tokenInfo, out string keyCachePartner, bool checkExpriveTime = true, bool checkSignature = true)
        {
            tokenInfo = null;
            keyCachePartner = null;
            string keyCacheActionNoneAuThen = CachePrefixKeyConstants.GetKeyCacheActionNoneAuthen();
            string strJson = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCacheActionNoneAuThen, RedisCacheMaster.DatabaseIndex);
            
            List<string> actionNoneCheck = null;
            if (!string.IsNullOrEmpty(strJson))
            {
                actionNoneCheck = JsonConvert.DeserializeObject<List<string>>(strJson);
            }
            else
            {
                //BaseRequest rq = new BaseRequest();
                //rq.data_info = new Dictionary<string, string>();
                //var obj = _openIdService.GetActionNoneAuthen(rq).Result;
                //if (obj != null)
                //{
                //    string jsonRes = JsonConvert.SerializeObject(obj);
                //    var action = JsonConvert.DeserializeObject<List<sys_action_common>>(jsonRes);
                //    actionNoneCheck = action.Select(n => n.actioncode).ToList();
                //    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint,
                //        keyCacheActionNoneAuThen, JsonConvert.SerializeObject(actionNoneCheck),
                //        DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now,
                //        RedisCacheMaster.DatabaseIndex);
                //}
            }
            return "vdfvdfvdfvdfvdfv";
            //#region Kiểm tra acction có thuộc nhóm cần phải check token không
            //if (actionNoneCheck != null && actionNoneCheck.Contains(header.action))
            //{
            //    if (string.IsNullOrEmpty(header.envcode))
            //    {
            //        return "Thiếu thông tin môi trường truy cập";
            //    }
            //    tokenInfo = new openid_access_token() { evncode = header.envcode };
            //    return null;
            //}
            //#endregion;
            //if (header.check_ip_backlist && (string.IsNullOrEmpty(header.ip_remote_ipv4) && string.IsNullOrEmpty(header.ip_remote_ipv6)))
            //{
            //    return "Thiếu thông tin IP truy cập";
            //}
            //try
            //{
            //    string keyCache = "";
            //    long timeNow = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
            //    #region Token private
            //    if (header.token.Length <= 64)
            //    {
            //        keyCache = CachePrefixKeyConstants.GetKeyCachePartnerPrivate(header.partner_code, header.token);
            //        keyCachePartner = keyCache;
            //        string jsonPrivate = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
            //        sys_partner_cache userdbPrivate = null;
            //        if (string.IsNullOrEmpty(jsonPrivate))
            //        {
            //            BaseRequest rq = new BaseRequest();
            //            rq.data_info = new Dictionary<string, string>();
            //            rq.data_info.Add("partner_code", header.partner_code);
            //            rq.data_info.Add("api_username", "");
            //            rq.data_info.Add("api_password", "");
            //            rq.data_info.Add("authen", "authen");
            //            rq.data_info.Add("token", header.token);
            //            rq.data_info.Add("cat_partner", "PRIVATE");
            //            var obj = _openIdService.GetPartnerWithConfig(rq).Result;
            //            if (obj == null)
            //            {
            //                return "Thông tin access token không hợp lệ hoặc thông tin tài khoản đã bị thay đổi.";
            //            }
            //            string jsonRes = JsonConvert.SerializeObject(obj);
            //            userdbPrivate = JsonConvert.DeserializeObject<sys_partner_cache>(jsonRes);
            //            _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(userdbPrivate), DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            //        }
            //        else
            //        {
            //            userdbPrivate = JsonConvert.DeserializeObject<sys_partner_cache>(jsonPrivate);
            //        }
            //        if (userdbPrivate == null || string.IsNullOrEmpty(userdbPrivate.config_envcode))
            //        {
            //            return "Thông tin access token không hợp lệ.";
            //        }
            //        string signaturePrivate = Utilities.Sha256Hash(header.payload + "." + userdbPrivate.config_secret_key);
            //        if (checkSignature && header.signature != signaturePrivate)
            //        {
            //            return "Chữ ký dữ liệu không hợp lệ.";
            //        }
            //        if (header.check_ip_backlist && !string.IsNullOrEmpty(userdbPrivate.config_blacklist_ip))
            //        {
            //            var arr = userdbPrivate.config_blacklist_ip.Split(';');
            //            if (arr.Contains(header.ip_remote_ipv4.Trim()) || arr.Contains(header.ip_remote_ipv6.Trim()))
            //            {
            //                return "IP truy cập tồn tại trong danh sách bị hạn chế.";
            //            }
            //        }
            //        tokenInfo = new openid_access_token();
            //        tokenInfo.evncode = userdbPrivate.config_envcode;
            //        return null;
            //    }
            //    #endregion
            //    #region Token public
            //    var arrToken = header.token.Split(".");
            //    if (arrToken == null || arrToken.Length != 3)
            //    {
            //        return "Token không hợp lệ";
            //    }
            //    var headerToken = arrToken[0];
            //    var payloadToken = arrToken[1];
            //    var signatureToken = arrToken[2];
            //    string jsonPayload = ESCS.COMMON.Common.Utilities.DecryptByKey(ESCS.COMMON.Common.Utilities.Base64UrlDecode(payloadToken), OpenIDConfig.KeyHashPayloadToken);
            //    TokenPayload tokenPayload = JsonConvert.DeserializeObject<TokenPayload>(jsonPayload);
            //    keyCache = CachePrefixKeyConstants.GetKeyCachePartnerPublic(header.partner_code, tokenPayload.username);
            //    keyCachePartner = keyCache;
            //    string json = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
            //    sys_partner_cache userdb = null;
            //    if (string.IsNullOrEmpty(json))
            //    {
            //        BaseRequest rq = new BaseRequest();
            //        rq.data_info = new Dictionary<string, string>();
            //        rq.data_info.Add("partner_code", header.partner_code);
            //        rq.data_info.Add("authen", "noauthen");
            //        rq.data_info.Add("api_username", "");
            //        rq.data_info.Add("api_password", "");
            //        rq.data_info.Add("token", "");
            //        rq.data_info.Add("envcode", tokenPayload.envcode);
            //        rq.data_info.Add("cat_partner", "PUBLIC");
            //        var obj = _openIdService.GetPartnerWithConfig(rq).Result;
            //        if (obj == null)
            //        {
            //            return "Thông tin access token không hợp lệ hoặc thông tin tài khoản đã bị thay đổi.";
            //        }
            //        string jsonRes = JsonConvert.SerializeObject(obj);
            //        userdb = JsonConvert.DeserializeObject<sys_partner_cache>(jsonRes);
            //        _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(userdb), DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            //    }
            //    else
            //    {
            //        userdb = JsonConvert.DeserializeObject<sys_partner_cache>(json);
            //    }
            //    string signaturePublic = Utilities.Sha256Hash(header.payload + "." + userdb.config_secret_key);
            //    if (checkSignature && header.signature != signaturePublic)
            //    {
            //        return "Chữ ký dữ liệu không hợp lệ.";
            //    }
            //    var arrPublic = userdb.config_blacklist_ip?.Split(';');
            //    if (header.check_ip_backlist && !string.IsNullOrEmpty(userdb.config_blacklist_ip) && arrPublic != null && (arrPublic.Contains(header.ip_remote_ipv4.Trim()) || arrPublic.Contains(header.ip_remote_ipv6.Trim())))
            //    {
            //        return "IP truy cập tồn tại trong danh sách bị hạn chế.";
            //    }
            //    var tokenServer = headerToken + "." + payloadToken + "." + Utilities.HMACSHA256(headerToken + "." + payloadToken, userdb.config_secret_key);
            //    if (header.token != tokenServer)
            //    {
            //        return "Thông tin access token không hợp lệ.";
            //    }
            //    if (tokenPayload.time_exprive <= timeNow && checkExpriveTime)
            //    {
            //        return "Access Token đã hết hạn.";
            //    }
            //    if (header.partner_code.ToUpper() != tokenPayload.partner_code.ToUpper() || header.partner_code.ToUpper() != userdb.partner_code.ToUpper())
            //    {
            //        return "Không được sử dụng access token của đối tác khác.";
            //    }
            //    tokenInfo = new openid_access_token() { evncode = userdb.config_envcode };
            //    #endregion
            //    return null;
            //}
            //catch
            //{
            //    return "Lỗi trong quá trình xác thực access token do cấu trúc đã bị thay đổi.";
            //}
        }
        /// <summary>
        /// Xác thực refresh token
        /// </summary>
        /// <param name="header"></param>
        /// <param name="auth"></param>
        /// <param name="keyCache"></param>
        /// <param name="authNew"></param>
        /// <returns></returns>
        [NonAction]
        public string VetifyRefreshToken(HeaderRequest header, authentication auth, string keyCache, out authentication authNew)
        {
            authNew = new authentication();
            if (header == null ||string.IsNullOrEmpty(header.partner_code) || string.IsNullOrEmpty(auth.refesh_token))
                return "Thông tin xác thực chưa đầy đủ";
            try
            {
                long timeNow = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var arrRfToken = auth.refesh_token.Split(".");
                if (arrRfToken == null || arrRfToken.Length != 3)
                {
                    return "Refresh Token không hợp lệ";
                }
                var headerToken = arrRfToken[0];
                var payloadToken = arrRfToken[1];
                var signatureToken = arrRfToken[2];
                string json = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
                sys_partner_cache userDb = JsonConvert.DeserializeObject<sys_partner_cache>(json);
                var tokenServer = headerToken + "." + payloadToken + "." + Utilities.HMACSHA256(headerToken + "." + payloadToken, userDb.config_secret_key);
                if (auth.refesh_token != tokenServer)
                {
                    return "Refresh token không hợp lệ";
                }
                string jsonPayloadRefresh = ESCS.COMMON.Common.Utilities.DecryptByKey(ESCS.COMMON.Common.Utilities.Base64UrlDecode(payloadToken), OpenIDConfig.KeyHashPayloadToken);
                openid_refresh_token payload = JsonConvert.DeserializeObject<openid_refresh_token>(jsonPayloadRefresh);
                var accessTokenClient = JsonConvert.DeserializeObject<TokenPayload>(ESCS.COMMON.Common.Utilities.DecryptByKey(ESCS.COMMON.Common.Utilities.Base64UrlDecode(auth.access_token.Split(".")[1]), OpenIDConfig.KeyHashPayloadToken));
                var accessTokenInRefresh = JsonConvert.DeserializeObject<TokenPayload>(ESCS.COMMON.Common.Utilities.DecryptByKey(ESCS.COMMON.Common.Utilities.Base64UrlDecode(payload.access_token.Split(".")[1]), OpenIDConfig.KeyHashPayloadToken));
                if (
                    accessTokenInRefresh.partner_code != accessTokenClient.partner_code ||
                    accessTokenInRefresh.envcode != accessTokenClient.envcode ||
                    accessTokenInRefresh.username != accessTokenClient.username ||
                    accessTokenInRefresh.password != accessTokenClient.password ||
                    accessTokenInRefresh.time_begin_session != accessTokenClient.time_begin_session
                    )
                {
                    return "Không thể dùng Refresh token này để cấp lại cho Access token đang sử dụng";
                }
                
                if (payload.time_exprive< timeNow)
                {
                    return "Phiên làm việc đã hết hạn";
                }
                var timebegin = Convert.ToInt64(DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveAccessTokenMinute).ToString("yyyyMMddHHmmss"));
                authNew.refesh_token = auth.refesh_token;
                if (timebegin <= payload.time_exprive)
                {
                    authNew.time_exprive = timebegin;
                }
                else
                {
                    authNew.time_exprive = payload.time_exprive.Value;
                }
                authNew.environment = userDb.config_envcode;
                var jsonPayload = new TokenPayload()
                {
                    partner_code = header.partner_code.ToUpper(),
                    envcode = userDb.config_envcode,
                    username = userDb.config_username,
                    password = userDb.config_password,
                    time_exprive = authNew.time_exprive,
                    time_begin_session = accessTokenClient.time_begin_session
                };
                var headerAccess = JWTHelper.GetTokenPublicHeader();
                var payloadAccess = JWTHelper.GetTokenPublicPayload(jsonPayload);
                string access_token = JWTHelper.GetToken(headerAccess, payloadAccess, userDb.config_secret_key);
                authNew.access_token = access_token;
                return null;
            }
            catch
            {
                return "Lỗi trong quá trình xác thực refresh token do cấu trúc đã bị thay đổi.";
            }
        }
        [NonAction]
        public string GetPartnerByEnvirontment(HeaderRequest header, string payload, out string  keyCache, out sys_partner_cache userdb)
        {
            keyCache = "";
            userdb = null;
            if (header.check_ip_backlist && (string.IsNullOrEmpty(header.ip_remote_ipv4) && string.IsNullOrEmpty(header.ip_remote_ipv6)))
            {
                return "Thiếu thông tin IP truy cập";
            }
            header.ip_remote_ipv4 = header.ip_remote_ipv4 ?? "";
            header.ip_remote_ipv6 = header.ip_remote_ipv6 ?? "";
            if (header == null || string.IsNullOrEmpty(header.partner_code) || string.IsNullOrEmpty(header.signature) || string.IsNullOrEmpty(header.action))
                return "Thông tin xác thực chưa đầy đủ (ePartnerCode, eSignature, eAction)";
            var arrSignature = header.signature.Split('.');
            if (arrSignature.Length!=3)
            {
                return "Chữ ký không hợp lệ";
            }
            header.signature = arrSignature[0] + "." + payload + "." + arrSignature[2];
            var jsonHeader = Utilities.Base64UrlDecode(arrSignature[0]);
            TokenHeader tokenHeader = null;
            try
            {
                tokenHeader = JsonConvert.DeserializeObject<TokenHeader>(jsonHeader);
            }
            catch
            {
                return "Chữ ký không hợp lệ";
            }
            BaseRequest rq = new BaseRequest();
            rq.data_info = new Dictionary<string, string>();
            rq.data_info.Add("partner_code", header.partner_code);
            rq.data_info.Add("authen", "noauthen");
            rq.data_info.Add("api_username", "");
            rq.data_info.Add("api_password", "");
            rq.data_info.Add("token", "");
            rq.data_info.Add("envcode", tokenHeader.envcode);
            rq.data_info.Add("cat_partner", "PUBLIC");
            var obj = _openIdService.GetPartnerWithConfig(rq).Result;
            if (obj == null)
            {
                return "Thông tin access token không hợp lệ hoặc thông tin tài khoản đã bị thay đổi.";
            }
            string jsonRes = JsonConvert.SerializeObject(obj);
            userdb = JsonConvert.DeserializeObject<sys_partner_cache>(jsonRes);
            keyCache = CachePrefixKeyConstants.GetKeyCachePartnerPublic(header.partner_code, userdb.config_username);
            _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(userdb), DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            
            if (!JWTHelper.VerifySignature(header.signature, userdb.config_secret_key))
            {
                return "Chữ ký không hợp lệ";
            }
            return null;
        }
    }
}