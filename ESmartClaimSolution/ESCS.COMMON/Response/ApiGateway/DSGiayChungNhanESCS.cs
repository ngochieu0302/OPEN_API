using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Response.ApiGateway
{
    public class DSGiayChungNhanESCS
    {
        public List<GiayChungNhanESCS> hd { get; set; }
        public List<GiayChungNhanNguoiESCS> hd_nguoi { get; set; }
    }
    public class GiayChungNhanESCS
    {
        public string ma_chi_nhanh { get; set; }
        public string ten_chi_nhanh { get; set; }
        public string so_id_hdong { get; set; }
        public string so_id_gcn { get; set; }
        public string so_gcn { get; set; }
        public string so_hdong { get; set; }
        public string bien_so_xe { get; set; }
        public string ten_chu_xe { get; set; }
        public string ngay_hl_bh { get; set; }//format dd/MM/yyyy
        public string ngay_kt_bh { get; set; }//format dd/MM/yyyy
        public string loai_gcn { get; set; }//TN,BB
    }
    public class GiayChungNhanNguoiESCS
    {
        public string so_id_hdong { get; set; }
        public string so_id_gcn { get; set; }
        public string ten_ndbh { get; set; }
        public string so_cmt { get; set; }
        public string ngay_sinh { get; set; }
        public string goi_bh { get; set; }
        public string ngay_hl { get; set; }//format dd/MM/yyyy
        public string ngay_kt { get; set; }//format dd/MM/yyyy
        public string ten_kh { get; set; }
        public string so_hd { get; set; }
        public string dien_thoai { get; set; }
        public string email { get; set; }
        public string san_pham { get; set; }
        public string loai_gcn { get; set; }//C/M
    }

}
