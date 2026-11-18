using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class MaSanPham
    {
        public MaSanPham() { }
        public MaSanPham(String id, int so_luong) 
        {
            m_Id = id;
            m_SoLuong = so_luong;
        }

        /// <summary>
        /// Tạo lô từ một dòng của bảng MA_SAN_PHAM (chưa gán SanPham).
        /// REFACTOR #52 Replace Constructor with Factory Method – thay cho 4 đoạn
        /// "new MaSanPham(); sp.Id = ...; sp.SoLuong = ...; ..." chép ở XuatKhoController và MaSanPhamController.
        /// Lô không có hạn dùng nhận NgayHetHan = DateTime.MaxValue (xếp sau cùng khi xuất FIFO).
        /// </summary>
        public static MaSanPham TuDongDuLieu(System.Data.DataRow row)
        {
            MaSanPham lo = new MaSanPham(Convert.ToString(row["ID"]), CuahangNongduoc.DuLieu.LaySoNguyen(row["SO_LUONG"]));
            lo.GiaNhap = CuahangNongduoc.DuLieu.LaySoLon(row, "DON_GIA_NHAP");
            lo.NgayNhap = CuahangNongduoc.DuLieu.LayNgay(row, "NGAY_NHAP", DateTime.MinValue);
            lo.NgaySanXuat = CuahangNongduoc.DuLieu.LayNgay(row, "NGAY_SAN_XUAT", DateTime.MinValue);
            lo.NgayHetHan = CuahangNongduoc.DuLieu.LayNgay(row, "NGAY_HET_HAN", DateTime.MaxValue);
            return lo;
        }

        private String m_Id;

        public String Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
      
        private SanPham m_SanPham;

        public SanPham SanPham
        {
            get { return m_SanPham; }
            set
            {
                // REFACTOR #24 – giữ hai chiều liên kết lô <-> sản phẩm luôn khớp nhau
                if (m_SanPham == value) return;
                if (m_SanPham != null) m_SanPham.BoLo(this);
                m_SanPham = value;
                if (m_SanPham != null) m_SanPham.GanLo(this);
            }
        }
        private long m_GiaNhap;

        public long GiaNhap
        {
            get { return m_GiaNhap; }
            set { m_GiaNhap = value; }
        }
        private int m_SoLuong;

        public int SoLuong
        {
            get { return m_SoLuong; }
            set { m_SoLuong = value; }
        }

        private long m_ThanhTien;

        public long ThanhTien
        {
            get { return m_ThanhTien; }
            set { m_ThanhTien = value; }
        }


        private DateTime m_NgayNhap;

        public DateTime NgayNhap
        {
            get { return m_NgayNhap; }
            set { m_NgayNhap = value; }
        }
        private DateTime m_NgaySanXuat;

        public DateTime NgaySanXuat
        {
            get { return m_NgaySanXuat; }
            set { m_NgaySanXuat = value; }
        }
        private DateTime  m_NgayHetHan;

        public DateTime  NgayHetHan
        {
            get { return m_NgayHetHan; }
            set { m_NgayHetHan = value; }
        }
        /// <summary>
        /// Giá nhập bình quân gia quyền của sản phẩm thuộc lô này (lấy từ SAN_PHAM.DON_GIA_NHAP),
        /// nếu chưa nạp sản phẩm thì dùng giá nhập của chính lô.
        /// REFACTOR #14 Hide Delegate – bên ngoài không cần đi xuyên lo.SanPham.DonGiaNhap.
        /// </summary>
        public long GiaBinhQuan
        {
            get { return m_SanPham != null ? m_SanPham.DonGiaNhap : m_GiaNhap; }
        }

        private PhieuNhap m_PhieuNhap;

        public PhieuNhap PhieuNhap
        {
            get { return m_PhieuNhap; }
            set { m_PhieuNhap = value; }
        }

        /// <summary>Ten SP phang (cho Report RDLC – tranh #Error khi nested SanPham.Value.TenSanPham).</summary>
        private String m_TenSanPham;
        public String TenSanPham
        {
            get
            {
                if (!string.IsNullOrEmpty(m_TenSanPham)) return m_TenSanPham;
                if (m_SanPham != null && m_SanPham.TenSanPham != null) return m_SanPham.TenSanPham;
                return "";
            }
            set { m_TenSanPham = value; }
        }


	
    }
}
