using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.SMS.MCM
{
    public class mcm_calback
    {
        public string PartnerCode { get; set; }
        /// <summary>
        /// Mã tin nhắn, được trả về sau khi gọi API gửi tin
        /// </summary>
        public string SMSID { get; set; }
        /// <summary>
        /// Tổng số tin gửi thất bại
        /// </summary>
        public string SendFailed { get; set; }
        /// <summary>
        /// 1: Chờ duyệt
        /// 2: Đang chờ gửi
        /// 3: Đang gửi
        /// 4: Bị từ chối
        /// 5: Đã gửi xong
        /// </summary>
        public string SendStatus { get; set; }
        /// <summary>
        /// Tổng số tin nhắn gửi thành công
        /// </summary>
        public string SendSuccess { get; set; }
        /// <summary>
        /// Tổng số tiền gửi tin
        /// </summary>
        public string TotalPrice { get; set; }
        /// <summary>
        /// Tổng số người nhận
        /// </summary>
        public string TotalReceiver { get; set; }
        /// <summary>
        /// Tổng số tin nhắn cần gửi tin
        /// </summary>
        public string TotalSent { get; set; }
        /// <summary>
        /// RequestId
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// Loại tin nhắn gửi
        /// </summary>
        public string TypeId { get; set; }
        /// <summary>
        /// Trạng thái này sẽ không trả về nếu tin Zalo thành công
        /// 1: Viettel
        /// 2: Mobi
        /// 3: Vina
        /// 4: Vietnammobile
        /// 5: Gtel
        /// 6: ITel
        /// </summary>
        public string telcoid { get; set; }
    }
}
