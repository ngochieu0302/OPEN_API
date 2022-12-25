using ESCS.COMMON.Common;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ESCS.DAL.Repository.Oracle
{
    public static class OracleDynamicParametersExtention
    {
        /// <summary>
        /// Mapping mảng với model
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="_params"></param>
        /// <param name="paramArrayName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static OracleDynamicParameters MapArrayValue<B>(this OracleDynamicParameters _params, string paramArrayName, IEnumerable<B> data, bool isArrObject = true)
        {
            if (data == null || data.Count() <= 0)
            {
                return _params;
            }

            string prefix = "a_" + paramArrayName + "_";
            foreach (var _param in _params.parameters)
            {
                if (!_param.Key.StartsWith("a_"))
                {
                    continue;
                }
                if (isArrObject)
                {
                    if (_param.Key.StartsWith(prefix))
                    {
                        string field = _param.Key.Substring(prefix.Length, _param.Key.Length - prefix.Length);
                        _params.parameters[prefix + field].Value = data.Select(n => n.GetType().GetProperty(field)?.GetValue(n, null)).ToArray();
                        _params.parameters[prefix + field].ArrayBindSize = data.Count();
                    }
                }
                else
                {
                    prefix = "a_" + paramArrayName;
                    if (_param.Key.StartsWith(prefix))
                    {
                        _params.parameters[prefix].Value = data.ToArray();
                        _params.parameters[prefix].ArrayBindSize = data.Count();
                    }
                }
            }
            return _params;
        }
        public static OracleDynamicParameters MapArrayValue(this OracleDynamicParameters _params, string arr_name, JObject obj, bool isArrObject = true)
        {
            if (obj == null)
            {
                return _params;
            }
            var arr = obj[arr_name].Values<JToken>();
            if (isArrObject)
            {
                string paramArrayName = "a_" + arr_name + "_";
                List<string> keys = obj[arr_name].Values<JToken>().FirstOrDefault()?.Value<JObject>().Properties().Select(p => p.Name).ToList();
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        if (_params.parameters.ContainsKey(paramArrayName + key))
                        {
                            if (_params.parameters[paramArrayName + key].DbType == OracleDbType.Decimal)
                            {
                                var value = Array.ConvertAll(arr.Select(n => n.Value<string>(key)).ToArray(), decimal.Parse);
                                _params.parameters[paramArrayName + key].Value = value;
                                _params.parameters[paramArrayName + key].ArrayBindSize = value.Count();
                            }
                            else if (_params.parameters[paramArrayName + key].DbType == OracleDbType.Date)
                            {

                                var value = Array.ConvertAll(arr.Select(n => n.Value<string>(key)).ToArray(), s => DateTime.ParseExact(s, OracleRepositoryConstant.FORMAT_DATE, CultureInfo.InvariantCulture));
                                _params.parameters[paramArrayName + key].Value = value;
                                _params.parameters[paramArrayName + key].ArrayBindSize = value.Count();
                            }
                            else
                            {
                                var value = arr.Select(n => n.Value<string>(key)).ToArray();
                                _params.parameters[paramArrayName + key].Value = value;
                                _params.parameters[paramArrayName + key].ArrayBindSize = value.Count();
                            }

                        }
                    }
                }
            }
            else
            {
                var value = arr.Select(n => n.Value<object>())?.ToArray();
                if (_params.parameters.ContainsKey("a_" + arr_name) && value != null && value.FirstOrDefault() != null)
                {
                    _params.parameters["a_" + arr_name].Value = value;
                    _params.parameters["a_" + arr_name].ArrayBindSize = value == null ? 0 : value.Count();
                }
            }
            foreach (var _param in _params.parameters)
            {
                int countData = arr.Count();
                if (_param.Value.Value != DBNull.Value)
                {
                    continue;
                }
                if (isArrObject)
                {
                    var value = new object[countData];
                    for (int i = 0; i < value.Count(); i++)
                    {
                        value[i] = DBNull.Value;
                    }
                    _param.Value.Value = value;
                    _param.Value.ArrayBindSize = countData;
                }
                else
                {
                    _param.Value.ArrayBindSize = 0;
                }
            }
            return _params;
        }
        /// <summary>
        /// Mapping mảng không dùng model
        /// </summary>
        /// <param name="_params"></param>
        /// <param name="arr_name"></param>
        /// <param name="obj"></param>
        /// <param name="isArrObject"></param>
        /// <returns></returns>
        public static OracleDynamicParameters MapArrayValue(this OracleDynamicParameters _params, string arr_name, Dictionary<string, string> obj, bool isArrObject = true)
        {
            if (obj == null)
            {
                return _params;
            }
            string prefix = "a_" + arr_name + "_";
            foreach (var _param in _params.parameters)
            {
                if (!_param.Key.StartsWith("a_"))
                {
                    continue;
                }
                if (isArrObject)
                {
                    if (_param.Key.StartsWith(prefix))
                    {
                        string field = _param.Key.Substring(prefix.Length, _param.Key.Length - prefix.Length);
                        var arr_value = obj.Where(n => n.Key.StartsWith(arr_name) && n.Key.EndsWith("[" + field + "]")).Select(n => n.Value).ToArray();
                        if (_params.parameters[_param.Key].DbType == OracleDbType.Decimal)
                        {
                            var value = Array.ConvertAll<string, decimal?>(arr_value, delegate (string s) {
                                if (string.IsNullOrEmpty(s))
                                {
                                    return null;
                                }
                                return Convert.ToDecimal(s);
                            });
                            _params.parameters[_param.Key].Value = value;
                            _params.parameters[_param.Key].ArrayBindSize = value.Count();
                        }
                        else if (_params.parameters[_param.Key].DbType == OracleDbType.Date)
                        {
                            var value = Array.ConvertAll(arr_value, s => DateTime.ParseExact(s, OracleRepositoryConstant.FORMAT_DATE, CultureInfo.InvariantCulture));
                            _params.parameters[_param.Key].Value = value;
                            _params.parameters[_param.Key].ArrayBindSize = value.Count();
                        }
                        else
                        {
                            var value = arr_value;
                            _params.parameters[_param.Key].Value = value;
                            _params.parameters[_param.Key].ArrayBindSize = value.Count();
                        }
                    }
                }
                else
                {
                    var arr_value = obj.Where(n => n.Key.StartsWith(arr_name)).Select(n => n.Value).ToArray();//.FirstOrDefault()?.Split(',')
                    if (_params.parameters["a_" + arr_name].DbType == OracleDbType.Decimal)
                    {
                        var value = Array.ConvertAll(arr_value, decimal.Parse);
                        _params.parameters["a_" + arr_name].Value = value;
                        _params.parameters["a_" + arr_name].ArrayBindSize = value.Count();
                    }
                    else if (_params.parameters["a_" + arr_name].DbType == OracleDbType.Date)
                    {
                        var value = Array.ConvertAll(arr_value, s => DateTime.ParseExact(s, OracleRepositoryConstant.FORMAT_DATE, CultureInfo.InvariantCulture));
                        _params.parameters["a_" + arr_name].Value = value;
                        _params.parameters["a_" + arr_name].ArrayBindSize = value.Count();
                    }
                    else
                    {
                        var value = arr_value;
                        _params.parameters["a_" + arr_name].Value = value;
                        _params.parameters["a_" + arr_name].ArrayBindSize = value == null ? 0 : value.Count();
                    }
                    break;
                }

            }
            return _params;
        }
    }
}
