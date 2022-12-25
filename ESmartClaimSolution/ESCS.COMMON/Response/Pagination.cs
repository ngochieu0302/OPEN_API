using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Response
{
    public class Pagination
    {
        public decimal tong_so_dong { get; set; }
        public object data { get; set; }
    }
    public class PaginationGenneric<T>
    {
        public decimal trang { get; set; }
        public decimal tong_so_dong { get; set; }
        public IEnumerable<T> data { get; set; }
        public OutValue out_value { get; set; }
    }
    public class OutValue
    {
        public decimal tong_so_dong { get; set; }
        public decimal trang { get; set; }
    }
}
