using CuahangNongduoc.Entities;
using CuahangNongduoc.Controller;
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
        /// <summary>Menu Báo cáo → Thống kê theo ngày (yêu cầu 4, tạo lúc chạy).</summary>
        private ToolStripMenuItem mnuBaocaoThongKe;

        /// <summary>True nếu form đóng vì người dùng chọn Đăng xuất (frmDangNhap sẽ hiện lại).</summary>
        public bool DaDangXuat
        {
            get { return _dangXuat; }
        }

        /// <summary>
        /// REFACTOR #43 Remove Parameter – bỏ hàm khởi tạo frmMain(NguoiDung nd): người dùng đã có trong
        /// PhienDangNhap (frmDangNhap gọi BatDau trước khi mở form), truyền thêm tham số chỉ gây trùng lặp.
        /// Chưa đăng nhập thì form tự đóng ở frmMain_Load.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
            _nguoiDung = PhienDangNhap.NguoiDungHienTai;
        }
        /// <summary>
        /// Mở một form con trong cửa sổ chính: nếu form chưa mở (hoặc đã đóng) thì tạo mới, nếu đang mở thì đưa lên trước.
        /// REFACTOR #1 Extract Method – thay cho 19 khối if/else giống hệt nhau trong các hàm mnu..._Click.
        /// </summary>
        private T MoFormCon<T>(T formDangMo) where T : Form, new()
        {
            if (formDangMo == null || formDangMo.IsDisposed)
            {
                T formMoi = new T();
                formMoi.MdiParent = this;
                formMoi.Show();
                return formMoi;
            }
            formDangMo.Activate();
            return formDangMo;
        }

        frmDonViTinh DonViTinh = null;

        private void mnuDonViTinh_Click(object sender, EventArgs e)
        {
            DonViTinh = MoFormCon(DonViTinh);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\CoolSoft\\CuahangNongduoc");

            //if (regKey == null)
            //{
            //    DataService.ChuoiKetNoi = "";
            //}
            //else
            //{
            //    try
            //    {
            //        DataService.ChuoiKetNoi = (String)regKey.GetValue("ConnectString");
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
            if (!_nguoiDung.LaNguoiDungThat)
            {
                MessageBox.Show("Bạn chưa đăng nhập!", "Cua hang Nong duoc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            this.Text = "Cửa hàng Nông dược - Xin chào: " + _nguoiDung.HoTen + " (" + _nguoiDung.TenVaiTro + ")";
            TaoMucTaiKhoan();
            TaoThanhTrangThai();
            DamBaoMenuDoanhThu();    // Gắn Doanh thu vào menu + sidebar
            DamBaoMenuThongKe();
            PhanQuyenTheoVaiTro();   // Gọi phân quyền
            PhanQuyenTaskPane();
            HienThanhChucNang();     // Hiển thị thanh chức năng bên trái ngay khi mở form
        }
        frmSanPham SanPham = null;
        private void mnuSanPham_Click(object sender, EventArgs e)
        {
            SanPham = MoFormCon(SanPham);
        }
        frmKhachHang KhachHang = null;
        private void mnuKhachHang_Click(object sender, EventArgs e)
        {
            KhachHang = MoFormCon(KhachHang);
        }
        frmDaiLy DaiLy = null;
        private void mnuDaiLy_Click(object sender, EventArgs e)
        {
            DaiLy = MoFormCon(DaiLy);

        }
        frmNhapHang NhapHang = null;

        frmDanhsachPhieuNhap DanhsachPhieuNhap = null;
        private void mnuDanhsachPhieuNhap_Click(object sender, EventArgs e)
        {
            DanhsachPhieuNhap = MoFormCon(DanhsachPhieuNhap);
        }

        private void mnuNhapHang_Click(object sender, EventArgs e)
        {
            NhapHang = MoFormCon(NhapHang);
        }
        frmDanhsachPhieuBanLe BanLe = null;
        private void mnuBanHangKH_Click(object sender, EventArgs e)
        {
            BanLe = MoFormCon(BanLe);
        }
        frmDanhsachPhieuBanSi BanSi = null;
        private void mnuBanHangDL_Click(object sender, EventArgs e)
        {
            BanSi = MoFormCon(BanSi);
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
            ThanhToan = MoFormCon(ThanhToan);
        }
        frmDunoKhachhang DunoKhachhang = null;
        private void mnuTonghopDuno_Click(object sender, EventArgs e)
        {
            DunoKhachhang = MoFormCon(DunoKhachhang);
        }
        frmDoanhThu DoanhThu = null;
        private void mnuBaocaoDoanhThu_Click(object sender, EventArgs e)
        {
            DoanhThu = MoFormCon(DoanhThu);

        }

        frmSoLuongTon SoLuongTon = null;
        private void mnuBaocaoSoluongton_Click(object sender, EventArgs e)
        {

            SoLuongTon = MoFormCon(SoLuongTon);

        }
        frmSoLuongBan SoLuongBan = null;
        private void mnuSoLuongBan_Click(object sender, EventArgs e)
        {
            SoLuongBan = MoFormCon(SoLuongBan);
        }
        frmSanphamHethan SanphamHethan = null;
        private void mnuSanphamHethan_Click(object sender, EventArgs e)
        {
            SanphamHethan = MoFormCon(SanphamHethan);
        }

        private void mnuThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        frmThongtinCuahang ThongtinCuahang = null;
        private void mnuTuychinhThongtin_Click(object sender, EventArgs e)
        {

            ThongtinCuahang = MoFormCon(ThongtinCuahang);
        }
        frmThongtinLienhe ThongtinLienhe = null;
        private void mnuTrogiupLienhe_Click(object sender, EventArgs e)
        {
            ThongtinLienhe = MoFormCon(ThongtinLienhe);
        }

        frmNhaCungCap NhaCungCap = null;
        private void mnuNhaCungCap_Click(object sender, EventArgs e)
        {
            NhaCungCap = MoFormCon(NhaCungCap);
        }
        frmLyDoChi LyDoChi = null;
        private void mnuLyDoChi_Click(object sender, EventArgs e)
        {
            LyDoChi = MoFormCon(LyDoChi);
        }

        frmPhieuChi PhieuChi = null;
        private void mnuPhieuChi_Click(object sender, EventArgs e)
        {
            PhieuChi = MoFormCon(PhieuChi);
        }

        private void mnuTrogiupHuongdan_Click(object sender, EventArgs e)
        {
            // Help.ShowHelp(this, "CPP.CHM");
        }

        /// <summary>
        /// Thêm "Doanh thu" vào menu Báo cáo; sửa sidebar itemTonghopDoanhthu (đang ghi nhầm "Số lượng bán").
        /// Không đổi layout Designer ngoài text + sự kiện.
        /// </summary>
        frmThongKe ThongKe = null;

        private void DamBaoMenuThongKe()
        {
            if (mnuBaocaoThongKe != null) return;
            mnuBaocaoThongKe = new ToolStripMenuItem("Thống kê theo ngày");
            mnuBaocaoThongKe.Name = "mnuBaocaoThongKe";
            mnuBaocaoThongKe.Click += delegate { ThongKe = MoFormCon(ThongKe); };
            mnuBaocao.DropDownItems.Insert(0, mnuBaocaoThongKe);
        }

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

        /// <summary>
        /// Ẩn/hiện menu và nút công cụ theo quyền của vai trò đang đăng nhập.
        /// REFACTOR #38 Replace Conditional with Polymorphism – thay switch theo số vai trò bằng QuyenVaiTro.
        /// </summary>
        private void PhanQuyenTheoVaiTro()
        {
            // PATTERN #5 Singleton (nơi dùng) – lấy quyền từ phiên đăng nhập duy nhất PhienDangNhap.HienTai.
            // REFACTOR #38 Replace Conditional with Polymorphism (nơi dùng) – mỗi vai trò tự trả lời DuocPhep().
            QuyenVaiTro quyen = PhienDangNhap.HienTai.Quyen;
            foreach (KeyValuePair<ToolStripItem, String> muc in BangChucNang())
            {
                muc.Key.Visible = quyen.DuocPhep(muc.Value);
            }
        }

        /// <summary>Mỗi menu / nút công cụ ứng với chức năng nào.</summary>
        private List<KeyValuePair<ToolStripItem, String>> BangChucNang()
        {
            List<KeyValuePair<ToolStripItem, String>> bang = new List<KeyValuePair<ToolStripItem, String>>();
            ToolStripItem[] mucs = new ToolStripItem[] {
                mnuQuanLy, mnuNghiepVu, mnuBaocao, mnuTuychinh,
                mnuDonViTinh, mnuSanPham, mnuKhachHang, mnuDaiLy, mnuNhaCungCap, mnuNhapHang,
                mnuBanHang, mnuBanHangKH, mnuBanHangDL, mnuThanhtoan, mnuPhieuChi, mnuLyDoChi,
                mnuTonghopDuno, mnuBaocaoSoluongton, mnuSoLuongBan, mnuSanphamHethan, mnuBaocaoDoanhThu, mnuBaocaoThongKe,
                mnuTuychinhThongtin, mnuQuanLyTaiKhoan,
                toolSanPham, toolNhaCungCap, toolKhachHang, toolDaiLy, toolNhapHang,
                toolBanSi, toolBanLe, toolPhieuChi, toolThanhtoan, toolTonKho };
            String[] chucNang = new String[] {
                ChucNang.QUAN_LY, ChucNang.NGHIEP_VU, ChucNang.BAO_CAO, ChucNang.TUY_CHINH,
                ChucNang.DON_VI_TINH, ChucNang.SAN_PHAM, ChucNang.KHACH_HANG, ChucNang.DAI_LY, ChucNang.NHA_CUNG_CAP, ChucNang.NHAP_HANG,
                ChucNang.BAN_HANG, ChucNang.BAN_LE, ChucNang.BAN_SI, ChucNang.THANH_TOAN, ChucNang.PHIEU_CHI, ChucNang.LY_DO_CHI,
                ChucNang.TONG_HOP_DU_NO, ChucNang.TON_KHO, ChucNang.SO_LUONG_BAN, ChucNang.SAN_PHAM_HET_HAN, ChucNang.DOANH_THU, ChucNang.THONG_KE,
                ChucNang.THONG_TIN_CUA_HANG, ChucNang.QUAN_LY_TAI_KHOAN,
                ChucNang.SAN_PHAM, ChucNang.NHA_CUNG_CAP, ChucNang.KHACH_HANG, ChucNang.DAI_LY, ChucNang.NHAP_HANG,
                ChucNang.BAN_SI, ChucNang.BAN_LE, ChucNang.PHIEU_CHI, ChucNang.THANH_TOAN, ChucNang.TON_KHO };
            System.Diagnostics.Debug.Assert(mucs.Length == chucNang.Length);
            for (int i = 0; i < mucs.Length; i++)
            {
                if (mucs[i] != null)    // mnuBaocaoDoanhThu có thể chưa được tạo
                    bang.Add(new KeyValuePair<ToolStripItem, String>(mucs[i], chucNang[i]));
            }
            return bang;
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
        // REFACTOR #2 Inline Method – handler itemQuanLyTaiKhoan_Click (chỉ gọi lại hàm này) đã bị bỏ;
        // mục itemQuanLyTaiKhoan trong frmMain.Designer.cs nay gắn thẳng vào mnuQuanLyTaiKhoan_Click.
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
    }
}