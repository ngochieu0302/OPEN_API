using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.HealthClaim
{
    public class mic_day_du_lieu_bt
    {
        public string ma_dvi { get; set; }
        public string pas { get; set; }
        public string checksum { get; set; }
        public mic_day_du_lieu_bt_hs hs { get; set; }
        public List<mic_day_du_lieu_bt_dk> dk { get; set; }
        public List<mic_day_du_lieu_bt_grv> grv { get; set; }
        public List<mic_day_du_lieu_bt_vph> vph { get; set; }
        public List<mic_day_du_lieu_bt_lsb> lsb { get; set; }
        public List<mic_day_du_lieu_bt_bkhac> bkhac { get; set; }
    }
    public class mic_day_du_lieu_bt_hs
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hs { get; set; }
        public decimal? ngay_ps { get; set; }
        public string loai_hs { get; set; }
        public string so_hs { get; set; }
        public string ma_chi_nhanh_cap { get; set; }
        public string so_hd { get; set; }
        public decimal? so_id_hd { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string so_gcn { get; set; }
        public string ngay_gui { get; set; }
        public string ngay_mo { get; set; }
        public string ngay_dong { get; set; }
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
    }
    public class mic_day_du_lieu_bt_dk
    {
        public string ma_chi_nhanh { get; set; }
        public decimal? so_id_hdong { get; set; }
        public decimal? so_id_gcn { get; set; }
        public string nghiep_vu { get; set; }
        public string loai { get; set; }
        public string loaiq { get; set; }
        public string ten_ql { get; set; }
        public decimal? so_tien_yc { get; set; }
        public decimal? pt_bt { get; set; }
        public decimal? dxuat { get; set; }
        public string ghichu { get; set; }
    }
    public class mic_day_du_lieu_bt_grv
    {
        public string ma_bv { get; set; }
        public string ten_bv { get; set; }
        public string ng_ra { get; set; }
        public string gi_bv { get; set; }
        public string ng_bv { get; set; }
        public decimal? tien_bv { get; set; }
    }
    public class mic_day_du_lieu_bt_vph
    {
        public string gi_bv { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string tien { get; set; }
        public string loai { get; set; }
    }
    public class mic_day_du_lieu_bt_lsb
    {
        public string ma { get; set; }
        public string ten { get; set; }
        public string muc { get; set; }
    }
    public class mic_day_du_lieu_bt_bkhac
    {
        public string ten { get; set; }
    }
}
