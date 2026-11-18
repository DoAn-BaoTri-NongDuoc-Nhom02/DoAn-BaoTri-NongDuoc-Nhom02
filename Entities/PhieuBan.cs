using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    public class PhieuBan
    {
        private readonly String m_Id;

        /// <summary>
        /// Mã phiếu chỉ gán một lần khi tạo.
        /// REFACTOR #50 Remove Setting Method – bỏ setter của Id vì mã phiếu không được đổi sau khi tạo.
        /// </summary>
        public PhieuBan(String id)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentException("Mã phiếu bán không được rỗng.");
            m_Id = id;
        }

        public String Id
        {
            get { return m_Id; }
        }

        private KhachHang m_KH;

        public KhachHang KhachHang
        {
            get { return m_KH; }
            set { m_KH = value; }
        }

        private DateTime m_NgayBan;

        public DateTime NgayBan
        {
            get { return m_NgayBan; }
            set { m_NgayBan = value; }
        }
        private long m_TongTien;

        public long TongTien
        {
            get { return m_TongTien; }
            set { m_TongTien = value; }
        }
        private long m_DaTra;

        public long DaTra
        {
            get { return m_DaTra; }
            set { m_DaTra = value; }
        }
        private long m_ConNo;

        public long ConNo
        {
            get { return m_ConNo; }
            set { m_ConNo = value; }
        }

        // REFACTOR #28 Encapsulate Collection.
        // Trước đây ChiTiet có setter công khai, nơi khác thay cả danh sách hoặc sửa thẳng bên trong.
        // Nay bên ngoài chỉ đọc được bản chỉ-đọc; muốn thay đổi phải qua các hàm bên dưới.
        private List<ChiTietPhieuBan> m_ChiTiet;

        /// <summary>Chi tiết (chỉ đọc); null nếu chưa nạp.</summary>
        public IList<ChiTietPhieuBan> ChiTiet
        {
            get { return m_ChiTiet == null ? null : m_ChiTiet.AsReadOnly(); }
        }

        /// <summary>Nạp (thay) toàn bộ chi tiết.</summary>
        public void GanChiTiet(IEnumerable<ChiTietPhieuBan> danhSach)
        {
            m_ChiTiet = danhSach == null ? new List<ChiTietPhieuBan>() : new List<ChiTietPhieuBan>(danhSach);
        }

        public void ThemChiTiet(ChiTietPhieuBan ct)
        {
            if (ct == null) throw new ArgumentNullException("ct");
            if (m_ChiTiet == null) m_ChiTiet = new List<ChiTietPhieuBan>();
            m_ChiTiet.Add(ct);
        }

        public bool XoaChiTiet(ChiTietPhieuBan ct)
        {
            return m_ChiTiet != null && m_ChiTiet.Remove(ct);
        }

        // PM-19
        public long ChietKhau { get; set; }
        public long PhiVanChuyen { get; set; }
        public decimal VAT { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
        public string SoHoaDonVAT { get; set; }

        // Yêu cầu 4: hóa đơn có dịch vụ phụ, phí vận chuyển, chiết khấu, giảm giá; ghi người lập
        /// <summary>Tổng thành tiền các dòng hàng (trước chiết khấu, giảm giá, phí).</summary>
        public long TienHang { get; set; }
        /// <summary>Tiền giảm theo chiến lược giảm giá đã chọn.</summary>
        public long GiamGia { get; set; }
        /// <summary>Mã cách giảm giá (ChienLuocGiamGia.MA_...).</summary>
        public string KieuGiam { get; set; }
        public long TienDichVu { get; set; }
        public string HinhThucThanhToan { get; set; }
        /// <summary>Nhân viên lập hóa đơn.</summary>
        public CuahangNongduoc.Entities.NguoiDung NguoiLap { get; set; }

        private readonly List<DichVuPhatSinh> m_DichVu = new List<DichVuPhatSinh>();

        public IList<DichVuPhatSinh> DichVu
        {
            get { return m_DichVu.AsReadOnly(); }
        }

        public void GanDichVu(IEnumerable<DichVuPhatSinh> dichVu)
        {
            m_DichVu.Clear();
            if (dichVu != null) m_DichVu.AddRange(dichVu);
        }

        /// <summary>Các dòng diễn giải cách tính tổng tiền (để in lên hóa đơn).</summary>
        public IList<KeyValuePair<string, long>> DienGiaiTien { get; set; }

	
    }
}
