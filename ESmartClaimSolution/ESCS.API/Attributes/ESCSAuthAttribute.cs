using ESCS.COMMON.Common;
using ESCS.COMMON.Contants;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS.API.Attributes
{
    public class ESCSAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(AppSettings.KeyExpiryTime))
                throw new Exception("Thiếu thông tin cài đặt ứng dụng KeyExpiryTime");
            try
            {
                var KeyExpiryTimeStr = Utilities.Base64Decode(Utilities.DecryptByKey(AppSettings.KeyExpiryTime, ESCSConstants.HASHKEY));
                int KeyExpiryTimeNum = int.Parse(KeyExpiryTimeStr);
                if (int.Parse(DateTime.Now.ToString("yyyyMMdd")) >= KeyExpiryTimeNum)
                {
                    throw new Exception("EXPIRE");
                }
            }
            catch(Exception ex)
            {
                if (ex.Message == "EXPIRE")
                {
                    throw new Exception("Thời gian sử dụng hết hạn, vui lòng liên hệ tới quản trị viên");
                }
                throw new Exception("Cấu hình thời gian sử dụng không hợp lệ");
            }
        }
    }
}
