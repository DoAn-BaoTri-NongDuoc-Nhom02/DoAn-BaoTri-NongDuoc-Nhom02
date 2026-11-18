using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class PhieuNhap
    {
        // REFACTOR #50 Remove Setting Method – mã phiếu chỉ gán qua hàm khởi tạo.
        public PhieuNhap(String id)
        {
            m_Id = id;
        }
        private readonly String m_Id;

        public String Id
        {
            get { return m_Id; }
        }
        private DateTime m_NgayNhap;

        public DateTime NgayNhap
        {
            get { return m_NgayNhap; }
            set { m_NgayNhap = value; }
        }
        private long m_TongTien;

        public long TongTien
        {
            get { return m_TongTien; }
            set { m_TongTien = value; }
        }
        private long m_Datra;

        public long DaTra
        {
            get { return m_Datra; }
            set { m_Datra = value; }
        }

        private long m_ConNo;

        public long ConNo
        {
            get { return m_ConNo; }
            set { m_ConNo = value; }
        }

        private NhaCungCap m_NCC;

        public NhaCungCap NhaCungCap
        {
            get { return m_NCC; }
            set { m_NCC = value; }
        }

	

        // REFACTOR #28 Encapsulate Collection.
        // Trước đây ChiTiet có setter công khai, nơi khác thay cả danh sách hoặc sửa thẳng bên trong.
        // Nay bên ngoài chỉ đọc được bản chỉ-đọc; muốn thay đổi phải qua các hàm bên dưới.
        private List<MaSanPham> m_ChiTiet;

        /// <summary>Chi tiết (chỉ đọc); null nếu chưa nạp.</summary>
        public IList<MaSanPham> ChiTiet
        {
            get { return m_ChiTiet == null ? null : m_ChiTiet.AsReadOnly(); }
        }

        /// <summary>Nạp (thay) toàn bộ chi tiết.</summary>
        public void GanChiTiet(IEnumerable<MaSanPham> danhSach)
        {
            m_ChiTiet = danhSach == null ? new List<MaSanPham>() : new List<MaSanPham>(danhSach);
        }

        public void ThemChiTiet(MaSanPham ct)
        {
            if (ct == null) throw new ArgumentNullException("ct");
            if (m_ChiTiet == null) m_ChiTiet = new List<MaSanPham>();
            m_ChiTiet.Add(ct);
        }

        public bool XoaChiTiet(MaSanPham ct)
        {
            return m_ChiTiet != null && m_ChiTiet.Remove(ct);
        }

	
    }
}
