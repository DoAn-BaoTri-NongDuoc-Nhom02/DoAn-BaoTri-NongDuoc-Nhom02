using System;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// Nghiệp vụ chung của một kênh bán (lẻ / sỉ).
    ///
    /// PATTERN #1 Factory Method.
    /// Hàm LapPhieuMoi() (ở lớp cha) không tự "new" phiếu mà gọi hàm tạo TaoPhieu() do lớp con quyết định:
    /// BanLeService tạo PhieuBan, BanSiService tạo PhieuBanSi (có hạn mức nợ, điều khoản thanh toán).
    /// Form bán lẻ / bán sỉ chỉ cần giữ đúng loại service, phần còn lại dùng chung.
    /// </summary>
    public abstract class BanHangService
    {
        /// <summary>Factory method: tạo đúng loại phiếu cho kênh bán.</summary>
        protected abstract PhieuBan TaoPhieu(String maPhieu);

        /// <summary>"Bán lẻ" / "Bán sỉ".</summary>
        public abstract String TenKenh { get; }

        /// <summary>Đơn giá bán mặc định của sản phẩm theo kênh.</summary>
        public abstract long GiaBan(SanPham sanPham);

        /// <summary>Lập phiếu mới ở trạng thái Nháp.</summary>
        public PhieuBan LapPhieuMoi(String maPhieu, KhachHang khach, DateTime ngayBan)
        {
            PhieuBan phieu = TaoPhieu(maPhieu);
            phieu.KhachHang = khach;
            phieu.NgayBan = ngayBan.Date;
            phieu.TrangThai = TrangThaiHoaDon.NHAP;
            return phieu;
        }
    }

    public class BanLeService : BanHangService
    {
        protected override PhieuBan TaoPhieu(String maPhieu)
        {
            return new PhieuBan(maPhieu);
        }

        public override String TenKenh { get { return "Bán lẻ"; } }

        public override long GiaBan(SanPham sanPham)
        {
            return sanPham == null ? 0 : sanPham.GiaBanLe;
        }
    }

    public class BanSiService : BanHangService
    {
        protected override PhieuBan TaoPhieu(String maPhieu)
        {
            return new PhieuBanSi(maPhieu);
        }

        public override String TenKenh { get { return "Bán sỉ"; } }

        public override long GiaBan(SanPham sanPham)
        {
            return sanPham == null ? 0 : sanPham.GiaBanSi;
        }
    }
}
