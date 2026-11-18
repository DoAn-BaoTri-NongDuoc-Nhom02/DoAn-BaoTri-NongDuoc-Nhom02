using System;
using System.Collections.Generic;
using CuahangNongduoc.BanHang;

namespace CuahangNongduoc.BusinessObject
{
    /// <summary>
    /// Thông tin thêm của hóa đơn bán đang soạn: loại giá, chiết khấu, phí vận chuyển, VAT, ghi chú...
    /// REFACTOR #23 Duplicate Observed Data – trước đây các giá trị nghiệp vụ này là hàng loạt trường
    /// m_ChietKhau, m_PhiVC, m_VAT... nằm trong form frmBanLe / frmBanSi; nay form chỉ hiển thị
    /// và chép qua lại với đối tượng nghiệp vụ này.
    /// </summary>
    public class ThongTinHoaDon
    {
        public const int GIA_NHAP = 0;
        public const int GIA_SI = 1;
        public const int GIA_LE = 2;

        private int m_LoaiGia;
        private long m_ChietKhau;
        private long m_PhiVanChuyen;
        private decimal m_VAT;
        private String m_GhiChu = "";
        private String m_SoHoaDonVAT = "";
        private String m_TrangThai = "Nháp";
        private String m_HinhThucThanhToan = "Tiền mặt";
        private String m_NhanVien = "";
        private String m_NguoiTao = "";
        private String m_KieuGiam = ChienLuocGiamGia.MA_KHONG_GIAM;
        private decimal m_GiaTriGiam;
        private readonly List<DichVuPhatSinh> m_DichVu = new List<DichVuPhatSinh>();

        public ThongTinHoaDon(int loaiGia)
        {
            m_LoaiGia = loaiGia;
        }

        /// <summary>0 = giá nhập, 1 = giá sỉ, 2 = giá lẻ.</summary>
        public int LoaiGia { get { return m_LoaiGia; } set { m_LoaiGia = value; } }
        public long ChietKhau { get { return m_ChietKhau; } set { m_ChietKhau = value; } }
        public long PhiVanChuyen { get { return m_PhiVanChuyen; } set { m_PhiVanChuyen = value; } }
        /// <summary>Thuế VAT tính theo %.</summary>
        public decimal VAT { get { return m_VAT; } set { m_VAT = value; } }
        public String GhiChu { get { return m_GhiChu; } set { m_GhiChu = value ?? ""; } }
        public String SoHoaDonVAT { get { return m_SoHoaDonVAT; } set { m_SoHoaDonVAT = value ?? ""; } }
        public String TrangThai { get { return m_TrangThai; } set { m_TrangThai = value ?? "Nháp"; } }
        public String HinhThucThanhToan { get { return m_HinhThucThanhToan; } set { m_HinhThucThanhToan = value ?? "Tiền mặt"; } }
        public String NhanVien { get { return m_NhanVien; } set { m_NhanVien = value ?? ""; } }
        public String NguoiTao { get { return m_NguoiTao; } set { m_NguoiTao = value ?? ""; } }

        /// <summary>Mã cách giảm giá (ChienLuocGiamGia.MA_...).</summary>
        public String KieuGiam { get { return m_KieuGiam; } set { m_KieuGiam = value ?? ChienLuocGiamGia.MA_KHONG_GIAM; } }

        /// <summary>% giảm hoặc số tiền giảm, tùy KieuGiam.</summary>
        public decimal GiaTriGiam { get { return m_GiaTriGiam; } set { m_GiaTriGiam = Math.Max(0, value); } }

        public ChienLuocGiamGia ChienLuocGiam
        {
            get { return ChienLuocGiamGia.Tao(m_KieuGiam, m_GiaTriGiam); }
        }

        /// <summary>Dịch vụ phụ phát sinh trên hóa đơn (chỉ đọc; sửa qua GanDichVu).</summary>
        public IList<DichVuPhatSinh> DichVu
        {
            get { return m_DichVu.AsReadOnly(); }
        }

        public void GanDichVu(IEnumerable<DichVuPhatSinh> dichVu)
        {
            m_DichVu.Clear();
            if (dichVu != null) m_DichVu.AddRange(dichVu);
        }

        /// <summary>
        /// Tổng tiền phải trả = (tiền hàng − chiết khấu − giảm giá + phí vận chuyển + dịch vụ) × (1 + VAT%).
        /// REFACTOR #10 Move Method – công thức trước đây nằm trong frmBanLe/frmBanSi.
        /// Công thức được ghép từ các lớp bọc trong BangTinhTien() (xem PATTERN #9 Decorator).
        /// </summary>
        public long TinhTongTien(long tienHang)
        {
            return TinhTongTien(tienHang, null);
        }

        public long TinhTongTien(long tienHang, KhachHang khach)
        {
            return BangTinhTien(tienHang, khach).TinhTien();
        }

        /// <summary>Chuỗi các lớp bọc tính tiền (dùng để tính tổng và in diễn giải).</summary>
        public ThanhPhanTien BangTinhTien(long tienHang, KhachHang khach)
        {
            ThanhPhanTien tien = new TienHang(tienHang);
            tien = new TruChietKhau(tien, m_ChietKhau);
            tien = new TruGiamGia(tien, ChienLuocGiam, tienHang, khach);
            tien = new CongPhiVanChuyen(tien, m_PhiVanChuyen);
            tien = new CongDichVu(tien, m_DichVu);
            tien = new CongVAT(tien, m_VAT);
            return tien;
        }

        public long TinhTienGiam(long tienHang, KhachHang khach)
        {
            return ChienLuocGiam.TinhTienGiam(tienHang, khach);
        }

        // ------------------------------------------------------------------
        // PATTERN #4 Prototype – tạo bản sao đầy đủ của thông tin hóa đơn (kể cả danh sách dịch vụ phụ)
        // mà không cần biết lớp cụ thể là ThongTinHoaDon hay ThongTinHoaDonSi: lớp con ghi đè ChepVao()
        // để chép thêm trường riêng. Dùng để: (1) lập phiếu mới từ "mẫu mặc định" của kênh bán,
        // (2) chụp trạng thái cho Memento, (3) soạn thay đổi trên bản nháp trước khi áp (Command).
        // ------------------------------------------------------------------
        public ThongTinHoaDon SaoChep()
        {
            ThongTinHoaDon banSao = TaoRong();
            ChepVao(banSao);
            return banSao;
        }

        /// <summary>Tạo đối tượng rỗng cùng lớp (lớp con ghi đè).</summary>
        protected virtual ThongTinHoaDon TaoRong()
        {
            return new ThongTinHoaDon(m_LoaiGia);
        }

        /// <summary>Chép toàn bộ dữ liệu sang đối tượng "dich" (lớp con gọi base rồi chép thêm).</summary>
        protected virtual void ChepVao(ThongTinHoaDon dich)
        {
            dich.m_LoaiGia = m_LoaiGia;
            dich.m_ChietKhau = m_ChietKhau;
            dich.m_PhiVanChuyen = m_PhiVanChuyen;
            dich.m_VAT = m_VAT;
            dich.m_GhiChu = m_GhiChu;
            dich.m_SoHoaDonVAT = m_SoHoaDonVAT;
            dich.m_TrangThai = m_TrangThai;
            dich.m_HinhThucThanhToan = m_HinhThucThanhToan;
            dich.m_NhanVien = m_NhanVien;
            dich.m_NguoiTao = m_NguoiTao;
            dich.m_KieuGiam = m_KieuGiam;
            dich.m_GiaTriGiam = m_GiaTriGiam;
            dich.m_DichVu.Clear();
            dich.m_DichVu.AddRange(m_DichVu);   // DichVuPhatSinh bất biến nên dùng chung được
        }

        // ------------------------------------------------------------------
        // PATTERN #18 Memento – chụp lại trạng thái thông tin hóa đơn thành một đối tượng "kín"
        // (bên ngoài không đọc/sửa được nội dung) và khôi phục lại khi cần (hoàn tác bằng Ctrl+Z).
        // ------------------------------------------------------------------
        public sealed class Memento
        {
            internal readonly ThongTinHoaDon TrangThai;
            internal Memento(ThongTinHoaDon trangThai) { TrangThai = trangThai; }
        }

        public Memento TaoMemento()
        {
            return new Memento(SaoChep());
        }

        public void KhoiPhuc(Memento m)
        {
            if (m == null) throw new ArgumentNullException("m");
            m.TrangThai.ChepVao(this);
        }

        public long TongTienDichVu
        {
            get
            {
                // PATTERN #8 Composite – tổng tiền dịch vụ = tiền của "gói" chứa mọi dịch vụ.
                return new GoiDichVu("Dịch vụ phụ", m_DichVu.ConvertAll<IDichVu>(delegate(DichVuPhatSinh d) { return d; })).SoTien;
            }
        }
    }

    /// <summary>
    /// Thông tin thêm riêng của hóa đơn bán sỉ.
    /// REFACTOR #58 Extract Subclass – điều khoản thanh toán và hạn mức nợ chỉ có ở bán sỉ,
    /// nên tách khỏi ThongTinHoaDon (bán lẻ không cần) thành lớp con.
    /// </summary>
    public class ThongTinHoaDonSi : ThongTinHoaDon
    {
        private String m_DieuKhoanThanhToan = "Trả ngay";
        private long m_HanMucNo;

        public ThongTinHoaDonSi()
            : base(GIA_SI)
        {
        }

        protected override ThongTinHoaDon TaoRong()
        {
            return new ThongTinHoaDonSi();
        }

        /// <summary>PATTERN #4 Prototype – chép thêm hai trường riêng của hóa đơn sỉ.</summary>
        protected override void ChepVao(ThongTinHoaDon dich)
        {
            base.ChepVao(dich);
            ThongTinHoaDonSi si = dich as ThongTinHoaDonSi;
            if (si != null)
            {
                si.m_DieuKhoanThanhToan = m_DieuKhoanThanhToan;
                si.m_HanMucNo = m_HanMucNo;
            }
        }

        public String DieuKhoanThanhToan { get { return m_DieuKhoanThanhToan; } set { m_DieuKhoanThanhToan = value ?? "Trả ngay"; } }

        /// <summary>Hạn mức nợ áp cho hóa đơn này (0 = không giới hạn).</summary>
        public long HanMucNo { get { return m_HanMucNo; } set { m_HanMucNo = value; } }
    }
}
