using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.MODEL.OpenID;
using ESCS.OPENID.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ESCS.OPENID.Controllers
{
    [SystemAuthen]
    public class CacheController : Controller
    {
        private readonly ICacheServer _cacheServer;
        public CacheController(ICacheServer cacheServer)
        {
            _cacheServer = cacheServer;
        }
        public IActionResult Storedprocedure()
        {
            return View();
        }
        public IActionResult ActionCode()
        {
            return View();
        }
        public IActionResult ResponseData()
        {
            return View();
        }
        public IActionResult PartnerInfo()
        {
            return View();
        }
        public IActionResult Get(cache_search model)
        {
            if (model == null)
                model = new cache_search();
            model.db_index = model.db_index ?? 0;
            string data = _cacheServer.Get<string>(RedisCacheReplication.ConnectionName, RedisCacheReplication.Endpoint, model.key, model.db_index.Value);
            return Ok(data);
        }
        public IActionResult Delete(cache_search model)
        {
            if (model == null)
                model = new cache_search();
            model.db_index = model.db_index ?? 0;
            bool data = _cacheServer.Remove(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, model.key, model.db_index.Value);
            return Ok(data?"Xóa cache thành công":"Xóa cache thất bại");
        }
        public IActionResult GetAllKeyParam(cache_search model)
        {
            if (model == null)
                model = new cache_search();
            model.db_index = model.db_index ?? 0;
            var pattern = "*ACTION.PARAM";
            if (!string.IsNullOrEmpty(model.database))
                pattern += "*"+model.database.ToUpper()+"*";
            if (!string.IsNullOrEmpty(model.schema))
                pattern += "*"+model.schema.ToUpper()+"*";
            if (!string.IsNullOrEmpty(model.search))
                pattern += "*" + model.search.ToUpper() + "*";
            List<string> data = _cacheServer.GetKeysByPatterm(RedisCacheReplication.Endpoint, pattern, model.db_index.Value);
            return Ok(data);
        }
        public IActionResult GetAllKeyActionCode(cache_search model)
        {
            if (model == null)
                model = new cache_search();
            model.db_index = model.db_index ?? 0;
            var pattern = "*CACHE.ACTION";
            if (!string.IsNullOrEmpty(model.search))
                pattern += "*" + model.search + "*";
            List<string> data = _cacheServer.GetKeysByPatterm(RedisCacheReplication.Endpoint, pattern, model.db_index.Value);
            return Ok(data);
        }
        public IActionResult GetAllKeyResponse(cache_search model)
        {
            if (model == null)
                model = new cache_search();
            model.db_index = model.db_index ?? 0;
            var pattern = "*CACHE.RESPONSE.DATA";
            if (!string.IsNullOrEmpty(model.search))
                pattern += "*" + model.search + "*";
            List<string> data = _cacheServer.GetKeysByPatterm(RedisCacheReplication.Endpoint, pattern, model.db_index.Value);
            return Ok(data);
        }
        public IActionResult GetAllKeyPartnerInfo(cache_search model)
        {
            if (model == null)
                model = new cache_search();
            model.db_index = model.db_index ?? 0;
            var pattern = "PARTNER";
            if (!string.IsNullOrEmpty(model.search))
                pattern += "*" + model.search + "*";
            List<string> data = _cacheServer.GetKeysByPatterm(RedisCacheReplication.Endpoint, pattern, model.db_index.Value);
            return Ok(data);
        }
    }
}
