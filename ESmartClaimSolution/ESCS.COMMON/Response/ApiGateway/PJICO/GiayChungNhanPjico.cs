using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Response.ApiGateway.PJICO
{
    public class GiayChungNhanPjico
    {
        public string DIVN_CODE { get; set; }//Mã đơn vị cấp đơn
        public string DIVN_NAME { get; set; }//Tên đơn vị cấp đơn
        public decimal? CONTRACT_ID { get; set; }//Số id hợp đồng (Chưa có)
        public string CONTRACT_NO { get; set; }//Số hợp đồng (Chưa có)
        public decimal? POL_SYS_ID { get; set; }//Số Id GCN
        public string POL_NO { get; set; }//Số GCN
        public string REGN_NO { get; set; }//Biển số xe
        public string POL_ASSURED_NAME { get; set; }//Tên khách hàng
        public string POL_FM_DT { get; set; }//Ngày hiệu lực bảo hiểm format: dd/MM/yyyy HH:mm
        public string POL_TO_DT { get; set; }//Ngày kết thúc hiệu lực bảo hiểm format: dd/MM/yyyy HH:mm
        public string POL_SC_CODE { get; set; }//Mã loại hình nghiệp vụ
        public string POL_TYPE { get; set; }//Loại giấy chứng nhận
        
    }
}
