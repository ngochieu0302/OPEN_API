using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.MODEL.ESCS
{
    public class bh_dich_vu<T>
    {
        public T dich_vu { get; set; }
        public List<bh_dich_vu_hang_muc> hang_muc { get; set; }
    }
}
