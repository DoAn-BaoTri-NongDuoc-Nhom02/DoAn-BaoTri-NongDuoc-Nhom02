using System;
using System.Collections.Generic;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc.BanHang
{
    /// <summary>
    /// PATTERN #14 Command – mỗi thao tác sửa hóa đơn đang soạn được gói thành một đối tượng "lệnh" có
    /// ThucHien() và HoanTac(). Form không tự sửa dữ liệu mà giao lệnh cho LichSuLenh; nhờ vậy thao tác
    /// có thể hoàn tác (Ctrl+Z trên form Bán lẻ / Bán sỉ) và thêm loại lệnh mới không phải sửa form.
    /// </summary>
    public interface ILenhHoaDon
    {
        String Ten { get; }
        void ThucHien();
        void HoanTac();
    }

    /// <summary>Lệnh áp các giá trị mới (chiết khấu, phí, VAT, giảm giá, dịch vụ...) vào thông tin hóa đơn.</summary>
    public class LenhCapNhatThongTin : ILenhHoaDon
    {
        private readonly ThongTinHoaDon m_Dich;
        private readonly ThongTinHoaDon.Memento m_GiaTriMoi;
        private ThongTinHoaDon.Memento m_Truoc;

        /// <param name="dich">Thông tin hóa đơn đang dùng trên form.</param>
        /// <param name="banNhap">Bản nháp đã sửa (tạo bằng dich.SaoChep()).</param>
        public LenhCapNhatThongTin(ThongTinHoaDon dich, ThongTinHoaDon banNhap)
        {
            if (dich == null) throw new ArgumentNullException("dich");
            if (banNhap == null) throw new ArgumentNullException("banNhap");
            m_Dich = dich;
            m_GiaTriMoi = banNhap.TaoMemento();
        }

        public String Ten { get { return "Cập nhật thông tin thêm của hóa đơn"; } }

        public void ThucHien()
        {
            m_Truoc = m_Dich.TaoMemento();     // Memento: nhớ trạng thái cũ để hoàn tác
            m_Dich.KhoiPhuc(m_GiaTriMoi);
        }

        public void HoanTac()
        {
            if (m_Truoc != null) m_Dich.KhoiPhuc(m_Truoc);
        }
    }

    /// <summary>Nơi thực hiện lệnh và giữ lịch sử để hoàn tác (Invoker của Command).</summary>
    public class LichSuLenh
    {
        private readonly Stack<ILenhHoaDon> m_DaLam = new Stack<ILenhHoaDon>();

        public void ThucHien(ILenhHoaDon lenh)
        {
            lenh.ThucHien();
            m_DaLam.Push(lenh);
        }

        public bool CoTheHoanTac { get { return m_DaLam.Count > 0; } }

        /// <summary>Hoàn tác lệnh gần nhất; trả về tên lệnh (null nếu không còn gì để hoàn tác).</summary>
        public String HoanTac()
        {
            if (m_DaLam.Count == 0) return null;
            ILenhHoaDon lenh = m_DaLam.Pop();
            lenh.HoanTac();
            return lenh.Ten;
        }

        public void XoaLichSu()
        {
            m_DaLam.Clear();
        }
    }
}
