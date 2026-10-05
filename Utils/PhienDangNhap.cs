using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc
{
    /// <summary>
    /// Phiên đăng nhập dùng chung cho toàn ứng dụng: ai đang đăng nhập, từ lúc nào.
    /// frmDangNhap gọi BatDau khi đăng nhập thành công và KetThuc khi đăng xuất / thoát.
    /// </summary>
    public static class PhienDangNhap
    {
        static NguoiDung m_NguoiDung;
        static DateTime m_ThoiDiemDangNhap;

        public static NguoiDung NguoiDungHienTai
        {
            get { return m_NguoiDung; }
        }

        public static bool DaDangNhap
        {
            get { return m_NguoiDung != null; }
        }

        public static DateTime ThoiDiemDangNhap
        {
            get { return m_ThoiDiemDangNhap; }
        }

        public static void BatDau(NguoiDung nd)
        {
            m_NguoiDung = nd;
            m_ThoiDiemDangNhap = DateTime.Now;
        }

        public static void KetThuc()
        {
            m_NguoiDung = null;
        }

        /// <summary>Người đang đăng nhập có một trong các vai trò (ID_VAI_TRO) này không.</summary>
        public static bool CoVaiTro(params int[] idVaiTro)
        {
            if (m_NguoiDung == null)
                return false;
            foreach (int id in idVaiTro)
            {
                if (m_NguoiDung.IDVaiTro == id)
                    return true;
            }
            return false;
        }
    }
}
