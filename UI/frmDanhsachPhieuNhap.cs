using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-16: Danh sach phieu nhap – cot day du, loc NCC/ngay/cong no, UX nhom nut, trang thai.
    /// </summary>
    public partial class frmDanhsachPhieuNhap : Form
    {
        PhieuNhapController ctrl = new PhieuNhapController();
        NhaCungCapController ctrlNCC = new NhaCungCapController();
        frmNhapHang NhapHang = null;

        private Panel panelFilter;
        private ComboBox cmbNCC;
        private DateTimePicker dtTuNgay;
        private DateTimePicker dtDenNgay;
        private CheckBox chkLocNgay;
        private ComboBox cmbCongNo;
        private Button btnLoc;
        private Button btnXoaLoc;
        private Label lblTongHop;
        private DataGridView dataGridView;
        private BindingNavigator bindingNavigator;
        private BindingSource bsNav = new BindingSource();

        // Nhom thao tac
        private ToolStripButton toolThem, toolXoa, toolTim, toolLamMoi;
        private ToolStripButton toolIn, toolThanhToan, toolThoat;

        public frmDanhsachPhieuNhap()
        {
            try { InitializeComponent(); } catch { }
            XayGiaoDien();
        }

        private void XayGiaoDien()
        {
            this.SuspendLayout();
            this.Controls.Clear();

            this.Text = "Danh sách phiếu nhập";
            this.Size = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9f);
            this.MinimumSize = new Size(900, 480);

            // ----- Filter panel -----
            panelFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 78,
                BackColor = Color.FromArgb(232, 245, 233),
                Padding = new Padding(8)
            };

            Label title = new Label
            {
                Text = "DANH SÁCH PHIẾU NHẬP",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 94, 32),
                AutoSize = true,
                Location = new Point(10, 6)
            };

            Label l1 = new Label { Text = "NCC:", AutoSize = true, Location = new Point(10, 40) };
            cmbNCC = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(45, 36),
                Width = 160
            };

            chkLocNgay = new CheckBox
            {
                Text = "Khoảng ngày",
                AutoSize = true,
                Location = new Point(220, 38),
                Checked = false
            };
            dtTuNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Location = new Point(330, 36),
                Width = 105,
                Enabled = false,
                Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            };
            Label l2 = new Label { Text = "→", AutoSize = true, Location = new Point(440, 40) };
            dtDenNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Location = new Point(460, 36),
                Width = 105,
                Enabled = false,
                Value = DateTime.Now.Date
            };
            chkLocNgay.CheckedChanged += (s, e) =>
            {
                dtTuNgay.Enabled = chkLocNgay.Checked;
                dtDenNgay.Enabled = chkLocNgay.Checked;
            };

            Label l3 = new Label { Text = "Công nợ:", AutoSize = true, Location = new Point(580, 40) };
            cmbCongNo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(645, 36),
                Width = 120
            };
            cmbCongNo.Items.AddRange(new object[] { "Tất cả", "Còn nợ", "Đã thanh toán" });
            cmbCongNo.SelectedIndex = 0;

            btnLoc = new Button
            {
                Text = "Lọc",
                Location = new Point(780, 34),
                Size = new Size(70, 26),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoc.Click += (s, e) => NapDuLieu();

            btnXoaLoc = new Button
            {
                Text = "Xóa lọc",
                Location = new Point(860, 34),
                Size = new Size(70, 26)
            };
            btnXoaLoc.Click += (s, e) =>
            {
                try { cmbNCC.SelectedIndex = 0; } catch { }
                chkLocNgay.Checked = false;
                cmbCongNo.SelectedIndex = 0;
                NapDuLieu();
            };

            panelFilter.Controls.AddRange(new Control[] {
                title, l1, cmbNCC, chkLocNgay, dtTuNgay, l2, dtDenNgay, l3, cmbCongNo, btnLoc, btnXoaLoc
            });

            // ----- Toolbar -----
            bindingNavigator = new BindingNavigator(true)
            {
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(200, 230, 201)
            };
            // clear default add/delete if we add custom
            try
            {
                bindingNavigator.AddNewItem = null;
                bindingNavigator.DeleteItem = null;
            }
            catch { }

            toolThem = MakeBtn("Thêm mới", toolThem_Click);
            toolXoa = MakeBtn("Xóa", toolXoa_Click);
            toolTim = MakeBtn("Tìm kiếm", toolTim_Click);
            toolLamMoi = MakeBtn("Làm mới", (s, e) => NapDuLieu());
            toolIn = MakeBtn("In", toolIn_Click);
            toolThanhToan = MakeBtn("Công nợ", toolThanhToan_Click);
            toolThoat = MakeBtn("Thoát", (s, e) => this.Close());

            bindingNavigator.Items.Add(toolThem);
            bindingNavigator.Items.Add(toolXoa);
            bindingNavigator.Items.Add(toolTim);
            bindingNavigator.Items.Add(toolLamMoi);
            bindingNavigator.Items.Add(new ToolStripSeparator());
            bindingNavigator.Items.Add(toolIn);
            bindingNavigator.Items.Add(new ToolStripSeparator());
            bindingNavigator.Items.Add(toolThanhToan);
            bindingNavigator.Items.Add(new ToolStripSeparator());
            bindingNavigator.Items.Add(toolThoat);

            // ----- Bottom summary -----
            Panel bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.FromArgb(255, 243, 224)
            };
            lblTongHop = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(191, 54, 12),
                Text = "  Tổng: 0 phiếu | Tổng tiền: 0 | Còn nợ: 0",
                Padding = new Padding(8, 0, 0, 0)
            };
            bottom.Controls.Add(lblTongHop);

            // ----- Grid -----
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoGenerateColumns = false
            };
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 125, 50);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.DoubleClick += dataGridView_DoubleClick;
            dataGridView.CellFormatting += dataGridView_CellFormatting;

            TaoCot();

            this.Controls.Add(dataGridView);
            this.Controls.Add(bottom);
            this.Controls.Add(bindingNavigator);
            this.Controls.Add(panelFilter);

            this.Load += frmDanhsachPhieuNhap_Load;
            this.ResumeLayout(true);
        }

        private ToolStripButton MakeBtn(string text, EventHandler click)
        {
            ToolStripButton b = new ToolStripButton
            {
                Text = text,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                TextImageRelation = TextImageRelation.ImageAboveText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            try
            {
                if (text == "Thêm") b.Image = global::CuahangNongduoc.Properties.Resources.add;
                else if (text == "Xóa") b.Image = global::CuahangNongduoc.Properties.Resources.remove;
                else if (text == "Tìm kiếm") b.Image = global::CuahangNongduoc.Properties.Resources.find;
                else if (text == "Làm mới") b.Image = global::CuahangNongduoc.Properties.Resources.reload;
                else if (text == "In") b.Image = global::CuahangNongduoc.Properties.Resources.printer;
                else if (text == "Thoát") b.Image = global::CuahangNongduoc.Properties.Resources.Thoat;
                else if (text == "Công nợ" || text == "Thanh toán") b.Image = global::CuahangNongduoc.Properties.Resources.save;
            }
            catch { }
            b.Click += click;
            return b;
        }

        private void TaoCot()
        {
            dataGridView.Columns.Clear();
            AddCol("ID", "Mã phiếu", 80);
            AddCol("TEN_NCC", "Nhà cung cấp", 120);
            AddCol("ID_NHA_CUNG_CAP", "Mã NCC", 70);
            AddCol("NGAY_NHAP", "Ngày nhập", 90);
            AddCol("TONG_TIEN", "Tổng tiền", 100);
            AddCol("PHI_VAN_CHUYEN", "Vận chuyển", 90);
            AddCol("CHIET_KHAU", "Chiết khấu", 90);
            AddCol("VAT", "VAT", 70);
            AddCol("DA_TRA", "Đã trả", 90);
            AddCol("CON_NO", "Còn nợ", 90);
            AddCol("NGUOI_NHAP", "Người nhập", 100);
            AddCol("GHI_CHU", "Ghi chú", 120);
            AddCol("TRANG_THAI_HIENTHI", "Trạng thái", 100);
        }

        private void AddCol(string prop, string header, int w)
        {
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                Name = "col_" + prop,
                HeaderText = header,
                Width = w,
                MinimumWidth = 50
            };
            if (prop == "NGAY_NHAP")
                c.DefaultCellStyle.Format = "dd/MM/yyyy";
            if (prop == "TONG_TIEN" || prop == "DA_TRA" || prop == "CON_NO" ||
                prop == "PHI_VAN_CHUYEN" || prop == "CHIET_KHAU" || prop == "VAT")
                c.DefaultCellStyle.Format = "N0";
            dataGridView.Columns.Add(c);
        }

        private void frmDanhsachPhieuNhap_Load(object sender, EventArgs e)
        {
            try { PhieuNhapController.DamBaoCotMoRong(); } catch { }
            NapNCC();
            NapDuLieu();
        }

        private void NapNCC()
        {
            try
            {
                DataService ds = new DataService();
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(
                    "SELECT ID, HO_TEN FROM NHA_CUNG_CAP ORDER BY HO_TEN");
                ds.Load(cmd);
                DataTable t = ds.Copy();
                DataRow all = t.NewRow();
                all["ID"] = "";
                all["HO_TEN"] = "-- Tất cả NCC --";
                t.Rows.InsertAt(all, 0);
                cmbNCC.DisplayMember = "HO_TEN";
                cmbNCC.ValueMember = "ID";
                cmbNCC.DataSource = t;
            }
            catch
            {
                cmbNCC.Items.Clear();
                cmbNCC.Items.Add("-- Tất cả NCC --");
                cmbNCC.SelectedIndex = 0;
            }
        }

        private void NapDuLieu()
        {
            string idNcc = null;
            try
            {
                object v = cmbNCC.SelectedValue;
                if (v != null && v != DBNull.Value)
                {
                    string s = Convert.ToString(v);
                    if (!string.IsNullOrEmpty(s)) idNcc = s;
                }
            }
            catch { }

            DateTime? tu = null, den = null;
            if (chkLocNgay.Checked)
            {
                tu = dtTuNgay.Value.Date;
                den = dtDenNgay.Value.Date;
            }

            int? locNo = null;
            if (cmbCongNo.SelectedIndex == 1) locNo = 1;
            else if (cmbCongNo.SelectedIndex == 2) locNo = 0;

            try
            {
                DataTable raw = ctrl.DanhsachPhieuNhapLoc(idNcc, tu, den, locNo);
                DataTable view = BoSungCotHienThi(raw);
                bsNav.DataSource = view;
                dataGridView.DataSource = bsNav;
                bindingNavigator.BindingSource = bsNav;
                CapNhatTong(view);
                AnCotKhongCo(view);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable BoSungCotHienThi(DataTable src)
        {
            if (src == null) return new DataTable();
            DataTable t = src.Copy();
            if (!t.Columns.Contains("TRANG_THAI_HIENTHI"))
                t.Columns.Add("TRANG_THAI_HIENTHI", typeof(string));
            if (!t.Columns.Contains("TEN_NCC"))
                t.Columns.Add("TEN_NCC", typeof(string));

            foreach (DataRow r in t.Rows)
            {
                // Trang thai: uu tien cot TRANG_THAI, khong thi suy tu CON_NO
                string tt = "";
                if (t.Columns.Contains("TRANG_THAI") && r["TRANG_THAI"] != DBNull.Value)
                    tt = Convert.ToString(r["TRANG_THAI"]);
                if (string.IsNullOrEmpty(tt))
                {
                    decimal conNo = 0;
                    try { if (r["CON_NO"] != DBNull.Value) conNo = Convert.ToDecimal(r["CON_NO"]); } catch { }
                    decimal tong = 0;
                    try { if (r["TONG_TIEN"] != DBNull.Value) tong = Convert.ToDecimal(r["TONG_TIEN"]); } catch { }
                    if (tong <= 0) tt = "Nháp";
                    else if (conNo > 0) tt = "Còn nợ";
                    else tt = "Hoàn tất";
                }
                r["TRANG_THAI_HIENTHI"] = tt;

                if (t.Columns.Contains("TEN_NCC") && (r["TEN_NCC"] == DBNull.Value || string.IsNullOrEmpty(Convert.ToString(r["TEN_NCC"]))))
                {
                    // keep empty – may only have ID_NHA_CUNG_CAP
                }
            }
            return t;
        }

        private void AnCotKhongCo(DataTable t)
        {
            string[] opt = { "PHI_VAN_CHUYEN", "CHIET_KHAU", "VAT", "NGUOI_NHAP", "GHI_CHU" };
            foreach (string c in opt)
            {
                string name = "col_" + c;
                if (dataGridView.Columns.Contains(name))
                {
                    // hien neu cot co trong data (ke ca null)
                    dataGridView.Columns[name].Visible = t.Columns.Contains(c);
                }
            }
        }

        private void CapNhatTong(DataTable t)
        {
            long tong = 0, no = 0;
            int n = 0;
            if (t != null)
            {
                n = t.Rows.Count;
                foreach (DataRow r in t.Rows)
                {
                    try { if (r["TONG_TIEN"] != DBNull.Value) tong += Convert.ToInt64(r["TONG_TIEN"]); } catch { }
                    try { if (r["CON_NO"] != DBNull.Value) no += Convert.ToInt64(r["CON_NO"]); } catch { }
                }
            }
            lblTongHop.Text = string.Format("  Tổng: {0} phiếu  |  Tổng tiền: {1:N0}  |  Còn nợ: {2:N0}", n, tong, no);
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            try
            {
                if (dataGridView.Columns[e.ColumnIndex].DataPropertyName == "TRANG_THAI_HIENTHI")
                {
                    string v = Convert.ToString(e.Value);
                    if (v == "Còn nợ") e.CellStyle.ForeColor = Color.Firebrick;
                    else if (v == "Hoàn tất") e.CellStyle.ForeColor = Color.ForestGreen;
                    else if (v == "Hủy") e.CellStyle.ForeColor = Color.Gray;
                }
                if (dataGridView.Columns[e.ColumnIndex].DataPropertyName == "CON_NO")
                {
                    decimal d = 0;
                    try { d = Convert.ToDecimal(e.Value); } catch { }
                    if (d > 0) e.CellStyle.ForeColor = Color.Firebrick;
                }
            }
            catch { }
        }

        private DataRowView DongHienTai()
        {
            try { return bsNav.Current as DataRowView; }
            catch { return null; }
        }

        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            MoNhapHang();
        }

        private void toolThem_Click(object sender, EventArgs e)
        {
            if (NhapHang == null || NhapHang.IsDisposed)
            {
                NhapHang = new frmNhapHang();
                NhapHang.Show();
            }
            else NhapHang.Activate();
        }

        private void MoNhapHang()
        {
            if (NhapHang == null || NhapHang.IsDisposed)
            {
                try { NhapHang = new frmNhapHang(ctrl); }
                catch { NhapHang = new frmNhapHang(); }
                NhapHang.Show();
            }
            else NhapHang.Activate();
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            DataRowView row = DongHienTai();
            if (row == null)
            {
                MessageBox.Show("Chọn phiếu cần xóa.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Xóa phiếu " + Convert.ToString(row["ID"]) + "?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                bsNav.RemoveCurrent();
                ctrl.Save();
                CapNhatTong(bsNav.DataSource as DataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không xóa được:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                NapDuLieu();
            }
        }

        private void toolTim_Click(object sender, EventArgs e)
        {
            try
            {
                frmTimPhieuNhap Tim = new frmTimPhieuNhap();
                if (Tim.ShowDialog(this) != DialogResult.OK) return;

                string idNcc = "";
                try { idNcc = Tim.IdNhaCungCap; } catch { }
                DateTime? ngay = null;
                try { if (!Tim.BoQuaNgay) ngay = Tim.dtNgayNhap.Value.Date; } catch { }
                decimal? tongTu = null, tongDen = null;
                try
                {
                    if (Tim.LocTongTien) { tongTu = Tim.TongTienTu; tongDen = Tim.TongTienDen; }
                }
                catch { }

                int n = 0;
                try
                {
                    n = ctrl.TimPhieuNhapNangCao(Tim.MaPhieu, idNcc, ngay, tongTu, tongDen, Tim.NguoiLap, Tim.KhoNhap);
                }
                catch
                {
                    if (!string.IsNullOrEmpty(idNcc) && ngay.HasValue)
                    {
                        ctrl.TimPhieuNhap(idNcc, ngay.Value);
                        n = ctrl.SoPhieu;
                    }
                }

                // dong bo grid tu controller – dung lai loc hien thi
                if (n == 0)
                    MessageBox.Show("Không tìm thấy phiếu.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                NapDuLieu(); // don gian: sau tim van co the loc lai; hoac hien ket qua tim
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolIn_Click(object sender, EventArgs e)
        {
            DataRowView row = DongHienTai();
            if (row == null)
            {
                MessageBox.Show("Chọn phiếu để in.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                string ma = Convert.ToString(row["ID"]);
                PhieuNhapController c = new PhieuNhapController();
                var ph = c.LayPhieuNhap(ma);
                frmInPhieuNhap f = new frmInPhieuNhap(ph);
                f.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void toolThanhToan_Click(object sender, EventArgs e)
        {
            DataRowView row = DongHienTai();
            if (row == null)
            {
                MessageBox.Show("Chọn phiếu để xem công nợ.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string ma = Convert.ToString(row["ID"]);
            string ncc = "";
            try
            {
                if (row.Row.Table.Columns.Contains("TEN_NCC"))
                    ncc = Convert.ToString(row["TEN_NCC"]);
            }
            catch { }
            decimal tong = 0, daTra = 0, conNo = 0;
            try { tong = Convert.ToDecimal(row["TONG_TIEN"]); } catch { }
            try { daTra = Convert.ToDecimal(row["DA_TRA"]); } catch { }
            try { conNo = Convert.ToDecimal(row["CON_NO"]); } catch { }

            string msg = string.Format(
                "Phiếu: {0}\nNhà cung cấp: {1}\n\nTổng tiền: {2:N0}\nĐã trả: {3:N0}\nCòn nợ: {4:N0}\n\n" +
                "(Liên kết phiếu chi / hóa đơn NCC: bổ sung khi có chứng từ gốc trong module kế toán.)",
                ma, ncc, tong, daTra, conNo);
            MessageBox.Show(msg, "Thông tin công nợ phiếu nhập", MessageBoxButtons.OK,
                conNo > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }
    }
}
