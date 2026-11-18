using System;
using System.Collections.Generic;
using System.Globalization;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BanHang
{
    /// <summary>Dữ liệu một hóa đơn mà điều kiện khuyến mãi được phép xét (ngữ cảnh của Interpreter).</summary>
    public class NguCanhKhuyenMai
    {
        private readonly Dictionary<String, decimal> m_Bien = new Dictionary<String, decimal>();

        public NguCanhKhuyenMai(long tienHang, KhachHang khach, DateTime ngay)
        {
            DaiLy daiLy = khach as DaiLy;
            m_Bien["TIEN_HANG"] = tienHang;
            m_Bien["LA_DAI_LY"] = daiLy != null ? 1 : 0;
            m_Bien["CAP_DAI_LY"] = daiLy != null ? daiLy.CapDaiLy : 0;
            m_Bien["THANG"] = ngay.Month;
            m_Bien["NGAY"] = ngay.Day;
        }

        public decimal Lay(String ten)
        {
            decimal v;
            if (!m_Bien.TryGetValue(ten, out v))
                throw new ArgumentException("Điều kiện khuyến mãi dùng biến không có: " + ten);
            return v;
        }
    }

    /// <summary>
    /// PATTERN #15 Interpreter – điều kiện khuyến mãi được viết thành câu ngắn, ví dụ
    ///     "TIEN_HANG >= 10000000 VA LA_DAI_LY"   hoặc   "THANG = 11 VA TIEN_HANG >= 1000000 HOAC CAP_DAI_LY = 1"
    /// Văn phạm:  BieuThuc := Va ('HOAC' Va)* ;  Va := DieuKien ('VA' DieuKien)* ;
    ///            DieuKien := 'KHONG' DieuKien | Bien (('>='|'<='|'>'|'<'|'='|'<>') So)?
    /// Mỗi quy tắc văn phạm là một lớp (BieuThucHoac, BieuThucVa, BieuThucKhong, SoSanh, BienDung) có hàm
    /// DanhGia(ngữ cảnh). Đổi chương trình khuyến mãi chỉ cần sửa câu điều kiện, không phải sửa code.
    /// </summary>
    public abstract class DieuKienKhuyenMai
    {
        public abstract bool DanhGia(NguCanhKhuyenMai nguCanh);

        /// <summary>Phân tích câu điều kiện thành cây biểu thức.</summary>
        public static DieuKienKhuyenMai PhanTich(String cau)
        {
            return new BoPhanTich(cau).PhanTich();
        }
    }

    public class BieuThucHoac : DieuKienKhuyenMai
    {
        private readonly DieuKienKhuyenMai m_Trai, m_Phai;
        public BieuThucHoac(DieuKienKhuyenMai trai, DieuKienKhuyenMai phai) { m_Trai = trai; m_Phai = phai; }
        public override bool DanhGia(NguCanhKhuyenMai nc) { return m_Trai.DanhGia(nc) || m_Phai.DanhGia(nc); }
    }

    public class BieuThucVa : DieuKienKhuyenMai
    {
        private readonly DieuKienKhuyenMai m_Trai, m_Phai;
        public BieuThucVa(DieuKienKhuyenMai trai, DieuKienKhuyenMai phai) { m_Trai = trai; m_Phai = phai; }
        public override bool DanhGia(NguCanhKhuyenMai nc) { return m_Trai.DanhGia(nc) && m_Phai.DanhGia(nc); }
    }

    public class BieuThucKhong : DieuKienKhuyenMai
    {
        private readonly DieuKienKhuyenMai m_Trong;
        public BieuThucKhong(DieuKienKhuyenMai trong) { m_Trong = trong; }
        public override bool DanhGia(NguCanhKhuyenMai nc) { return !m_Trong.DanhGia(nc); }
    }

    /// <summary>Biến đứng một mình: đúng khi khác 0 (ví dụ LA_DAI_LY).</summary>
    public class BienDung : DieuKienKhuyenMai
    {
        private readonly String m_Ten;
        public BienDung(String ten) { m_Ten = ten; }
        public override bool DanhGia(NguCanhKhuyenMai nc) { return nc.Lay(m_Ten) != 0; }
    }

    public class SoSanh : DieuKienKhuyenMai
    {
        private readonly String m_Bien, m_PhepSoSanh;
        private readonly decimal m_GiaTri;

        public SoSanh(String bien, String phepSoSanh, decimal giaTri)
        {
            m_Bien = bien; m_PhepSoSanh = phepSoSanh; m_GiaTri = giaTri;
        }

        public override bool DanhGia(NguCanhKhuyenMai nc)
        {
            decimal v = nc.Lay(m_Bien);
            switch (m_PhepSoSanh)
            {
                case ">=": return v >= m_GiaTri;
                case "<=": return v <= m_GiaTri;
                case ">": return v > m_GiaTri;
                case "<": return v < m_GiaTri;
                case "=": return v == m_GiaTri;
                case "<>": return v != m_GiaTri;
            }
            throw new InvalidOperationException("Phép so sánh lạ: " + m_PhepSoSanh);
        }
    }

    /// <summary>Tách câu thành từ và dựng cây biểu thức theo văn phạm ở trên.</summary>
    internal class BoPhanTich
    {
        private readonly List<String> m_Tu = new List<String>();
        private int m_ViTri;

        public BoPhanTich(String cau)
        {
            if (String.IsNullOrEmpty(cau)) throw new ArgumentException("Điều kiện khuyến mãi rỗng.");
            String s = cau.Replace(">=", " >= ").Replace("<=", " <= ").Replace("<>", " <> ");
            s = s.Replace(">", " > ").Replace("<", " < ").Replace("=", " = ");
            s = s.Replace(" >  = ", " >= ").Replace(" <  = ", " <= ").Replace(" <  > ", " <> ");
            foreach (String t in s.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
                m_Tu.Add(t.ToUpperInvariant());
        }

        public DieuKienKhuyenMai PhanTich()
        {
            DieuKienKhuyenMai kq = DocHoac();
            if (m_ViTri < m_Tu.Count) throw new FormatException("Thừa từ \"" + m_Tu[m_ViTri] + "\" trong điều kiện khuyến mãi.");
            return kq;
        }

        private String Xem() { return m_ViTri < m_Tu.Count ? m_Tu[m_ViTri] : null; }

        private DieuKienKhuyenMai DocHoac()
        {
            DieuKienKhuyenMai trai = DocVa();
            while (Xem() == "HOAC") { m_ViTri++; trai = new BieuThucHoac(trai, DocVa()); }
            return trai;
        }

        private DieuKienKhuyenMai DocVa()
        {
            DieuKienKhuyenMai trai = DocDieuKien();
            while (Xem() == "VA") { m_ViTri++; trai = new BieuThucVa(trai, DocDieuKien()); }
            return trai;
        }

        private DieuKienKhuyenMai DocDieuKien()
        {
            String tu = Xem();
            if (tu == null) throw new FormatException("Điều kiện khuyến mãi bị thiếu.");
            m_ViTri++;
            if (tu == "KHONG") return new BieuThucKhong(DocDieuKien());

            String phep = Xem();
            if (phep == ">=" || phep == "<=" || phep == ">" || phep == "<" || phep == "=" || phep == "<>")
            {
                m_ViTri++;
                String so = Xem();
                decimal giaTri;
                if (so == null || !decimal.TryParse(so.Replace(".", "").Replace(",", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out giaTri))
                    throw new FormatException("Sau \"" + tu + " " + phep + "\" phải là một số.");
                m_ViTri++;
                return new SoSanh(tu, phep, giaTri);
            }
            return new BienDung(tu);
        }
    }

    /// <summary>Một chương trình khuyến mãi: điều kiện (dạng câu) + phần trăm giảm.</summary>
    public class ChuongTrinhKhuyenMai
    {
        private readonly String m_Ten;
        private readonly String m_DieuKien;
        private readonly DieuKienKhuyenMai m_BieuThuc;
        private readonly decimal m_PhanTram;

        public ChuongTrinhKhuyenMai(String ten, String dieuKien, decimal phanTram)
        {
            m_Ten = ten;
            m_DieuKien = dieuKien;
            m_BieuThuc = DieuKienKhuyenMai.PhanTich(dieuKien);
            m_PhanTram = phanTram;
        }

        public String Ten { get { return m_Ten; } }
        public String DieuKien { get { return m_DieuKien; } }
        public decimal PhanTram { get { return m_PhanTram; } }

        public bool ApDungDuoc(NguCanhKhuyenMai nc) { return m_BieuThuc.DanhGia(nc); }

        /// <summary>Các chương trình đang chạy (có thể chuyển sang đọc từ CSDL).</summary>
        public static List<ChuongTrinhKhuyenMai> DanhSachMacDinh()
        {
            List<ChuongTrinhKhuyenMai> ds = new List<ChuongTrinhKhuyenMai>();
            ds.Add(new ChuongTrinhKhuyenMai("Đại lý mua từ 10 triệu", "TIEN_HANG >= 10000000 VA LA_DAI_LY", 5));
            ds.Add(new ChuongTrinhKhuyenMai("Hóa đơn từ 5 triệu", "TIEN_HANG >= 5000000", 3));
            ds.Add(new ChuongTrinhKhuyenMai("Khuyến mãi vụ Đông Xuân (tháng 11-12)", "THANG >= 11 VA TIEN_HANG >= 1000000", 2));
            return ds;
        }
    }

    /// <summary>Chiến lược giảm giá dùng Interpreter: áp chương trình khuyến mãi có % cao nhất mà hóa đơn thỏa.</summary>
    public class GiamTheoKhuyenMai : ChienLuocGiamGia
    {
        private readonly List<ChuongTrinhKhuyenMai> m_ChuongTrinh;
        private readonly DateTime m_NgayBan;

        public GiamTheoKhuyenMai() : this(ChuongTrinhKhuyenMai.DanhSachMacDinh(), DateTime.Today) { }

        public GiamTheoKhuyenMai(List<ChuongTrinhKhuyenMai> chuongTrinh, DateTime ngayBan)
        {
            m_ChuongTrinh = chuongTrinh;
            m_NgayBan = ngayBan;
        }

        public override String Ma { get { return MA_KHUYEN_MAI; } }
        public override String Ten { get { return "Khuyến mãi theo điều kiện"; } }

        /// <summary>Chương trình được áp (null nếu không có).</summary>
        public ChuongTrinhKhuyenMai ChonChuongTrinh(long tienHang, KhachHang khach)
        {
            NguCanhKhuyenMai nc = new NguCanhKhuyenMai(tienHang, khach, m_NgayBan);
            ChuongTrinhKhuyenMai tot = null;
            foreach (ChuongTrinhKhuyenMai ct in m_ChuongTrinh)
                if (ct.ApDungDuoc(nc) && (tot == null || ct.PhanTram > tot.PhanTram)) tot = ct;
            return tot;
        }

        protected override long TinhGiam(long tienHang, KhachHang khach)
        {
            ChuongTrinhKhuyenMai ct = ChonChuongTrinh(tienHang, khach);
            return ct == null ? 0 : (long)Math.Round(tienHang * ct.PhanTram / 100m, 0);
        }
    }
}
