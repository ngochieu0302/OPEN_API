using F23.StringSimilarity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ESCS.COMMON.Common
{
    public class TextCompare
    {
        private static string[] TEXTREPLACE = new string[] { "DICHVUSUACHUA", "SONSC", "HANBATLAI", "THAYMOI", "THAYTHE", "SUACHUA", "CONG", "CONGPH", "CONGTT", "CONGSC", "NHANCONG", "SON",
                                                             "THAY", "CANCHINH", "THAOLAP", "DANHBONG", "GO", "SUA", "THAO", "VATTU", "LAPDATLAI", "PHUTUNG", "PHUCVU", "PHUHOI", "PHUCHI",
                                                             "MOPSAU", "NAN", "MOP", "XUOC", "VO", "DICHVU", "PHUCHOI", "KHACPHUC", "HOANTHIEN", "CANCHINH", "TT", "SC","DICHVUTHAO","HONAN" };
        private static string[] TIEN_SON = new string[] { "SONSC", "SON" };
        private static string[] TIEN_NHAN_CONG = new string[] { "DICHVUSUACHUA", "HANBATLAI", "SUACHUA", "CONG", "CONGPH", "CONGTT", "CONGSC", "NHANCONG", "CANCHINH", "THAOLAP", "DANHBONG", "GO", "SUA", "THAO", "VATTU", "LAPDATLAI", "PHUTUNG", "PHUCVU", "PHUHOI", "PHUCHI",
                                                              "NAN", "DICHVU", "PHUCHOI", "KHACPHUC", "HOANTHIEN", "CANCHINH", "SC", "DICHVUTHAO","HONAN"};
        private static string[] TIEN_VAT_TU = new string[] { "THAYMOI", "THAYTHE", "THAY", "TT" };
        public static double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            source = SEOTitle(source);
            target = SEOTitle(target);
            var jw = new RatcliffObershelp();
            return jw.Similarity(source, target);
        }
        public static string GetCurrency(string target)
        {
            string loai_tien = "";
            var textreplace = target.ToUpper();
            for (int i = 0; i < TIEN_SON.Length; i++)
            {
                if (textreplace.ToUpper().StartsWith(TIEN_SON[i]))
                {
                    loai_tien = "TIEN_SON";
                }
            }
            for (int i = 0; i < TIEN_NHAN_CONG.Length; i++)
            {
                if (textreplace.ToUpper().StartsWith(TIEN_NHAN_CONG[i]))
                {
                    loai_tien = "TIEN_NHAN_CONG";
                }
            }
            for (int i = 0; i < TIEN_VAT_TU.Length; i++)
            {
                if (textreplace.ToUpper().StartsWith(TIEN_VAT_TU[i]))
                {
                    loai_tien = "TIEN_VAT_TU";
                }
            }
            return loai_tien;
        }
        public static string SEOTitle(string target)
        {
            target = target.ToLower();
            target = Regex.Replace(target, @"[áàạảãâầấậểễẩẫăắằắặẳẵ]", "a");
            target = Regex.Replace(target, @"[éèẹẻẽêếềệểễ]", "e");
            target = Regex.Replace(target, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            target = Regex.Replace(target, @"[íìịỉĩ]", "i");
            target = Regex.Replace(target, @"[ýỳỵỉỹ]", "y");
            target = Regex.Replace(target, @"[úùụủũưứừựửữ]", "u");
            target = Regex.Replace(target, @"[đ]", "d");
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
            var textreplace = target.ToUpper();
            for (int i = 0; i < TEXTREPLACE.Length; i++)
            {
                if (textreplace.ToUpper().StartsWith(TEXTREPLACE[i]))
                {
                    textreplace = textreplace.Replace(TEXTREPLACE[i], "");
                }
            }
            return textreplace;
        }
    }
}
