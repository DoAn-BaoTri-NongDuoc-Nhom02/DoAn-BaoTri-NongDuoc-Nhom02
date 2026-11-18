using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class ChiTietPhieuBan
    {
        // REFACTOR #25 Change Bidirectional Association to Unidirectional.
        // Đã bỏ con trỏ ngược ChiTietPhieuBan.PhieuBan: không nơi nào gán hay đọc nó,
        // chiều PhieuBan.ChiTiet -> ChiTietPhieuBan là đủ.
        private MaSanPham m_MaSP;

        public MaSanPham MaSanPham
        {
            get { return m_MaSP; }
            set { m_MaSP = value; }
        }
        private int m_SoLuong;

        public int SoLuong
        {
            get { return m_SoLuong; }
            set { m_SoLuong = value; }
        }
        private long m_DonGia;

        public long DonGia
        {
            get { return m_DonGia; }
            set { m_DonGia = value; }
        }
        private long m_ThanhTien;

        public long ThanhTien
        {
            get { return m_ThanhTien; }
            set { m_ThanhTien = value; }
        }

    }
}
