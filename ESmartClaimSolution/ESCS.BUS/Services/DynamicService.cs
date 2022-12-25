using ESCS.COMMON.Caches;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.Common;
using ESCS.COMMON.ExceptionHandlers;
using ESCS.COMMON.Oracle;
using ESCS.COMMON.Request;
using ESCS.COMMON.Response;
using ESCS.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ESCS.BUS.Services
{
    public interface IDynamicService
    {
        #region Thành phần cũ
        Pagination GetPaging(BaseRequest model, HeaderRequest partner);
        Task<Pagination> GetPagingAsync(BaseRequest model, HeaderRequest partner);
        object GetScalar(BaseRequest model, HeaderRequest partner);
        Task<object> GetScalarAsync(BaseRequest model, HeaderRequest partner);
        object Get(BaseRequest model, HeaderRequest partner);
        Task<object> GetAsync(BaseRequest model, HeaderRequest partner);
        object GetList(BaseRequest model, HeaderRequest partner);
        Task<object> GetListAsync(BaseRequest model, HeaderRequest partner);
        object GetMultiple(BaseRequest model, HeaderRequest partner);
        Task<object> GetMultipleAsync(BaseRequest model, HeaderRequest partner);
        Task<DataSet> GetMultipleToDataSetAsync(IDictionary<string, object> model, HeaderRequest partner);
        Task<DataSet> GetDataSetAsync(IDictionary<string, object> model, HeaderRequest partner);
        int PostData(BaseRequest model, HeaderRequest partner);
        Task<int> PostDataAsync(BaseRequest model, HeaderRequest partner);
        Task<int> PostDataMultipleAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs);
        Task<object> PostDataMultipleScalarAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs);
        void RemoveCacheParam();
        #endregion
        #region Dữ liệu Dictionary
        Task<object> ExcuteAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs = null, Action<Dictionary<string, object>> actionOutPutValue = null);
        Task<dynamic> ExcuteDynamicAsync(BaseRequest model, HeaderRequest partner, Dictionary<string, bool> prefixs = null, Action<Dictionary<string, object>> actionOutPutValue = null);
        ActionConnection GetConnection(HeaderRequest partner);
        Task ClearCacheActions(HeaderRequest partner, ICacheServer _cacheServer, string actions);
        Task<dynamic> ExcuteDynamicNewAsync(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null);
        Task<IEnumerable<T>> ExcuteListAsync<T>(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) where T : class;
        Task<T> ExcuteSingleAsync<T>(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) where T : class;
        Task ExcuteReaderAsync(IDictionary<string, object> model, HeaderRequest partner, Action<IDataReader, List<string>> readData);
        #endregion
    }
    public class DynamicService : IDynamicService
    {
        private readonly IDynamicRepository _dynamicRepository;
        public DynamicService(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }
        public object Get(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return _dynamicRepository.Get(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<object> GetAsync(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return await _dynamicRepository.GetAsync(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public object GetScalar(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return _dynamicRepository.GetScalar(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<object> GetScalarAsync(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return await _dynamicRepository.GetScalarAsync(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public object GetList(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return _dynamicRepository.GetList(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<object> GetListAsync(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return await _dynamicRepository.GetListAsync(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public object GetMultiple(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return _dynamicRepository.GetMultiple(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<object> GetMultipleAsync(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return await _dynamicRepository.GetMultipleAsync(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<DataSet> GetMultipleToDataSetAsync(IDictionary<string, object> model, HeaderRequest partner)
        {
            try
            {
                return await _dynamicRepository.GetMultipleToDataSetAsync(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<DataSet> GetDataSetAsync(IDictionary<string, object> model, HeaderRequest partner)
        {
            try
            {
                return await _dynamicRepository.GetDataSetAsync(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public int PostData(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return _dynamicRepository.PostData(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<int> PostDataAsync(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return await _dynamicRepository.PostDataAsync(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<Pagination> GetPagingAsync(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return await _dynamicRepository.GetPagingAsync(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public Pagination GetPaging(BaseRequest model, HeaderRequest partner)
        {
            try
            {
                return _dynamicRepository.GetPaging(model, partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public void RemoveCacheParam()
        {
            OracleRepositoryConstant.listParam = null;
        }
        public ActionConnection GetConnection(HeaderRequest partner)
        {
            try
            {
                return _dynamicRepository.GetConnection(partner);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<int> PostDataMultipleAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs)
        {
            try
            {
                return await _dynamicRepository.PostDataMultipleAsync(model, partner, prefixs);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<object> PostDataMultipleScalarAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs)
        {
            try
            {
                return await _dynamicRepository.PostDataMultipleScalarAsync(model, partner, prefixs);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<object> ExcuteAsync(BaseRequest model, HeaderRequest partner, IEnumerable<string> prefixs = null, Action<Dictionary<string, object>> actionOutPutValue = null)
        {
            try
            {
                return await _dynamicRepository.ExcuteAsync(model, partner, prefixs, actionOutPutValue);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<dynamic> ExcuteDynamicAsync(BaseRequest model, HeaderRequest partner, Dictionary<string, bool> prefixs = null, Action<Dictionary<string, object>> actionOutPutValue = null)
        {
            try
            {
                return await _dynamicRepository.ExcuteDynamicAsync(model, partner, prefixs, actionOutPutValue);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task ClearCacheActions(HeaderRequest partner, ICacheServer _cacheServer, string actions)
        {
            if (!string.IsNullOrEmpty(actions))
            {
                foreach (var action in actions.Split(","))
                {
                    try
                    {
                        await Task.Run(() => {
                            HeaderRequest partner_new = new HeaderRequest();
                            partner_new.action = action;
                            partner_new.envcode = partner.envcode;
                            partner_new.token = partner.token;
                            partner_new.partner_code = partner.partner_code;
                            var conn = GetConnection(partner_new);
                            string keyCache = Utilities.GetKeyCache(conn, null);
                            if (conn!=null && !string.IsNullOrEmpty(RedisCacheMaster.ConnectionName) && !string.IsNullOrEmpty(RedisCacheMaster.Endpoint))
                            {
                                _cacheServer.RemoveKeyCacheByPattern(RedisCacheMaster.Endpoint, keyCache, RedisCacheMaster.DatabaseIndex);
                            }
                        });
                    }
                    catch
                    {

                    }
                }
            }
        }
        public async Task<dynamic> ExcuteDynamicNewAsync(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null)
        {
            try
            {
                return await _dynamicRepository.ExcuteDynamicNewAsync(model, partner, actionOutPutValue);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<IEnumerable<T>> ExcuteListAsync<T>(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) where T : class
        {
            try
            {
                return await _dynamicRepository.ExcuteListAsync<T>(model, partner, actionOutPutValue);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task<T> ExcuteSingleAsync<T>(IDictionary<string, object> model, HeaderRequest partner, Action<Dictionary<string, object>> actionOutPutValue = null) where T : class
        {
            try
            {
                return await _dynamicRepository.ExcuteSingleAsync<T>(model, partner, actionOutPutValue);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
        public async Task ExcuteReaderAsync(IDictionary<string, object> model, HeaderRequest partner, Action<IDataReader,List<string>> readData)
        {
            try
            {
                await _dynamicRepository.ExcuteReaderAsync(model, partner, readData);
            }
            catch (Exception ex)
            {
                throw new OracleDbException(ex);
            }
        }
    }
}
