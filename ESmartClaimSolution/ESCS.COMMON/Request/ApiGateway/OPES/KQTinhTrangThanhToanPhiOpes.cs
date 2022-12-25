using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Request.ApiGateway.OPES
{
    public class KQTinhTrangThanhToanPhiOpes
    {
        public List<KyThanhToanOpes> hd { get; set; }
    }
    public class KyThanhToanOpes
    {
        public string ky_tt { get; set; }
        public decimal? so_tien_tt { get; set; }
        public string ngay_tt { get; set; }
        public decimal? so_tien_da_tt { get; set; }
        public string ghi_chu { get; set; }
    }
}
