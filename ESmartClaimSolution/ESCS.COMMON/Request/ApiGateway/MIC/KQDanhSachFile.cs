using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class KQDanhSachFileObj
    {
        public List<KQDanhSachFile> ds_file { get; set; }
    }
    public class KQDanhSachFile
    {
        public string ma_chi_nhanh { get; set; }
        public string so_id { get; set; }
        public string ma_file { get; set; }
        public string loai { get; set; }
        public string loai_file { get; set; }
        public string ten_file { get; set; }
        public string nhom_anh { get; set; }
        public string pm { get; set; }

        public string duong_dan { get; set; }
        public string extension { get; set; }
        public string file { get; set; }
    }
}
