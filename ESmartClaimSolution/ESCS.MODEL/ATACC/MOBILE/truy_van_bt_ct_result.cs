using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ATACC.MOBILE
{
    public class truy_van_bt_ct_result
    {
        public List<truy_van_bt_ct_result_table> Table { get; set; }
        public List<truy_van_bt_ct_result_table1> Table1 { get; set; }
        public List<truy_van_bt_ct_result_table2> Table2 { get; set; }
    }
    public class truy_van_bt_ct_result_table
    {
        public string MA_DVI { get; set; }
        public decimal? SO_ID { get; set; }
        public string NV_1 { get; set; }
        public decimal? SO_ID_HD { get; set; }
        public decimal? SO_ID_DT { get; set; }
        public string MA_DVI_QL { get; set; }
        public string DIEN_THOAI { get; set; }
        public string NGUOI_TB { get; set; }
        public string NHOM { get; set; }
        public string LOAI { get; set; }
        public string SO_HS { get; set; }
        public string NGAY_HT { get; set; }
        public string NGAY_QD { get; set; }
        public decimal? TIEN_YC_1 { get; set; }
        public decimal? TIEN_GIAM_1 { get; set; }
        public decimal? TIEN_DUYET_1 { get; set; }
        public string GHI_CHU { get; set; }
        public string MA_CV { get; set; }
        public string TRANG_THAI { get; set; }
        public string TRANG_THAI_MA { get; set; }
        public string TIEN_YC { get; set; }
        public string TIEN_GIAM { get; set; }
        public string TIEN_DUYET { get; set; }
        public string TRANG_THAI_HUY { get; set; }
    }
    public class truy_van_bt_ct_result_table1
    {
        public string NGAY { get; set; }
        public string ND { get; set; }
    }
    public class truy_van_bt_ct_result_table2
    {
        public string MA_DVI { get; set; }
        public decimal? SO_ID { get; set; }
        public string TEN { get; set; }
        public string LINK_FILE { get; set; }
        public decimal? LATITUDE { get; set; }
        public decimal? LONGITUDE { get; set; }
        public string MA_DVI_NH { get; set; }
        public string NSD { get; set; }
        public string NGAY_NH { get; set; }
        public string IDVUNG { get; set; }
        public string TRANG_THAI { get; set; }
        public string TEN1 { get; set; }
        public string root_link { get; set; }
    }
}
