using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ESCS.API.Attributes;
using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ESCS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ESCSAuth]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ILogMongoService<LogContent> _logContent;
        private readonly ICacheServer _cacheServer;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ILogMongoService<LogContent> logContent, ICacheServer cacheServer)
        {
            _logger = logger;
            _logContent = logContent;
            _cacheServer = cacheServer;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            //var pathFileTemplate = @"D:\wwwroot\FILE_CAM_XOA\CTYBHABC\MAU_IN\PDF\mau_in_xe_bthuong_pasc_v2.xml";
            //System.Xml.XmlDocument xmlTemplateDoc = new System.Xml.XmlDocument();
            //xmlTemplateDoc.Load(pathFileTemplate);
            //XmlNamespaceManager nsmgr = new XmlNamespaceManager((XmlNameTable)new NameTable());
            //nsmgr.AddNamespace("w", "http://schemas.microsoft.com/office/word/2003/wordml");
            //nsmgr.AddNamespace("aml", "http://schemas.microsoft.com/aml/2001/core");
            //var nodewp = xmlTemplateDoc.SelectSingleNode("//w:p/aml:annotation[@w:name='BEGIN_VCX_XE']", nsmgr).ParentNode;


            //throw new Exception("Thanh test thông báo lỗi realtime");
            //_logContent.AddLogAsync(new LogContent() { content = "WeatherForecastController - Kết nối thành công" });
            //_cacheServer.Set(RedisCacheMaster.ConnectionName, RedisCacheMaster.Endpoint, "KET_NOI_HE_THONG", "WeatherForecastController - Kết nối thành công", DateTime.Now.AddMinutes(OpenIDConfig.TimeLiveDataCacheMinute) - DateTime.Now, RedisCacheMaster.DatabaseIndex);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
