using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.Entities
{
    public class NguoiDung
    {
        // REFACTOR #26 Replace Magic Number with Symbolic Constant – số 1 (tài khoản admin gốc, không được xóa)
        // trước đây viết thẳng trong NguoiDungService.Xoa và câu SQL của NguoiDungRepository.Xoa.
        /// <summary>ID của tài khoản Admin mặc định (tạo sẵn trong script CSDL).</summary>
        public const int ID_ADMIN_MAC_DINH = 1;

        public int ID { get; set; }
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string HoTen { get; set; }
        public int IDVaiTro { get; set; }

        /// <summary>Vai trò dạng đối tượng (thay cho so sánh số IDVaiTro).</summary>
        [System.ComponentModel.Browsable(false)]
        public LoaiVaiTro LoaiVaiTro
        {
            get { return LoaiVaiTro.TuMa(IDVaiTro); }
        }
        public string TenVaiTro { get; set; }   // join thêm
        public bool TrangThai { get; set; }

        /// <summary>false với đối tượng "chưa đăng nhập" (NguoiDungChuaDangNhap).</summary>
        [System.ComponentModel.Browsable(false)]
        public virtual bool LaNguoiDungThat
        {
            get { return true; }
        }

        /// <summary>Đối tượng dùng thay cho null khi chưa có ai đăng nhập.</summary>
        public static readonly NguoiDung ChuaDangNhap = new NguoiDungChuaDangNhap();
    }

    /// <summary>
    /// Người dùng "rỗng" khi chưa đăng nhập.
    /// REFACTOR #39 Introduce Null Object – trước đây PhienDangNhap.NguoiDungHienTai trả về null
    /// và nơi dùng phải tự kiểm tra (frmMain, frmDoiMatKhau...), quên kiểm tra là NullReferenceException.
    /// </summary>
    public class NguoiDungChuaDangNhap : NguoiDung
    {
        public NguoiDungChuaDangNhap()
        {
            ID = 0;
            TenDangNhap = "";
            HoTen = "(chưa đăng nhập)";
            IDVaiTro = 0;
            TenVaiTro = "";
            TrangThai = false;
        }

        public override bool LaNguoiDungThat
        {
            get { return false; }
        }
    }
}
