using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Response.ApiGateway.PJICO
{
    public class KQThongTinGCNPjico
    {
        //SẢN PHẨM TNDS BẮT BUỘC Ô TÔ
        public string POL_DIVN_CODE { get; set; }//Mã chi nhánh
        public string POL_SC_CODE { get; set; }//Mã loại hình bảo hiểm (5106) (5101)
        public string POL_ISSUE_DT { get; set; }//Ngày cấp đơn format: 18/08/2022 09:33
        public string POL_CUST_CODE { get; set; }//Mã khai thác viên
        public string POL_NO { get; set; }//Số đơn (Số giấy chứng nhận) (P-22/TCT/TSA/5101/001094)
        public string POL_DEPT_CODE { get; set; }//Mã phòng ban
        public string POL_ASSR_CODE { get; set; }//Mã chủ xe
        public string POL_ASSURED_NAME { get; set; }//Tên chủ xe
        public string POL_ADDR1 { get; set; }//Địa chỉ chủ xe
        public string POL_PAYER_CODE { get; set; }//Mã khách hàng
        public string POL_PAYER_NAME { get; set; }//Tên khách hàng
        public string POL_PAYER_ADDR1 { get; set; }//Địa chỉ khách hàng
        public string POL_FM_DT { get; set; }//Ngày bắt đầu hiệu lực bảo hiểm format: 18/08/2022 09:33
        public string POL_TO_DT { get; set; }//Ngày kết thúc hiệu lực bảo hiểm format: 18/08/2022 09:33
        public string CUST_PHONE { get; set; }//Số điện thoại khách hàng
        public string CUST_EMAIL_ID_PER { get; set; }//Email khách hàng
        public string CUST_REF_ID1 { get; set; }//MST/CMT/CCCD khách hàng
        public string CUST_TYPE { get; set; }//Loại khách hàng
        public string VEH_REGN_NO { get; set; }//Biển số xe
        public string VEH_CHASSIS_NO { get; set; }//Số khung
        public string VEH_ENGINE_NO { get; set; }//Số máy
        public string VEH_TYPE { get; set; }//Loại xe
        public string VEH_TYPE_NAME { get; set; }//Tên loại xe
        public string VEH_USE_TYPE { get; set; }//Mục đích sử dụng
        public string VEH_SEATS { get; set; }//Số chỗ ngồi

        public decimal? RISK_LC_SI_BHDSBB { get; set; }//Số tiền bảo hiểm TNDS BB
        public decimal? RISK_LC_PREM_BHDSBB { get; set; }//Phí bảo hiểm TNDS BB

        public decimal? RSMI_LC_SI_NG { get; set; }//MTN về người TNDS BB (5101)
        public decimal? RSMI_LC_SI_TS { get; set; }//MTN về tài sản TNDS BB(5101-1)
        public decimal? RSMI_LC_SI_HK { get; set; }//MTN về hành khách TNDS BB (5101-2)

        public decimal? RISK_LC_SI_BHDSTN { get; set; }//Số tiền bảo hiểm TNDS tự nguyện
        public decimal? RISK_LC_PREM_BHDSTN { get; set; }//Phí bảo hiểm TNDS tự nguyện

        public decimal? RSMI_LC_SI_TN_NG { get; set; }//MTN về người TNDS tự nguyện(5102)
        public decimal? RSMI_LC_SI_TN_TS { get; set; }//MTN về tài sản TNDS tự nguyện(5102-1)
        public decimal? RSMI_LC_SI_TN_HK { get; set; }//MTN về hành khách TNDS tự nguyện(5102-2)
        public string RSMI_LC_SI_TN_MAX { get; set; }//Tổng MTN tối đa - Đang là chữ 

        public decimal? RISK_LC_SI_BHTNDSHH { get; set; }//Tổng MTN hàng hóa 
        public string RSMI_LC_SI_HH { get; set; }//MTN hàng hóa/tấn 
        public string VEH_GOOD { get; set; }//Loại hàng hóa
        public string DL_DESC { get; set; }//Mức miễn thường hàng(5103)
        public decimal? RISK_LC_PREM_BHTNDSHH { get; set; }//Phí bảo hiểm TNDS hàng hóa
        public string RSMI_HH_CC { get; set; }//Điều khoản bổ sung Hàng hóa chính chủ

        public decimal? RSMI_TNNN_SEATS { get; set; }//Số chỗ ngồi trên xe - PJICO trả lời là số người tham gia bảo hiểm
        
        public decimal? RISK_LC_SI_BHTNDSNN { get; set; }//Tổng MTN người ngồi trên xe (5104)
        public decimal? RISK_LC_PREM_BHTNDSNN { get; set; }//Phí bảo hiểm Tai nạn người ngồi 

        public decimal? RISK_LC_SI_BHTNDSLP { get; set; }//Tổng MTN lái phụ xe (5105)
        public decimal? RISK_LC_PREM_BHTNDSLP { get; set; }//Phí bảo hiểm Tai nạn lái phụ xe
        public decimal? RSMI_TNLP_SEATS { get; set; }//Số lái phụ xe (Phát triển thêm)
        public decimal? VAT_TOTAL { get; set; }//Tổng VAT

        //SẢN PHẨM TỰ NGUYỆN
        public string MAKE_NO { get; set; }//Hãng xe
        public string MODEL_NO { get; set; }//Hiệu xe
        public string VEH_WEIGHT { get; set; }//Trọng tải
        public string VEH_NO_PASS { get; set; }//Số chỗ
        public string VEH_YEAR { get; set; }//Năm sản xuất

        public decimal? VEH_LC_AMT { get; set; }//Giá trị xe
        public decimal? VCX_LC_SI { get; set; }//STBH vật chất xe (5106)
        public decimal? VCX_DL_VAL { get; set; }//Số tiền khấu trừ vật chất xe

        public string VCX_ITEM { get; set; }//Danh sách điều khoản bổ sung
        public string VCX_ITEM_02_DEDUCT { get; set; }//Mức khấu trừ điều khoản bổ sung 002
        public string VCX_ITEM_03_DEDUCT { get; set; }//Mức khấu trừ điều khoản bổ sung 003
        public string VCX_ITEM_06_DEDUCT { get; set; }//Mức khấu trừ điều khoản bổ sung 006

        public decimal? TN_SI_NG { get; set; }//MTN về người BH TNDS Tự nguyện (5102)
        public decimal? TN_SI_TS { get; set; }//MTN về tài sản BH TNDS tự nguyện(5102 -1)
        public decimal? TN_SI_HK { get; set; }//MTN về hành khách BH TNDS tự nguyện (5102 -2)
        public decimal? TN_SI_MAX { get; set; }//MTN tối đa BH TNDS tự nguyện

        public decimal? HH_WEIGHT { get; set; }//Trọng tải hàng hóa (đơn vị: tấn)
        public decimal? HH_LC_SI { get; set; }//MTN BH hàng hóa / tấn(5103)
        public string HH_GOOD_TYPE { get; set; }//Loại hàng hóa
        public decimal? HH_TOTAL_SI { get; set; }//Tổng MTN hàng hóa

        public string NN_SEAT_NO { get; set; }//Số chỗ người ngồi trên xe
        public decimal? NN_LC_SI { get; set; }//MTN BH Tai nạn người ngồi trên xe (5104)

        public string LP_DRIVER_NO { get; set; }//Số lái phụ xe
        public decimal? LP_LC_SI { get; set; }//MTN bảo hiểm tai nạn lái phụ xe(5105)

        public decimal? TOTAL { get; set; }//Tổng phí (phí + VAT)
        public string IS_NEW { get; set; }//Đơn mới hay tái tục (Y: mới, N: tái tục)
    }
}
