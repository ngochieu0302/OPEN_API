using ESCS.COMMON.Common;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using RazorEngine.Compilation.ImpromptuInterface.Dynamic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ESCS.COMMON.ExtensionMethods
{
    public static class ExtensionMethod
    {
        public static Dictionary<string, string> ToDictionaryModel<T>(this T model)
        {
            return model.GetType()
                 .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                      .ToDictionary(prop => prop.Name, prop => prop.GetValue(model, null) == null ? null : prop.GetValue(model, null).ToString());
        }
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
        public static DateTime? NumberToDateTime(this long? time)
        {
            if (time == null)
            {
                return null;
            }
            string strTime = time.ToString();
            int year = Convert.ToInt32(strTime.Substring(0, 4));
            int month = Convert.ToInt32(strTime.Substring(4, 2));
            int day = Convert.ToInt32(strTime.Substring(6, 2));
            int hh = Convert.ToInt32(strTime.Substring(8, 2));
            int mm = Convert.ToInt32(strTime.Substring(10, 2));
            int ss = Convert.ToInt32(strTime.Substring(12, 2));
            return new DateTime(year, month, day, hh, mm, ss);
        }
        public static T Clone<T>(this T obj)
        {
            var inst = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            return (T)inst?.Invoke(obj, null);
        }
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            if (collection == null)
            {
                return;
            }
            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
            }
        }
        public static object GetValue(this IDictionary<string, object> data, string key, out int? size, OracleDbType? type = null)
        {
            size = null;
            var start = key.Substring(0, 2);
            var param_name = key.Substring(2, key.Length - 2);
            if (data.ContainsKey(param_name))
            {
                if (start == "b_")
                {
                    var val = data[param_name];
                    #region Mặc định kiểu số nếu truyền null là 0
                    if (type == OracleDbType.Decimal)
                    {
                        if ((val == null || string.IsNullOrEmpty(val.ToString())))
                        {
                            val = 0;
                        }
                        else
                        {
                            string strVal = val.ToString().Replace(",", "").Trim();
                            var isNumeric = decimal.TryParse(strVal, out decimal n);
                            if (isNumeric)
                            {
                                return Convert.ToDecimal(strVal);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                    #endregion
                    return val;
                }
                if (start == "a_")
                {
                    if (type != null)
                    {
                        switch (type)
                        {
                            case OracleDbType.Decimal:
                                //default số là 0
                                var arrDecimal = ((JArray)data[param_name]).Select(jv => (jv == null || string.IsNullOrEmpty(jv.Value<string>()) || !decimal.TryParse(jv.Value<string>().Replace(",", "").Trim(), out decimal n)) ? 0 : Convert.ToDecimal(jv.Value<string>().Replace(",", "").Trim())).ToArray();
                                size = arrDecimal.Length;
                                return arrDecimal;
                            case OracleDbType.Date:
                                var arrDate = ((JArray)data[param_name]).Select(jv => DateTime.ParseExact(jv.ToString(), OracleRepositoryConstant.FORMAT_DATE, CultureInfo.InvariantCulture)).ToArray();
                                size = arrDate.Length;
                                return arrDate;
                            default:
                                var arrString = ((JArray)data[param_name]).Select(jv => jv == null ? "" : jv.ToString()).ToArray();
                                size = arrString.Length;
                                return arrString;
                        }
                    }

                }
            }
            return null;
        }
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            return source;
        }
        public static IQueryable<TSource> WhereIf<TSource>(this IMongoQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            return source;
        }
        public static void AddWithExists<T, V>(this IDictionary<T, V> source, T key, V value)
        {
            if (!source.ContainsKey(key))
                source.Add(key, value);
            else
                source[key] = value;
        }
        public static string GetString(this IDictionary<string, object> source, string key)
        {
            if (!source.ContainsKey(key))
                return "";
            else if (source[key] == null)
                return "";
            return source[key].ToString();
        }
        public static string GetString(this IDictionary<string, string> source, string key)
        {
            if (!source.ContainsKey(key))
                return "";
            else if (source[key] == null)
                return "";
            return source[key].ToString();
        }
        public static string GetValueStringObj(this object obj, string propertyName)
        {
            return obj?.GetType()?.GetProperty(propertyName)?.GetValue(obj, null)?.ToString();
        }
        public static string ChuanHoaDuongDanRemote(this string duong_dan)
        {
            if (AppSettings.FolderSharedUsed && duong_dan.StartsWith(@"\") && !duong_dan.StartsWith(@"\\"))
                duong_dan = @"\" + duong_dan;
            return duong_dan;
        }
        public static string ChuanHoaLink(this string duong_dan)
        {
            if (duong_dan!=null)
                duong_dan = duong_dan.Replace(@"\","/");
            return duong_dan;
        }
        public static string GetOutValue(this object obj, string propertyName)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (dictionary.ContainsKey(propertyName))
                    return dictionary[propertyName].ToString();
                return null;
            }
            catch
            {
                return null;
            }
        }
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
        public static string RemoveZeroFromEnd(this string value, string suffix)
        {
            if (value.EndsWith(suffix))
            {
                return value.Substring(0, value.Length - suffix.Length);
            }
            else
            {
                return value;
            }
        }
        public static string FormatStringToInterger(this string value)
        {
            if (value.EndsWith(",00"))
                value = value.RemoveZeroFromEnd(",00").Replace(".", "").Replace(" ", "");
            else if (value.EndsWith(".00"))
                value = value.RemoveZeroFromEnd(".00").Replace(".", "").Replace(" ", "");
            else if (value.EndsWith(",0"))
                value = value.RemoveZeroFromEnd(",0").Replace(".", "").Replace(" ", "");
            else if (value.EndsWith(".0"))
                value = value.RemoveZeroFromEnd(".0").Replace(".", "").Replace(" ", "");
            else
                value = value.Replace(".", "").Replace(",", "").Replace(" ", "");
            return value;
        }
        public static string FormatString(this string value)
        {
            value = Regex.Replace(value.Trim(), @"[0-9]", "").Trim();
            return value;
        }
        public static string BAFBODAU(this string target)
        {
            target = target.ToLower();
            target = Regex.Replace(target, @"[áàạảãâầấậểễẩẫăắằắặẳẵ]", "a");
            target = Regex.Replace(target, @"[éèẹẻẽêếềệểễ]", "e");
            target = Regex.Replace(target, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            target = Regex.Replace(target, @"[íìịỉĩ]", "i");
            target = Regex.Replace(target, @"[ýỳỵỉỹ]", "y");
            target = Regex.Replace(target, @"[úùụủũưứừựửữ]", "u");
            target = Regex.Replace(target, @"[đ]", "d");
            target = Regex.Replace(target, @"[0-9]", "");
            target = Regex.Replace(target.Trim(), @"[^a-z-\s]", "").Trim();
            target = Regex.Replace(target.Trim(), @"\s+", "");
            target = Regex.Replace(target, @"\s", "");
            while (true)
            {
                if (target.IndexOf("--") != -1)
                {
                    target = target.Replace("--", "");
                }
                else
                {
                    break;
                }
            }
            target = target.ToUpper();
            return target;
        }
        public static List<T> ConvertDataTable<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public static IEnumerable<(T item, int index)> SelectWithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
        public static string FormatMoney(this decimal? number)
        {
            if (number == null)
                return "0";
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");  
            var val = number.Value.ToString("#,###", cul.NumberFormat);
            return string.IsNullOrEmpty(val) ? "0" : val;
        }
    }
}
