using ESCS.COMMON.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway
{
    public class CheckSumCommon
    {
        public static string MICTruyVanXe(string merchant_secret, string ma_dvi, string ma_chi_nhanh, string so_gcn, string so_hdong, string bien_so_xe, string so_khung, string so_may, string cmt_kh, string mst_kh)
        {
            string chuoi = merchant_secret + ma_dvi + ma_chi_nhanh + so_gcn + so_hdong + bien_so_xe + so_khung + so_may + cmt_kh + mst_kh;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICThongTinGCN(string merchant_secret, string ma_dvi, string ma_chi_nhanh, string so_id_hdong, string so_id_gcn)
        {
            string chuoi = merchant_secret + ma_dvi + ma_chi_nhanh + so_id_hdong + so_id_gcn;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICThongTinGCNNguoi(string merchant_secret, string ma_dvi, string ma_chi_nhanh, string so_id_hdong, string so_id_gcn)
        {
            string chuoi = merchant_secret + ma_dvi + ma_chi_nhanh + so_id_hdong + so_id_gcn;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICChuyenDLBoiThuong(string merchant_secret, string ma_dvi, string ma_chi_nhanh, decimal? so_id_hs)
        {
            string chuoi = merchant_secret + ma_dvi + ma_chi_nhanh  + (so_id_hs==null?"": ((long)so_id_hs).ToString());
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICThongTinThanhToanPhi(string merchant_secret, string ma_dvi, string so_id_hd)
        {
            string chuoi = merchant_secret + ma_dvi + so_id_hd;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICDanhSachFile(string merchant_secret, string ma_dvi, string ma_chi_nhanh, string so_id_hdong, string so_id_gcn)
        {
            string chuoi = merchant_secret + ma_dvi + ma_chi_nhanh + so_id_hdong + so_id_gcn;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICLayFile(string merchant_secret, string ma_dvi, string ma_chi_nhanh, string so_id_hdong, string ma_file)
        {
            string chuoi = merchant_secret + ma_dvi + ma_chi_nhanh + so_id_hdong + ma_file;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICTichHopEmailSMS(string merchant_secret, string ma_dvi)
        {
            string chuoi = merchant_secret + ma_dvi;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICSendSMS(string merchant_secret, string ma_dvi, string ma_chi_nhanh, string so_gcn, string so_hdong, string bien_so_xe, string so_khung, string so_may, string cmt_kh, string mst_kh)
        {
            string chuoi = merchant_secret + ma_dvi + ma_chi_nhanh + so_gcn + so_hdong + bien_so_xe + so_khung + so_may + cmt_kh + mst_kh;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string MICTruyVanNguoi(string merchant_secret, string ma_dvi, string ma_chi_nhanh, string so_gcn, string ngay_sinh, string cmt_ndbh, string ctrinh)
        {
            string chuoi = merchant_secret + ma_dvi + ma_chi_nhanh + so_gcn + ngay_sinh + cmt_ndbh + ctrinh;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, merchant_secret);
            return hmacSha1.Replace("=", "%3d").Replace(" ", "+");
        }
        public static string DIGINSTruyVanNguoi(string secret_key, string id, string partner, string code, long dateTime)
        {
            string chuoi = "SecretKey="+ secret_key + "&Id="+ id + "&UserName="+ partner + "&Code="+ code + "&AlgorithmEncryption=HMACSHA256&dateTime="+ dateTime;
            string hmacSha1 = Utilities.HmacSha256Digest(chuoi, secret_key);
            return hmacSha1;
        }
    }
}
