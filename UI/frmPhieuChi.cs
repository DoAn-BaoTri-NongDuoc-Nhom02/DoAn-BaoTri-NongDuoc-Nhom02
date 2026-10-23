using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmPhieuChi : Form
    {
        LyDoChiController ctrlLyDo = new LyDoChiController();
        PhieuChiController ctrl = new PhieuChiController();

        StatusStrip statusStrip;
        ToolStripStatusLabel lblCheDo;
        ToolStripStatusLabel lblTong;
        ToolStripStatusLabel lblThongBao;
        ToolStripTextBox txtTimNhanh;
        bool cheDoSua;
        bool tamDungLoc;

        public frmPhieuChi()
        {
            InitializeComponent();
        }

        private void frmThanhToan_Load(object sender, EventArgs e)
        {
            try { UiTheme.Apply(this); } catch { }
            try { GiaoDienChung.BatDau(); } catch { }
            this.Text = "Phiếu chi";
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            // Dong bo cac form khac: khong ep mau xanh toan form
            this.BackColor = Color.FromArgb(192, 255, 192); // giong Nhap hang
            try { bindingNavigator.CountItemFormat = "/ {0} bản ghi"; } catch { }
            ApDungPM26NhanVaTien();
            DongBoMauFormVoiDuAn();
            TaoThanhTrangThai();
            SapXepNut();
            ToMauLuoi();
            TaiLaiTatCa();
            DatCheDoSua(false);
            DongBoMauFormVoiDuAn();
            ToMauLuoi();
        }



        void DongBoMauFormVoiDuAn()
        {
            // Chuan form Nhap hang: xanh nhat 192,255,192
            Color xanhNhat = Color.FromArgb(192, 255, 192);
            try
            {
                this.BackColor = xanhNhat;
                foreach (Control c in this.Controls)
                {
                    if (c is GroupBox || c is Panel)
                        c.BackColor = xanhNhat;
                    if (c is StatusStrip)
                        c.BackColor = xanhNhat;
                }
                if (bindingNavigator != null)
                {
                    bindingNavigator.BackColor = xanhNhat;
                    bindingNavigator.RenderMode = ToolStripRenderMode.System;
                }
            }
            catch { }
        }

        void ApDungPM26NhanVaTien()
        {
            try
            {
                foreach (Control c in this.Controls)
                    DoiNhanNgay(c);
            }
            catch { }
            try { txtMaPhieu.ReadOnly = true; } catch { }
            try
            {
                numTongTien.ThousandsSeparator = true;
                numTongTien.DecimalPlaces = 0;
                numTongTien.Maximum = 999999999999;
                numTongTien.Minimum = 0;
            }
            catch { }
            try
            {
                txtGhiChu.Multiline = true;
                if (txtGhiChu.Height < 40) txtGhiChu.Height = 40;
                txtGhiChu.ScrollBars = ScrollBars.Vertical;
            }
            catch { }
        }

        void DoiNhanNgay(Control parent)
        {
            if (parent == null) return;
            foreach (Control c in parent.Controls)
            {
                if (c is Label)
                {
                    string tx = c.Text ?? "";
                    if (tx.IndexOf("thanh toan", StringComparison.OrdinalIgnoreCase) >= 0
                        || tx.IndexOf("thanh toán", StringComparison.OrdinalIgnoreCase) >= 0)
                        c.Text = "Ngày chi";
                }
                if (c.HasChildren) DoiNhanNgay(c);
            }
        }

        bool CoQuyenSua
        {
            get
            {
                try { return true; }
                catch { return true; }
            }
        }

        void ThemCotNguoiLap()
        {
            try
            {
                if (dataGridView.Columns.Contains("colNguoiLap")) return;
                foreach (DataGridViewColumn c in dataGridView.Columns)
                {
                    string n = ((c.Name ?? "") + (c.DataPropertyName ?? "")).ToUpper();
                    if (n.Contains("NGUOI")) return;
                }
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.Name = "colNguoiLap";
                col.HeaderText = "Người lập phiếu";
                col.DataPropertyName = "NGUOI_LAP";
                col.Width = 110;
                col.ReadOnly = true;
                dataGridView.Columns.Add(col);
            }
            catch { }
        }

        private void TaoThanhTrangThai()
        {
            if (statusStrip != null) return;
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(192, 255, 192);
            statusStrip.SizingGrip = false;
            lblCheDo = new ToolStripStatusLabel("Chế độ xem");
            lblCheDo.BorderSides = ToolStripStatusLabelBorderSides.Right;
            lblCheDo.AutoSize = false;
            lblCheDo.Width = 130;
            lblTong = new ToolStripStatusLabel("Tổng: 0");
            lblTong.BorderSides = ToolStripStatusLabelBorderSides.Right;
            lblTong.AutoSize = false;
            lblTong.Width = 150;
            lblThongBao = new ToolStripStatusLabel("Sẵn sàng.");
            lblThongBao.Spring = true;
            lblThongBao.TextAlign = ContentAlignment.MiddleLeft;
            statusStrip.Items.Add(lblCheDo);
            statusStrip.Items.Add(lblTong);
            statusStrip.Items.Add(lblThongBao);
            this.Controls.Add(statusStrip);
        }


        private static Image VeIcon(string loai)
        {
            return GiaoDienChung.Icon(loai);
        }

        private ToolStripButton Nut(string name, string text, Image img, EventHandler click)
        {
            ToolStripButton co = bindingNavigator.Items[name] as ToolStripButton;
            ToolStripButton b = co ?? new ToolStripButton();
            b.Name = name;
            b.Text = text;
            b.Image = img;
            b.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            b.TextImageRelation = TextImageRelation.ImageAboveText;
            b.ImageScaling = ToolStripItemImageScaling.None;
            b.Click -= click;
            b.Click += click;
            return b;
        }

        private void SapXepNut()
        {
            // Giong Nhap hang
            bindingNavigator.BackColor = Color.FromArgb(192, 255, 192);
            bindingNavigator.RenderMode = ToolStripRenderMode.System;
            bindingNavigator.GripStyle = ToolStripGripStyle.Hidden;
            bindingNavigator.ImageScalingSize = new Size(24, 24);

            if (toolAdd != null) { toolAdd.ImageScaling = ToolStripItemImageScaling.None; toolAdd.TextImageRelation = TextImageRelation.ImageAboveText; }
            if (toolDelete != null) { toolDelete.ImageScaling = ToolStripItemImageScaling.None; toolDelete.TextImageRelation = TextImageRelation.ImageAboveText; }
            if (toolSave != null) { toolSave.ImageScaling = ToolStripItemImageScaling.None; toolSave.TextImageRelation = TextImageRelation.ImageAboveText; }
            if (toolIn != null) { toolIn.ImageScaling = ToolStripItemImageScaling.None; toolIn.TextImageRelation = TextImageRelation.ImageAboveText; }
            if (toolTimKiem != null) { toolTimKiem.ImageScaling = ToolStripItemImageScaling.None; toolTimKiem.TextImageRelation = TextImageRelation.ImageAboveText; }

            ToolStripButton sua = Nut("toolSua", "Chỉnh sửa", VeIcon("sua"), toolSua_Click);
            ToolStripButton huy = Nut("toolHuy", "Hủy", VeIcon("huy"), toolHuy_Click);
            ToolStripButton nhatKy = Nut("toolNhatKy", "Nhật ký", VeIcon("nhatky"), toolNhatKy_Click);
            ToolStripButton thoat = Nut("toolThoat", "Thoát", VeIcon("thoat"), toolThoat_Click);

            foreach (ToolStripItem it in new ToolStripItem[] { sua, huy, nhatKy, thoat })
                if (bindingNavigator.Items.Contains(it)) bindingNavigator.Items.Remove(it);

            int idx = bindingNavigator.Items.IndexOf(toolSave);
            if (idx < 0) idx = bindingNavigator.Items.Count - 1;
            bindingNavigator.Items.Insert(idx + 1, sua);
            bindingNavigator.Items.Insert(idx + 2, huy);
            bindingNavigator.Items.Insert(idx + 3, nhatKy);
            bindingNavigator.Items.Insert(idx + 4, thoat);

            if (bindingNavigator.Items["txtTimNhanh"] == null)
            {
                bindingNavigator.Items.Add(new ToolStripSeparator());
                txtTimNhanh = new ToolStripTextBox();
                txtTimNhanh.Name = "txtTimNhanh";
                txtTimNhanh.AutoSize = false;
                txtTimNhanh.Width = 220;
                txtTimNhanh.Text = "Tìm theo tất cả các cột...";
                txtTimNhanh.ForeColor = Color.Gray;
                txtTimNhanh.Enter += delegate
                {
                    if (txtTimNhanh.ForeColor == Color.Gray) { txtTimNhanh.Text = ""; txtTimNhanh.ForeColor = Color.Black; }
                };
                txtTimNhanh.Leave += delegate
                {
                    if (txtTimNhanh.Text.Trim().Length == 0)
                    {
                        txtTimNhanh.Text = "Tìm theo tất cả các cột...";
                        txtTimNhanh.ForeColor = Color.Gray;
                    }
                };
                txtTimNhanh.TextChanged += delegate { LocNhanh(); };
                ToolStripButton xoaLoc = Nut("toolXoaLoc", "Xóa lọc", null, toolXoaLoc_Click);
                xoaLoc.DisplayStyle = ToolStripItemDisplayStyle.Text;
                xoaLoc.AutoSize = true;
                xoaLoc.Margin = new Padding(8, 0, 4, 0);
                bindingNavigator.Items.Add(txtTimNhanh);
                bindingNavigator.Items.Add(xoaLoc);
            }
        }

        private void ToMauLuoi()
        {
            // Giong Nhap hang: header mac dinh, khong xanh dam
            dataGridView.EnableHeadersVisualStyles = true;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = dataGridView.Font;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 255, 232);
            dataGridView.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            dataGridView.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.FixedSingle;
            dataGridView.RowHeadersVisible = true;
        }

        private void DatTieuDeCot()
        {
            foreach (DataGridViewColumn c in dataGridView.Columns)
            {
                string k = ((c.DataPropertyName ?? "") + " " + c.Name).ToUpper();
                if (k.Contains("LY_DO")) { c.HeaderText = "Lý do chi"; continue; }
                if (k.Contains("NGAY")) { c.HeaderText = "Ngày chi"; c.DefaultCellStyle.Format = "dd/MM/yyyy"; continue; }
                if (k.Contains("TONG") || k.Contains("TIEN")) { c.HeaderText = "Số tiền"; c.DefaultCellStyle.Format = "N0"; c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; continue; }
                if (k.Contains("GHI")) { c.HeaderText = "Ghi chú"; continue; }
                if (c.DataPropertyName == "ID") c.HeaderText = "Mã phiếu";
                if (k.Contains("NGUOI") || k == "NGUOI_LAP") { c.HeaderText = "Người lập phiếu"; continue; }
            }
        }

        private void ThongBao(string s) { if (lblThongBao != null) lblThongBao.Text = s; }

        private void CapNhatTong()
        {
            int n = 0;
            decimal tongTien = 0;
            decimal tongThang = 0;
            try { if (bindingNavigator.BindingSource != null) n = bindingNavigator.BindingSource.Count; } catch { }
            try
            {
                DateTime dauThang = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                if (bindingNavigator.BindingSource != null)
                {
                    foreach (object o in bindingNavigator.BindingSource)
                    {
                        DataRowView rv = o as DataRowView;
                        if (rv == null) continue;
                        decimal st = 0;
                        try
                        {
                            if (rv.Row.Table.Columns.Contains("TONG_TIEN") && rv["TONG_TIEN"] != DBNull.Value)
                                st = Convert.ToDecimal(rv["TONG_TIEN"]);
                        }
                        catch { }
                        tongTien += st;
                        try
                        {
                            if (rv.Row.Table.Columns.Contains("NGAY_CHI") && rv["NGAY_CHI"] != DBNull.Value)
                            {
                                DateTime ng = Convert.ToDateTime(rv["NGAY_CHI"]);
                                if (ng >= dauThang) tongThang += st;
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            if (lblTong != null)
            {
                lblTong.Text = string.Format("Tổng: {0} phiếu | {1:N0} đ | Tháng này: {2:N0} đ", n, tongTien, tongThang);
                try { lblTong.Width = 320; } catch { }
            }
        }

        private void DatCheDoSua(bool sua)
        {
            cheDoSua = sua;
            if (lblCheDo != null)
            {
                lblCheDo.Text = sua ? "Chế độ SỬA" : "Chế độ xem";
                lblCheDo.ForeColor = sua ? Color.Firebrick : SystemColors.ControlText;
            }
            try
            {
                txtMaPhieu.ReadOnly = true;
                txtGhiChu.ReadOnly = !sua;
                cmbLyDoChi.Enabled = sua;
                dtNgayChi.Enabled = sua;
                numTongTien.Enabled = sua;
                numTongTien.ReadOnly = !sua;
            }
            catch { }
        }

        private void TaiLaiTatCa()
        {
            try
            {
                try
                {
                    txtMaPhieu.DataBindings.Clear();
                    cmbLyDoChi.DataBindings.Clear();
                    dtNgayChi.DataBindings.Clear();
                    numTongTien.DataBindings.Clear();
                    txtGhiChu.DataBindings.Clear();
                    dataGridView.DataSource = null;
                    bindingNavigator.BindingSource = null;
                }
                catch { }
                ctrlLyDo.HienthiAutoComboBox(cmbLyDoChi);
                ctrlLyDo.HienthiDataGridviewComboBox(colLyDoChi);
                ctrl.HienthiPhieuChi(bindingNavigator, dataGridView, cmbLyDoChi, txtMaPhieu, dtNgayChi, numTongTien, txtGhiChu);
                if (bindingNavigator.BindingSource != null) bindingNavigator.BindingSource.Filter = "";
                DatTieuDeCot();
                ThemCotNguoiLap();
                CapNhatTong();
                DatCheDoSua(false);
                ThongBao("Đã tải toàn bộ phiếu chi. Gõ ô tìm để lọc.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải phiếu chi:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private DataTable LayBang()
        {
            if (bindingNavigator.BindingSource == null) return null;
            object src = bindingNavigator.BindingSource.DataSource;
            if (src is DataTable) return (DataTable)src;
            if (src is DataView) return ((DataView)src).Table;
            return null;
        }

        private void LocNhanh()
        {
            if (tamDungLoc) return;
            if (bindingNavigator.BindingSource == null || txtTimNhanh == null) return;
            string raw = txtTimNhanh.Text ?? "";
            if (txtTimNhanh.ForeColor == Color.Gray || raw.Trim().Length == 0)
            {
                bindingNavigator.BindingSource.Filter = "";
                CapNhatTong();
                return;
            }
            DataTable t = LayBang();
            if (t == null) return;
            string s = raw.Trim().Replace("'", "''");
            System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
            foreach (DataColumn c in t.Columns)
                parts.Add("Convert([" + c.ColumnName + "], 'System.String') LIKE '%" + s + "%'");
            try
            {
                bindingNavigator.BindingSource.Filter = string.Join(" OR ", parts.ToArray());
                CapNhatTong();
                ThongBao("Đang lọc \"" + raw.Trim() + "\" — " + bindingNavigator.BindingSource.Count + " phiếu.");
            }
            catch
            {
                bindingNavigator.BindingSource.Filter = "";
            }
        }

        private void toolXoaLoc_Click(object sender, EventArgs e)
        {
            tamDungLoc = true;
            try
            {
                if (txtTimNhanh != null)
                {
                    txtTimNhanh.ForeColor = Color.Gray;
                    txtTimNhanh.Text = "Tìm theo tất cả các cột...";
                }
                if (bindingNavigator.BindingSource != null)
                {
                    bindingNavigator.BindingSource.Filter = "";
                    bindingNavigator.BindingSource.RemoveFilter();
                }
            }
            catch { }
            finally { tamDungLoc = false; }

            TaiLaiTatCa();
            ThongBao("Đã xóa lọc. Danh sách đã tải lại toàn bộ.");
        }

        private void toolAdd_Click(object sender, EventArgs e)
        {
            if (!CoQuyenSua)
            {
                MessageBox.Show("Bạn không có quyền thêm phiếu chi.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DatCheDoSua(true);
            long maphieu = ThamSo.PhieuChi;
            ThamSo.PhieuChi = maphieu + 1;
            DataRow row = ctrl.NewRow();
            row["ID"] = maphieu;
            row["NGAY_CHI"] = DateTime.Now.Date;
            row["TONG_TIEN"] = 0;
            ctrl.Add(row);
            bindingNavigator.BindingSource.MoveLast();
            CapNhatTong();
            try { txtMaPhieu.ReadOnly = true; } catch { }
            try { cmbLyDoChi.Focus(); } catch { txtMaPhieu.Focus(); }
            NhatKyPhieu.Ghi("Chi", "Thêm", maphieu.ToString(), cmbLyDoChi.Text, "Tạo phiếu chi mới");
            ThongBao("Đã thêm phiếu " + maphieu + ". Chọn lý do, số tiền rồi bấm Lưu.");
        }

        private void toolSua_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Count == 0)
            {
                MessageBox.Show("Chưa có phiếu để sửa.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DatCheDoSua(true);
            cmbLyDoChi.Focus();
            ThongBao("Đang sửa phiếu " + txtMaPhieu.Text + ". Sửa xong bấm Lưu. Bấm Hủy để bỏ.");
        }

        private void toolHuy_Click(object sender, EventArgs e)
        {
            TaiLaiTatCa();
            ThongBao("Đã hủy, tải lại danh sách.");
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn xóa phiếu chi này?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            if (!CoQuyenSua)
            {
                MessageBox.Show("Bạn không có quyền xóa phiếu chi.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Count == 0) return;
            if (MessageBox.Show("Bạn chắc chắn xóa phiếu chi này?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string ma = txtMaPhieu.Text;
            string ten = cmbLyDoChi.Text;
            bindingNavigator.BindingSource.RemoveCurrent();
            try
            {
                ctrl.Save();
                NhatKyPhieu.Ghi("Chi", "Xóa", ma, ten, "Xóa phiếu chi");
                CapNhatTong();
                ThongBao("Đã xóa phiếu chi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiLaiTatCa();
            }
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                try { txtMaPhieu.ReadOnly = true; } catch { }
            try { cmbLyDoChi.Focus(); } catch { txtMaPhieu.Focus(); }
                if (bindingNavigator.BindingSource != null) bindingNavigator.BindingSource.EndEdit();
                dataGridView.EndEdit();
                ctrl.Save();
                NhatKyPhieu.Ghi("Chi", "Lưu", txtMaPhieu.Text, cmbLyDoChi.Text,
                    "Số tiền " + numTongTien.Value.ToString("N0") + ". " + txtGhiChu.Text);
                DatCheDoSua(false);
                CapNhatTong();
                ThongBao("Đã lưu phiếu chi.");
                MessageBox.Show("Đã lưu phiếu chi.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) { e.Cancel = true; }

        private void toolIn_Click(object sender, EventArgs e)
        {
            // PM-29: canh bao + tieu de ro + PhieuIn (khong ReportViewer)
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Count == 0)
            {
                MessageBox.Show("Chưa có phiếu chi nào để in.\nHãy thêm và lưu phiếu trước.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (bindingNavigator.BindingSource.Current == null)
            {
                MessageBox.Show("Chọn một phiếu chi trên danh sách trước khi in.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string ma = (txtMaPhieu.Text ?? "").Trim();
            if (ma.Length == 0)
            {
                MessageBox.Show("Phiếu chưa có mã. Hãy lưu phiếu trước khi in.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (numTongTien.Value <= 0)
            {
                if (MessageBox.Show("Số tiền đang bằng 0. Vẫn in phiếu này?", this.Text,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }
            try
            {
                string cuaHang = "CỬA HÀNG NÔNG DƯỢC";
                try
                {
                    CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
                    if (ch != null && !string.IsNullOrEmpty(ch.TenCuaHang))
                        cuaHang = ch.TenCuaHang;
                }
                catch { }

                string lyDo = (cmbLyDoChi.Text ?? "").Trim();
                string tieuDe = "PHIẾU CHI";
                if (lyDo.Length > 0)
                    tieuDe = "PHIẾU CHI — " + lyDo.ToUpper();

                PhieuIn.Mo(tieuDe, cuaHang, new string[] {
                    "Mã phiếu|" + ma,
                    "Lý do chi|" + lyDo,
                    "Ngày chi|" + dtNgayChi.Value.ToString("dd/MM/yyyy"),
                    "Số tiền|" + numTongTien.Value.ToString("N0") + " đ",
                    "Ghi chú|" + (txtGhiChu.Text ?? "")
                }, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThemLyDoChi_Click(object sender, EventArgs e)
        {
            frmLyDoChi Chi = new frmLyDoChi();
            Chi.ShowDialog();
            ctrlLyDo.HienthiAutoComboBox(cmbLyDoChi);
        }

        private void toolTimKiem_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || LayBang() == null) TaiLaiTatCa();
            frmTimPhieuChi Tim = new frmTimPhieuChi();
            Tim.StartPosition = FormStartPosition.CenterParent;
            if (Tim.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                DateTime ngay = Tim.dtNgayChi.Value.Date;
                string tu = ngay.ToString("MM/dd/yyyy");
                string den = ngay.AddDays(1).ToString("MM/dd/yyyy");
                bool locNgay = true;
                try { if (Tim.chkTheoNgay != null) locNgay = Tim.chkTheoNgay.Checked; } catch { }
                string theoNgay = locNgay
                    ? ("NGAY_CHI >= #" + tu + "# AND NGAY_CHI < #" + den + "#")
                    : "";
                string theoLyDo = "";
                if (Tim.cmbLyDo.SelectedValue != null)
                    theoLyDo = "Convert(ID_LY_DO_CHI, 'System.String') = '" + Tim.cmbLyDo.SelectedValue.ToString().Replace("'", "''") + "'";
                else if (Tim.cmbLyDo.Text != null && Tim.cmbLyDo.Text.Trim().Length > 0)
                    theoLyDo = "Convert(ID_LY_DO_CHI, 'System.String') LIKE '%" + Tim.cmbLyDo.Text.Trim().Replace("'", "''") + "%'";

                string theoMa = "";
                try
                {
                    if (Tim.txtMaPhieu != null && Tim.txtMaPhieu.Text.Trim().Length > 0)
                        theoMa = "Convert(ID, 'System.String') LIKE '%" + Tim.txtMaPhieu.Text.Trim().Replace("'", "''") + "%'";
                }
                catch { }
                string theoNguoi = "";
                try
                {
                    if (Tim.txtNguoiLap != null && Tim.txtNguoiLap.Text.Trim().Length > 0)
                        theoNguoi = "Convert(NGUOI_LAP, 'System.String') LIKE '%" + Tim.txtNguoiLap.Text.Trim().Replace("'", "''") + "%'";
                }
                catch { }
                string theoTien = "";
                try
                {
                    decimal a, b;
                    if (Tim.txtSoTienTu != null && decimal.TryParse(Tim.txtSoTienTu.Text.Replace(",", "").Replace(".", ""), out a))
                        theoTien = "TONG_TIEN >= " + a.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    if (Tim.txtSoTienDen != null && decimal.TryParse(Tim.txtSoTienDen.Text.Replace(",", "").Replace(".", ""), out b))
                        theoTien = (theoTien.Length > 0 ? theoTien + " AND " : "")
                            + "TONG_TIEN <= " + b.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }

                System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
                if (theoNgay.Length > 0) parts.Add(theoNgay);
                if (theoLyDo.Length > 0) parts.Add(theoLyDo);
                if (theoMa.Length > 0) parts.Add(theoMa);
                if (theoNguoi.Length > 0) parts.Add(theoNguoi);
                if (theoTien.Length > 0) parts.Add(theoTien);
                string filter = parts.Count > 0 ? string.Join(" AND ", parts.ToArray()) : "";
                bindingNavigator.BindingSource.Filter = filter;
                CapNhatTong();
                if (bindingNavigator.BindingSource.Count == 0)
                {
                    bindingNavigator.BindingSource.Filter = "";
                    CapNhatTong();
                    MessageBox.Show("Không thấy phiếu chi khớp. Danh sách đã hiện lại.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else ThongBao("Tìm thấy " + bindingNavigator.BindingSource.Count + " phiếu. Bấm Xóa lọc để xem hết.");
            }
            catch (Exception ex)
            {
                try { if (bindingNavigator.BindingSource != null) bindingNavigator.BindingSource.Filter = ""; } catch { }
                MessageBox.Show("Tìm lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiLaiTatCa();
            }
        }

        private void toolNhatKy_Click(object sender, EventArgs e)
        {
            using (frmNhatKyPhieu f = new frmNhatKyPhieu("Chi", "phiếu chi",
                txtMaPhieu != null ? txtMaPhieu.Text : "",
                cmbLyDoChi != null ? cmbLyDoChi.Text : ""))
                f.ShowDialog(this);
        }

        private void toolThoat_Click(object sender, EventArgs e) { this.Close(); }
    }
}
