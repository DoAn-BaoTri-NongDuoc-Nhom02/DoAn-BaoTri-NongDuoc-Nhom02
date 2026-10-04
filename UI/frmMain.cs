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
        public frmMain(NguoiDung nd)
        {
            InitializeComponent();
            _nguoiDung = nd;
        }
        public frmMain()
        {
            InitializeComponent();
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

            DataService.OpenConnection();

            if (_nguoiDung != null)
            {
                this.Text = "Cửa hàng Nông dược - Xin chào: " + _nguoiDung.HoTen + " (" + _nguoiDung.TenVaiTro + ")";
                PhanQuyenTheoVaiTro();   // Gọi phân quyền
            }

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
        frmDanhsachPhieuNhap NhapHang = null;
        private void mnuNhapHang_Click(object sender, EventArgs e)
        {
            if (NhapHang == null || NhapHang.IsDisposed)
            {
                NhapHang = new frmDanhsachPhieuNhap();
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

                    toolPhieuChi.Visible = true;
                    toolThanhtoan.Visible = true;
                    toolTonKho.Visible = true;
                    break;
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