using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.Controller;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc
{
    /// <summary>
    /// Phiên đăng nhập dùng chung cho toàn ứng dụng: ai đang đăng nhập, từ lúc nào, có quyền gì.
    ///
    /// PATTERN #5 Singleton.
    /// Cả chương trình chỉ có MỘT phiên đăng nhập; mọi form (bán hàng ghi người lập phiếu, thống kê
    /// lọc theo nhân viên, phân quyền menu...) phải thấy cùng một người dùng. Lớp có hàm khởi tạo private,
    /// đối tượng duy nhất lấy qua PhienDangNhap.HienTai.
    /// Các thuộc tính static bên dưới (NguoiDungHienTai, BatDau...) chỉ là lối tắt tới HienTai để code cũ không phải sửa.
    /// </summary>
    public sealed class PhienDangNhap
    {
        private static readonly PhienDangNhap s_DuyNhat = new PhienDangNhap();

        private NguoiDung m_NguoiDung;
        private DateTime m_ThoiDiemBatDau;
        private QuyenVaiTro m_Quyen;

        private PhienDangNhap()
        {
        }

        /// <summary>Phiên đăng nhập duy nhất của chương trình.</summary>
        public static PhienDangNhap HienTai
        {
            get { return s_DuyNhat; }
        }

        // ---------------- Thành viên của đối tượng duy nhất ----------------

        /// <summary>Người đang đăng nhập; chưa đăng nhập thì là NguoiDung.ChuaDangNhap (không bao giờ null).</summary>
        public NguoiDung NguoiDung
        {
            get { return m_NguoiDung ?? NguoiDung.ChuaDangNhap; }
        }

        public bool DangDangNhap
        {
            get { return NguoiDung.LaNguoiDungThat; }
        }

        public DateTime ThoiDiemBatDau
        {
            get { return m_ThoiDiemBatDau; }
        }

        /// <summary>Quyền của vai trò đang đăng nhập.</summary>
        public QuyenVaiTro Quyen
        {
            get
            {
                if (m_Quyen == null)
                    m_Quyen = QuyenVaiTro.Cua(NguoiDung.LoaiVaiTro);
                return m_Quyen;
            }
        }

        public void Mo(NguoiDung nd)
        {
            m_NguoiDung = nd;
            m_Quyen = null;
            m_ThoiDiemBatDau = DateTime.Now;
        }

        public void Dong()
        {
            m_NguoiDung = null;
            m_Quyen = null;
        }

        public bool Co(params LoaiVaiTro[] vaiTro)
        {
            foreach (LoaiVaiTro v in vaiTro)
            {
                if (NguoiDung.LoaiVaiTro == v)
                    return true;
            }
            return false;
        }

        // ---------------- Lối tắt tĩnh (giữ tương thích code cũ) ----------------

        public static NguoiDung NguoiDungHienTai
        {
            get { return s_DuyNhat.NguoiDung; }
        }

        public static bool DaDangNhap
        {
            get { return s_DuyNhat.DangDangNhap; }
        }

        /// <summary>Tên ghi vào nhật ký: tên đăng nhập trong phần mềm (chưa đăng nhập thì lấy tên tài khoản Windows).</summary>
        public static String TenGhiNhatKy
        {
            get
            {
                NguoiDung nd = s_DuyNhat.NguoiDung;
                return nd.LaNguoiDungThat ? nd.TenDangNhap : Environment.UserName;
            }
        }

        public static DateTime ThoiDiemDangNhap
        {
            get { return s_DuyNhat.ThoiDiemBatDau; }
        }

        public static void BatDau(NguoiDung nd)
        {
            s_DuyNhat.Mo(nd);
        }

        public static void KetThuc()
        {
            s_DuyNhat.Dong();
        }

        public static bool CoVaiTro(params LoaiVaiTro[] vaiTro)
        {
            return s_DuyNhat.Co(vaiTro);
        }
    }
}
