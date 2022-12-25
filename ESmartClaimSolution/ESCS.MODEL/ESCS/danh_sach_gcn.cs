using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class danh_sach_gcn
    {
        public string ma_doi_tac_ql { get; set; }//ma_doi_tac
        public string ma_chi_nhanh_ql { get; set; }//ma_chi_nhanh
        public string so_id_hd { get; set; }//so_id_hdong
        public string so_id_gcn { get; set; }//so_id_gcn
        public string so_hd { get; set; }//so_hdong
        public string bien_xe { get; set; }//bien_so_xe
        public int ngay_hl { get; set; }//ngay_hl_bh
        public int ngay_kt { get; set; }//ngay_kt_bh
        public string loai { get; set; }//loai_gcn
        public string loai_ten { get; set; }//loai_gcn
    }
}
