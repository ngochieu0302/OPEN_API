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
    public class bh_tich_hop_api_log_service: BaseRepository
    {
        public async Task<BaseResponse<int>> AddLogApi(pbh_tich_hop_api_log_nh_param model) 
        {
            return await Insert(StoreProcedureContants.PBH_TICH_HOP_API_LOG_NH, model);
        }
    }
}
