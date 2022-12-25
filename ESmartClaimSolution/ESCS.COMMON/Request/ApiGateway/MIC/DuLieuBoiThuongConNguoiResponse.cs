using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class DuLieuBoiThuongConNguoiResponse
    {
        public string status { get; set; }
        public string message_code { get; set; }
        public string message_body { get; set; }
        public DuLieuBoiThuongConNguoiSoHS out_value { get; set; }
    }
    public class DuLieuBoiThuongConNguoiSoHS
    {
        public string so_ho_so { get; set; }
    }
}
