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
