using CuahangNongduoc.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // PATTERN #19 Observer (nơi dùng) – đăng ký các bên nghe sự kiện "hóa đơn đã lưu" khi khởi động.
            CuahangNongduoc.BanHang.SuKienHoaDon.DangKyMacDinh();
            Application.Run(new frmDangNhap());

        }

    }
}