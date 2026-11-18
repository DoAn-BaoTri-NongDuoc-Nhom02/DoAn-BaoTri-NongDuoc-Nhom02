using System;
using System.Collections.Generic;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// Lắp ráp một hóa đơn bán hoàn chỉnh từng bước.
    ///
    /// PATTERN #3 Builder.
    /// Hóa đơn có nhiều phần tùy chọn (dịch vụ phụ, phí vận chuyển, chiết khấu, giảm giá, VAT, người lập...).
    /// Thay vì một hàm khởi tạo dài hoặc gán từng thuộc tính rải rác trong form, form gọi lần lượt các bước
    /// (mỗi bước trả lại chính builder để gọi nối tiếp) rồi Build(). Build() kiểm tra hợp lệ, tính các khoản
    /// tiền (qua Decorator / Strategy) và trả về PhieuBan đúng loại kênh (qua Factory Method).
    /// </summary>
    public class HoaDonBuilder
    {
        private readonly BanHangService m_Kenh;
        private readonly String m_MaPhieu;
        private KhachHang m_Khach;
        private DateTime m_NgayBan = DateTime.Today;
        private long m_TienHang;
        private long m_DaTra;
        private NguoiDung m_NguoiLap;
        private ThongTinHoaDon m_ThongTin = new ThongTinHoaDon(ThongTinHoaDon.GIA_LE);

        public HoaDonBuilder(BanHangService kenh, String maPhieu)
        {
            if (kenh == null) throw new ArgumentNullException("kenh");
            m_Kenh = kenh;
            m_MaPhieu = maPhieu == null ? "" : maPhieu.Trim();
        }

        public HoaDonBuilder ChoKhachHang(KhachHang khach) { m_Khach = khach; return this; }
        public HoaDonBuilder NgayBan(DateTime ngay) { m_NgayBan = ngay.Date; return this; }
        public HoaDonBuilder TienHang(long tienHang) { m_TienHang = tienHang; return this; }
        public HoaDonBuilder DaTra(long daTra) { m_DaTra = daTra; return this; }
        public HoaDonBuilder NguoiLap(NguoiDung nguoiLap) { m_NguoiLap = nguoiLap; return this; }

        /// <summary>Lấy toàn bộ thông tin thêm (chiết khấu, phí, VAT, dịch vụ, giảm giá, ghi chú...) đã nhập trên form.</summary>
        public HoaDonBuilder ThongTinThem(ThongTinHoaDon thongTin)
        {
            if (thongTin != null) m_ThongTin = thongTin;
            return this;
        }

        public HoaDonBuilder ChietKhau(long soTien) { m_ThongTin.ChietKhau = soTien; return this; }
        public HoaDonBuilder PhiVanChuyen(long soTien) { m_ThongTin.PhiVanChuyen = soTien; return this; }
        public HoaDonBuilder VAT(decimal phanTram) { m_ThongTin.VAT = phanTram; return this; }

        public HoaDonBuilder GiamGia(String maChienLuoc, decimal giaTri)
        {
            m_ThongTin.KieuGiam = maChienLuoc;
            m_ThongTin.GiaTriGiam = giaTri;
            return this;
        }

        public HoaDonBuilder ThemDichVu(String ten, long soTien)
        {
            List<DichVuPhatSinh> ds = new List<DichVuPhatSinh>(m_ThongTin.DichVu);
            ds.Add(new DichVuPhatSinh(ten, soTien));
            m_ThongTin.GanDichVu(ds);
            return this;
        }

        /// <summary>Kiểm tra và tạo hóa đơn. Thiếu dữ liệu bắt buộc thì ném NghiepVuException.</summary>
        public PhieuBan Build()
        {
            if (m_MaPhieu.Length == 0) throw new NghiepVuException("Chưa có mã phiếu bán.");
            if (m_Khach == null || String.IsNullOrEmpty(m_Khach.Id)) throw new NghiepVuException("Chưa chọn khách hàng.");
            if (m_TienHang <= 0) throw new NghiepVuException("Hóa đơn chưa có dòng hàng nào.");
            if (m_DaTra < 0) throw new NghiepVuException("Số tiền đã trả không được âm.");

            PhieuBan phieu = m_Kenh.LapPhieuMoi(m_MaPhieu, m_Khach, m_NgayBan);

            ThanhPhanTien bangTinh = m_ThongTin.BangTinhTien(m_TienHang, m_Khach);
            phieu.TienHang = m_TienHang;
            phieu.ChietKhau = m_ThongTin.ChietKhau;
            phieu.GiamGia = m_ThongTin.TinhTienGiam(m_TienHang, m_Khach);
            phieu.KieuGiam = m_ThongTin.KieuGiam;
            phieu.PhiVanChuyen = m_ThongTin.PhiVanChuyen;
            phieu.GanDichVu(m_ThongTin.DichVu);
            phieu.TienDichVu = m_ThongTin.TongTienDichVu;
            phieu.VAT = m_ThongTin.VAT;
            phieu.TongTien = bangTinh.TinhTien();
            phieu.DaTra = m_DaTra;
            phieu.ConNo = Math.Max(0, phieu.TongTien - m_DaTra);
            phieu.GhiChu = m_ThongTin.GhiChu;
            phieu.SoHoaDonVAT = m_ThongTin.SoHoaDonVAT;
            phieu.HinhThucThanhToan = m_ThongTin.HinhThucThanhToan;
            phieu.NguoiLap = m_NguoiLap;

            // PATTERN #23 Visitor (nơi dùng) – cho người thăm đi qua bảng tính tiền để lấy các dòng diễn giải.
            DienGiaiTienVisitor dienGiai = new DienGiaiTienVisitor();
            bangTinh.ChapNhan(dienGiai);
            phieu.DienGiaiTien = dienGiai.CacDong;

            PhieuBanSi phieuSi = phieu as PhieuBanSi;
            ThongTinHoaDonSi thongTinSi = m_ThongTin as ThongTinHoaDonSi;
            if (phieuSi != null && thongTinSi != null)
            {
                phieuSi.HanMucNo = thongTinSi.HanMucNo;
                phieuSi.DieuKhoanThanhToan = thongTinSi.DieuKhoanThanhToan;
            }
            return phieu;
        }
    }
}
