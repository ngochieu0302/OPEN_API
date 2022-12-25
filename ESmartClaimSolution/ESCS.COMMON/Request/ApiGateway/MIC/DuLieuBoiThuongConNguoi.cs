using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    //Thông tin hồ sơ cũng
    public class DuLieuBoiThuongMIC
    {
        public HoSoMIC hs { get; set; }
    }
    public class HoSoMIC
    {
        public string loai_hop_dong { get; set; }
    }

    //Thông tin Model dữ liệu hồ sơ cũ
    public class DuLieuBoiThuongConNguoi_CU_TICH_HOP
    {
        public string ma_dvi { get; set; }
        public string pas { get; set; }
        public string checksum { get; set; }
        public DuLieuBoiThuongConNguoiHS_CU hs { get; set; }
        public List<DuLieuBoiThuongConNguoiDK_CU> nv { get; set; }
        public List<DuLieuBoiThuongConNguoiGRV> grv { get; set; }
        public List<DuLieuBoiThuongConNguoiVPH> vph { get; set; }
        public List<DuLieuBoiThuongConNguoiLSB> lsb { get; set; }
        public List<DuLieuBoiThuongConNguoiBKHAC> bkhac { get; set; }
    }
    public class DuLieuBoiThuongConNguoi_CU
    {
        public string ma_dvi { get; set; }
        public string pas { get; set; }
        public string checksum { get; set; }
        public DuLieuBoiThuongConNguoiHS_CU hs { get; set; }
        public List<DuLieuBoiThuongConNguoiDK_CU> dk { get; set; }
        public List<DuLieuBoiThuongConNguoiGRV> grv { get; set; }
        public List<DuLieuBoiThuongConNguoiVPH> vph { get; set; }
        public List<DuLieuBoiThuongConNguoiLSB> lsb { get; set; }
        public List<DuLieuBoiThuongConNguoiBKHAC> bkhac { get; set; }

        public DuLieuBoiThuongConNguoi_CU_TICH_HOP GetDuLieuTichHop()
        {
            DuLieuBoiThuongConNguoi_CU_TICH_HOP th = new DuLieuBoiThuongConNguoi_CU_TICH_HOP();
            th.ma_dvi = this.ma_dvi;
            th.pas = this.pas;
            th.checksum = this.checksum;
            th.hs = this.hs;
            th.nv = this.dk;
            th.grv = this.grv;
            th.vph = this.vph;
            th.lsb = this.lsb;
            th.bkhac = this.bkhac;
            return th;
        }
    }
    public class DuLieuBoiThuongConNguoiHS_CU
    {
        public string ma_chi_nhanh { get; set; }
        public long? so_id_hs { get; set; }
        public string ngay_ps { get; set; }
        public string so_hs { get; set; }
        public string ma_chi_nhanh_cap { get; set; }
        public string so_hdong { get; set; }
        public long? so_id_gcn { get; set; }
        public string so_cv_kn { get; set; }
        public string ngay_ton_that { get; set; }
        public string ngay_tbao { get; set; }
        public string ngay_duyet { get; set; }
        public string loai_hop_dong { get; set; }
    }
    public class DuLieuBoiThuongConNguoiDK_CU
    {
        public string nghiep_vu { get; set; }
        public string ma_tien_bh { get; set; }
        public long? so_tien_bh { get; set; }//decimal?
        public long? so_tien_tt { get; set; }//decimal?
        public long? khau_tru { get; set; }//decimal?
        public long? so_tien_bt { get; set; }//decimal?
        public long? so_tien_bt_vnd { get; set; }//decimal?
        public long? thue_bt { get; set; }//decimal
        public long? thue_bt_vnd { get; set; }//decimal
    }
    //Thông tin Model dữ liệu hồ sơ mới
    public class DuLieuBoiThuongConNguoi
    {
        public string ma_dvi { get; set; }
        public string pas { get; set; }
        public string checksum { get; set; }
        public DuLieuBoiThuongConNguoiHS hs { get; set; }
        public List<DuLieuBoiThuongConNguoiDK> dk { get; set; }
        public List<DuLieuBoiThuongConNguoiGRV> grv { get; set; }
        public List<DuLieuBoiThuongConNguoiVPH> vph { get; set; }
        public List<DuLieuBoiThuongConNguoiLSB> lsb { get; set; }
        public List<DuLieuBoiThuongConNguoiBKHAC> bkhac { get; set; }
    }
    public class DuLieuBoiThuongConNguoiHS
    {
        public string ma_chi_nhanh { get; set; }
        public long? so_id_hs { get; set; }//decimal?
        public string ngay_ps { get; set; }
        public string loai_hs { get; set; }
        public string so_hs { get; set; }
        public string ma_chi_nhanh_cap { get; set; }
        public string so_hd { get; set; }
        public long? so_id_hd { get; set; }//decimal?
        public long? so_id_gcn { get; set; }//decimal?
        public string so_gcn { get; set; }
        public string ngay_gui { get; set; }
        public string ngay_mo { get; set; }
        public string ngay_do { get; set; }
        public string so_cv_kn { get; set; }
        public string ngay_xr { get; set; }
        public string ma_nn { get; set; }
        public string lhe_ten { get; set; }
        public string lhe_mobi { get; set; }
        public string lhe_mail { get; set; }
        public string ngh_nh { get; set; }
        public string ngh_tk { get; set; }
        public string ngh_ten { get; set; }
        public string n_trinh { get; set; }
        public string n_duyet { get; set; }
        public string ngay_duyet { get; set; }
        public string ket_luan { get; set; }
        public string tt_nhanh { get; set; }
        public string ctb { get; set; }
        public string loai_hop_dong { get; set; }
    }
    public class DuLieuBoiThuongConNguoiDK
    {
        public string ma_chi_nhanh { get; set; }
        public long? so_id_hdong { get; set; }//decimal?
        public long? so_id_gcn { get; set; }//decimal?
        public string nghiep_vu { get; set; }
        public string loai { get; set; }
        public string loaiq { get; set; }
        public string ten_ql { get; set; }
        public long? so_tien_yc { get; set; }//decimal?
        public long? pt_bt { get; set; }//decimal?
        public long? dxuat { get; set; }//decimal?
        public string ghichu { get; set; }
    }
    public class DuLieuBoiThuongConNguoiGRV
    {
        public string ma_bv { get; set; }
        public string ten_bv { get; set; }
        public string ng_ra { get; set; }
        public string gi_bv { get; set; }
        public string ng_bv { get; set; }
        public long? tien_bv { get; set; }//decimal?
    }
    public class DuLieuBoiThuongConNguoiVPH
    {
        public string gi_bv { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string tien { get; set; }
        public string loai { get; set; }
    }
    public class DuLieuBoiThuongConNguoiLSB
    {
        public string ma { get; set; }
        public string ten { get; set; }
        public string muc { get; set; }
    }
    public class DuLieuBoiThuongConNguoiBKHAC
    {
        public string ten { get; set; }
    }
}
