using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC
{
    public class data_abic_create
    {
        public string RspCode { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }
        public IEnumerable<res_push_data_abic_create> data { get; set; }
    }
    public class res_push_data_abic_create
    {
        public decimal? so_id { get; set; }
        public string ma_dvi_ql { get; set; }
        public string ma_dvi { get; set; }
        public string so_hs { get; set; }
        public string ngay_ht { get; set; }
        public string nguon { get; set; }
        public decimal? so_id_hd { get; set; }
        public decimal? so_id_dt { get; set; }
        public string ten_dt { get; set; }
        public string dien_thoai { get; set; }
        public string email { get; set; }
        public string ngay_sinh { get; set; }
        public string cmt { get; set; }
        public string gioi_tinh { get; set; }
        public string so_gcn { get; set; }
        public string loai_kh { get; set; }
        public string ngay_tb { get; set; }
        public string gio_tb { get; set; }
        public string nguoi_tb { get; set; }
        public string nguoi_tbla { get; set; }
        public string dthoai_nguoi_tb { get; set; }
        public string email_nguoi_tb { get; set; }
        public string ma_nt { get; set; }
        public string hinh_thuc_tt { get; set; }
        public string chu_tk { get; set; }
        public string so_tk { get; set; }
        public string ngan_hang { get; set; }
        public string chi_nhanh { get; set; }
        public string ngay_btv_nhan { get; set; }
        public string nsd { get; set; }
        public string ngay_duyet { get; set; }
        public string ngay_duyet_bt { get; set; }
        public decimal? ngay_huy { get; set; }
        public decimal? ngay_tchoi { get; set; }
        public string nsd_bt { get; set; }
        public string nsd_duyet { get; set; }
        public string nsd_tuchoi { get; set; }
        public int? lan { get; set; }
        public string benh_vien { get; set; }
        public string nha_thuoc { get; set; }
        public string nguyen_nhan_ma { get; set; }
        public string hinh_thuc { get; set; }
        public string chan_doan { get; set; }
        public string ngay_vv { get; set; }
        public string ngay_rv { get; set; }
        public string ngay_xra { get; set; }
        public string noi_xra_tnan { get; set; }
        public string nguyen_nhan_tnan { get; set; }
        public string hau_qua_tnan { get; set; }
        public string ghi_chu { get; set; }
        public string loai { get; set; }
        public string lh_nv { get; set; }
        public decimal? so_ngay_yc { get; set; }
        public decimal? so_ngay_duyet { get; set; }
        public decimal? tien_uoc_dp { get; set; }
        public decimal? tien_yc { get; set; }
        public decimal? tien_giam { get; set; }
        public string nguyen_nhan_giam { get; set; }
        public decimal? tien_bt { get; set; }
        public string trang_thai { get; set; }
        public string so_hd { get; set; }
        public string ma_kh { get; set; }
        public string ten_kh { get; set; }
        public decimal? lan_bl { get; set; }
        public string noi_dung_bl { get; set; }
        public string ngay_hl { get; set; }
        public string ngay_kt { get; set; }
        public string lhnv { get; set; }
        public string ma_nv { get; set; }
        public string nsd_tn { get; set; }
        public string nsd_tr { get; set; }
        public string goi_bh { get; set; }
        public string goi_bh_ten { get; set; }
        public string nsd_bl { get; set; }
        public string ngayd { get; set; }
        public string ngayc { get; set; }
        public string csyt { get; set; }
        public string nhathuoc { get; set; }
        public string chi_tiet_ql { get; set; }
        public string ngay_bd { get; set; }
        public string ngay_dd { get; set; }
        public string trang_thai_1 { get; set; }
        public string dieu_tri_ten { get; set; }
        public string nguyen_nhan { get; set; }
        public string ngay_tbao { get; set; }
        public string ngay_tt { get; set; }
        public string ghi_chu_nb { get; set; }
        public string nn_giam { get; set; }
        public string text { get; set; }
        public string cvu { get; set; }
    }
}
