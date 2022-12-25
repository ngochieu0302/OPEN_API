using ESCS.COMMON.ExceptionHandlers;
using ESCS.COMMON.Response;
using ESCS.DAL.OpenID;
using ESCS.MODEL.OpenID;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.BUS.OpenID
{
    public interface IErrorCodeService
    {
        Task<PaginationGenneric<sys_error_code>> GetPaging(sys_error_code search);
        Task<int> Save(sys_error_code model);
        Task<sys_error_code> Get(sys_error_code search);
    }
    public class ErrorCodeService: IErrorCodeService
    {
        private readonly IErrorCodeRepository _errorCodeRepository;
        public ErrorCodeService(IErrorCodeRepository errorCodeRepository)
        {
            _errorCodeRepository = errorCodeRepository;
        }

        public Task<PaginationGenneric<sys_error_code>> GetPaging(sys_error_code search)
        {
            return _errorCodeRepository.GetPaging(search);
        }
        public Task<sys_error_code> Get(sys_error_code search)
        {
            return _errorCodeRepository.Get(search);
        }

        public Task<int> Save(sys_error_code model)
        {
            try
            {
                return _errorCodeRepository.Save(model);
            }
            catch(Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
    }
}
