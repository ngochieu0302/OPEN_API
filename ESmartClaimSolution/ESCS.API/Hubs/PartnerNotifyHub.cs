using ESCS.BUS.Services;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.Hubs;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.MODEL.OpenID.ModelView;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS.API.Hubs
{
    public class MessageChat
    {
        public string ma_doi_tac_nsd { get; set; }
        public string ma_chi_nhanh_nsd { get; set; }
        public string nsd { get; set; }
        public string pas { get; set; }

        /// <summary>
        /// ConnectionId nhận
        /// </summary>
        public string connectionId { get; set; }
        /// <summary>
        /// Mã nội dung tin nhắn
        /// </summary>
        public string ma { get; set; }
        /// <summary>
        /// Mã đối tác người gửi
        /// </summary>
        public string ma_dtac_gui { get; set; }
        /// <summary>
        /// Mã người sử dụng gửi
        /// </summary>
        public string nsd_gui { get; set; }
        /// <summary>
        /// Tên người sử dụng gửi
        /// </summary>
        public string nsd_gui_ten { get; set; }
        /// <summary>
        /// Mã đối tác người nhận
        /// </summary>
        public string ma_dtac_nhan { get; set; }
        /// <summary>
        /// Mã người sử dụng nhận
        /// </summary>
        public string nsd_nhan { get; set; }
        /// <summary>
        /// Tên người sử dụng nhan
        /// </summary>
        public string nsd_nhan_ten { get; set; }
        /// <summary>
        /// Mã đối tác của hồ sơ
        /// </summary>
        public string ma_doi_tac { get; set; }
        /// <summary>
        /// Số id hồ sơ
        /// </summary>
        public string so_id { get; set; }
        /// <summary>
        /// Số hợp đồng
        /// </summary>
        public string so_hd { get; set; }
        /// <summary>
        /// Thời gian gửi
        /// </summary>
        public decimal? tg_gui { get; set; }
        /// <summary>
        /// Nội dung gửi
        /// </summary>
        public string nd { get; set; }
        /// <summary>
        /// Mã nội dung đang trả lời
        /// </summary>
        public string ma_cha { get; set; }
        /// <summary>
        /// Nội dung đang trả lời
        /// </summary>
        public string nd_ma_cha { get; set; }
        /// <summary>
        /// File dạng base64
        /// </summary>
        public string file_base64 { get; set; }
        /// <summary>
        /// Tên file
        /// </summary>
        public string ten_file { get; set; }
        /// <summary>
        /// Loại nội dung chát: text, file
        /// </summary>
        public string loai_nd { get; set; }
        /// <summary>
        /// Trạng thái đọc
        /// </summary>
        public decimal? tthai_doc { get; set; }
    }
    public class PartnerNotifyHub : Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IDynamicService _dynamicService;
        private readonly ICacheServer _cacheServer;
        private readonly IOpenIdService _openIdService;
        private readonly ILogMongoService<LogContent> _logContent;
        public PartnerNotifyHub(
            IUserConnectionManager userConnectionManager,
            IDynamicService dynamicService,
            ICacheServer cacheServer,
            IOpenIdService openIdService,
            ILogMongoService<LogContent> logContent)
        {
            _userConnectionManager = userConnectionManager;
            _dynamicService = dynamicService;
            _cacheServer = cacheServer;
            _openIdService = openIdService;
            _logContent = logContent;
        }
        public string GetConnectionId()
        {
            var httpContext = this.Context.GetHttpContext();
            MessageNotify messageNotify = new MessageNotify();
            messageNotify.ma_doi_tac = httpContext.Request.Query["ma_doi_tac"];
            messageNotify.nsd = httpContext.Request.Query["nsd"];
            messageNotify.connectionid = Context.ConnectionId;
            string key = "HUBCONNECTION:" + messageNotify.ma_doi_tac + ":" + messageNotify.nsd;
            string key_connection_id = "HUBCONNECTION_CONNECTION_ID:" + Context.ConnectionId;
            _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, key_connection_id, key, DateTime.Now.AddHours(24) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            
            List<string> connectionString = _cacheServer.Get<List<string>>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, key, RedisCacheMaster.DatabaseIndex);
            if (connectionString==null)
                connectionString = new List<string>();
            if (!connectionString.Contains(Context.ConnectionId))
            {
                connectionString.Add(Context.ConnectionId);
                _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, key, connectionString, DateTime.Now.AddHours(24) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
            }
            return Context.ConnectionId;
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            string key_connection_id = "HUBCONNECTION_CONNECTION_ID:" + connectionId;
            string key_user = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, key_connection_id, RedisCacheMaster.DatabaseIndex);
            List<string> dsConnectionUser = _cacheServer.Get<List<string>>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, key_user, RedisCacheMaster.DatabaseIndex);
            if (dsConnectionUser!=null && dsConnectionUser.Contains(connectionId))
            {
                dsConnectionUser.Remove(connectionId);
                _cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, key_user, dsConnectionUser, DateTime.Now.AddHours(24) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
                _cacheServer.Remove(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, key_connection_id, RedisCacheMaster.DatabaseIndex);
            }
            var value = await Task.FromResult(0);
        }
        public async Task ChatMessage(string str, string ePartnerCode, string eAction, string eAuthToken, string eEnvirontment)
        {
            try
            {
                MessageChat message = JsonConvert.DeserializeObject<MessageChat>(str);
                var httpContext = this.Context.GetHttpContext();
                HeaderRequest header = httpContext.Request.GetHeader();
                header.action = eAction;
                header.partner_code = ePartnerCode;
                header.envcode = eEnvirontment;
                header.token = eAuthToken;
                try
                {
                    Task task = new Task(async () =>
                    {
                        Dictionary<string, bool> prefix = new Dictionary<string, bool>();
                        Dictionary<string, string> myChat = message.ToDictionaryModel();
                        BaseRequest rq = new BaseRequest(myChat);
                        BaseResponse<dynamic> res = new BaseResponse<dynamic>();
                        var outPut = new Dictionary<string, object>();
                        res.data_info = await _dynamicService.ExcuteDynamicAsync(rq, header, prefix, outValue =>
                        {
                            res.out_value = outValue;
                        });
                    });
                    task.Start();
                }
                catch (Exception ex)
                {
                    //_logContent.AddLogAsync(new LogContent() { content = "Lỗi ghi chat vào DB: " + ex.Message });
                }
                var connectionid = this.Context.ConnectionId;
                string key = "HUBCONNECTION:" + message.ma_dtac_nhan + ":" + message.nsd_nhan;
                List<string> dsConnectionUser = _cacheServer.Get<List<string>>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, key, RedisCacheMaster.DatabaseIndex);
                if (dsConnectionUser!=null)
                {
                    foreach (var connId in dsConnectionUser)
                    {
                        message.connectionId = connId;
                        await Clients.Client(connId).SendAsync("receiveMessage", message);
                    }
                }
            }
            catch
            {

            }
        }
        private string VetifyToken(HeaderRequest header, out openid_access_token tokenInfo, out string keyCachePartner, bool checkExpriveTime = true, bool checkSignature = true)
        {
            tokenInfo = null;
            keyCachePartner = null;
            string keyCache = "";
            if (header.check_ip_backlist && (string.IsNullOrEmpty(header.ip_remote_ipv4) && string.IsNullOrEmpty(header.ip_remote_ipv6)))
            {
                return "Thiếu thông tin IP truy cập";
            }
            try
            {
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
                    if (userdbPrivate == null || string.IsNullOrEmpty(userdbPrivate.config_envcode))
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
                    _cacheServer.Set<string>(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, keyCache, JsonConvert.SerializeObject(userdb), DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);
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
                if (header.check_ip_backlist && !string.IsNullOrEmpty(userdb.config_blacklist_ip) && arrPublic != null && (arrPublic.Contains(header.ip_remote_ipv4.Trim()) || arrPublic.Contains(header.ip_remote_ipv6.Trim())))
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
                if (header.partner_code.ToUpper() != tokenPayload.partner_code.ToUpper() || header.partner_code.ToUpper() != userdb.partner_code.ToUpper())
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
    }
}
