using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESCS.COMMON.ExtensionMethods;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.COMMON.Response;
using ESCS.OPENID.Attributes;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace ESCS.OPENID.Controllers
{
    [SystemAuthen]
    public class LogController : BaseController
    {
        private readonly IMongoDBService<LogRequest> _logRequest;
        private readonly IMongoDBService<LogResponse> _logResponse;
        public LogController(IMongoDBService<LogRequest> logRequest, IMongoDBService<LogResponse> logResponse)
        {
            _logRequest = logRequest;
            _logResponse = logResponse;
        }
        public IActionResult RequestIndex()
        {
            return View();
        }
        public async Task<IActionResult> RequestData(LogRequestSearch search)
        {
            search.trang = search.trang ?? 1;
            search.so_dong = search.so_dong ?? 20;
            var data = await _logRequest.All
                        .WhereIf(!string.IsNullOrEmpty(search.tim), n=> 
                            n.schema.ToLower().Contains(search.tim.ToLower()) ||
                            n.host.ToLower().Contains(search.tim.ToLower()) ||
                            n.path.ToLower().Contains(search.tim.ToLower()) ||
                            n.query_string.ToLower().Contains(search.tim.ToLower())||
                            n.body.ToLower().Contains(search.tim.ToLower()) ||
                            n.headers["eaction"].ToLower().Contains(search.tim.ToLower()) ||
                            n.headers["eAction"].ToLower().Contains(search.tim.ToLower()) ||
                            n.headers["epartnercode"].ToLower().Contains(search.tim.ToLower()) ||
                            n.headers["ePartnerCode"].ToLower().Contains(search.tim.ToLower()) ||
                            n.headers["esignature"].ToLower().Contains(search.tim.ToLower()) ||
                            n.headers["eSignature"].ToLower().Contains(search.tim.ToLower())
                        )
                        .OrderByDescending(n=>n.time_request)
                        .ToPagedListAsync(search.trang.Value, search.so_dong.Value);
            PaginationGenneric<LogRequest> res = new PaginationGenneric<LogRequest>();
            res.data = data;
            res.tong_so_dong = data.GetMetaData().TotalItemCount;
            return Ok(res);
        }
        public IActionResult ResponseDataById(string id)
        {
            var data = _logResponse.All.Where(n => n.id == id).FirstOrDefault();
            return Ok(data);
        }
    }
}
