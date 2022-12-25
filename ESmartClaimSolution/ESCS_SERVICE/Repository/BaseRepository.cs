using Dapper;
using ESCS_SERVICE.Common;
using ESCS_SERVICE.Config;
using ESCS_SERVICE.Contants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS_SERVICE.Repository
{
    public class BaseRepository
    {
        public async Task<BaseResponse<int>> Insert<T>(string storedPorcecure, T model) where T : class
        {
            BaseResponse<int> result = new BaseResponse<int>();
            using (var connection = new SqlConnection(ConnectionStrings.ESCSConnectionString))
            {
                try
                {
                    int identity = await connection.ExecuteScalarAsync<int>(storedPorcecure, model, commandType: CommandType.StoredProcedure);
                    result.data_info = identity;
                }
                catch(Exception ex)
                {
                    result.data_info = 0;
                    result.state_info.status = HttpStatus.STATUS_NOTOK;
                    result.state_info.message_body = ex.Message;
                }
                return result;
            }
        }
    }
}
