using ESCS.COMMON.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class ATACCUtils
    {
        public static bool XacThucDangKy(dang_ky model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.tai_khoan + model.mat_khau + model.dien_thoai + model.ho_ten;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }
        public static bool XacThucDangNhap(dang_nhap model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.user_name + model.pass;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucTuDongDangNhap(dang_nhap_tu_dong model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.ma_thiet_bi + model.pm;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucQuyenMatKhau(quen_mat_khau model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.user_name;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucThayDoiMatKhau(thay_doi_mat_khau model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.user_name + model.pass + model.new_pass;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucSuaTTDangKy(sua_tt_dang_ky model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.user_name + model.pass + model.cmt + model.email;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucKhaiBaoHSBT(kb_hs_btcn model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.so_id_hd + model.so_id_dt + model.nguon + model.cmnd + model.ho_ten_ndbh + model.ngay_sinh + model.dthoai_tb + model.email_tb;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucXemCtietHSBT(truy_van_bt_ct model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.dvi + model.so_id + model.nv;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucKtraNDBH(check_ndbh_btcn model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.ho_ten + model.cmt + model.ngay_sinh;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucDsNguoiThan(view_hd_kh model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.ten + model.cmt + model.ngay_sinh + model.dien_thoai;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucQloiChiTiet(view_hd_kh_ct model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.ma_dvi_ql + model.so_id_hd + model.so_id_dt;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucGuiEmailBSHS(bo_sung_hs model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.nv + model.ma_cv + model.dien_thoai;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucBannerQC(banner_qc model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.status;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucTruyVanBT(truy_van_bt model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd))
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.nv + model.loai + model.dien_thoai;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }

        public static bool XacThucUpload(atacc_upload model, out string checksum)
        {
            checksum = "";
            if (string.IsNullOrEmpty(model.ma_doi_tac_nsd)) 
                return false;
            var config = CoreApiConfig.Items.Where(n => n.Partner == model.ma_doi_tac_nsd).FirstOrDefault();
            string chuoi = config.Secret + model.jobId + model.so_id_hs + model.ma_hang_muc + model.departmentId + model.type_product;
            string hmacSha1 = Utilities.HMACSHA1(chuoi, config.Secret).Replace("=", "%3d").Replace(" ", "+");
            checksum = hmacSha1;
            return hmacSha1 == model.checksum;
        }
    }
}
