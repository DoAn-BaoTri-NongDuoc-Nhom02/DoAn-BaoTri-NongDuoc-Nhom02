using System;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// Cách tính tiền giảm giá / khuyến mãi trên hóa đơn.
    ///
    /// PATTERN #21 Strategy.
    /// Mỗi cách giảm giá là một lớp con có cùng hàm TinhTienGiam(); người bán chọn cách giảm khi lập
    /// hóa đơn, phần tính tổng tiền không cần biết đang dùng cách nào. Thêm cách giảm mới (ví dụ theo
    /// mùa vụ) chỉ cần thêm lớp con, không sửa form hay công thức tổng.
    /// Mã chiến lược được lưu vào cột PHIEU_BAN.KIEU_GIAM để thống kê.
    /// </summary>
    public abstract class ChienLuocGiamGia
    {
        public const String MA_KHONG_GIAM = "KHONG";
        public const String MA_PHAN_TRAM = "PHAN_TRAM";
        public const String MA_SO_TIEN = "SO_TIEN";
        public const String MA_CAP_DAI_LY = "CAP_DAI_LY";
        public const String MA_KHUYEN_MAI = "KHUYEN_MAI";

        /// <summary>Mã lưu vào CSDL.</summary>
        public abstract String Ma { get; }

        /// <summary>Tên hiển thị cho người dùng.</summary>
        public abstract String Ten { get; }

        /// <summary>Số tiền được giảm trên phần tiền hàng.</summary>
        protected abstract long TinhGiam(long tienHang, KhachHang khach);

        /// <summary>Tiền giảm, luôn trong khoảng 0..tiền hàng.</summary>
        public long TinhTienGiam(long tienHang, KhachHang khach)
        {
            if (tienHang <= 0) return 0;
            long giam = TinhGiam(tienHang, khach);
            if (giam < 0) return 0;
            return Math.Min(giam, tienHang);
        }

        public override String ToString() { return Ten; }

        /// <summary>Tạo lại chiến lược từ mã đã lưu và giá trị người dùng nhập (% hoặc số tiền).</summary>
        public static ChienLuocGiamGia Tao(String ma, decimal giaTri)
        {
            switch (ma)
            {
                case MA_PHAN_TRAM: return new GiamTheoPhanTram(giaTri);
                case MA_SO_TIEN: return new GiamSoTienCoDinh((long)giaTri);
                case MA_CAP_DAI_LY: return new GiamTheoCapDaiLy();
                case MA_KHUYEN_MAI: return new GiamTheoKhuyenMai();
                default: return new KhongGiamGia();
            }
        }

        /// <summary>Các lựa chọn hiển thị trong hộp thoại (ma, ten).</summary>
        public static String[][] DanhSachLuaChon()
        {
            return new String[][] {
                new String[] { MA_KHONG_GIAM, "Không giảm" },
                new String[] { MA_PHAN_TRAM, "Giảm theo % tiền hàng" },
                new String[] { MA_SO_TIEN, "Giảm số tiền cố định" },
                new String[] { MA_CAP_DAI_LY, "Chiết khấu theo cấp đại lý" },
                new String[] { MA_KHUYEN_MAI, "Khuyến mãi theo điều kiện (tự động)" } };
        }
    }

    public class KhongGiamGia : ChienLuocGiamGia
    {
        public override String Ma { get { return MA_KHONG_GIAM; } }
        public override String Ten { get { return "Không giảm"; } }
        protected override long TinhGiam(long tienHang, KhachHang khach) { return 0; }
    }

    /// <summary>Giảm p% trên tiền hàng (khuyến mãi theo đợt).</summary>
    public class GiamTheoPhanTram : ChienLuocGiamGia
    {
        private readonly decimal m_PhanTram;

        public GiamTheoPhanTram(decimal phanTram)
        {
            if (phanTram < 0 || phanTram > 100)
                throw new ArgumentOutOfRangeException("phanTram", "Phần trăm giảm phải trong khoảng 0–100.");
            m_PhanTram = phanTram;
        }

        public override String Ma { get { return MA_PHAN_TRAM; } }
        public override String Ten { get { return "Giảm " + m_PhanTram.ToString("0.##") + "%"; } }

        protected override long TinhGiam(long tienHang, KhachHang khach)
        {
            return (long)Math.Round(tienHang * m_PhanTram / 100m, 0);
        }
    }

    /// <summary>Giảm thẳng một số tiền.</summary>
    public class GiamSoTienCoDinh : ChienLuocGiamGia
    {
        private readonly long m_SoTien;

        public GiamSoTienCoDinh(long soTien)
        {
            m_SoTien = Math.Max(0, soTien);
        }

        public override String Ma { get { return MA_SO_TIEN; } }
        public override String Ten { get { return "Giảm " + m_SoTien.ToString("#,##0") + " đ"; } }

        protected override long TinhGiam(long tienHang, KhachHang khach)
        {
            return m_SoTien;
        }
    }

    /// <summary>Đại lý cấp 1 được giảm 5%, cấp 2 được giảm 3%; khách lẻ không giảm.</summary>
    public class GiamTheoCapDaiLy : ChienLuocGiamGia
    {
        public const decimal PHAN_TRAM_CAP_1 = 5;
        public const decimal PHAN_TRAM_CAP_2 = 3;

        public override String Ma { get { return MA_CAP_DAI_LY; } }
        public override String Ten { get { return "Chiết khấu theo cấp đại lý"; } }

        protected override long TinhGiam(long tienHang, KhachHang khach)
        {
            DaiLy daiLy = khach as DaiLy;
            if (daiLy == null) return 0;
            decimal phanTram = daiLy.CapDaiLy == 1 ? PHAN_TRAM_CAP_1 : PHAN_TRAM_CAP_2;
            return (long)Math.Round(tienHang * phanTram / 100m, 0);
        }
    }
}
