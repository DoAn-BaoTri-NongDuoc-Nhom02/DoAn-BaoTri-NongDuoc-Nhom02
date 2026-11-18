using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class SanPham
    {
        public SanPham() { }
        public SanPham(String id, String tensp)
        {
            m_Id = id;
            m_TenSP = tensp;
        }
        private String m_Id;

        public String Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        private String m_TenSP;

        public String TenSanPham
        {
            get { return m_TenSP; }
            set { m_TenSP = value; }
        }
        private int m_SoLuong;

        public int SoLuong
        {
            get { return m_SoLuong; }
            set { m_SoLuong = value; }
        }
        private long m_DonGiaNhap;

        public long DonGiaNhap
        {
            get { return m_DonGiaNhap; }
            set { m_DonGiaNhap = value; }
        }
        private long m_GiaBanSi;

        public long GiaBanSi
        {
            get { return m_GiaBanSi; }
            set { m_GiaBanSi = value; }
        }
        private long m_GiaBanLe;

        public long GiaBanLe
        {
            get { return m_GiaBanLe; }
            set { m_GiaBanLe = value; }
        }
        private DonViTinh m_DonViTinh;

        public DonViTinh DonViTinh
        {
            get { return m_DonViTinh; }
            set { m_DonViTinh = value; }
        }

	
	

        // REFACTOR #24 Change Unidirectional Association to Bidirectional – trước đây chỉ có chiều
        // MaSanPham.SanPham (lô -> sản phẩm). Thêm chiều ngược SanPham.DanhSachLo để từ sản phẩm biết
        // các lô của nó (tính tồn theo lô, lô sắp hết hạn). Chiều ngược được giữ đồng bộ trong setter MaSanPham.SanPham.
        private readonly Dictionary<String, MaSanPham> m_CacLo = new Dictionary<String, MaSanPham>();

        /// <summary>Các lô đã nạp của sản phẩm này.</summary>
        public IList<MaSanPham> DanhSachLo
        {
            get { return new List<MaSanPham>(m_CacLo.Values).AsReadOnly(); }
        }

        /// <summary>Tổng số lượng còn trong các lô đã nạp.</summary>
        public int TongTonTheoLo
        {
            get
            {
                int tong = 0;
                foreach (MaSanPham lo in m_CacLo.Values) tong += lo.SoLuong;
                return tong;
            }
        }

        internal void GanLo(MaSanPham lo)
        {
            if (lo != null && lo.Id != null) m_CacLo[lo.Id] = lo;
        }

        internal void BoLo(MaSanPham lo)
        {
            if (lo != null && lo.Id != null) m_CacLo.Remove(lo.Id);
        }
    }
}
