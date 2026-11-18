using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc.BanHang
{
    /// <summary>Bên muốn được báo khi có hóa đơn bán vừa lưu.</summary>
    public interface IQuanSatHoaDon
    {
        void HoaDonDaLuu(PhieuBan phieu);
    }

    /// <summary>
    /// Nơi phát sự kiện "hóa đơn đã lưu".
    ///
    /// PATTERN #19 Observer.
    /// Sau khi lưu hóa đơn có nhiều việc phụ phải làm (ghi nhật ký, làm mới bộ đệm công nợ khách,
    /// cập nhật màn hình thống kê đang mở...). Phần lưu (BanHangFacade) chỉ "phát" sự kiện; các bên quan sát
    /// tự đăng ký / hủy đăng ký. Thêm việc mới không phải sửa phần lưu hóa đơn.
    /// </summary>
    public static class SuKienHoaDon
    {
        private static readonly List<IQuanSatHoaDon> s_BenNghe = new List<IQuanSatHoaDon>();

        public static void DangKy(IQuanSatHoaDon benNghe)
        {
            if (benNghe != null && !s_BenNghe.Contains(benNghe))
                s_BenNghe.Add(benNghe);
        }

        public static void HuyDangKy(IQuanSatHoaDon benNghe)
        {
            s_BenNghe.Remove(benNghe);
        }

        /// <summary>Đăng ký các bên nghe mặc định (gọi một lần khi chương trình khởi động).</summary>
        public static void DangKyMacDinh()
        {
            DangKy(GhiNhatKyHoaDon.DuyNhat);
            DangKy(LamMoiBoDemKhachHang.DuyNhat);
        }

        /// <summary>Báo cho mọi bên nghe. Lỗi của một bên nghe không làm hỏng việc lưu đã xong.</summary>
        public static void PhatHoaDonDaLuu(PhieuBan phieu)
        {
            foreach (IQuanSatHoaDon benNghe in s_BenNghe.ToArray())
            {
                try
                {
                    benNghe.HoaDonDaLuu(phieu);
                }
                catch (Exception ex)
                {
                    NhatKyLoi.Ghi("SuKienHoaDon", "", benNghe.GetType().Name + ": " + ex.Message);
                }
            }
        }
    }

    /// <summary>Ghi một dòng nhật ký cho mỗi hóa đơn vừa lưu (file nhat_ky_hoa_don.log cạnh file .exe).</summary>
    public class GhiNhatKyHoaDon : IQuanSatHoaDon
    {
        public static readonly GhiNhatKyHoaDon DuyNhat = new GhiNhatKyHoaDon();

        public void HoaDonDaLuu(PhieuBan phieu)
        {
            String dong = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + " | " + phieu.Id
                + " | " + (phieu.KhachHang == null ? "" : phieu.KhachHang.HoTen)
                + " | tổng " + phieu.TongTien.ToString("#,##0")
                + " | giảm " + (phieu.ChietKhau + phieu.GiamGia).ToString("#,##0")
                + " | " + (phieu.NguoiLap == null ? "" : phieu.NguoiLap.TenDangNhap)
                + " | " + phieu.TrangThai + "\r\n";
            File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nhat_ky_hoa_don.log"), dong, Encoding.UTF8);
        }
    }

    /// <summary>Công nợ / thông tin khách có thể đã đổi: bỏ bộ đệm khách hàng.</summary>
    public class LamMoiBoDemKhachHang : IQuanSatHoaDon
    {
        public static readonly LamMoiBoDemKhachHang DuyNhat = new LamMoiBoDemKhachHang();

        public void HoaDonDaLuu(PhieuBan phieu)
        {
            KhachHangController.LamMoiBoDem();
        }
    }
}
