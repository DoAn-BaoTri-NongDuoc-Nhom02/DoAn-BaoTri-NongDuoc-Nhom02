using System;
using System.Collections.Generic;

namespace CuahangNongduoc.BusinessObject
{
    /// <summary>
    /// PATTERN #8 Composite – một dịch vụ trên hóa đơn có thể là dịch vụ lẻ (giao hàng, bốc xếp...)
    /// hoặc một GÓI gồm nhiều dịch vụ (ví dụ "Gói chăm sóc lúa" = phun thuốc hộ + tư vấn kỹ thuật),
    /// và gói lại có thể chứa gói con. Cả hai cùng giao diện IDichVu nên phần tính tiền (CongDichVu)
    /// chỉ gọi SoTien mà không cần phân biệt lẻ hay gói.
    /// </summary>
    public interface IDichVu
    {
        String Ten { get; }
        long SoTien { get; }
        /// <summary>Các dịch vụ lẻ bên trong (để lưu CSDL / in từng dòng).</summary>
        IEnumerable<DichVuPhatSinh> CacDichVuLe();
    }

    /// <summary>Dịch vụ lẻ (lá của cây Composite).</summary>
    public class DichVuPhatSinh : IDichVu
    {
        private readonly String m_Ten;
        private readonly long m_SoTien;

        public DichVuPhatSinh(String ten, long soTien)
        {
            if (String.IsNullOrEmpty(ten)) throw new ArgumentException("Chưa nhập tên dịch vụ.");
            if (soTien < 0) throw new ArgumentException("Tiền dịch vụ không được âm.");
            m_Ten = ten.Trim();
            m_SoTien = soTien;
        }

        public String Ten { get { return m_Ten; } }
        public long SoTien { get { return m_SoTien; } }

        public IEnumerable<DichVuPhatSinh> CacDichVuLe()
        {
            yield return this;
        }
    }

    /// <summary>Gói dịch vụ (nút của cây Composite): tiền = tổng tiền các thành phần.</summary>
    public class GoiDichVu : IDichVu
    {
        private readonly String m_Ten;
        private readonly List<IDichVu> m_ThanhPhan = new List<IDichVu>();

        public GoiDichVu(String ten, IEnumerable<IDichVu> thanhPhan)
        {
            m_Ten = ten ?? "";
            if (thanhPhan != null) m_ThanhPhan.AddRange(thanhPhan);
        }

        public String Ten { get { return m_Ten; } }

        public void Them(IDichVu dichVu)
        {
            if (dichVu != null) m_ThanhPhan.Add(dichVu);
        }

        public long SoTien
        {
            get
            {
                long tong = 0;
                foreach (IDichVu dv in m_ThanhPhan) tong += dv.SoTien;
                return tong;
            }
        }

        public IEnumerable<DichVuPhatSinh> CacDichVuLe()
        {
            foreach (IDichVu dv in m_ThanhPhan)
                foreach (DichVuPhatSinh le in dv.CacDichVuLe())
                    yield return le;
        }

        /// <summary>Gói mẫu dùng trong hộp thoại "Thông tin thêm".</summary>
        public static GoiDichVu GoiChamSocLua()
        {
            return new GoiDichVu("Gói chăm sóc lúa", new IDichVu[] {
                new DichVuPhatSinh("Phun thuốc hộ (gói chăm sóc lúa)", 150000),
                new DichVuPhatSinh("Tư vấn kỹ thuật tại ruộng (gói chăm sóc lúa)", 50000) });
        }
    }
}
