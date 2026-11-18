using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.BanHang;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    public partial class frmBanLe: Form
    {
        SanPhamController ctrlSanPham = new SanPhamController();
        KhachHangController ctrlKhachHang = new KhachHangController();
        MaSanPhamController ctrlMaSanPham = new MaSanPhamController();
        PhieuBanController ctrlPhieuBan = new PhieuBanController();
        ChiTietPhieuBanController ctrlChiTiet = new ChiTietPhieuBanController();
        XuatKhoController ctrlXuatKho = new XuatKhoController();
        IList<MaSanPham> deleted = new List<MaSanPham>();
        Controll status = Controll.Normal;
        // Thông tin thêm của hóa đơn (chiết khấu, phí vận chuyển, VAT...) nhập qua hộp thoại khi Lưu.
        // PATTERN #2 Abstract Factory (nơi dùng) – một nhà máy tạo đủ bộ đối tượng của kênh bán lẻ.
        private static readonly IKenhBanFactory s_NhaMayKenh = new KenhBanLeFactory();
        // PATTERN #4 Prototype (nơi dùng) – "mẫu mặc định" của kênh; mỗi phiếu mới là một bản sao của mẫu.
        private static readonly ThongTinHoaDon s_MauThongTin = (ThongTinHoaDon)s_NhaMayKenh.TaoThongTinHoaDon();
        private ThongTinHoaDon m_ThongTin = (ThongTinHoaDon)s_MauThongTin.SaoChep();
        // PATTERN #14 Command (nơi dùng) – lịch sử lệnh để hoàn tác bằng Ctrl+Z.
        private readonly LichSuLenh m_LichSuLenh = new LichSuLenh();
        private readonly BanHangService m_KenhBan = s_NhaMayKenh.TaoDichVuBan();
        private HoaDonMediator m_Mediator;


        public frmBanLe()
        {
            InitializeComponent();
            status = Controll.AddNew;
        }

     
        public frmBanLe(PhieuBanController ctrlPB)
            : this()
        {
            this.ctrlPhieuBan = ctrlPB;
            status = Controll.Normal;
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(HoanTacBangPhim);
            // Mediator điều phối các ô đơn giá / số lượng / thành tiền / tổng tiền / đã trả / còn nợ
            // PATTERN #17 Mediator (nơi dùng) – các ô số báo cho mediator thay vì tự sửa lẫn nhau.
            m_Mediator = new HoaDonMediator(numDonGia, numSoLuong, numThanhTien, numTongTien, numDaTra, numConNo);

            // Cấu hình xuất kho: FIFO thì hệ thống tự phân lô, chỉ định thì cho chọn lô.
            // PATTERN #21 Strategy (nơi dùng) – chiến lược xuất kho (FIFO / chỉ định) quyết định có cho chọn lô không.
            cmbMaSanPham.Enabled = ChienLuocXuatKho.TheoCauHinh().ChoChonLo;

            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            cmbSanPham.SelectedIndexChanged += new EventHandler(cmbSanPham_SelectedIndexChanged);

            ctrlKhachHang.HienthiKhachLeComboBox(cmbKhachHang);

            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator,cmbKhachHang, txtMaPhieu, dtNgayLapPhieu, numTongTien, numDaTra, numConNo);

            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);
            
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);


            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMa(LoaiChungTu.PhieuBan).ToString();
            }
            else
            {
                this.Allow(false);
            }


        }

        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
            {
                ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            }
        }


        void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSanPham.SelectedValue != null)
            {
                MaSanPhamController ctrlMSP = new MaSanPhamController();

                cmbMaSanPham.SelectedIndexChanged -= new EventHandler(cmbMaSanPham_SelectedIndexChanged);
                ctrlMSP.HienThiAutoComboBox(cmbSanPham.SelectedValue.ToString(), cmbMaSanPham);
                cmbMaSanPham.SelectedIndexChanged += new EventHandler(cmbMaSanPham_SelectedIndexChanged);

                // Lô đầu danh sách (hạn dùng gần nhất) được chọn sẵn: cập nhật giá/thông tin ngay.
                cmbMaSanPham_SelectedIndexChanged(cmbMaSanPham, EventArgs.Empty);
            }
        }

        void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMaSanPham.SelectedValue == null)
            {
                // Sản phẩm hết hàng: không còn lô nào để hiển thị.
                numDonGia.Value = 0;
                txtGiaNhap.Text = "";
                txtGiaXuat.Text = "";
                return;
            }

            MaSanPhamController ctrl = new MaSanPhamController();
            MaSanPham masp = ctrl.LayMaSanPham(cmbMaSanPham.SelectedValue.ToString());
            if (m_ThongTin.LoaiGia == 0) numDonGia.Value = masp.SanPham.DonGiaNhap;
            else if (m_ThongTin.LoaiGia == 1) numDonGia.Value = masp.SanPham.GiaBanSi;
            else numDonGia.Value = masp.SanPham.GiaBanLe;
            txtGiaNhap.Text = masp.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = masp.SanPham.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = masp.SanPham.GiaBanLe.ToString("#,###0");
            txtGiaBQGQ.Text = masp.SanPham.DonGiaNhap.ToString("#,###0");
            txtGiaXuat.Text = ctrlXuatKho.TinhGiaXuat(masp).ToString("#,###0");

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // PATTERN #21 Strategy (nơi dùng) – REFACTOR #31: phân lô theo chiến lược xuất kho, không còn if/else theo cấu hình.
            ChienLuocXuatKho chienLuoc = ChienLuocXuatKho.TheoCauHinh();

            if (cmbSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Sản phẩm !", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (chienLuoc.ChoChonLo && cmbMaSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Mã sản phẩm !", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Vui lòng nhập Số lượng !", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numDonGia.Value * numSoLuong.Value != numThanhTien.Value)
            {
                MessageBox.Show("Thành tiền sai!", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    ThemDongChiTiet(chienLuoc);
                }
                catch (TonKhoException ex)
                {
                    MessageBox.Show(ex.Message, "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // Thêm hàng vào phiếu: FIFO thì tách theo từng lô (hạn dùng gần nhất trước), chỉ định thì đúng lô đã chọn.
        // Số lượng các dòng đã có trong phiếu (chưa lưu) được tính vào để không bán vượt tồn.
        void ThemDongChiTiet(ChienLuocXuatKho chienLuoc)
        {
            int soLuong = (int)numSoLuong.Value;
            IDictionary<String, int> daChon = ctrlChiTiet.SoLuongChuaLuu();

            String idLo = cmbMaSanPham.SelectedValue == null ? null : cmbMaSanPham.SelectedValue.ToString();
            IList<PhanBoLo> phanBo = chienLuoc.PhanLo(ctrlXuatKho, cmbSanPham.SelectedValue.ToString(), idLo, soLuong, daChon);

            // Khóa chính chi tiết là (phiếu, lô): không thêm trùng vào dòng đã lưu trước đó.
            foreach (PhanBoLo pb in phanBo)
            {
                if (ctrlChiTiet.DaCoDongDaLuu(pb.Lo.Id))
                {
                    throw new TonKhoException("Lô '" + pb.Lo.Id + "' đã có trong phiếu đã lưu. " +
                        "Hãy xóa dòng đó rồi thêm lại với số lượng mới.");
                }
            }

            foreach (PhanBoLo pb in phanBo)
            {
                DataRow dong = ctrlChiTiet.TimDongChuaLuu(pb.Lo.Id);
                if (dong != null)
                {
                    // Cùng lô đã có dòng chưa lưu: gộp số lượng.
                    int sl = Convert.ToInt32(dong["SO_LUONG"]) + pb.SoLuong;
                    dong["SO_LUONG"] = sl;
                    dong["THANH_TIEN"] = numDonGia.Value * sl;
                }
                else
                {
                    DataRow row = ctrlChiTiet.NewRow();
                    row["ID_MA_SAN_PHAM"] = pb.Lo.Id;
                    row["ID_PHIEU_BAN"] = txtMaPhieu.Text;
                    row["DON_GIA"] = numDonGia.Value;
                    row["SO_LUONG"] = pb.SoLuong;
                    row["THANH_TIEN"] = numDonGia.Value * pb.SoLuong;
                    ctrlChiTiet.Add(row);
                }
            }
            numTongTien.Value += numThanhTien.Value;
        }

        // Dòng vừa thêm (chưa lưu) chưa trừ kho nên khi xóa không được hoàn kho.
        void GhiNhanXoaDong(DataRowView row)
        {
            if (row.Row.RowState != DataRowState.Added)
            {
                deleted.Add(new MaSanPham(Convert.ToString(row["ID_MA_SAN_PHAM"]), Convert.ToInt32(row["SO_LUONG"])));
            }
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            // PM-19: hop thoai thong tin them (KHONG doi layout form chinh)
            if (!NhapThongTinThemPM19()) return;
            if (MessageBox.Show("Xác nhận lưu phiếu bán lẻ?", "Bán lẻ",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            CapNhatTongTien();
            if (Luu())
            {
                status = Controll.Normal;
                this.Allow(false);
            }
        }

        // Trả về false nếu chưa lưu được (thiếu hàng, trùng mã phiếu...): phiếu vẫn ở trạng thái đang soạn.
        bool Luu()
        {
            if (!KiemTraTruocKhiLuu())
                return false;

            try
            {
                if (status == Controll.AddNew)
                {
                    if (!ThemMoi())
                        return false;
                }
                else
                {
                    CapNhat();
                }
                // REFACTOR #35 Consolidate Duplicate Conditional Fragments – cả hai nhánh (thêm mới /
                // cập nhật) trước đây đều tự gọi ctrlChiTiet.Save(); nay gọi một lần sau if/else.
                // Phiếu luôn được ghi trước chi tiết (chi tiết tham chiếu tới phiếu).
                ctrlChiTiet.Save();
                return true;
            }
            catch (TonKhoException ex)
            {
                MessageBox.Show(ex.Message, "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        // Kiểm tra tồn kho cho các dòng mới TRƯỚC khi ghi bất cứ thứ gì (phiếu, chi tiết, kho).
        bool KiemTraTruocKhiLuu()
        {
            IDictionary<String, int> hoanTra = new Dictionary<String, int>();
            foreach (MaSanPham masp in deleted)
            {
                int cu;
                hoanTra.TryGetValue(masp.Id, out cu);
                hoanTra[masp.Id] = cu + masp.SoLuong;
            }

            try
            {
                ctrlChiTiet.KiemTraTonKho(hoanTra);
                return true;
            }
            catch (TonKhoException ex)
            {
                MessageBox.Show(ex.Message, "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        void CapNhat()
        {
            foreach (MaSanPham masp in deleted)
            {
                CuahangNongduoc.DataLayer.MaSanPhamFactory.CapNhatSoLuong(masp.Id, masp.SoLuong);
            }
            deleted.Clear();

            ctrlPhieuBan.Update();
        }

        /// <summary>Ctrl+Z: hoàn tác lần sửa "Thông tin thêm" gần nhất (PATTERN #14 Command + #18 Memento).</summary>
        void HoanTacBangPhim(object sender, KeyEventArgs e)
        {
            if (!(e.Control && e.KeyCode == Keys.Z) || !m_LichSuLenh.CoTheHoanTac) return;
            if (dgvDanhsachSP.IsCurrentCellInEditMode) return;   // đang sửa ô trên lưới thì để lưới tự xử lý
            String ten = m_LichSuLenh.HoanTac();
            CapNhatTongTien();
            e.Handled = true;
            MessageBox.Show("Đã hoàn tác: " + ten, m_KenhBan.TenKenh, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>Khách đang chọn trên form (khách rỗng nếu chưa chọn).</summary>
        KhachHang KhachDangChon()
        {
            return ctrlKhachHang.LayKhachHang(Convert.ToString(cmbKhachHang.SelectedValue));
        }

        /// <summary>
        /// Lưu phiếu mới: lắp hóa đơn bằng HoaDonBuilder (Builder) rồi giao cho BanHangFacade (Facade)
        /// kiểm tra (Chain of Responsibility), chuyển trạng thái (State), ghi CSDL và báo sự kiện (Observer).
        /// </summary>
        bool ThemMoi()
        {
            try
            {
                // PATTERN #3 Builder (nơi dùng) – lắp hóa đơn từng bước rồi Build().
                // PATTERN #1 Factory Method (nơi dùng) – Build() gọi m_KenhBan.LapPhieuMoi() để tạo đúng loại phiếu lẻ / sỉ.
                PhieuBan hoaDon = new HoaDonBuilder(m_KenhBan, txtMaPhieu.Text)
                    .ChoKhachHang(KhachDangChon())
                    .NgayBan(dtNgayLapPhieu.Value)
                    .TienHang(TinhTienHang())
                    .DaTra((long)numDaTra.Value)
                    .ThongTinThem(m_ThongTin)
                    .NguoiLap(PhienDangNhap.NguoiDungHienTai)
                    .Build();

                // PATTERN #10 Facade (nơi dùng) – một lời gọi thay cho chuỗi bước kiểm tra / ghi phiếu / chi tiết / dịch vụ / báo sự kiện.
                new BanHangFacade(ctrlPhieuBan, ctrlChiTiet, s_NhaMayKenh.TaoChuoiKiemTra()).LuuHoaDonMoi(hoaDon);

                m_ThongTin.TrangThai = hoaDon.TrangThai;
                numTongTien.Value = hoaDon.TongTien;
                return true;
            }
            catch (NghiepVuException ex)
            {
                MessageBox.Show(ex.Message, m_KenhBan.TenKenh, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void toolLuu_Them_Click(object sender, EventArgs e)
        {
            m_ThongTin = (ThongTinHoaDon)s_MauThongTin.SaoChep();   // Prototype: phiếu mới không mang theo chiết khấu/phí của phiếu trước
            m_LichSuLenh.XoaLichSu();
            ctrlPhieuBan = new PhieuBanController();
            status = Controll.AddNew;
            txtMaPhieu.Text = ThamSo.LayMa(LoaiChungTu.PhieuBan).ToString();
            numTongTien.Value = 0;
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            this.Allow(true);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Bán lẻ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                BindingSource bs = ((BindingSource)dgvDanhsachSP.DataSource);
                DataRowView row = (DataRowView)bs.Current;
                numTongTien.Value -= Convert.ToInt64(row["THANH_TIEN"]);
                GhiNhanXoaDong(row);
                bs.RemoveCurrent();
            }
           
        }

        private void dgvDanhsachSP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Bán lẻ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                BindingSource bs = ((BindingSource)dgvDanhsachSP.DataSource);
                DataRowView row = (DataRowView)bs.Current;
                numTongTien.Value -= Convert.ToInt64(row["THANH_TIEN"]);
                GhiNhanXoaDong(row);

            }
        }

        private void toolLuuIn_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu bán hiện tại!", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                String ma_phieu = txtMaPhieu.Text;

                PhieuBanController ctrlPB = new PhieuBanController();

                CuahangNongduoc.BusinessObject.PhieuBan ph = ctrlPB.LayPhieuBan(ma_phieu);

                frmInPhieuBan InPhieuBan = new frmInPhieuBan(ph);

                InPhieuBan.Show();

            }
        }

        private void dgvDanhsachSP_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>Trạng thái (State) của phiếu đang chọn trên thanh điều hướng.</summary>
        // PATTERN #20 State (nơi dùng) – trạng thái hóa đơn quyết định có cho Sửa / Xóa không.
        TrangThaiHoaDon TrangThaiPhieuDangChon()
        {
            DataRowView view = bindingNavigator.BindingSource.Current as DataRowView;
            if (view == null || !view.Row.Table.Columns.Contains("TRANG_THAI"))
                return TrangThaiHoaDon.TuTen(null);
            return TrangThaiHoaDon.TuTen(DuLieu.LayChuoi(view["TRANG_THAI"]));
        }

        private void toolChinhSua_Click(object sender, EventArgs e)
        {
            TrangThaiHoaDon trangThai = TrangThaiPhieuDangChon();
            if (!trangThai.ChoSua)
            {
                MessageBox.Show("Hóa đơn đang ở trạng thái \"" + trangThai.Ten + "\", không được sửa.", m_KenhBan.TenKenh,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            status = Controll.Edit;
            this.Allow(true);
        }

        void Allow(bool val)
        {
            txtMaPhieu.Enabled = val;
            dtNgayLapPhieu.Enabled = val;
            numTongTien.Enabled = val;
            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dgvDanhsachSP.Enabled = val;
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                if (MessageBox.Show("Bạn có muốn lưu lại Phiếu bán này không?", "Phieu Ban Le", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!this.Luu())
                        return;   // chưa lưu được: ở lại để người dùng sửa
                }
            }
            this.Close();
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            TrangThaiHoaDon trangThai = TrangThaiPhieuDangChon();
            if (!trangThai.ChoXoa)
            {
                MessageBox.Show("Hóa đơn đang ở trạng thái \"" + trangThai.Ten + "\", không được xóa.", m_KenhBan.TenKenh,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataRowView view =  (DataRowView)bindingNavigator.BindingSource.Current;
            if (view != null)
            {
                if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Bán lẻ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();
                    IList<ChiTietPhieuBan> ds = ctrl.ChiTietPhieuBan(view["ID"].ToString());
                    foreach (ChiTietPhieuBan ct in ds)
                    {
                        CuahangNongduoc.DataLayer.MaSanPhamFactory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
                    }
                    bindingNavigator.BindingSource.RemoveCurrent();
                    ctrlPhieuBan.Save(PhienDangNhap.NguoiDungHienTai);
                }
            }
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            frmKhachHang KhachHang = new frmKhachHang();
            KhachHang.ShowDialog();
            ctrlKhachHang.HienthiKhachLeComboBox(cmbKhachHang);
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
        }



        // PM-19: khong doi design — dung Dialog
        bool NhapThongTinThemPM19()
        {
            using (Form dlg = new Form())
            {
                dlg.Text = "Thông tin thêm (bán lẻ)";
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;

                int y = 12;
                ComboBox cGia = new ComboBox();
                cGia.Left = 120; cGia.Top = y; cGia.Width = 200;
                cGia.DropDownStyle = ComboBoxStyle.DropDownList;
                cGia.Items.AddRange(new object[] { "Giá nhập", "Giá sỉ", "Giá lẻ" });
                cGia.SelectedIndex = m_ThongTin.LoaiGia;
                dlg.Controls.Add(new Label { Text = "Loại giá:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(cGia); y += 28;

                TienUpDown nCK = new TienUpDown(m_ThongTin.ChietKhau);
                nCK.Left = 120; nCK.Top = y; nCK.Width = 200;
                dlg.Controls.Add(new Label { Text = "Chiết khấu:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(nCK); y += 28;

                TienUpDown nPhi = new TienUpDown(m_ThongTin.PhiVanChuyen);
                nPhi.Left = 120; nPhi.Top = y; nPhi.Width = 200;
                dlg.Controls.Add(new Label { Text = "Phí VC:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(nPhi); y += 28;

                NumericUpDown nVAT = new NumericUpDown();
                nVAT.Left = 120; nVAT.Top = y; nVAT.Width = 200;
                nVAT.Maximum = 100; nVAT.DecimalPlaces = 1; nVAT.Value = m_ThongTin.VAT;
                dlg.Controls.Add(new Label { Text = "VAT %:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(nVAT); y += 28;

                TextBox tGC = new TextBox();
                tGC.Left = 120; tGC.Top = y; tGC.Width = 200; tGC.Text = m_ThongTin.GhiChu;
                dlg.Controls.Add(new Label { Text = "Ghi chú:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(tGC); y += 28;

                TextBox tHD = new TextBox();
                tHD.Left = 120; tHD.Top = y; tHD.Width = 200; tHD.Text = m_ThongTin.SoHoaDonVAT;
                dlg.Controls.Add(new Label { Text = "Số HĐ VAT:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(tHD); y += 28;

                ComboBox cTT = new ComboBox();
                cTT.Left = 120; cTT.Top = y; cTT.Width = 200;
                cTT.DropDownStyle = ComboBoxStyle.DropDownList;
                cTT.Items.AddRange(new object[] { "Nháp", "Đã lưu", "Hủy" });
                int ix = cTT.Items.IndexOf(m_ThongTin.TrangThai);
                cTT.SelectedIndex = ix >= 0 ? ix : 0;
                dlg.Controls.Add(new Label { Text = "Trạng thái:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(cTT); y += 28;

                ComboBox cHTTT = new ComboBox();
                cHTTT.Left = 120; cHTTT.Top = y; cHTTT.Width = 200;
                cHTTT.DropDownStyle = ComboBoxStyle.DropDownList;
                cHTTT.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Công nợ" });
                int ih = cHTTT.Items.IndexOf(m_ThongTin.HinhThucThanhToan);
                cHTTT.SelectedIndex = ih >= 0 ? ih : 0;
                dlg.Controls.Add(new Label { Text = "Hình thức TT:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(cHTTT); y += 28;

                TextBox tNV = new TextBox();
                tNV.Left = 120; tNV.Top = y; tNV.Width = 200; tNV.Text = m_ThongTin.NhanVien;
                dlg.Controls.Add(new Label { Text = "NV lập phiếu:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(tNV); y += 28;

                TextBox tNT = new TextBox();
                tNT.Left = 120; tNT.Top = y; tNT.Width = 200; tNT.Text = m_ThongTin.NguoiTao;
                dlg.Controls.Add(new Label { Text = "Người tạo:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(tNT); y += 36;

                // Yêu cầu 4: giảm giá và dịch vụ phụ phát sinh
                ucDichVuGiamGia ucDV = new ucDichVuGiamGia();
                ucDV.Left = 12; ucDV.Top = y;
                ucDV.NapTu(m_ThongTin);
                dlg.Controls.Add(ucDV); y += ucDV.Height + 8;

                Button ok = new Button();
                ok.Text = "OK"; ok.DialogResult = DialogResult.OK; ok.Left = 160; ok.Top = y; ok.Width = 75;
                Button cancel = new Button();
                cancel.Text = "Bỏ qua"; cancel.DialogResult = DialogResult.Cancel; cancel.Left = 245; cancel.Top = y; cancel.Width = 75;
                dlg.Controls.Add(ok); dlg.Controls.Add(cancel);
                dlg.AcceptButton = ok; dlg.CancelButton = cancel;
                dlg.ClientSize = new Size(360, y + 50);

                DialogResult r = dlg.ShowDialog(this);
                if (r == DialogResult.Cancel) return true;
                // Soạn trên bản nháp (Prototype) rồi áp bằng lệnh (Command) để có thể hoàn tác.
                ThongTinHoaDon banNhap = (ThongTinHoaDon)m_ThongTin.SaoChep();
                banNhap.LoaiGia = cGia.SelectedIndex;
                banNhap.ChietKhau = nCK.SoTien;
                banNhap.PhiVanChuyen = nPhi.SoTien;
                banNhap.VAT = nVAT.Value;
                banNhap.GhiChu = tGC.Text ?? "";
                banNhap.SoHoaDonVAT = tHD.Text ?? "";
                banNhap.TrangThai = cTT.Text ?? "Nháp";
                banNhap.HinhThucThanhToan = cHTTT.Text ?? "Tiền mặt";
                banNhap.NhanVien = tNV.Text ?? "";
                banNhap.NguoiTao = tNT.Text ?? "";
                ucDV.GhiVao(banNhap);
                m_LichSuLenh.ThucHien(new LenhCapNhatThongTin(m_ThongTin, banNhap));
                return true;
            }
        }

        /// <summary>
        /// Tổng thành tiền các dòng hàng trên lưới.
        /// REFACTOR #5 Replace Temp with Query – trước đây là biến tạm tienHang cộng dồn ngay trong hàm tính tổng.
        /// </summary>
        long TinhTienHang()
        {
            BindingSource bs = dgvDanhsachSP.DataSource as BindingSource;
            if (bs == null)
                return (long)numTongTien.Value;

            long tien = 0;
            foreach (object item in bs)
            {
                DataRowView rv = item as DataRowView;
                if (rv == null) continue;
                // REFACTOR #54 Replace Exception with Test – kiểm tra ô rỗng (DBNull)
                // thay vì để Convert ném lỗi rồi nuốt bằng catch rỗng.
                tien += DuLieu.LaySoLon(rv["THANH_TIEN"]);
            }
            return tien;
        }

        /// <summary>Tính lại tổng tiền và còn nợ trên form (công thức nằm ở ThongTinHoaDon.TinhTongTien).</summary>
        void CapNhatTongTien()
        {
            try
            {
                // PATTERN #9 Decorator (nơi dùng) – tổng tiền tính qua chuỗi lớp bọc (chiết khấu, giảm giá, phí, dịch vụ, VAT).
                numTongTien.Value = m_ThongTin.TinhTongTien(TinhTienHang(), KhachDangChon());
                numConNo.Value = numTongTien.Value - numDaTra.Value;
            }
            catch (ArgumentOutOfRangeException) { }   // vượt giới hạn của ô số: giữ giá trị cũ
        }


     }
}