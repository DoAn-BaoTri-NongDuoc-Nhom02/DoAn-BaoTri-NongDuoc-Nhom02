using System;
using System.Collections.Generic;
using System.Text;
using CuahangNongduoc.DAL.Repositories;
using CuahangNongduoc.Entities;


namespace CuahangNongduoc.BLL.Service
{
    public class NguoiDungService
    {
        private readonly NguoiDungRepository _repo = new NguoiDungRepository();

        public NguoiDung DangNhap(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap?.Trim()) || string.IsNullOrEmpty(matKhau?.Trim()))
                return null;
            return _repo.DangNhap(tenDangNhap.Trim(), matKhau);
        }

        public List<NguoiDung> GetAll()
        {
            return _repo.GetAll();
        }

        public List<VaiTro> GetAllVaiTro()
        {
            return _repo.GetAllVaiTro();
        }

        /// <summary>Thêm tài khoản; lỗi nghiệp vụ ném NghiepVuException.</summary>
        public void Them(NguoiDung nd)
        {
            // Thay IsNullOrWhiteSpace bằng cách này
            if (string.IsNullOrEmpty(nd.TenDangNhap) || nd.TenDangNhap.Trim() == "" ||
                string.IsNullOrEmpty(nd.MatKhau) || nd.MatKhau.Trim() == "" ||
                string.IsNullOrEmpty(nd.HoTen) || nd.HoTen.Trim() == "")
            {
                throw new NghiepVuException("Vui lòng nhập đầy đủ thông tin!");
            }

            if (_repo.KiemTraTenDangNhapTonTai(nd.TenDangNhap))
                throw new NghiepVuException("Tên đăng nhập đã tồn tại!");

            if (!_repo.Them(nd))
                throw new NghiepVuException("Thêm thất bại!");
        }

        /// <summary>Sửa tài khoản; lỗi nghiệp vụ ném NghiepVuException.</summary>
        public void Sua(NguoiDung nd)
        {
            // Thay IsNullOrWhiteSpace bằng cách này
            if (string.IsNullOrEmpty(nd.TenDangNhap) || nd.TenDangNhap.Trim() == "" ||
                string.IsNullOrEmpty(nd.MatKhau) || nd.MatKhau.Trim() == "" ||
                string.IsNullOrEmpty(nd.HoTen) || nd.HoTen.Trim() == "")
            {
                throw new NghiepVuException("Vui lòng nhập đầy đủ thông tin!");
            }

            if (_repo.KiemTraTenDangNhapTonTai(nd.TenDangNhap, nd.ID))
                throw new NghiepVuException("Tên đăng nhập đã tồn tại!");

            if (!_repo.Sua(nd))
                throw new NghiepVuException("Cập nhật thất bại!");
        }

        public const int DO_DAI_MAT_KHAU_TOI_THIEU = 4;

        /// <summary>Tự đổi mật khẩu của chính mình; lỗi nghiệp vụ ném NghiepVuException.</summary>
        public void DoiMatKhau(int id, string matKhauCu, string matKhauMoi, string xacNhan)
        {
            if (string.IsNullOrEmpty(matKhauCu) || string.IsNullOrEmpty(matKhauMoi) || string.IsNullOrEmpty(xacNhan))
                throw new NghiepVuException("Vui lòng nhập đầy đủ thông tin!");

            if (matKhauMoi != xacNhan)
                throw new NghiepVuException("Xác nhận mật khẩu mới không khớp!");

            if (matKhauMoi.Length < DO_DAI_MAT_KHAU_TOI_THIEU)
                throw new NghiepVuException("Mật khẩu mới phải có ít nhất " + DO_DAI_MAT_KHAU_TOI_THIEU + " ký tự!");

            if (matKhauMoi == matKhauCu)
                throw new NghiepVuException("Mật khẩu mới phải khác mật khẩu hiện tại!");

            if (!_repo.KiemTraMatKhau(id, matKhauCu))
                throw new NghiepVuException("Mật khẩu hiện tại không đúng!");

            if (!_repo.DoiMatKhau(id, matKhauMoi))
                throw new NghiepVuException("Đổi mật khẩu thất bại!");
        }

        public void Xoa(int id)
        {
            if (id == NguoiDung.ID_ADMIN_MAC_DINH)
                throw new NghiepVuException("Không được xóa tài khoản Admin mặc định!");

            if (!_repo.Xoa(id))
                throw new NghiepVuException("Xóa thất bại!");
        }
    }

}
