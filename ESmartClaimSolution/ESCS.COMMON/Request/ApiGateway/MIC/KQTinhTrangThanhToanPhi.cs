using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.MIC
{
    public class KQTinhTrangThanhToanPhi
    {
        public List<KyThanhToan> hd { get; set; }
    }
    public class KyThanhToan
    {
        public string ky_tt { get; set; }
        public decimal? so_tien { get; set; }
        public string ngay_tt { get; set; }
        public decimal? so_tien_da_tt { get; set; }
        public string ghi_chu { get; set; }
    }
}
