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
    public partial class frmBanSi: Form
    {
        SanPhamController ctrlSanPham = new SanPhamController();
        KhachHangController ctrlKhachHang = new KhachHangController();
        MaSanPhamController ctrlMaSanPham = new MaSanPhamController();
        PhieuBanController ctrlPhieuBan = new PhieuBanController();
        ChiTietPhieuBanController ctrlChiTiet = new ChiTietPhieuBanController();
        XuatKhoController ctrlXuatKho = new XuatKhoController();
        IList<MaSanPham> deleted = new List<MaSanPham>();


        Controll status = Controll.Normal;

        // PM-20: luu gia tri bo sung — KHONG doi layout form (nhap qua Dialog khi Luu)
        private long m_ChietKhau = 0, m_PhiVC = 0, m_HanMucNo = 0;
        private decimal m_VAT = 0;
        private string m_GhiChu = "", m_SoHDVAT = "", m_TrangThai = "Nháp", m_DieuKhoan = "Trả ngay";
        private int m_LoaiGia = 1; // 0=nhap, 1=si, 2=le


        public frmBanSi()
        {
            InitializeComponent();
            status = Controll.AddNew;
            this.dgvDanhsachSP.DataError -= this.dgvDanhsachSP_DataError;
            this.dgvDanhsachSP.DataError += this.dgvDanhsachSP_DataError;
        }


        public frmBanSi(PhieuBanController ctrlPB)
            : this()
        {
            this.ctrlPhieuBan = ctrlPB;
            status = Controll.Normal;
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            this.dgvDanhsachSP.DataError -= this.dgvDanhsachSP_DataError;
            this.dgvDanhsachSP.DataError += this.dgvDanhsachSP_DataError;
            ThemCotLuoiPM20(); // Ten SP + Chiet khau (chi luoi, khong doi layout form)

            // Cấu hình xuất kho: FIFO thì hệ thống tự phân lô, chỉ định thì cho chọn lô.
            cmbMaSanPham.Enabled = (CauHinhKho.PhuongPhapXuatKho == PhuongPhapXuatKho.ChiDinh);

            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);

            cmbSanPham.SelectedIndexChanged += new EventHandler(cmbSanPham_SelectedIndexChanged);

            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);

            
            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator,cmbKhachHang, txtMaPhieu, dtNgayLapPhieu, numTongTien, numDaTra, numConNo);
            bindingNavigator.BindingSource.CurrentChanged += new EventHandler(BindingSource_CurrentChanged);
            
            ctrlChiTiet.HienThiChiTiet(dgvDanhsachSP, txtMaPhieu.Text);
            DamBaoComboMaSanPhamHopLe();


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
                DamBaoComboMaSanPhamHopLe();
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
            else if (m_LoaiGia == 2) numDonGia.Value = masp.SanPham.GiaBanLe;
            else numDonGia.Value = masp.SanPham.GiaBanSi;
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
                MessageBox.Show("Vui lòng chọn Sản phẩm !", "Phieu Ban Si", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (chiDinh && cmbMaSanPham.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Mã sản phẩm !", "Phieu Ban Si", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Vui lòng nhập Số lượng !", "Phieu Ban Si", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numDonGia.Value * numSoLuong.Value != numThanhTien.Value)
            {
                MessageBox.Show("Thành tiền sai!", "Phieu Ban Si", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    ThemDongChiTiet(chiDinh);
                }
                catch (TonKhoException ex)
                {
                    MessageBox.Show(ex.Message, "Phieu Ban Si", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            try
            {
                if (dgvDanhsachSP.Columns.Contains("colTenSanPham") && dgvDanhsachSP.Rows.Count > 0)
                {
                    DataGridViewRow last = dgvDanhsachSP.Rows[dgvDanhsachSP.Rows.Count - 1];
                    if (last != null && !last.IsNewRow)
                        last.Cells["colTenSanPham"].Value = cmbSanPham.Text;
                }
            }
            catch { }
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
            // PM-20: hop thoai thong tin them — form chinh khong doi design
            NhapThongTinThemPM20();
            if (MessageBox.Show("Xác nhận lưu phiếu bán sỉ?", "Bán sỉ",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            ApDungTongTienPM20();
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
                MessageBox.Show(ex.Message, "Phieu Ban Si", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(ex.Message, "Phieu Ban Si", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            ApDungTongTienPM20();
            row["TONG_TIEN"] = numTongTien.Value;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;
            TrySet(row, "CHIET_KHAU", m_ChietKhau);
            TrySet(row, "PHI_VAN_CHUYEN", m_PhiVC);
            TrySet(row, "VAT", m_VAT);
            TrySet(row, "GHI_CHU", m_GhiChu ?? "");
            TrySet(row, "TRANG_THAI", m_TrangThai ?? "Nháp");
            TrySet(row, "SO_HD_VAT", m_SoHDVAT ?? "");
            TrySet(row, "DIEU_KHOAN_TT", m_DieuKhoan ?? "");
            TrySet(row, "HAN_MUC_NO", m_HanMucNo);
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
            DamBaoComboMaSanPhamHopLe();
            this.Allow(true);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Bán sỉ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Bán sỉ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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

                 if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Bán sỉ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

        private void toolXemLai_Click(object sender, EventArgs e)
        {
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham);
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            frmDaiLy DaiLy = new frmDaiLy();
            DaiLy.ShowDialog();
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
            
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham SanPham = new frmSanPham();
            SanPham.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
        }
        


        // ========== PM-20: logic only, khong doi Designer/layout ==========
        void ThemCotLuoiPM20()
        {
            try
            {
                if (!dgvDanhsachSP.Columns.Contains("colTenSanPham"))
                {
                    DataGridViewTextBoxColumn colTen = new DataGridViewTextBoxColumn();
                    colTen.Name = "colTenSanPham";
                    colTen.HeaderText = "Tên sản phẩm";
                    colTen.ReadOnly = true;
                    colTen.Width = 140;
                    int idx = 1;
                    for (int i = 0; i < dgvDanhsachSP.Columns.Count; i++)
                        if (dgvDanhsachSP.Columns[i].Name == "colMaSanPham") { idx = i + 1; break; }
                    dgvDanhsachSP.Columns.Insert(idx, colTen);
                }
                if (!dgvDanhsachSP.Columns.Contains("colChietKhauDong"))
                {
                    DataGridViewTextBoxColumn colCK = new DataGridViewTextBoxColumn();
                    colCK.Name = "colChietKhauDong";
                    colCK.HeaderText = "Chiết khấu";
                    colCK.Width = 80;
                    dgvDanhsachSP.Columns.Add(colCK);
                }
            }
            catch { }
        }

        void NhapThongTinThemPM20()
        {
            using (Form dlg = new Form())
            {
                dlg.Text = "Thông tin thêm — Bán sỉ";
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                int y = 12;
                Label L(string tx) { return new Label { Text = tx, Left = 12, Top = y, AutoSize = true }; }
                ComboBox cGia = new ComboBox { Left = 130, Top = y - 2, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
                cGia.Items.AddRange(new object[] { "Giá nhập", "Giá sỉ", "Giá lẻ" });
                cGia.SelectedIndex = m_LoaiGia;
                dlg.Controls.Add(L("Loại giá:")); dlg.Controls.Add(cGia); y += 28;

                NumericUpDown nCK = new NumericUpDown { Left = 130, Top = y - 2, Width = 220, Maximum = 999999999, ThousandsSeparator = true, Value = m_ChietKhau };
                dlg.Controls.Add(L("Chiết khấu:")); dlg.Controls.Add(nCK); y += 28;

                NumericUpDown nPhi = new NumericUpDown { Left = 130, Top = y - 2, Width = 220, Maximum = 999999999, ThousandsSeparator = true, Value = m_PhiVC };
                dlg.Controls.Add(L("Phí VC / phụ thu:")); dlg.Controls.Add(nPhi); y += 28;

                NumericUpDown nVAT = new NumericUpDown { Left = 130, Top = y - 2, Width = 220, Maximum = 100, DecimalPlaces = 1, Value = m_VAT };
                dlg.Controls.Add(L("VAT %:")); dlg.Controls.Add(nVAT); y += 28;

                ComboBox cDK = new ComboBox { Left = 130, Top = y - 2, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
                cDK.Items.AddRange(new object[] { "Trả ngay", "Công nợ 7 ngày", "Công nợ 30 ngày" });
                int idk = cDK.Items.IndexOf(m_DieuKhoan); cDK.SelectedIndex = idk >= 0 ? idk : 0;
                dlg.Controls.Add(L("ĐK thanh toán:")); dlg.Controls.Add(cDK); y += 28;

                NumericUpDown nHM = new NumericUpDown { Left = 130, Top = y - 2, Width = 220, Maximum = 999999999, ThousandsSeparator = true, Value = m_HanMucNo };
                dlg.Controls.Add(L("Hạn mức nợ:")); dlg.Controls.Add(nHM); y += 28;

                TextBox tGC = new TextBox { Left = 130, Top = y - 2, Width = 220, Text = m_GhiChu };
                dlg.Controls.Add(L("Ghi chú GH/TT:")); dlg.Controls.Add(tGC); y += 28;

                TextBox tHD = new TextBox { Left = 130, Top = y - 2, Width = 220, Text = m_SoHDVAT };
                dlg.Controls.Add(L("Số HĐ VAT:")); dlg.Controls.Add(tHD); y += 28;

                ComboBox cTT = new ComboBox { Left = 130, Top = y - 2, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
                cTT.Items.AddRange(new object[] { "Nháp", "Đã lưu", "Hủy" });
                int ix = cTT.Items.IndexOf(m_TrangThai); cTT.SelectedIndex = ix >= 0 ? ix : 0;
                dlg.Controls.Add(L("Trạng thái:")); dlg.Controls.Add(cTT); y += 36;

                Button ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 180, Top = y, Width = 80 };
                Button bo = new Button { Text = "Bỏ qua", DialogResult = DialogResult.Cancel, Left = 270, Top = y, Width = 80 };
                dlg.Controls.Add(ok); dlg.Controls.Add(bo);
                dlg.AcceptButton = ok; dlg.CancelButton = bo;
                dlg.ClientSize = new Size(400, y + 50);

                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                m_LoaiGia = cGia.SelectedIndex;
                m_ChietKhau = (long)nCK.Value;
                m_PhiVC = (long)nPhi.Value;
                m_VAT = nVAT.Value;
                m_DieuKhoan = cDK.Text ?? "Trả ngay";
                m_HanMucNo = (long)nHM.Value;
                m_GhiChu = tGC.Text ?? "";
                m_SoHDVAT = tHD.Text ?? "";
                m_TrangThai = cTT.Text ?? "Nháp";
            }
        }

        void ApDungTongTienPM20()
        {
            try
            {
                long tienHang = 0;
                BindingSource bs = dgvDanhsachSP.DataSource as BindingSource;
                if (bs != null)
                {
                    foreach (object item in bs)
                    {
                        System.Data.DataRowView rv = item as System.Data.DataRowView;
                        if (rv == null) continue;
                        try { tienHang += Convert.ToInt64(rv["THANH_TIEN"]); } catch { }
                    }
                }
                else
                {
                    try { tienHang = (long)numTongTien.Value; } catch { }
                }
                decimal baseAmt = tienHang - m_ChietKhau + m_PhiVC;
                if (baseAmt < 0) baseAmt = 0;
                numTongTien.Value = (long)Math.Round(baseAmt * (1 + m_VAT / 100m), 0);
                try { numConNo.Value = numTongTien.Value - numDaTra.Value; } catch { }
            }
            catch { }
        }

        void DamBaoComboMaSanPhamHopLe()
        {
            try
            {
                if (colMaSanPham == null) return;
                try { ctrlMaSanPham.HienThiDataGridViewComboBox(colMaSanPham); } catch { }
                System.Data.DataTable srcTbl = colMaSanPham.DataSource as System.Data.DataTable;
                if (srcTbl == null) return;
                System.Collections.Generic.List<string> coSan = new System.Collections.Generic.List<string>();
                foreach (System.Data.DataRow r in srcTbl.Rows)
                {
                    try
                    {
                        string sid = Convert.ToString(r["ID"]);
                        if (sid != null && sid.Length > 0 && !coSan.Contains(sid)) coSan.Add(sid);
                    }
                    catch { }
                }
                System.Collections.Generic.List<string> canThem = new System.Collections.Generic.List<string>();
                BindingSource bs = dgvDanhsachSP.DataSource as BindingSource;
                if (bs != null)
                {
                    foreach (object item in bs)
                    {
                        System.Data.DataRowView rv = item as System.Data.DataRowView;
                        if (rv == null) continue;
                        try
                        {
                            string id = Convert.ToString(rv["ID_MA_SAN_PHAM"]);
                            if (id != null && id.Length > 0 && !coSan.Contains(id) && !canThem.Contains(id))
                                canThem.Add(id);
                        }
                        catch { }
                    }
                }
                for (int i = 0; i < canThem.Count; i++)
                {
                    try
                    {
                        System.Data.DataRow nr = srcTbl.NewRow();
                        if (srcTbl.Columns.Contains("ID")) nr["ID"] = canThem[i];
                        srcTbl.Rows.Add(nr);
                    }
                    catch { }
                }
                if (canThem.Count > 0)
                {
                    colMaSanPham.DataSource = null;
                    colMaSanPham.DisplayMember = "ID";
                    colMaSanPham.ValueMember = "ID";
                    colMaSanPham.DataSource = srcTbl;
                }
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