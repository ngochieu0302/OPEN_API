using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESCS.BUS.OpenID;
using ESCS.MODEL.OpenID.ModelView;
using ESCS.OPENID.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ESCS.OPENID.Controllers
{
    [SystemAuthen]
    public class PartnerController : BaseController
    {
        private readonly IPartnerService _partnerService;
        public PartnerController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetPaging(openid_sys_partner searchModel)
        {
            searchModel.envcode = GetUser().config_envcode;
            var res = await _partnerService.GetPaging(searchModel);
            return Json(res);
        }
        [HttpPost]
        public async Task<IActionResult> GetDetail(openid_sys_partner searchModel)
        {
            var res = await _partnerService.GetDetail(searchModel);
            return Json(res);
        }
    }
}
