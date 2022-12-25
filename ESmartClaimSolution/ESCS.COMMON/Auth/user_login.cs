using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESCS.COMMON.Auth
{
    public class user_login
    {
        [Required(ErrorMessage = "Bạn chưa nhập tài khoản")]
        public string username { get; set; }
        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required(ErrorMessage = "Bạn chưa nhập captcha")]
        public string captcha { get; set; }

    }
    public class user_login_escs
    {
        [Required(ErrorMessage = "Bạn chưa nhập tài khoản")]
        public string username { get; set; }
        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        //[Required(ErrorMessage = "Bạn chưa nhập captcha")]
        //public string captcha { get; set; }
    }
}
