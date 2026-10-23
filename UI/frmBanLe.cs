using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
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
        // PM-19: gia tri bo sung (khong doi design form - nhap qua hop thoai khi Luu)
        private long m_ChietKhau = 0, m_PhiVC = 0;
        private decimal m_VAT = 0;
        private string m_GhiChu = "", m_SoHDVAT = "", m_TrangThai = "Nháp", m_HinhThucTT = "Tiền mặt", m_NhanVien = "", m_NguoiTao = "";
        private int m_LoaiGia = 2; // 0 nhap, 1 si, 2 le


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

            // Cấu hình xuất kho: FIFO thì hệ thống tự phân lô, chỉ định thì cho chọn lô.
            cmbMaSanPham.Enabled = (CauHinhKho.PhuongPhapXuatKho == PhuongPhapXuatKho.ChiDinh);

            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            cmbSanPham.SelectedIndexChanged += new EventHandler(cmbSanPham_SelectedIndexChanged);

            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);

            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator,cmbKhachHang, txtMaPhieu, dtNgayLapPhieu, numTongTien, numDaTra, numConNo);

            bindingNavigator.BindingSource.CurrentChanged -= new EventHandler(BindingSource_CurrentChanged);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);
            
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);


            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
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
            if (m_LoaiGia == 0) numDonGia.Value = masp.SanPham.DonGiaNhap;
            else if (m_LoaiGia == 1) numDonGia.Value = masp.SanPham.GiaBanSi;
            else numDonGia.Value = masp.SanPham.GiaBanLe;
            txtGiaNhap.Text = masp.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = masp.SanPham.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = masp.SanPham.GiaBanLe.ToString("#,###0");
            txtGiaBQGQ.Text = masp.SanPham.DonGiaNhap.ToString("#,###0");
            txtGiaXuat.Text = ctrlXuatKho.TinhGiaXuat(masp).ToString("#,###0");

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool chiDinh = (CauHinhKho.PhuongPhapXuatKho == PhuongPhapXuatKho.ChiDinh);

            if (cmbSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Sản phẩm !", "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (chiDinh && cmbMaSanPham.SelectedValue == null)
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
                    ThemDongChiTiet(chiDinh);
                }
                catch (TonKhoException ex)
                {
                    MessageBox.Show(ex.Message, "Phieu Ban Le", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // Thêm hàng vào phiếu: FIFO thì tách theo từng lô (hạn dùng gần nhất trước), chỉ định thì đúng lô đã chọn.
        // Số lượng các dòng đã có trong phiếu (chưa lưu) được tính vào để không bán vượt tồn.
        void ThemDongChiTiet(bool chiDinh)
        {
            int soLuong = (int)numSoLuong.Value;
            IDictionary<String, int> daChon = ctrlChiTiet.SoLuongDangChon();

            IList<PhanBoLo> phanBo;
            if (chiDinh)
                phanBo = ctrlXuatKho.PhanLoChiDinh(cmbMaSanPham.SelectedValue.ToString(), soLuong, daChon);
            else
                phanBo = ctrlXuatKho.PhanLoFIFO(cmbSanPham.SelectedValue.ToString(), soLuong, daChon);

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

        private void numDonGia_ValueChanged(object sender, EventArgs e)
        {
            numThanhTien.Value = numDonGia.Value * numSoLuong.Value;
        }

        private void numTongTien_ValueChanged(object sender, EventArgs e)
        {
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            // PM-19: hop thoai thong tin them (KHONG doi layout form chinh)
            if (!NhapThongTinThemPM19()) return;
            if (MessageBox.Show("Xác nhận lưu phiếu bán lẻ?", "Bán lẻ",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            ApDungTongTienPM19();
            if (Luu())
            {
                m_TrangThai = "Đã lưu";
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
                    return ThemMoi();
                }
                CapNhat();
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
                CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(masp.Id, masp.SoLuong);
            }
            deleted.Clear();

            ctrlChiTiet.Save();

            ctrlPhieuBan.Update();
        }

        bool ThemMoi()
        {
            PhieuBanController ctrl = new PhieuBanController();

            if (ctrl.LayPhieuBan(txtMaPhieu.Text) != null)
            {
                MessageBox.Show("Mã Phiếu bán này đã tồn tại !", "Phieu Nhap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            DataRow row = ctrlPhieuBan.NewRow();
            row["ID"] = txtMaPhieu.Text;
            row["ID_KHACH_HANG"] = cmbKhachHang.SelectedValue;
            row["NGAY_BAN"] = dtNgayLapPhieu.Value.Date;
            ApDungTongTienPM19();
            row["TONG_TIEN"] = numTongTien.Value;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;
            // Chi ghi cot mo rong NEU CSDL da co (khong bat buoc ALTER)
            TrySet(row, "CHIET_KHAU", m_ChietKhau);
            TrySet(row, "PHI_VAN_CHUYEN", m_PhiVC);
            TrySet(row, "VAT", m_VAT);
            TrySet(row, "GHI_CHU", m_GhiChu ?? "");
            TrySet(row, "TRANG_THAI", m_TrangThai ?? "Nháp");
            TrySet(row, "SO_HD_VAT", m_SoHDVAT ?? "");
            TrySet(row, "HINH_THUC_TT", m_HinhThucTT ?? "Tiền mặt");
            TrySet(row, "NHAN_VIEN", m_NhanVien ?? "");
            TrySet(row, "NGUOI_TAO", m_NguoiTao ?? "");
            ctrlPhieuBan.Add(row);

            if (ThamSo.LaSoNguyen(txtMaPhieu.Text))
            {
                long so = Convert.ToInt64(txtMaPhieu.Text);
                if (so >= ThamSo.LayMaPhieuBan())
                {
                    ThamSo.GanMaPhieuBan(so + 1);
                }
            }

            ctrlPhieuBan.Save();

            ctrlChiTiet.Save();
            return true;
        }

        private void toolLuu_Them_Click(object sender, EventArgs e)
        {
            ctrlPhieuBan = new PhieuBanController();
            status = Controll.AddNew;
            txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
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

        private void toolChinhSua_Click(object sender, EventArgs e)
        {
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
            DataRowView view =  (DataRowView)bindingNavigator.BindingSource.Current;
            if (view != null)
            {
                if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Bán lẻ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();
                    IList<ChiTietPhieuBan> ds = ctrl.ChiTietPhieuBan(view["ID"].ToString());
                    foreach (ChiTietPhieuBan ct in ds)
                    {
                        CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
                    }
                    bindingNavigator.BindingSource.RemoveCurrent();
                    ctrlPhieuBan.Save();
                }
            }
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            frmKhachHang KhachHang = new frmKhachHang();
            KhachHang.ShowDialog();
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, false);
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
                cGia.SelectedIndex = m_LoaiGia;
                dlg.Controls.Add(new Label { Text = "Loại giá:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(cGia); y += 28;

                NumericUpDown nCK = new NumericUpDown();
                nCK.Left = 120; nCK.Top = y; nCK.Width = 200;
                nCK.Maximum = 999999999; nCK.ThousandsSeparator = true; nCK.Value = m_ChietKhau;
                dlg.Controls.Add(new Label { Text = "Chiết khấu:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(nCK); y += 28;

                NumericUpDown nPhi = new NumericUpDown();
                nPhi.Left = 120; nPhi.Top = y; nPhi.Width = 200;
                nPhi.Maximum = 999999999; nPhi.ThousandsSeparator = true; nPhi.Value = m_PhiVC;
                dlg.Controls.Add(new Label { Text = "Phí VC:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(nPhi); y += 28;

                NumericUpDown nVAT = new NumericUpDown();
                nVAT.Left = 120; nVAT.Top = y; nVAT.Width = 200;
                nVAT.Maximum = 100; nVAT.DecimalPlaces = 1; nVAT.Value = m_VAT;
                dlg.Controls.Add(new Label { Text = "VAT %:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(nVAT); y += 28;

                TextBox tGC = new TextBox();
                tGC.Left = 120; tGC.Top = y; tGC.Width = 200; tGC.Text = m_GhiChu;
                dlg.Controls.Add(new Label { Text = "Ghi chú:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(tGC); y += 28;

                TextBox tHD = new TextBox();
                tHD.Left = 120; tHD.Top = y; tHD.Width = 200; tHD.Text = m_SoHDVAT;
                dlg.Controls.Add(new Label { Text = "Số HĐ VAT:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(tHD); y += 28;

                ComboBox cTT = new ComboBox();
                cTT.Left = 120; cTT.Top = y; cTT.Width = 200;
                cTT.DropDownStyle = ComboBoxStyle.DropDownList;
                cTT.Items.AddRange(new object[] { "Nháp", "Đã lưu", "Hủy" });
                int ix = cTT.Items.IndexOf(m_TrangThai);
                cTT.SelectedIndex = ix >= 0 ? ix : 0;
                dlg.Controls.Add(new Label { Text = "Trạng thái:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(cTT); y += 28;

                ComboBox cHTTT = new ComboBox();
                cHTTT.Left = 120; cHTTT.Top = y; cHTTT.Width = 200;
                cHTTT.DropDownStyle = ComboBoxStyle.DropDownList;
                cHTTT.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Công nợ" });
                int ih = cHTTT.Items.IndexOf(m_HinhThucTT);
                cHTTT.SelectedIndex = ih >= 0 ? ih : 0;
                dlg.Controls.Add(new Label { Text = "Hình thức TT:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(cHTTT); y += 28;

                TextBox tNV = new TextBox();
                tNV.Left = 120; tNV.Top = y; tNV.Width = 200; tNV.Text = m_NhanVien;
                dlg.Controls.Add(new Label { Text = "NV lập phiếu:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(tNV); y += 28;

                TextBox tNT = new TextBox();
                tNT.Left = 120; tNT.Top = y; tNT.Width = 200; tNT.Text = m_NguoiTao;
                dlg.Controls.Add(new Label { Text = "Người tạo:", Left = 12, Top = y + 4, AutoSize = true });
                dlg.Controls.Add(tNT); y += 36;

                Button ok = new Button();
                ok.Text = "OK"; ok.DialogResult = DialogResult.OK; ok.Left = 160; ok.Top = y; ok.Width = 75;
                Button cancel = new Button();
                cancel.Text = "Bỏ qua"; cancel.DialogResult = DialogResult.Cancel; cancel.Left = 245; cancel.Top = y; cancel.Width = 75;
                dlg.Controls.Add(ok); dlg.Controls.Add(cancel);
                dlg.AcceptButton = ok; dlg.CancelButton = cancel;
                dlg.ClientSize = new Size(360, y + 50);

                DialogResult r = dlg.ShowDialog(this);
                if (r == DialogResult.Cancel) return true;
                m_LoaiGia = cGia.SelectedIndex;
                m_ChietKhau = (long)nCK.Value;
                m_PhiVC = (long)nPhi.Value;
                m_VAT = nVAT.Value;
                m_GhiChu = tGC.Text ?? "";
                m_SoHDVAT = tHD.Text ?? "";
                m_TrangThai = cTT.Text ?? "Nháp";
                m_HinhThucTT = cHTTT.Text ?? "Tiền mặt";
                m_NhanVien = tNV.Text ?? "";
                m_NguoiTao = tNT.Text ?? "";
                return true;
            }
        }

        void ApDungTongTienPM19()
        {
            try
            {
                long tienHang = 0;
                if (dgvDanhsachSP.DataSource is BindingSource)
                {
                    foreach (DataRowView rv in (BindingSource)dgvDanhsachSP.DataSource)
                    {
                        try { tienHang += Convert.ToInt64(rv["THANH_TIEN"]); } catch { }
                    }
                }
                else
                {
                    try { tienHang = (long)numTongTien.Value; } catch { }
                }
                decimal baseAmt = tienHang - m_ChietKhau + m_PhiVC;
                if (baseAmt < 0) baseAmt = 0;
                long tong = (long)Math.Round(baseAmt * (1 + m_VAT / 100m), 0);
                numTongTien.Value = tong;
                try { numConNo.Value = numTongTien.Value - numDaTra.Value; } catch { }
            }
            catch { }
        }

        static void TrySet(DataRow row, string col, object val)
        {
            try
            {
                if (row.Table.Columns.Contains(col))
                    row[col] = val ?? DBNull.Value;
            }
            catch { }
        }

     }
}