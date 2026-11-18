using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class KhachHang
    {
        public KhachHang() { }
        public KhachHang(String id)
        {
            m_Id = id;
        }
        public KhachHang(String id, String hoten)
        {
            m_Id = id;
            m_HoTen = hoten;
        }
        private String m_Id;

        public String Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        private String m_HoTen;

        public String HoTen
        {
            get { return m_HoTen; }
            set { m_HoTen = value; }
        }
        private String m_DiaChi;

        public String DiaChi
        {
            get { return m_DiaChi; }
            set { m_DiaChi = value; }
        }
        private String m_DienThoai;

        public String DienThoai
        {
            get { return m_DienThoai; }
            set { m_DienThoai = value; }
        }
        // REFACTOR #30 Replace Type Code with Subclasses.
        // Trước đây loại khách là cờ bool LoaiKH (false = khách lẻ, true = đại lý) do bên ngoài gán.
        // Nay loại khách do lớp con quyết định: DaiLy (khách lẻ là chính KhachHang); LoaiKH chỉ còn để đọc cho code cũ.

        /// <summary>Khách này có phải đại lý không (lớp con DaiLy trả về true).</summary>
        public virtual bool LaDaiLy
        {
            get { return false; }
        }

        /// <summary>Giữ cho code cũ / báo cáo: true = đại lý.</summary>
        public bool LoaiKH
        {
            get { return LaDaiLy; }
        }

        /// <summary>Tạo đúng lớp con theo giá trị cột LOAI_KH.</summary>
        public static KhachHang TaoTheoLoai(bool laDaiLy)
        {
            if (laDaiLy) return new DaiLy();
            return new KhachHang();   // khách lẻ (lớp KhachLe đã gộp vào KhachHang – REFACTOR #65)
        }

    }
}
