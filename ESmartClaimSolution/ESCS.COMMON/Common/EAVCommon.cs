using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class EAVCommon
    {
        public static IEnumerable<EAVModel> GetValueByKey(string json, string key, string loai, string stt)
        {
            JObject jobject = JObject.Parse(json);
            var model = jobject.SelectTokens(key).Select(n => new EAVModel() { ma = n.Path, gia_tri = n.Value<string>(), loai = loai, stt = stt });
            return model;
        }
    }
}
