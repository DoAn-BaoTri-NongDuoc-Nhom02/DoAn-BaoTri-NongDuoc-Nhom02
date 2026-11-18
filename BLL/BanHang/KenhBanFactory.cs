using System;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// PATTERN #2 Abstract Factory – tạo trọn MỘT BỘ đối tượng đi cùng nhau của một kênh bán:
    /// dịch vụ bán hàng (giá lẻ/sỉ, loại phiếu), thông tin thêm của hóa đơn (lẻ / sỉ có hạn mức nợ)
    /// và chuỗi kiểm tra trước khi lưu (bán sỉ có thêm kiểm tra hạn mức nợ đại lý).
    /// Form Bán lẻ / Bán sỉ chỉ chọn một nhà máy (KenhBanLeFactory / KenhBanSiFactory) là có đủ bộ
    /// đối tượng khớp nhau, không thể lỡ ghép "phiếu sỉ" với "kiểm tra của bán lẻ".
    /// </summary>
    public interface IKenhBanFactory
    {
        String TenKenh { get; }
        BanHangService TaoDichVuBan();
        ThongTinHoaDon TaoThongTinHoaDon();
        MatXichKiemTra TaoChuoiKiemTra();
    }

    public class KenhBanLeFactory : IKenhBanFactory
    {
        public String TenKenh { get { return "Bán lẻ"; } }

        public BanHangService TaoDichVuBan() { return new BanLeService(); }

        public ThongTinHoaDon TaoThongTinHoaDon() { return new ThongTinHoaDon(ThongTinHoaDon.GIA_LE); }

        /// <summary>Bán lẻ: trùng mã phiếu -> mức giảm giá (khách lẻ không có hạn mức nợ).</summary>
        public MatXichKiemTra TaoChuoiKiemTra()
        {
            MatXichKiemTra dau = new KiemTraMaPhieuChuaCo();
            dau.Noi(new KiemTraMucGiamGia());
            return dau;
        }
    }

    public class KenhBanSiFactory : IKenhBanFactory
    {
        public String TenKenh { get { return "Bán sỉ"; } }

        public BanHangService TaoDichVuBan() { return new BanSiService(); }

        public ThongTinHoaDon TaoThongTinHoaDon() { return new ThongTinHoaDonSi(); }

        /// <summary>Bán sỉ: trùng mã phiếu -> hạn mức nợ đại lý -> mức giảm giá.</summary>
        public MatXichKiemTra TaoChuoiKiemTra()
        {
            return MatXichKiemTra.ChuoiMacDinh();
        }
    }
}
