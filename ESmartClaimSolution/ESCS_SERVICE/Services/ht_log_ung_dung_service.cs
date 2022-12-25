using ESCS_SERVICE.Common;
using ESCS_SERVICE.Contants;
using ESCS_SERVICE.Model.Param;
using ESCS_SERVICE.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS_SERVICE.Services
{
    public class ht_log_ung_dung_service : BaseRepository
    {
        public async Task<BaseResponse<int>> AddLogRequest(pht_log_ung_dung_nh_param model)
        {
            return await Insert(StoreProcedureContants.PHT_LOG_UNG_DUNG_NH, model);
        }
    }
}
