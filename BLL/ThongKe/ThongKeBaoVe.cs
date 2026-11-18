using System;
using System.Data;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.ThongKe
{
    /// <summary>
    /// Lớp "đại diện" kiểm soát quyền khi xem thống kê.
    ///
    /// PATTERN #12 Proxy (protection proxy).
    /// Form thống kê làm việc với IThongKe nên không phân biệt được đang dùng thống kê thật hay proxy.
    /// Proxy kiểm tra quyền của người đang đăng nhập (PhienDangNhap) trước khi chuyển lời gọi cho thống kê thật:
    /// không có quyền thống kê thì chặn; nhân viên bán hàng xem thống kê có cột nhân viên (ID_NGUOI_DUNG)
    /// thì chỉ còn các dòng của chính mình (tổng cộng tính lại). Admin, Kế toán xem tất cả.
    /// </summary>
    public class ThongKeBaoVe : IThongKe
    {
        private readonly IThongKe m_That;

        public ThongKeBaoVe(IThongKe thongKeThat)
        {
            if (thongKeThat == null) throw new ArgumentNullException("thongKeThat");
            m_That = thongKeThat;
        }

        private static QuyenVaiTro Quyen
        {
            get { return PhienDangNhap.HienTai.Quyen; }
        }

        private static bool ChiXemCuaMinh
        {
            get { return !Quyen.DuocPhep(ChucNang.THONG_KE_TAT_CA_NHAN_VIEN); }
        }

        public String TieuDe
        {
            get
            {
                if (ChiXemCuaMinh)
                    return m_That.TieuDe + " – " + PhienDangNhap.NguoiDungHienTai.HoTen;
                return m_That.TieuDe;
            }
        }

        public IKenhXuat KenhXuat
        {
            get { return m_That.KenhXuat; }
            set { m_That.KenhXuat = value; }
        }

        public DataTable ThucHien(KhoangNgay khoang)
        {
            if (!Quyen.DuocPhep(ChucNang.THONG_KE))
                throw new NghiepVuException("Tài khoản của bạn không có quyền xem thống kê.");

            DataTable bang = m_That.ThucHien(khoang);
            if (!ChiXemCuaMinh || !bang.Columns.Contains("ID_NGUOI_DUNG"))
                return bang;

            int idCuaToi = PhienDangNhap.NguoiDungHienTai.ID;
            ThongKeTheoKhoangNgay.BoDongTong(bang);
            for (int i = bang.Rows.Count - 1; i >= 0; i--)
            {
                object id = bang.Rows[i]["ID_NGUOI_DUNG"];
                if (id == DBNull.Value || Convert.ToInt32(id) != idCuaToi)
                    bang.Rows.RemoveAt(i);
            }
            ThongKeTheoKhoangNgay.ThemDongTong(bang);
            return bang;
        }

        public void Xuat(KhoangNgay khoang)
        {
            if (KenhXuat == null) throw new InvalidOperationException("Chưa chọn kênh xuất cho thống kê.");
            KenhXuat.Xuat(TieuDe + " (" + khoang + ")", ThucHien(khoang));
        }
    }
}
