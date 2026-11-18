using System;
using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// Một bước trong cách tính tổng tiền hóa đơn.
    ///
    /// PATTERN #9 Decorator.
    /// Tổng tiền = tiền hàng, rồi lần lượt "bọc" thêm: trừ chiết khấu, trừ giảm giá, cộng phí vận chuyển,
    /// cộng dịch vụ phụ, cộng VAT. Mỗi lớp bọc chỉ biết phần của mình và gọi tiếp lớp bên trong, nên có thể
    /// thêm / bớt / đổi thứ tự khoản phí mà không sửa các lớp khác. Mỗi lớp còn tự ghi một dòng diễn giải
    /// để in lên hóa đơn.
    /// </summary>
    public abstract class ThanhPhanTien
    {
        public abstract long TinhTien();

        /// <summary>PATTERN #23 Visitor – nhận một "người thăm" và gọi đúng hàm Tham...() của lớp mình.</summary>
        public abstract void ChapNhan(IThamTien nguoiTham);
    }

    /// <summary>Thành phần gốc: tổng thành tiền các dòng hàng.</summary>
    public class TienHang : ThanhPhanTien
    {
        private readonly long m_SoTien;

        public TienHang(long soTien) { m_SoTien = soTien; }

        public override long TinhTien() { return m_SoTien; }

        public override void ChapNhan(IThamTien nguoiTham)
        {
            nguoiTham.ThamTienHang(this);
        }
    }

    /// <summary>Lớp bọc chung: giữ thành phần bên trong.</summary>
    public abstract class BoSungTien : ThanhPhanTien
    {
        protected readonly ThanhPhanTien m_BenTrong;

        protected BoSungTien(ThanhPhanTien benTrong)
        {
            if (benTrong == null) throw new ArgumentNullException("benTrong");
            m_BenTrong = benTrong;
        }

        /// <summary>Số tiền lớp này cộng thêm (âm nếu là khoản trừ).</summary>
        protected abstract long PhanThem(long tienBenTrong);

        public abstract String NoiDung { get; }

        /// <summary>Số tiền lớp này thêm vào (âm nếu là khoản trừ).</summary>
        public long SoTienThem
        {
            get { return PhanThem(m_BenTrong.TinhTien()); }
        }

        /// <summary>Phần riêng của từng lớp con: gọi hàm Tham... tương ứng.</summary>
        protected abstract void GoiNguoiTham(IThamTien nguoiTham);

        public override long TinhTien()
        {
            long trong = m_BenTrong.TinhTien();
            return trong + PhanThem(trong);
        }

        /// <summary>Thăm lớp bên trong trước rồi tới lớp này (đi từ tiền hàng ra ngoài).</summary>
        public override void ChapNhan(IThamTien nguoiTham)
        {
            m_BenTrong.ChapNhan(nguoiTham);
            GoiNguoiTham(nguoiTham);
        }
    }

    public class TruChietKhau : BoSungTien
    {
        protected override void GoiNguoiTham(IThamTien nguoiTham) { nguoiTham.ThamChietKhau(this); }

        private readonly long m_ChietKhau;

        public TruChietKhau(ThanhPhanTien benTrong, long chietKhau) : base(benTrong) { m_ChietKhau = Math.Max(0, chietKhau); }

        public override String NoiDung { get { return "Chiết khấu"; } }
        protected override long PhanThem(long tienBenTrong) { return -m_ChietKhau; }
    }

    /// <summary>Trừ tiền giảm giá tính theo một chiến lược giảm giá (Strategy) trên phần tiền hàng.</summary>
    public class TruGiamGia : BoSungTien
    {
        protected override void GoiNguoiTham(IThamTien nguoiTham) { nguoiTham.ThamGiamGia(this); }

        private readonly long m_TienGiam;
        private readonly String m_TenChienLuoc;

        public TruGiamGia(ThanhPhanTien benTrong, ChienLuocGiamGia chienLuoc, long tienHangGoc, KhachHang khach)
            : base(benTrong)
        {
            ChienLuocGiamGia cl = chienLuoc ?? new KhongGiamGia();
            m_TienGiam = cl.TinhTienGiam(tienHangGoc, khach);
            m_TenChienLuoc = cl.Ten;
        }

        public long TienGiam { get { return m_TienGiam; } }

        public override String NoiDung { get { return "Giảm giá (" + m_TenChienLuoc + ")"; } }
        protected override long PhanThem(long tienBenTrong) { return -m_TienGiam; }
    }

    public class CongPhiVanChuyen : BoSungTien
    {
        protected override void GoiNguoiTham(IThamTien nguoiTham) { nguoiTham.ThamPhiVanChuyen(this); }

        private readonly long m_Phi;

        public CongPhiVanChuyen(ThanhPhanTien benTrong, long phi) : base(benTrong) { m_Phi = Math.Max(0, phi); }

        public override String NoiDung { get { return "Phí vận chuyển"; } }
        protected override long PhanThem(long tienBenTrong) { return m_Phi; }
    }

    public class CongDichVu : BoSungTien
    {
        protected override void GoiNguoiTham(IThamTien nguoiTham) { nguoiTham.ThamDichVu(this); }

        private readonly long m_TongDichVu;

        public CongDichVu(ThanhPhanTien benTrong, IEnumerable<DichVuPhatSinh> dichVu) : base(benTrong)
        {
            // PATTERN #8 Composite (nơi dùng) – gom mọi dịch vụ thành một gói rồi hỏi tổng tiền của gói.
            List<IDichVu> ds = new List<IDichVu>();
            if (dichVu != null) foreach (DichVuPhatSinh dv in dichVu) ds.Add(dv);
            m_TongDichVu = new GoiDichVu("Dịch vụ phụ", ds).SoTien;
        }

        public long TongDichVu { get { return m_TongDichVu; } }

        public override String NoiDung { get { return "Dịch vụ phụ"; } }
        protected override long PhanThem(long tienBenTrong) { return m_TongDichVu; }
    }

    /// <summary>Cộng VAT% (khoản trước thuế âm thì tính là 0).</summary>
    public class CongVAT : BoSungTien
    {
        protected override void GoiNguoiTham(IThamTien nguoiTham) { nguoiTham.ThamVAT(this); }

        private readonly decimal m_PhanTram;

        public CongVAT(ThanhPhanTien benTrong, decimal phanTram) : base(benTrong) { m_PhanTram = Math.Max(0, phanTram); }

        public override String NoiDung { get { return "VAT " + m_PhanTram.ToString("0.##") + "%"; } }

        protected override long PhanThem(long tienBenTrong)
        {
            decimal truocThue = Math.Max(0, tienBenTrong);
            long sauThue = (long)Math.Round(truocThue * (1 + m_PhanTram / 100m), 0);
            return sauThue - tienBenTrong;
        }
    }

    /// <summary>
    /// PATTERN #23 Visitor – "người thăm" đi qua từng lớp của bảng tính tiền (Decorator) và làm một việc
    /// riêng cho từng loại khoản (tiền hàng, chiết khấu, giảm giá, phí vận chuyển, dịch vụ, VAT).
    /// Thêm một cách xử lý mới (in hóa đơn, tổng hợp khoản giảm, xuất file...) chỉ cần viết một lớp
    /// người thăm mới, không phải sửa các lớp ThanhPhanTien.
    /// </summary>
    public interface IThamTien
    {
        void ThamTienHang(TienHang tien);
        void ThamChietKhau(TruChietKhau khoan);
        void ThamGiamGia(TruGiamGia khoan);
        void ThamPhiVanChuyen(CongPhiVanChuyen khoan);
        void ThamDichVu(CongDichVu khoan);
        void ThamVAT(CongVAT khoan);
    }

    /// <summary>Người thăm tạo các dòng diễn giải (nội dung, số tiền) để in lên hóa đơn.</summary>
    public class DienGiaiTienVisitor : IThamTien
    {
        private readonly List<KeyValuePair<String, long>> m_Dong = new List<KeyValuePair<String, long>>();

        public List<KeyValuePair<String, long>> CacDong { get { return m_Dong; } }

        public void ThamTienHang(TienHang tien) { m_Dong.Add(new KeyValuePair<String, long>("Tiền hàng", tien.TinhTien())); }
        public void ThamChietKhau(TruChietKhau khoan) { Them(khoan); }
        public void ThamGiamGia(TruGiamGia khoan) { Them(khoan); }
        public void ThamPhiVanChuyen(CongPhiVanChuyen khoan) { Them(khoan); }
        public void ThamDichVu(CongDichVu khoan) { Them(khoan); }
        public void ThamVAT(CongVAT khoan) { Them(khoan); }

        private void Them(BoSungTien khoan)
        {
            if (khoan.SoTienThem != 0)
                m_Dong.Add(new KeyValuePair<String, long>(khoan.NoiDung, khoan.SoTienThem));
        }
    }

    /// <summary>Người thăm cộng riêng các khoản được giảm (chiết khấu + giảm giá) và các khoản thu thêm.</summary>
    public class TongHopKhoanVisitor : IThamTien
    {
        private long m_TongGiam;
        private long m_TongThuThem;

        public long TongGiam { get { return m_TongGiam; } }
        public long TongThuThem { get { return m_TongThuThem; } }

        public void ThamTienHang(TienHang tien) { }
        public void ThamChietKhau(TruChietKhau khoan) { m_TongGiam -= khoan.SoTienThem; }
        public void ThamGiamGia(TruGiamGia khoan) { m_TongGiam -= khoan.SoTienThem; }
        public void ThamPhiVanChuyen(CongPhiVanChuyen khoan) { m_TongThuThem += khoan.SoTienThem; }
        public void ThamDichVu(CongDichVu khoan) { m_TongThuThem += khoan.SoTienThem; }
        public void ThamVAT(CongVAT khoan) { m_TongThuThem += khoan.SoTienThem; }
    }
}
