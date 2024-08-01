using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLSPBH2.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string HoTen { get; set; }

        [Required]
        public string MSSV { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu không khớp.")]
        public string XacNhanMatKhau { get; set; }
    }

}