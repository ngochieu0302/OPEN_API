using ESCS.COMMON.Common;
using ESCS.COMMON.Request.ApiGateway.MIC;
using ESCS.COMMON.Response.ApiGateway.PJICO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.Response.ApiGateway
{
    public class ESCSConverter
    {
        public static List<GiayChungNhanESCS> ConvertGCNPjicoToESCS(IEnumerable<GiayChungNhanPjico> data)
        {
            List<GiayChungNhanESCS> dataESCS = new List<GiayChungNhanESCS>();
            if (data == null || data.Count() <= 0)
                return dataESCS;
            foreach (var item in data)
            {
                var gcn = new GiayChungNhanESCS()
                {
                    ma_chi_nhanh = item.DIVN_CODE,
                    ten_chi_nhanh = item.DIVN_NAME,
                    so_id_hdong = item.CONTRACT_ID != null ? ((long)item.CONTRACT_ID.Value).ToString() : "0",
                    so_hdong = item.CONTRACT_NO,
                    so_id_gcn = item.POL_SYS_ID != null ? ((long)item.POL_SYS_ID.Value).ToString() : "0",
                    so_gcn = item.POL_NO,
                    bien_so_xe = item.REGN_NO,
                    ten_chu_xe = item.POL_ASSURED_NAME,
                    ngay_hl_bh = item.POL_FM_DT,
                    ngay_kt_bh = item.POL_TO_DT,
                    loai_gcn = item.POL_TYPE,
                };
                if (string.IsNullOrEmpty(gcn.so_id_hdong)|| gcn.so_id_hdong=="0")
                    gcn.so_id_hdong = gcn.so_id_gcn;
                dataESCS.Add(gcn);
            }
            return dataESCS;
        }
        public static List<GiayChungNhanNguoiESCS> ConvertGCNNguoiPjicoToESCS(IEnumerable<GiayChungNhanPjico> data)
        {
            List<GiayChungNhanNguoiESCS> dataESCS = new List<GiayChungNhanNguoiESCS>();
            if (data == null || data.Count() <= 0)
                return dataESCS;
            //foreach (var item in data)
            //{
            //    var gcn = new GiayChungNhanNguoiESCS()
            //    {
            //        ma_chi_nhanh = item.DIVN_CODE,
            //        ten_chi_nhanh = item.DIVN_NAME,
            //        so_id_hdong = item.CONTRACT_ID != null ? ((long)item.CONTRACT_ID.Value).ToString() : "0",
            //        so_hdong = item.CONTRACT_NO,
            //        so_id_gcn = item.POL_SYS_ID != null ? ((long)item.POL_SYS_ID.Value).ToString() : "0",
            //        so_gcn = item.POL_NO,
            //        ten_kh = item.REGN_NO,
            //        ten_chu_xe = item.POL_ASSURED_NAME,
            //        ngay_hl_bh = item.POL_FM_DT,
            //        ngay_kt_bh = item.POL_TO_DT,
            //        loai_gcn = item.POL_TYPE,
            //    };
            //    if (string.IsNullOrEmpty(gcn.so_id_hdong) || gcn.so_id_hdong == "0")
            //        gcn.so_id_hdong = gcn.so_id_gcn;
            //    dataESCS.Add(gcn);
            //}
            return dataESCS;
        }
        public static KQThongTinGCN ConvertGCNPjicoToESCSDetail(string so_hdong, string so_id_hd, string so_id_gcn, List<KQThongTinGCNPjico> lstData)
        {
            if (string.IsNullOrEmpty(so_id_hd) || so_id_hd=="0")
                so_id_hd = so_id_gcn;
            
            foreach (var data in lstData)
            {
                KQThongTinGCN dataESCS = new KQThongTinGCN();
                if (data == null)
                    continue;
                if (string.IsNullOrEmpty(so_hdong))
                    so_hdong = data.POL_NO;

                dataESCS.hd = new List<HopDong>();
                dataESCS.hd.Add(new HopDong()
                {
                    ma_chi_nhanh = data.POL_DIVN_CODE,
                    so_id_hdong = Convert.ToDecimal(so_id_hd),//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                    ngay_ps = "",//Không sử dụng
                    ma_nhan_vien = data.POL_CUST_CODE,
                    phong_kd = data.POL_DEPT_CODE,
                    so_hdong = data.POL_NO,//Chưa có - dùng tạm của GCN POL_NO
                    loai_hd = "G",//Chưa có ('T', 'B', 'G') - PJICO mặc định là G
                    so_hdong_goc = data.POL_NO,//Chưa có - Không cần thiết - dùng tạm của GCN POL_NO
                    ma_kh = data.POL_PAYER_CODE,
                    ten_kh = data.POL_PAYER_NAME,
                    dchi_kh = data.POL_PAYER_ADDR1,
                    gio_hl_hd = TachNgayGio(data.POL_FM_DT, "GIO"),//Chưa có - dùng tạm của GCN POL_FM_DT
                    ngay_hl_hd = TachNgayGio(data.POL_FM_DT, "NGAY"),//Chưa có - dùng tạm của GCN POL_FM_DT
                    gio_kt_hd = TachNgayGio(data.POL_TO_DT, "GIO"),//Chưa có - dùng tạm của GCN POL_TO_DT
                    ngay_kt_hd = TachNgayGio(data.POL_TO_DT, "NGAY"),//Chưa có - dùng tạm của GCN POL_TO_DT
                    ngay_cap_hd = TachNgayGio(data.POL_ISSUE_DT, "NGAY"),//Chưa có - dùng tạm của GCN POL_ISSUE_DT
                    so_id_hdong_goc = Convert.ToDecimal(so_id_gcn),//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                    so_id_hdong_dau = Convert.ToDecimal(so_id_gcn),//Chưa có - Không cần thiết - dùng tạm số id giấy chứng nhận POL_SYS_ID
                    dien_thoai_kh = data.CUST_PHONE,
                    email_kh = data.CUST_EMAIL_ID_PER,
                    so_cmt_kh = data.CUST_REF_ID1,
                    mst_kh = data.CUST_REF_ID1,
                    loai_kh = data.CUST_TYPE == "C" ? "T" : "C",//PJICO C - Doanh nghiệp <=> ESCS T - Tổ chức 
                    ma_gt = "",//Chưa có
                    ten_gt = ""//Chưa có
                });
                dataESCS.gcn = new List<GiayChungNhan>();
                var gcn = new GiayChungNhan()
                {
                    ma_chi_nhanh = data.POL_DIVN_CODE,
                    so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                    so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                    so_gcn = data.POL_NO,
                    ten_chu_xe = data.POL_ASSURED_NAME,
                    dchi_chu_xe = data.POL_ADDR1,
                    md_su_dung = data.VEH_USE_TYPE == "Y" ? "C" : "K",
                    loai_xe = data.VEH_TYPE,
                    ten_loai_xe = data.VEH_TYPE_NAME,
                    bien_so_xe = data.VEH_REGN_NO,
                    so_khung = data.VEH_CHASSIS_NO,
                    so_may = data.VEH_ENGINE_NO,
                    hang_xe = data.MAKE_NO,
                    hieu_xe = data.MODEL_NO,
                    trong_tai = data.VEH_WEIGHT,
                    so_cho = "",//VEH_NO_PASS(TN), VEH_SEATS(BB)
                    so_nguoi_tgbh = "0",//Chưa có RSMI_TNNN_SEATS(BB), NN_SEAT_NO(TN)
                    so_lphu_xe = "0",
                    stbh_tnds_tn = "0",
                    nam_sx = data.VEH_YEAR,//format yyyy
                    gia_tri_xe = data.VEH_LC_AMT == null ? "0" : data.VEH_LC_AMT.Value.ToString(),
                    loai_bh = "",//Chưa có (5101 - BB, 5106 - TN)//Mình tự phân tích
                    gio_hl_gcn = TachNgayGio(data.POL_FM_DT, "GIO"),//POL_FM_DT format dd/MM/yyyy HH:mm
                    ngay_hl_gcn = TachNgayGio(data.POL_FM_DT, "NGAY"),//POL_FM_DT
                    gio_kt_gcn = TachNgayGio(data.POL_TO_DT, "GIO"),//POL_TO_DT
                    ngay_kt_gcn = TachNgayGio(data.POL_TO_DT, "NGAY"),//POL_TO_DT
                    ngay_cap_gcn = TachNgayGio(data.POL_ISSUE_DT, "NGAY"),//POL_ISSUE_DT
                    nhom_xe = ""//Không dùng
                };
                if (!string.IsNullOrEmpty(data.POL_SC_CODE) && data.POL_SC_CODE == "5101")
                {
                    gcn.so_cho = data.VEH_SEATS;
                    gcn.so_nguoi_tgbh = data.RSMI_TNNN_SEATS == null ? "0" : data.RSMI_TNNN_SEATS.Value.ToString();
                    gcn.loai_bh = "B";
                    gcn.so_lphu_xe = data.RSMI_TNLP_SEATS == null ? "0" : data.RSMI_TNLP_SEATS.Value.ToString();
                    gcn.stbh_tnds_tn = data.RSMI_LC_SI_TN_MAX;
                }
                else
                {
                    gcn.so_cho = data.VEH_NO_PASS;
                    gcn.so_nguoi_tgbh = string.IsNullOrEmpty(data.NN_SEAT_NO) && Utilities.IsNumber(data.NN_SEAT_NO) ? "0" : data.NN_SEAT_NO;
                    gcn.loai_bh = "T";
                    gcn.so_lphu_xe = string.IsNullOrEmpty(data.LP_DRIVER_NO) && Utilities.IsNumber(data.LP_DRIVER_NO) ? "0" : data.LP_DRIVER_NO;
                    gcn.stbh_tnds_tn = data.TN_SI_MAX == null ? "0" : data.TN_SI_MAX.Value.ToString();
                }
                dataESCS.gcn.Add(gcn);
                dataESCS.dk = new List<DieuKhoan>();
                //Bắt buộc
                //TNSD bắt buộc về người
                if (data.RSMI_LC_SI_NG != null && data.RSMI_LC_SI_NG != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5101",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RSMI_LC_SI_NG.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }
                //TNSD bắt buộc về tài sản
                if (data.RSMI_LC_SI_TS != null && data.RSMI_LC_SI_TS != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5101-1",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RSMI_LC_SI_TS.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }
                //TNSD bắt buộc về hành khách
                if (data.RSMI_LC_SI_HK != null && data.RSMI_LC_SI_HK != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5101-2",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RSMI_LC_SI_HK.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }

                //Cần bổ sung thêm tổng mức TNDS tự nguyện tối đa 
                //TNSD tự nguyện về người
                if (data.RSMI_LC_SI_TN_NG != null && data.RSMI_LC_SI_TN_NG != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5102",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RSMI_LC_SI_TN_NG.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }
                //TNSD tự nguyện về tài sản
                if (data.RSMI_LC_SI_TN_TS != null && data.RSMI_LC_SI_TN_TS != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5102-1",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RSMI_LC_SI_TN_TS.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }
                //TNSD tự nguyện về hành khách
                if (data.RSMI_LC_SI_TN_HK != null && data.RSMI_LC_SI_TN_HK != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5102-2",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RSMI_LC_SI_TN_HK.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }

                //TNSD của chủ xe đối với hàng hóa
                if (!string.IsNullOrEmpty(data.RSMI_LC_SI_HH) && data.RSMI_LC_SI_HH != "0")
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5103",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RSMI_LC_SI_HH,
                        so_tien_bh_toi_da = data.RISK_LC_SI_BHTNDSHH == null ? "0" : data.RISK_LC_SI_BHTNDSHH.Value.ToString(),
                        loai_hh = data.VEH_GOOD, //Bổ sung thêm cột trong db
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = data.DL_DESC,
                        loai_mt = "C",
                        ma_dkbs = data.RSMI_HH_CC
                    });
                }
                //TNSD tại nạn người ngồi trên xe
                if (data.RISK_LC_SI_BHTNDSNN != null && data.RISK_LC_SI_BHTNDSNN != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5104",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RISK_LC_SI_BHTNDSNN.Value.ToString(),
                        tl_phi = "0",
                        phi = data.RISK_LC_PREM_BHTNDSNN == null ? "0" : data.RISK_LC_PREM_BHTNDSNN.Value.ToString(),
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }
                //TNSD tại nạn lái, phụ xe
                if (data.RISK_LC_SI_BHTNDSLP != null && data.RISK_LC_SI_BHTNDSLP != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5105",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.RISK_LC_SI_BHTNDSLP.Value.ToString(),
                        tl_phi = "0",
                        phi = data.RISK_LC_PREM_BHTNDSLP == null ? "0" : data.RISK_LC_PREM_BHTNDSLP.Value.ToString(),
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }

                //Tự nguyện
                //Bảo hiểm vật chất xe (Thiếu lưu mức khấu trừ từng điều khoản bổ sung)
                if (data.VCX_LC_SI != null && data.VCX_LC_SI != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5106",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.VCX_LC_SI.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = data.VCX_DL_VAL == null ? "0" : data.VCX_DL_VAL.Value.ToString(),
                        loai_mt = "C",
                        ma_dkbs = data.VCX_ITEM
                    });
                }

                //Cần bổ sung thêm tổng mức TNDS tự nguyện tối đa 
                //TNSD tự nguyện về người
                if (data.TN_SI_NG != null && data.TN_SI_NG != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5102",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.TN_SI_NG.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }
                //TNSD tự nguyện về tài sản
                if (data.TN_SI_TS != null && data.TN_SI_TS != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5102-1",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.TN_SI_TS.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }
                //TNSD tự nguyện về hành khách
                if (data.TN_SI_HK != null && data.TN_SI_HK != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5102-2",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.TN_SI_HK.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }

                //TNSD tự nguyện của chủ xe đối với hàng hóa (HH_LC_SI - Mức trách nhiệm/tấn)
                if (data.HH_LC_SI != null && data.HH_LC_SI != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5103",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.HH_LC_SI.Value.ToString(),
                        so_tien_bh_toi_da = data.HH_TOTAL_SI == null ? "0" : data.HH_TOTAL_SI.Value.ToString(),
                        loai_hh = data.HH_GOOD_TYPE,
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = data.DL_DESC,
                        loai_mt = "C",
                        ma_dkbs = data.RSMI_HH_CC
                    });
                }
                //TNSD tự nguyện tại nạn người ngồi trên xe 
                //NN_SEAT_NO Đây cũng là số người ngồi trên xe
                if (data.NN_LC_SI != null && data.NN_LC_SI != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5104",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.NN_LC_SI.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }
                //TNSD tự nguyện tại nạn lái, phụ xe
                if (data.LP_LC_SI != null && data.LP_LC_SI != 0)
                {
                    dataESCS.dk.Add(new DieuKhoan()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5105",//POL_SC_CODE (Tự phân tích)
                        so_tien_bh = data.LP_LC_SI.Value.ToString(),
                        tl_phi = "0",
                        phi = "0",
                        thue = "0",
                        tien_mt = "0",
                        loai_mt = "K",
                        ma_dkbs = ""
                    });
                }

                dataESCS.dkbs = new List<DKBSNghiepVu>();
                if (!string.IsNullOrEmpty(data.VCX_ITEM_02_DEDUCT))
                {
                    dataESCS.dkbs.Add(new DKBSNghiepVu()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5106",//POL_SC_CODE (Tự phân tích)
                        ma_dkbs = "VCXE-MCBP1",
                        ten_dkbs = "Mất cắp bộ phận",
                        muc_ktru = data.VCX_ITEM_02_DEDUCT,
                        so_tien_ktru = 0
                    });
                }
                if (!string.IsNullOrEmpty(data.VCX_ITEM_03_DEDUCT))
                {
                    dataESCS.dkbs.Add(new DKBSNghiepVu()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5106",//POL_SC_CODE (Tự phân tích)
                        ma_dkbs = "VCXE-TX1",
                        ten_dkbs = "Thuê xe trong thời gian sửa chữa",
                        muc_ktru = data.VCX_ITEM_03_DEDUCT,
                        so_tien_ktru = 0
                    });
                }
                if (string.IsNullOrEmpty(data.VCX_ITEM_06_DEDUCT))
                {
                    dataESCS.dkbs.Add(new DKBSNghiepVu()
                    {
                        ma_chi_nhanh = data.POL_DIVN_CODE,
                        so_id_hdong = so_id_hd,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        so_id_gcn = so_id_gcn,//Chưa có - dùng tạm số id giấy chứng nhận POL_SYS_ID
                        nghiep_vu = "5106",//POL_SC_CODE (Tự phân tích)
                        ma_dkbs = "VCXE-TK",
                        ten_dkbs = "Thiệt hại động cơ do thủy kích",
                        muc_ktru = data.VCX_ITEM_06_DEDUCT,
                        so_tien_ktru = 0
                    });
                }

                dataESCS.dong_bh = new List<DongBH>();
                dataESCS.tai_bh = new List<TaiBH>();
                return dataESCS;

            }
            return new KQThongTinGCN();
        }

        private static string TachNgayGio(string ngay_gio, string loai)
        {
            if (string.IsNullOrEmpty(ngay_gio))
                return "";
            var arr = ngay_gio.Trim().Split(" ");
            if (loai == "NGAY")
                return arr[0];
            else if (loai == "GIO" && arr.Length == 2)
                return arr[1];
            else
                return "";
        }
    }
}
