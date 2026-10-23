using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmThanhToan : Form
    {
        KhachHangController ctrlKH = new KhachHangController();
        PhieuThanhToanController ctrl = new PhieuThanhToanController();

        StatusStrip statusStrip;
        ToolStripStatusLabel lblCheDo;
        ToolStripStatusLabel lblTong;
        ToolStripStatusLabel lblThongBao;
        ToolStripTextBox txtTimNhanh;
        bool cheDoSua;
        bool tamDungLoc;

        public frmThanhToan()
        {
            InitializeComponent();
        }

        private void frmThanhToan_Load(object sender, EventArgs e)
        {
            UiTheme.Apply(this);
            GiaoDienChung.BatDau();
            this.Text = "Phiếu thu";
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.BackColor = Color.FromArgb(236, 248, 236);
            try { bindingNavigator.CountItemFormat = "/ {0} bản ghi"; } catch { }

            TaoThanhTrangThai();
            SapXepNut();
            ToMauLuoi();
            TaiLaiTatCa();
            DatCheDoSua(false);
            if (bindingNavigator.BindingSource != null)
                bindingNavigator.BindingSource.AddingNew += BindingSource_AddingNew;
        }

        private void TaoThanhTrangThai()
        {
            if (statusStrip != null) return;
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(220, 242, 220);
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
            bindingNavigator.BackColor = Color.FromArgb(198, 239, 206);
            bindingNavigator.RenderMode = ToolStripRenderMode.Professional;
            bindingNavigator.GripStyle = ToolStripGripStyle.Hidden;
            bindingNavigator.ImageScalingSize = new Size(28, 28);

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
                    if (txtTimNhanh.ForeColor == Color.Gray)
                    {
                        txtTimNhanh.Text = "";
                        txtTimNhanh.ForeColor = Color.Black;
                    }
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
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 125, 90);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 32;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 115, 185);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView.RowHeadersVisible = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void DatTieuDeCot()
        {
            foreach (DataGridViewColumn c in dataGridView.Columns)
            {
                string k = ((c.DataPropertyName ?? "") + " " + c.Name).ToUpper();
                if (k.Contains("ID_KHACH")) { c.HeaderText = "Khách hàng"; continue; }
                if (k.Contains("NGAY")) { c.HeaderText = "Ngày thu"; c.DefaultCellStyle.Format = "dd/MM/yyyy"; continue; }
                if (k.Contains("TONG") || k.Contains("TIEN")) { c.HeaderText = "Số tiền"; c.DefaultCellStyle.Format = "N0"; c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; continue; }
                if (k.Contains("GHI")) { c.HeaderText = "Ghi chú"; continue; }
                if (k == "ID" || k.EndsWith(" ID") || c.DataPropertyName == "ID") c.HeaderText = "Mã phiếu";
            }
        }

        private void ThongBao(string s) { if (lblThongBao != null) lblThongBao.Text = s; }

        private void CapNhatTong()
        {
            int n = 0;
            try { if (bindingNavigator.BindingSource != null) n = bindingNavigator.BindingSource.Count; } catch { }
            if (lblTong != null) lblTong.Text = "Tổng số: " + n + " phiếu thu";
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
                txtMaPhieu.ReadOnly = !sua;
                txtGhiChu.ReadOnly = !sua;
                cmbKhachHang.Enabled = sua;
                dtNgayThanhToan.Enabled = sua;
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
                    cmbKhachHang.DataBindings.Clear();
                    dtNgayThanhToan.DataBindings.Clear();
                    numTongTien.DataBindings.Clear();
                    txtGhiChu.DataBindings.Clear();
                    dataGridView.DataSource = null;
                    bindingNavigator.BindingSource = null;
                }
                catch { }

                ctrlKH.HienthiChungAutoComboBox(cmbKhachHang);
                ctrlKH.HienthiKhachHangChungDataGridviewComboBox(colKhachHang);
                ctrl.HienthiPhieuThanhToan(bindingNavigator, dataGridView, cmbKhachHang, txtMaPhieu,
                    dtNgayThanhToan, numTongTien, txtGhiChu);

                if (bindingNavigator.BindingSource != null)
                    bindingNavigator.BindingSource.Filter = "";
                DatTieuDeCot();
                CapNhatTong();
                DatCheDoSua(false);
                ThongBao("Đã tải toàn bộ phiếu thu. Gõ vào ô tìm để lọc, không cần đúng ngày.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải phiếu thu:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void BindingSource_AddingNew(object sender, System.ComponentModel.AddingNewEventArgs e) { }

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
            {
                parts.Add("Convert([" + c.ColumnName + "], 'System.String') LIKE '%" + s + "%'");
            }
            try
            {
                bindingNavigator.BindingSource.Filter = string.Join(" OR ", parts.ToArray());
                CapNhatTong();
                ThongBao("Đang lọc \"" + raw.Trim() + "\" — " + bindingNavigator.BindingSource.Count + " phiếu. Bấm Xóa lọc để xem hết.");
            }
            catch (Exception ex)
            {
                bindingNavigator.BindingSource.Filter = "";
                ThongBao("Không lọc được: " + ex.Message);
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
            DatCheDoSua(true);
            long maphieu = ThamSo.LayMaPhieuThanhToan();
            ThamSo.GanMaPhieuThanhToan(maphieu + 1);
            DataRow row = ctrl.NewRow();
            row["ID"] = maphieu;
            row["NGAY_THANH_TOAN"] = DateTime.Now.Date;
            row["TONG_TIEN"] = 0;
            ctrl.Add(row);
            bindingNavigator.BindingSource.MoveLast();
            CapNhatTong();
            txtMaPhieu.Focus();
            NhatKyPhieu.Ghi("Thu", "Thêm", maphieu.ToString(), cmbKhachHang.Text, "Tạo phiếu thu mới");
            ThongBao("Đã thêm phiếu " + maphieu + ". Nhập khách hàng, số tiền rồi bấm Lưu.");
        }

        private void toolSua_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Count == 0)
            {
                MessageBox.Show("Chưa có phiếu để sửa.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DatCheDoSua(true);
            cmbKhachHang.Focus();
            ThongBao("Đang sửa phiếu " + txtMaPhieu.Text + ". Sửa xong bấm Lưu. Bấm Hủy để bỏ.");
        }

        private void toolHuy_Click(object sender, EventArgs e)
        {
            TaiLaiTatCa();
            ThongBao("Đã hủy thay đổi, tải lại danh sách.");
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn xóa phiếu thu này?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Count == 0) return;
            if (MessageBox.Show("Bạn chắc chắn xóa phiếu thu này?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string ma = txtMaPhieu.Text;
            string ten = cmbKhachHang.Text;
            bindingNavigator.BindingSource.RemoveCurrent();
            try
            {
                ctrl.Save();
                NhatKyPhieu.Ghi("Thu", "Xóa", ma, ten, "Xóa phiếu thu");
                CapNhatTong();
                ThongBao("Đã xóa phiếu thu.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa không thành công:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiLaiTatCa();
            }
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                txtMaPhieu.Focus();
                if (bindingNavigator.BindingSource != null) bindingNavigator.BindingSource.EndEdit();
                dataGridView.EndEdit();
                ctrl.Save();
                NhatKyPhieu.Ghi("Thu", cheDoSua ? "Sửa" : "Lưu", txtMaPhieu.Text, cmbKhachHang.Text,
                    "Số tiền " + numTongTien.Value.ToString("N0") + ". " + txtGhiChu.Text);
                DatCheDoSua(false);
                CapNhatTong();
                ThongBao("Đã lưu phiếu thu.");
                MessageBox.Show("Đã lưu phiếu thu.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu không thành công:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void toolIn_Click(object sender, EventArgs e)
        {
            // PM-30: PhieuIn + canh bao + tieu de "In Phieu Thanh Toan" — giu design
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Count == 0)
            {
                MessageBox.Show("Chưa có phiếu thu nào để in.\nHãy thêm và lưu phiếu trước.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (bindingNavigator.BindingSource.Current == null)
            {
                MessageBox.Show("Chọn một phiếu thu trên danh sách trước khi in.", this.Text,
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

                string kh = (cmbKhachHang.Text ?? "").Trim();
                string tieuDe = "PHIẾU THANH TOÁN (PHIẾU THU)";
                if (kh.Length > 0)
                    tieuDe = "PHIẾU THU — " + kh;

                PhieuIn.Mo(tieuDe, cuaHang, new string[] {
                    "Mã phiếu|" + ma,
                    "Khách hàng / Đại lý|" + kh,
                    "Ngày thu|" + dtNgayThanhToan.Value.ToString("dd/MM/yyyy"),
                    "Số tiền|" + numTongTien.Value.ToString("N0") + " đ",
                    "Ghi chú|" + (txtGhiChu.Text ?? "")
                }, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void toolTimKiem_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || LayBang() == null || LayBang().Rows.Count == 0)
                TaiLaiTatCa();

            frmTimPhieuThu Tim = new frmTimPhieuThu();
            Tim.StartPosition = FormStartPosition.CenterParent;
            if (Tim.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                object kh = Tim.cmbKhachHang.SelectedValue;
                string ten = Tim.cmbKhachHang.Text ?? "";
                DateTime ngay = Tim.dtNgayThu.Value.Date;
                string tu = ngay.ToString("MM/dd/yyyy");
                string den = ngay.AddDays(1).ToString("MM/dd/yyyy");
                bool locNgay = true;
                try { if (Tim.chkTheoNgay != null) locNgay = Tim.chkTheoNgay.Checked; } catch { }
                string theoNgay = locNgay
                    ? ("NGAY_THANH_TOAN >= #" + tu + "# AND NGAY_THANH_TOAN < #" + den + "#")
                    : "";
                string theoKhach = "";
                if (kh != null)
                    theoKhach = "Convert(ID_KHACH_HANG, 'System.String') = '" + kh.ToString().Replace("'", "''") + "'";

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
                    if (Tim.txtSoTienTu != null && decimal.TryParse(Tim.txtSoTienTu.Text.Replace(",", ""), out a))
                        theoTien = "TONG_TIEN >= " + a.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    if (Tim.txtSoTienDen != null && decimal.TryParse(Tim.txtSoTienDen.Text.Replace(",", ""), out b))
                        theoTien = (theoTien.Length > 0 ? theoTien + " AND " : "")
                            + "TONG_TIEN <= " + b.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
                string theoHTTT = "";
                try
                {
                    if (Tim.cboHinhThuc != null && Tim.cboHinhThuc.SelectedIndex > 0)
                    {
                        string ht = Tim.cboHinhThuc.Text.Replace("'", "''");
                        theoHTTT = "Convert(HINH_THUC_TT, 'System.String') LIKE '%" + ht + "%'";
                    }
                }
                catch { }

                System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
                if (theoNgay.Length > 0) parts.Add(theoNgay);
                if (theoKhach.Length > 0) parts.Add(theoKhach);
                if (theoMa.Length > 0) parts.Add(theoMa);
                if (theoNguoi.Length > 0) parts.Add(theoNguoi);
                if (theoTien.Length > 0) parts.Add(theoTien);
                if (theoHTTT.Length > 0) parts.Add(theoHTTT);
                string filter = parts.Count > 0 ? string.Join(" AND ", parts.ToArray()) : "";
                bindingNavigator.BindingSource.Filter = filter;

                CapNhatTong();
                if (bindingNavigator.BindingSource.Count == 0)
                {
                    bindingNavigator.BindingSource.Filter = "";
                    CapNhatTong();
                    MessageBox.Show(
                        "Không thấy phiếu thu khớp.\nDanh sách đã hiện lại toàn bộ.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ThongBao("Không khớp. Đã hiện lại toàn bộ.");
                }
                else
                {
                    ThongBao("Tìm thấy " + bindingNavigator.BindingSource.Count + " phiếu. Bấm Xóa lọc để xem hết.");
                }
            }
            catch (Exception ex)
            {
                try { if (bindingNavigator.BindingSource != null) bindingNavigator.BindingSource.Filter = ""; } catch { }
                MessageBox.Show("Tìm kiếm lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiLaiTatCa();
            }
        }

        private void toolNhatKy_Click(object sender, EventArgs e)
        {
            string ma = txtMaPhieu != null ? txtMaPhieu.Text : "";
            string ten = cmbKhachHang != null ? cmbKhachHang.Text : "";
            using (frmNhatKyPhieu f = new frmNhatKyPhieu("Thu", "phiếu thu", ma, ten))
                f.ShowDialog(this);
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    /// <summary>Nhật ký dùng chung cho phiếu thu và phiếu chi. Ghi file cạnh chương trình.</summary>
    public static class NhatKyPhieu
    {
        public static string DuongDan()
        {
            return Path.Combine(Application.StartupPath, "NhatKy_Phieu.txt");
        }

        public static void Ghi(string loai, string hanhDong, string ma, string ten, string noiDung)
        {
            try
            {
                string dong = string.Join("\t", new string[] {
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    "admin",
                    hanhDong ?? "",
                    ma ?? "",
                    ten ?? "",
                    (noiDung ?? "").Replace("\t", " ").Replace("\r", " ").Replace("\n", " "),
                    loai ?? ""
                });
                File.AppendAllText(DuongDan(), dong + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }

        public static DataTable Doc(string loai, string ma, bool tatCa)
        {
            DataTable t = new DataTable();
            t.Columns.Add("Thời gian");
            t.Columns.Add("Người thực hiện");
            t.Columns.Add("Hành động");
            t.Columns.Add("Mã số");
            t.Columns.Add("Tên");
            t.Columns.Add("Nội dung thay đổi");
            if (!File.Exists(DuongDan())) return t;
            string[] lines = File.ReadAllLines(DuongDan(), Encoding.UTF8);
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string[] p = lines[i].Split('\t');
                if (p.Length < 7) continue;
                if (!string.Equals(p[6], loai, StringComparison.OrdinalIgnoreCase)) continue;
                if (!tatCa && ma != null && ma.Length > 0 && p[3] != ma) continue;
                t.Rows.Add(p[0], p[1], p[2], p[3], p[4], p[5]);
            }
            return t;
        }
    }

    public class frmNhatKyPhieu : Form
    {
        DataGridView grid;
        CheckBox chkTatCa;
        Label lblDem;
        string loai, ma, ten, nhan;

        public frmNhatKyPhieu(string loai, string nhan, string ma, string ten)
        {
            this.loai = loai;
            this.nhan = nhan;
            this.ma = ma ?? "";
            this.ten = ten ?? "";
            Text = "Nhật ký thay đổi";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(760, 420);
            MinimumSize = new Size(640, 360);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9f);

            Panel head = new Panel();
            head.Dock = DockStyle.Top;
            head.Height = 42;
            head.BackColor = Color.FromArgb(198, 239, 206);
            Label title = new Label();
            title.AutoSize = false;
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleLeft;
            title.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            title.Padding = new Padding(10, 0, 0, 0);
            title.Name = "lblTieuDe";
            chkTatCa = new CheckBox();
            chkTatCa.Text = "Xem nhật ký của tất cả";
            chkTatCa.AutoSize = true;
            chkTatCa.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkTatCa.Location = new Point(520, 12);
            chkTatCa.CheckedChanged += delegate { Nap(); };
            head.Controls.Add(title);
            head.Controls.Add(chkTatCa);

            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 125, 90);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersHeight = 30;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);

            Panel foot = new Panel();
            foot.Dock = DockStyle.Bottom;
            foot.Height = 46;
            foot.BackColor = Color.FromArgb(236, 248, 236);
            lblDem = new Label();
            lblDem.AutoSize = true;
            lblDem.Location = new Point(12, 14);
            Button btnMoi = new Button();
            btnMoi.Text = "Làm mới";
            btnMoi.Size = new Size(90, 28);
            btnMoi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoi.Location = new Point(540, 9);
            btnMoi.Click += delegate { Nap(); };
            Button btnDong = new Button();
            btnDong.Text = "Đóng";
            btnDong.Size = new Size(90, 28);
            btnDong.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDong.Location = new Point(640, 9);
            btnDong.Click += delegate { Close(); };
            foot.Controls.Add(lblDem);
            foot.Controls.Add(btnMoi);
            foot.Controls.Add(btnDong);

            Controls.Add(grid);
            Controls.Add(foot);
            Controls.Add(head);
            Load += delegate { Nap(); };
        }

        void Nap()
        {
            bool tatCa = chkTatCa.Checked;
            grid.DataSource = NhatKyPhieu.Doc(loai, ma, tatCa);
            Label title = Controls[2].Controls["lblTieuDe"] as Label;
            if (title != null)
            {
                if (tatCa) title.Text = "  Nhật ký tất cả " + nhan;
                else title.Text = "  Nhật ký của: " + (ten.Length > 0 ? ten : "(chưa chọn)") + " (mã " + ma + ")";
            }
            lblDem.Text = grid.Rows.Count + " dòng";
        }
    }

    public static class PhieuIn
    {
        public static void Mo(string tieuDe, string cuaHang, string[] dong, IWin32Window owner)
        {
            PrintDocument doc = new PrintDocument();
            doc.DocumentName = tieuDe;
            doc.PrintPage += delegate(object s, PrintPageEventArgs ev)
            {
                Graphics g = ev.Graphics;
                Rectangle page = ev.MarginBounds;
                int x = page.Left;
                int y = page.Top;
                int w = page.Width;
                using (Pen vien = new Pen(Color.Black, 1.4f))
                using (Font fCua = new Font("Times New Roman", 12f, FontStyle.Bold))
                using (Font fTieu = new Font("Times New Roman", 20f, FontStyle.Bold))
                using (Font f = new Font("Times New Roman", 13f))
                using (Font fNho = new Font("Times New Roman", 11f, FontStyle.Italic))
                {
                    StringFormat giua = new StringFormat();
                    giua.Alignment = StringAlignment.Center;
                    g.DrawString(cuaHang, fCua, Brushes.Black, new RectangleF(x, y, w, 22), giua);
                    y += 26;
                    g.DrawString(tieuDe, fTieu, Brushes.Black, new RectangleF(x, y, w, 36), giua);
                    y += 40;
                    g.DrawLine(vien, x + 80, y, x + w - 80, y);
                    y += 16;

                    int tableTop = y;
                    int rowH = 32;
                    int labelW = 180;
                    g.DrawRectangle(vien, x, tableTop, w, rowH * dong.Length);
                    for (int i = 0; i < dong.Length; i++)
                    {
                        int ry = tableTop + i * rowH;
                        if (i > 0) g.DrawLine(vien, x, ry, x + w, ry);
                        g.DrawLine(vien, x + labelW, ry, x + labelW, ry + rowH);
                        string[] kv = (dong[i] ?? "").Split(new char[] { '|' }, 2);
                        string lb = kv.Length > 0 ? kv[0] : "";
                        string vl = kv.Length > 1 ? kv[1] : "";
                        RectangleF r1 = new RectangleF(x + 8, ry + 6, labelW - 12, rowH - 8);
                        RectangleF r2 = new RectangleF(x + labelW + 8, ry + 6, w - labelW - 16, rowH - 8);
                        g.DrawString(lb, f, Brushes.Black, r1);
                        using (Font fb = new Font("Times New Roman", 13f, FontStyle.Bold))
                            g.DrawString(vl, lb.IndexOf("tiền", StringComparison.OrdinalIgnoreCase) >= 0 ? fb : f, Brushes.Black, r2);
                    }
                    y = tableTop + rowH * dong.Length + 28;
                    g.DrawString("Ngày in: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fNho, Brushes.Black, x, y);
                    y += 50;
                    int cot = w / 3;
                    g.DrawString("Người nộp", f, Brushes.Black, new RectangleF(x, y, cot, 24), giua);
                    g.DrawString("Người lập phiếu", f, Brushes.Black, new RectangleF(x + cot, y, cot, 24), giua);
                    g.DrawString("Thủ quỹ", f, Brushes.Black, new RectangleF(x + cot * 2, y, cot, 24), giua);
                    g.DrawString("(Ký, ghi rõ họ tên)", fNho, Brushes.Black, new RectangleF(x, y + 22, cot, 20), giua);
                    g.DrawString("(Ký, ghi rõ họ tên)", fNho, Brushes.Black, new RectangleF(x + cot, y + 22, cot, 20), giua);
                    g.DrawString("(Ký, ghi rõ họ tên)", fNho, Brushes.Black, new RectangleF(x + cot * 2, y + 22, cot, 20), giua);
                }
            };
            using (PrintPreviewDialog pv = new PrintPreviewDialog())
            {
                pv.Document = doc;
                pv.Text = tieuDe;
                pv.Width = 900;
                pv.Height = 700;
                pv.ShowDialog(owner);
            }
        }
    }

    public static class GiaoDienChung
    {
        static bool bat;
        static System.Collections.Generic.Dictionary<int, bool> daTrangTri = new System.Collections.Generic.Dictionary<int, bool>();

        public static void BatDau()
        {
            if (bat) return;
            bat = true;
            Application.Idle += delegate
            {
                try
                {
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f == null || f.IsDisposed || f.IsMdiContainer) continue;
                        if (f.GetType().Name == "frmDangNhap") continue;
                        int id = f.GetHashCode();
                        if (daTrangTri.ContainsKey(id)) continue;
                        TrangTri(f);
                        daTrangTri[id] = true;
                    }
                }
                catch { }
            };
        }

        public static Image Icon(string loai)
        {
            Bitmap bmp = new Bitmap(28, 28);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);
                using (Pen dam = new Pen(Color.FromArgb(40, 40, 40), 1.6f))
                {
                    if (loai == "sua")
                    {
                        g.FillPolygon(Brushes.Gold, new Point[] { new Point(4, 22), new Point(8, 24), new Point(24, 8), new Point(20, 4) });
                        g.DrawLine(new Pen(Color.SaddleBrown, 2f), 4, 24, 9, 19);
                        g.FillPolygon(Brushes.LightGray, new Point[] { new Point(20, 4), new Point(24, 4), new Point(24, 8) });
                    }
                    else if (loai == "huy")
                    {
                        using (Pen p = new Pen(Color.DarkOrange, 3f))
                            g.DrawArc(p, 7, 6, 15, 15, 20, 280);
                        g.FillPolygon(Brushes.DarkOrange, new Point[] { new Point(6, 8), new Point(6, 16), new Point(13, 12) });
                    }
                    else if (loai == "nhatky")
                    {
                        g.FillRectangle(Brushes.White, 6, 3, 16, 22);
                        g.DrawRectangle(new Pen(Color.SteelBlue, 2f), 6, 3, 16, 22);
                        using (Pen p = new Pen(Color.SteelBlue, 1.6f))
                        {
                            g.DrawLine(p, 9, 9, 19, 9);
                            g.DrawLine(p, 9, 13, 19, 13);
                            g.DrawLine(p, 9, 17, 16, 17);
                        }
                    }
                    else if (loai == "thoat")
                    {
                        g.DrawRectangle(new Pen(Color.Firebrick, 2f), 4, 5, 11, 16);
                        using (Pen p = new Pen(Color.Firebrick, 2.4f))
                        {
                            p.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                            g.DrawLine(p, 14, 13, 24, 13);
                        }
                        g.FillPolygon(Brushes.Firebrick, new Point[] { new Point(20, 8), new Point(25, 13), new Point(20, 18) });
                    }
                    else
                    {
                        g.FillEllipse(Brushes.SeaGreen, 2, 2, 24, 24);
                    }
                }
            }
            return bmp;
        }

        public static void TrangTri(Form f)
        {
            try { f.BackColor = Color.FromArgb(236, 248, 236); } catch { }
            TrangTriControls(f.Controls);
            bool co = false;
            foreach (Control c in f.Controls)
                if (c is StatusStrip) co = true;
            if (!co)
            {
                StatusStrip ss = new StatusStrip();
                ss.Name = "statusGiaoDienChung";
                ss.BackColor = Color.FromArgb(220, 242, 220);
                ss.SizingGrip = false;
                ToolStripStatusLabel che = new ToolStripStatusLabel("Chế độ xem");
                che.BorderSides = ToolStripStatusLabelBorderSides.Right;
                che.AutoSize = false;
                che.Width = 120;
                ToolStripStatusLabel tb = new ToolStripStatusLabel(f.Text);
                tb.Spring = true;
                tb.TextAlign = ContentAlignment.MiddleLeft;
                ss.Items.Add(che);
                ss.Items.Add(tb);
                f.Controls.Add(ss);
            }
        }

        static void TrangTriControls(Control.ControlCollection cols)
        {
            foreach (Control c in cols)
            {
                BindingNavigator bn = c as BindingNavigator;
                if (bn != null)
                {
                    bn.BackColor = Color.FromArgb(198, 239, 206);
                    bn.RenderMode = ToolStripRenderMode.Professional;
                    bn.GripStyle = ToolStripGripStyle.Hidden;
                    foreach (ToolStripItem it in bn.Items)
                    {
                        ToolStripButton b = it as ToolStripButton;
                        if (b == null) continue;
                        if (b.DisplayStyle != ToolStripItemDisplayStyle.Text)
                        {
                            b.TextImageRelation = TextImageRelation.ImageAboveText;
                            b.ImageScaling = ToolStripItemImageScaling.None;
                        }
                        if (b.Name == "toolSua") b.Image = Icon("sua");
                        else if (b.Name == "toolHuy") b.Image = Icon("huy");
                        else if (b.Name == "toolNhatKy") b.Image = Icon("nhatky");
                        else if (b.Name == "toolThoat") b.Image = Icon("thoat");
                    }
                }
                DataGridView gv = c as DataGridView;
                if (gv != null)
                {
                    gv.EnableHeadersVisualStyles = false;
                    gv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 125, 90);
                    gv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    gv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                    if (gv.ColumnHeadersHeight < 30) gv.ColumnHeadersHeight = 30;
                    gv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                    gv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 115, 185);
                    gv.DefaultCellStyle.SelectionForeColor = Color.White;
                    gv.BackgroundColor = Color.White;
                }
                if (c.HasChildren) TrangTriControls(c.Controls);
            }
        }
    }

}
