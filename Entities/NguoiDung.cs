using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.Entities
{
    public class NguoiDung
    {
        public int ID { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public int IDVaiTro { get; set; }
        public string TenVaiTro { get; set; }   // join thêm
        public bool TrangThai { get; set; }
    }
}
