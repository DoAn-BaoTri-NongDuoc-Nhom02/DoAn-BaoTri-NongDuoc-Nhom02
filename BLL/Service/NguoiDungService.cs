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

        public string Them(NguoiDung nd)
        {
            // Thay IsNullOrWhiteSpace bằng cách này
            if (string.IsNullOrEmpty(nd.TenDangNhap) || nd.TenDangNhap.Trim() == "" ||
                string.IsNullOrEmpty(nd.MatKhau) || nd.MatKhau.Trim() == "" ||
                string.IsNullOrEmpty(nd.HoTen) || nd.HoTen.Trim() == "")
            {
                return "Vui lòng nhập đầy đủ thông tin!";
            }

            if (_repo.KiemTraTenDangNhapTonTai(nd.TenDangNhap))
                return "Tên đăng nhập đã tồn tại!";

            if (_repo.Them(nd))
                return "OK";
            return "Thêm thất bại!";
        }

        public string Sua(NguoiDung nd)
        {
            // Thay IsNullOrWhiteSpace bằng cách này
            if (string.IsNullOrEmpty(nd.TenDangNhap) || nd.TenDangNhap.Trim() == "" ||
                string.IsNullOrEmpty(nd.MatKhau) || nd.MatKhau.Trim() == "" ||
                string.IsNullOrEmpty(nd.HoTen) || nd.HoTen.Trim() == "")
            {
                return "Vui lòng nhập đầy đủ thông tin!";
            }

            if (_repo.KiemTraTenDangNhapTonTai(nd.TenDangNhap, nd.ID))
                return "Tên đăng nhập đã tồn tại!";

            if (_repo.Sua(nd))
                return "OK";
            return "Cập nhật thất bại!";
        }

        public const int DO_DAI_MAT_KHAU_TOI_THIEU = 4;

        /// <summary>Tự đổi mật khẩu của chính mình. Trả về "OK" hoặc thông báo lỗi.</summary>
        public string DoiMatKhau(int id, string matKhauCu, string matKhauMoi, string xacNhan)
        {
            if (string.IsNullOrEmpty(matKhauCu) || string.IsNullOrEmpty(matKhauMoi) || string.IsNullOrEmpty(xacNhan))
                return "Vui lòng nhập đầy đủ thông tin!";

            if (matKhauMoi != xacNhan)
                return "Xác nhận mật khẩu mới không khớp!";

            if (matKhauMoi.Length < DO_DAI_MAT_KHAU_TOI_THIEU)
                return "Mật khẩu mới phải có ít nhất " + DO_DAI_MAT_KHAU_TOI_THIEU + " ký tự!";

            if (matKhauMoi == matKhauCu)
                return "Mật khẩu mới phải khác mật khẩu hiện tại!";

            if (!_repo.KiemTraMatKhau(id, matKhauCu))
                return "Mật khẩu hiện tại không đúng!";

            if (_repo.DoiMatKhau(id, matKhauMoi))
                return "OK";
            return "Đổi mật khẩu thất bại!";
        }

        public string Xoa(int id)
        {
            if (id == 1)
                return "Không được xóa tài khoản Admin mặc định!";

            if (_repo.Xoa(id))
                return "OK";
            return "Xóa thất bại!";
        }
    }

}
