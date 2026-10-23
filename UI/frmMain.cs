using CuahangNongduoc.Entities;
using CuahangNongduoc.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmMain : Form
    {
        private NguoiDung _nguoiDung;
        private bool _dangXuat = false;
        /// <summary>Menu Báo cáo → Doanh thu (tạo runtime nếu Designer chưa có).</summary>
        private ToolStripMenuItem mnuBaocaoDoanhThu;

        /// <summary>True nếu form đóng vì người dùng chọn Đăng xuất (frmDangNhap sẽ hiện lại).</summary>
        public bool DaDangXuat
        {
            get { return _dangXuat; }
        }

        public frmMain(NguoiDung nd)
        {
            InitializeComponent();
            _nguoiDung = nd;
            if (PhienDangNhap.NguoiDungHienTai == null)
                PhienDangNhap.BatDau(nd);
        }
        public frmMain()
        {
            InitializeComponent();
            // Không có tham số: lấy người đang đăng nhập từ phiên dùng chung (null -> form tự đóng ở frmMain_Load).
            _nguoiDung = PhienDangNhap.NguoiDungHienTai;
        }
        frmDonViTinh DonViTinh = null;

        private void mnuDonViTinh_Click(object sender, EventArgs e)
        {
            if (DonViTinh == null || DonViTinh.IsDisposed)
            {
                DonViTinh = new frmDonViTinh();
                DonViTinh.MdiParent = this;
                DonViTinh.Show();

            }
            else
                DonViTinh.Activate();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\CoolSoft\\CuahangNongduoc");

            //if (regKey == null)
            //{
            //    DataService.m_ConnectString = "";
            //}
            //else
            //{
            //    try
            //    {
            //        DataService.m_ConnectString = (String)regKey.GetValue("ConnectString");
            //    }
            //    catch
            //    {
            //    }
            //    finally
            //    {
            //        regKey.Close();
            //    }
            //}

            //if (DataService.OpenConnection() == false)
            //{
            //    MessageBox.Show("Không thể kết nối dữ liệu!", "Cua hang Nong duoc", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    this.Close();
            //}

            if (!DataService.OpenConnection())
            {
                MessageBox.Show("Không thể kết nối dữ liệu!", "Cua hang Nong duoc", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Chưa đăng nhập thì không cho vào (trước đây _nguoiDung = null sẽ hiện ra toàn bộ menu).
            if (_nguoiDung == null)
            {
                MessageBox.Show("Bạn chưa đăng nhập!", "Cua hang Nong duoc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            this.Text = "Cửa hàng Nông dược - Xin chào: " + _nguoiDung.HoTen + " (" + _nguoiDung.TenVaiTro + ")";
            TaoMucTaiKhoan();
            TaoThanhTrangThai();
            DamBaoMenuDoanhThu();    // Gắn Doanh thu vào menu + sidebar
            PhanQuyenTheoVaiTro();   // Gọi phân quyền
            PhanQuyenTaskPane();
            HienThanhChucNang();     // Hiển thị thanh chức năng bên trái ngay khi mở form
        }
        frmSanPham SanPham = null;
        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            if (SanPham == null || SanPham.IsDisposed)
            {
                SanPham = new frmSanPham();
                SanPham.MdiParent = this;
                SanPham.Show();
            }
            else
                SanPham.Activate();
        }
        frmKhachHang KhachHang = null;
        private void mnuKhachHang_Click(object sender, EventArgs e)
        {
            if (KhachHang == null || KhachHang.IsDisposed)
            {
                KhachHang = new frmKhachHang();
                KhachHang.MdiParent = this;
                KhachHang.Show();
            }
            else
                KhachHang.Activate();
        }
        frmDaiLy DaiLy = null;
        private void mnuDaiLy_Click(object sender, EventArgs e)
        {
            if (DaiLy == null || DaiLy.IsDisposed)
            {
                DaiLy = new frmDaiLy();
                DaiLy.MdiParent = this;
                DaiLy.Show();
            }
            else
                DaiLy.Activate();

        }
        frmNhapHang NhapHang = null;

        frmDanhsachPhieuNhap DanhsachPhieuNhap = null;
        private void mnuDanhsachPhieuNhap_Click(object sender, EventArgs e)
        {
            if (DanhsachPhieuNhap == null || DanhsachPhieuNhap.IsDisposed)
            {
                DanhsachPhieuNhap = new frmDanhsachPhieuNhap();
                DanhsachPhieuNhap.MdiParent = this;
                DanhsachPhieuNhap.Show();
            }
            else
                DanhsachPhieuNhap.Activate();
        }

        private void mnuNhapHang_Click(object sender, EventArgs e)
        {
            if (NhapHang == null || NhapHang.IsDisposed)
            {
                NhapHang = new frmNhapHang();
                NhapHang.MdiParent = this;
                NhapHang.Show();
            }
            else
                NhapHang.Activate();
        }
        frmDanhsachPhieuBanLe BanLe = null;
        private void mnuBanHangKH_Click(object sender, EventArgs e)
        {
            if (BanLe == null || BanLe.IsDisposed)
            {
                BanLe = new frmDanhsachPhieuBanLe();
                BanLe.MdiParent = this;
                BanLe.Show();
            }
            else
                BanLe.Activate();
        }
        frmDanhsachPhieuBanSi BanSi = null;
        private void mnuBanHangDL_Click(object sender, EventArgs e)
        {
            if (BanSi == null || BanSi.IsDisposed)
            {
                BanSi = new frmDanhsachPhieuBanSi();
                BanSi.MdiParent = this;
                BanSi.Show();
            }
            else
                BanSi.Activate();
        }

        private void mnuThanhCongCu_Click(object sender, EventArgs e)
        {
            mnuThanhCongCu.Checked = !mnuThanhCongCu.Checked;
            toolStrip.Visible = mnuThanhCongCu.Checked;
        }

        private void mnuThanhChucNang_Click(object sender, EventArgs e)
        {
            mnuThanhChucNang.Checked = !mnuThanhChucNang.Checked;
            taskPane.Visible = mnuThanhChucNang.Checked;
        }
        frmThanhToan ThanhToan = null;
        private void mnuThanhtoan_Click(object sender, EventArgs e)
        {
            if (ThanhToan == null || ThanhToan.IsDisposed)
            {
                ThanhToan = new frmThanhToan();
                ThanhToan.MdiParent = this;
                ThanhToan.Show();
            }
            else
                ThanhToan.Activate();
        }
        frmDunoKhachhang DunoKhachhang = null;
        private void mnuTonghopDuno_Click(object sender, EventArgs e)
        {
            if (DunoKhachhang == null || DunoKhachhang.IsDisposed)
            {
                DunoKhachhang = new frmDunoKhachhang();
                DunoKhachhang.MdiParent = this;
                DunoKhachhang.Show();
            }
            else
                DunoKhachhang.Activate();
        }
        frmDoanhThu DoanhThu = null;
        private void mnuBaocaoDoanhThu_Click(object sender, EventArgs e)
        {
            if (DoanhThu == null || DoanhThu.IsDisposed)
            {
                DoanhThu = new frmDoanhThu();
                DoanhThu.MdiParent = this;
                DoanhThu.Show();
            }
            else
                DoanhThu.Activate();

        }

        frmSoLuongTon SoLuongTon = null;
        private void mnuBaocaoSoluongton_Click(object sender, EventArgs e)
        {

            if (SoLuongTon == null || SoLuongTon.IsDisposed)
            {
                SoLuongTon = new frmSoLuongTon();
                SoLuongTon.MdiParent = this;
                SoLuongTon.Show();
            }
            else
                SoLuongTon.Activate();

        }
        frmSoLuongBan SoLuongBan = null;
        private void mnuSoLuongBan_Click(object sender, EventArgs e)
        {
            if (SoLuongBan == null || SoLuongBan.IsDisposed)
            {
                SoLuongBan = new frmSoLuongBan();
                SoLuongBan.MdiParent = this;
                SoLuongBan.Show();
            }
            else
                SoLuongBan.Activate();
        }
        frmSanphamHethan SanphamHethan = null;
        private void mnuSanphamHethan_Click(object sender, EventArgs e)
        {
            if (SanphamHethan == null || SanphamHethan.IsDisposed)
            {
                SanphamHethan = new frmSanphamHethan();
                SanphamHethan.MdiParent = this;
                SanphamHethan.Show();
            }
            else
                SanphamHethan.Activate();
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        frmThongtinCuahang ThongtinCuahang = null;
        private void mnuTuychinhThongtin_Click(object sender, EventArgs e)
        {

            if (ThongtinCuahang == null || ThongtinCuahang.IsDisposed)
            {
                ThongtinCuahang = new frmThongtinCuahang();
                ThongtinCuahang.MdiParent = this;
                ThongtinCuahang.Show();
            }
            else
                ThongtinCuahang.Activate();
        }
        frmThongtinLienhe ThongtinLienhe = null;
        private void mnuTrogiupLienhe_Click(object sender, EventArgs e)
        {
            if (ThongtinLienhe == null || ThongtinLienhe.IsDisposed)
            {
                ThongtinLienhe = new frmThongtinLienhe();
                ThongtinLienhe.MdiParent = this;
                ThongtinLienhe.Show();
            }
            else
                ThongtinLienhe.Activate();
        }

        frmNhaCungCap NhaCungCap = null;
        private void mnuNhaCungCap_Click(object sender, EventArgs e)
        {
            if (NhaCungCap == null || NhaCungCap.IsDisposed)
            {
                NhaCungCap = new frmNhaCungCap();
                NhaCungCap.MdiParent = this;
                NhaCungCap.Show();
            }
            else
                NhaCungCap.Activate();
        }
        frmLyDoChi LyDoChi = null;
        private void mnuLyDoChi_Click(object sender, EventArgs e)
        {
            if (LyDoChi == null || LyDoChi.IsDisposed)
            {
                LyDoChi = new frmLyDoChi();
                LyDoChi.MdiParent = this;
                LyDoChi.Show();
            }
            else
                LyDoChi.Activate();
        }

        frmPhieuChi PhieuChi = null;
        private void mnuPhieuChi_Click(object sender, EventArgs e)
        {
            if (PhieuChi == null || PhieuChi.IsDisposed)
            {
                PhieuChi = new frmPhieuChi();
                PhieuChi.MdiParent = this;
                PhieuChi.Show();
            }
            else
                PhieuChi.Activate();
        }

        private void mnuTrogiupHuongdan_Click(object sender, EventArgs e)
        {
            // Help.ShowHelp(this, "CPP.CHM");
        }

        /// <summary>
        /// Thêm "Doanh thu" vào menu Báo cáo; sửa sidebar itemTonghopDoanhthu (đang ghi nhầm "Số lượng bán").
        /// Không đổi layout Designer ngoài text + sự kiện.
        /// </summary>
        private void DamBaoMenuDoanhThu()
        {
            if (mnuBaocaoDoanhThu == null)
            {
                foreach (ToolStripItem it in mnuBaocao.DropDownItems)
                {
                    if (it.Name == "mnuBaocaoDoanhThu" || it.Text == "Doanh thu")
                    {
                        mnuBaocaoDoanhThu = it as ToolStripMenuItem;
                        break;
                    }
                }
            }
            if (mnuBaocaoDoanhThu == null)
            {
                mnuBaocaoDoanhThu = new ToolStripMenuItem("Doanh thu");
                mnuBaocaoDoanhThu.Name = "mnuBaocaoDoanhThu";
                try
                {
                    mnuBaocaoDoanhThu.Image = global::CuahangNongduoc.Properties.Resources.Soluongban;
                }
                catch { }
                mnuBaocaoDoanhThu.Click += new EventHandler(mnuBaocaoDoanhThu_Click);
                mnuBaocao.DropDownItems.Insert(0, mnuBaocaoDoanhThu);
            }

            // Sidebar Báo cáo: đổi nhãn + gắn đúng form Doanh thu
            try
            {
                if (itemTonghopDoanhthu != null)
                {
                    itemTonghopDoanhthu.Text = "Doanh thu";
                    itemTonghopDoanhthu.Click -= new EventHandler(mnuSoLuongBan_Click);
                    itemTonghopDoanhthu.Click -= new EventHandler(mnuBaocaoDoanhThu_Click);
                    itemTonghopDoanhthu.Click += new EventHandler(mnuBaocaoDoanhThu_Click);
                }
            }
            catch { }

            // Đảm bảo "Số lượng tồn" trên sidebar có Click
            try
            {
                if (itemTonKho != null)
                {
                    itemTonKho.Click -= new EventHandler(mnuBaocaoSoluongton_Click);
                    itemTonKho.Click += new EventHandler(mnuBaocaoSoluongton_Click);
                }
            }
            catch { }
        }

        private void PhanQuyenTheoVaiTro()
        {
            // ========== 1. ẨN HẾT MENU CON ==========
            mnuDonViTinh.Visible = false;
            mnuSanPham.Visible = false;
            mnuKhachHang.Visible = false;
            mnuDaiLy.Visible = false;
            mnuNhaCungCap.Visible = false;
            mnuNhapHang.Visible = false;
            mnuBanHang.Visible = false;
            mnuBanHangKH.Visible = false;
            mnuBanHangDL.Visible = false;
            mnuThanhtoan.Visible = false;
            mnuPhieuChi.Visible = false;
            mnuLyDoChi.Visible = false;
            mnuTonghopDuno.Visible = false;
            mnuBaocaoSoluongton.Visible = false;
            mnuSoLuongBan.Visible = false;
            mnuSanphamHethan.Visible = false;
            if (mnuBaocaoDoanhThu != null) mnuBaocaoDoanhThu.Visible = false;
            mnuTuychinhThongtin.Visible = false;
            mnuQuanLyTaiKhoan.Visible = false;          // ← menu mới

            // ========== 2. ẨN HẾT MENU CHA ==========
            mnuQuanLy.Visible = false;
            mnuNghiepVu.Visible = false;
            mnuBaocao.Visible = false;
            mnuTuychinh.Visible = false;

            // ========== 3. ẨN HẾT NÚT TOOLBAR ==========
            toolSanPham.Visible = false;
            toolNhaCungCap.Visible = false;
            toolKhachHang.Visible = false;
            toolDaiLy.Visible = false;
            toolNhapHang.Visible = false;
            toolBanSi.Visible = false;
            toolBanLe.Visible = false;
            toolPhieuChi.Visible = false;
            toolThanhtoan.Visible = false;
            toolTonKho.Visible = false;

            // ========== 4. HIỆN THEO TỪNG VAI TRÒ ==========
            switch (_nguoiDung.IDVaiTro)
            {
                case 1: // ========== ADMIN ==========
                        // Hiện tất cả menu cha
                    mnuQuanLy.Visible = true;
                    mnuNghiepVu.Visible = true;
                    mnuBaocao.Visible = true;
                    mnuTuychinh.Visible = true;

                    // Hiện tất cả menu con
                    mnuDonViTinh.Visible = true;
                    mnuSanPham.Visible = true;
                    mnuKhachHang.Visible = true;
                    mnuDaiLy.Visible = true;
                    mnuNhaCungCap.Visible = true;
                    mnuNhapHang.Visible = true;
                    mnuBanHang.Visible = true;
                    mnuBanHangKH.Visible = true;
                    mnuBanHangDL.Visible = true;
                    mnuThanhtoan.Visible = true;
                    mnuPhieuChi.Visible = true;
                    mnuLyDoChi.Visible = true;
                    mnuTonghopDuno.Visible = true;
                    mnuBaocaoSoluongton.Visible = true;
                    mnuSoLuongBan.Visible = true;
                    mnuSanphamHethan.Visible = true;
                    if (mnuBaocaoDoanhThu != null) mnuBaocaoDoanhThu.Visible = true;
                    mnuTuychinhThongtin.Visible = true;
                    mnuQuanLyTaiKhoan.Visible = true;    // ← chỉ Admin mới thấy

                    // Hiện tất cả toolbar
                    toolSanPham.Visible = true;
                    toolNhaCungCap.Visible = true;
                    toolKhachHang.Visible = true;
                    toolDaiLy.Visible = true;
                    toolNhapHang.Visible = true;
                    toolBanSi.Visible = true;
                    toolBanLe.Visible = true;
                    toolPhieuChi.Visible = true;
                    toolThanhtoan.Visible = true;
                    toolTonKho.Visible = true;
                    break;

                case 2: // ========== NHÂN VIÊN BÁN HÀNG ==========
                    mnuQuanLy.Visible = true;
                    mnuNghiepVu.Visible = true;
                    mnuBaocao.Visible = true;

                    mnuKhachHang.Visible = true;
                    mnuBanHang.Visible = true;
                    mnuBanHangKH.Visible = true;
                    mnuBanHangDL.Visible = true;
                    mnuThanhtoan.Visible = true;
                    mnuBaocaoSoluongton.Visible = true;

                    toolKhachHang.Visible = true;
                    toolBanSi.Visible = true;
                    toolBanLe.Visible = true;
                    toolThanhtoan.Visible = true;
                    toolTonKho.Visible = true;
                    break;

                case 3: // ========== NHÂN VIÊN KHO ==========
                    mnuQuanLy.Visible = true;
                    mnuNghiepVu.Visible = true;
                    mnuBaocao.Visible = true;

                    mnuSanPham.Visible = true;
                    mnuNhapHang.Visible = true;
                    mnuBaocaoSoluongton.Visible = true;
                    mnuSanphamHethan.Visible = true;

                    toolSanPham.Visible = true;
                    toolNhapHang.Visible = true;
                    toolTonKho.Visible = true;
                    break;

                case 4: // ========== KẾ TOÁN ==========
                    mnuNghiepVu.Visible = true;
                    mnuBaocao.Visible = true;

                    mnuThanhtoan.Visible = true;
                    mnuPhieuChi.Visible = true;
                    mnuLyDoChi.Visible = true;
                    mnuTonghopDuno.Visible = true;
                    mnuBaocaoSoluongton.Visible = true;
                    mnuSoLuongBan.Visible = true;
                    if (mnuBaocaoDoanhThu != null) mnuBaocaoDoanhThu.Visible = true;

                    toolPhieuChi.Visible = true;
                    toolThanhtoan.Visible = true;
                    toolTonKho.Visible = true;
                    break;
            }
        }

        // Thanh chức năng bên trái (task pane) phải theo đúng quyền của menu, nếu không người dùng
        // bật "Thanh chức năng" là vào được các form đã bị ẩn khỏi menu.
        private void PhanQuyenTaskPane()
        {
            // 1. Cho phép các nhóm Expando hiển thị trước
            expando1.Visible = true;
            expando2.Visible = true;
            expando3.Visible = true;

            // 2. Phân quyền hiển thị các mục con trong TaskPane theo Menu
            // LƯU Ý: với mục con của menu (nằm trong dropdown đang đóng) thuộc tính .Visible luôn trả về false,
            // phải dùng .Available mới đọc đúng trạng thái đã phân quyền. Control trong TaskPane cũng vậy
            // (khi TaskPane chưa hiện thì .Visible của con trả về false) nên ta lưu cờ riêng ra mảng bool.
            bool[][] hien =
            {
                new bool[] { mnuSanPham.Available, mnuKhachHang.Available, mnuDaiLy.Available,
                             mnuNhaCungCap.Available, mnuQuanLyTaiKhoan.Available },
                new bool[] { mnuNhapHang.Available, mnuBanHangDL.Available, mnuBanHangKH.Available,
                             mnuThanhtoan.Available, mnuPhieuChi.Available },
                new bool[] {
                    mnuBaocaoDoanhThu != null && mnuBaocaoDoanhThu.Available,
                    mnuBaocaoSoluongton.Available,
                    mnuSanphamHethan.Available }
            };

            // 4. Sắp xếp lại các mục + nhóm (vị trí trong designer là cố định nên khi ẩn bớt sẽ bị hở/trống)
            taskPane.SuspendLayout();
            XPExplorerBar.Expando[] nhom = { expando1, expando2, expando3 };
            XPExplorerBar.TaskItem[][] mucCon =
            {
                new XPExplorerBar.TaskItem[] { itemSanPham, itemKhachHang, itemDaiLy, itemNhaCungCap, itemQuanLyTaiKhoan },
                new XPExplorerBar.TaskItem[] { itemNhapHang, itemBanSi, itemBanLe, itemThanhToan, itemPhieuChi },
                new XPExplorerBar.TaskItem[] { itemTonghopDoanhthu, itemTonKho, taskItem1 }
            };

            int y = 12;
            for (int i = 0; i < nhom.Length; i++)
            {
                int soMuc = 0;
                int yMuc = 32;
                for (int j = 0; j < mucCon[i].Length; j++)
                {
                    mucCon[i][j].Visible = hien[i][j];
                    if (!hien[i][j]) continue;
                    mucCon[i][j].Location = new Point(8, yMuc);
                    yMuc += 33;
                    soMuc++;
                }

                nhom[i].Visible = soMuc > 0;
                if (soMuc == 0) continue;

                int chieuCao = 32 + soMuc * 33 + 8;
                nhom[i].ExpandedHeight = chieuCao;
                nhom[i].Location = new Point(12, y);
                nhom[i].Size = new Size(233, chieuCao);
                y += chieuCao + 12;
            }
            taskPane.ResumeLayout(true);
        }

        // Hiện thanh chức năng bên trái và đồng bộ dấu check trong menu Hiển thị.
        private void HienThanhChucNang()
        {
            mnuThanhChucNang.Checked = true;
            taskPane.Visible = true;
            taskPane.PerformLayout();
            taskPane.Invalidate(true);
        }

        // Thêm "Đổi mật khẩu" và "Đăng xuất" vào menu Hệ thống (menu này luôn hiện với mọi vai trò).
        private void TaoMucTaiKhoan()
        {
            ToolStripMenuItem mnuDoiMatKhau = new ToolStripMenuItem("Đổi mật khẩu");
            mnuDoiMatKhau.Click += new EventHandler(mnuDoiMatKhau_Click);

            ToolStripMenuItem mnuDangXuat = new ToolStripMenuItem("Đăng xuất");
            mnuDangXuat.Click += new EventHandler(mnuDangXuat_Click);

            mnuHeThong.DropDownItems.Insert(0, mnuDoiMatKhau);
            mnuHeThong.DropDownItems.Insert(1, mnuDangXuat);
            mnuHeThong.DropDownItems.Insert(2, new ToolStripSeparator());
        }

        // Thanh trạng thái phía dưới: người đăng nhập, vai trò, giờ đăng nhập.
        private void TaoThanhTrangThai()
        {
            ToolStripStatusLabel lblNguoiDung = new ToolStripStatusLabel();
            lblNguoiDung.Text = "Người dùng: " + _nguoiDung.HoTen + " (" + _nguoiDung.TenDangNhap + ")"
                + "   |   Vai trò: " + _nguoiDung.TenVaiTro
                + "   |   Đăng nhập lúc: " + PhienDangNhap.ThoiDiemDangNhap.ToString("HH:mm dd/MM/yyyy");

            StatusStrip thanhTrangThai = new StatusStrip();
            thanhTrangThai.Items.Add(lblNguoiDung);
            this.Controls.Add(thanhTrangThai);
        }

        private void mnuDoiMatKhau_Click(object sender, EventArgs e)
        {
            using (frmDoiMatKhau frm = new frmDoiMatKhau())
            {
                frm.ShowDialog(this);
            }
        }

        private void mnuDangXuat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Đăng xuất",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _dangXuat = true;
                this.Close();
            }
        }

        frmQuanLyTaiKhoan QuanLyTK = null;
        private void mnuQuanLyTaiKhoan_Click(object sender, EventArgs e)
        {
            if (QuanLyTK == null || QuanLyTK.IsDisposed)
            {
                QuanLyTK = new UI.frmQuanLyTaiKhoan();
                QuanLyTK.MdiParent = this;
                QuanLyTK.Show();
            }
            else
            {
                QuanLyTK.Activate();
            }
        }

        private void itemQuanLyTaiKhoan_Click(object sender, EventArgs e)
        {
            mnuQuanLyTaiKhoan_Click(sender, e);
        }
    }
}