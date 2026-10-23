using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-33: So luong ton — hien thi, loc, tong, canh bao sap het, xuat CSV.
    /// Giu design xanh nhat (chuan Nhap hang). Khong ReportViewer / bieu do.
    /// </summary>
    public partial class frmSoLuongTon : Form
    {
        DataTable _bangGoc;
        DataGridView grid;
        ToolStrip tool;
        ToolStripTextBox txtTim;
        ToolStripComboBox cboMucTon;
        StatusStrip status;
        ToolStripStatusLabel lblTong;
        ToolStripStatusLabel lblCanhBao;
        const int MUC_SAP_HET_MAC_DINH = 10;

        public frmSoLuongTon()
        {
            InitializeComponent();
        }

        private void frmSoLuongTon_Load(object sender, EventArgs e)
        {
            this.Text = "Số lượng tồn kho";
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new Size(900, 500);
            this.BackColor = Color.FromArgb(192, 255, 192);

            try { if (reportViewer != null) { reportViewer.Visible = false; reportViewer.Dock = DockStyle.None; reportViewer.Size = new Size(1, 1); } } catch { }

            TaoThanhCongCu();
            TaoStatus();
            grid = TimHoacTaoLuoi();
            TaiDuLieu();
        }

        void TaoThanhCongCu()
        {
            if (tool != null) return;
            tool = new ToolStrip();
            tool.BackColor = Color.FromArgb(192, 255, 192);
            tool.ImageScalingSize = new Size(24, 24);
            tool.GripStyle = ToolStripGripStyle.Hidden;

            ToolStripButton btnLamMoi = new ToolStripButton("Làm mới");
            btnLamMoi.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnLamMoi.TextImageRelation = TextImageRelation.ImageAboveText;
            try { btnLamMoi.Image = global::CuahangNongduoc.Properties.Resources.reload; } catch { }
            btnLamMoi.Click += delegate { TaiDuLieu(); };
            tool.Items.Add(btnLamMoi);

            tool.Items.Add(new ToolStripSeparator());
            tool.Items.Add(new ToolStripLabel("Tìm mã/tên:"));
            txtTim = new ToolStripTextBox();
            txtTim.AutoSize = false;
            txtTim.Width = 180;
            txtTim.TextChanged += delegate { ApDungLoc(); };
            tool.Items.Add(txtTim);

            tool.Items.Add(new ToolStripLabel("Mức tồn:"));
            cboMucTon = new ToolStripComboBox();
            cboMucTon.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMucTon.Items.AddRange(new object[] { "Tất cả", "Còn hàng (>0)", "Sắp hết (≤ mức)", "Hết hàng (=0)" });
            cboMucTon.SelectedIndex = 0;
            cboMucTon.SelectedIndexChanged += delegate { ApDungLoc(); };
            tool.Items.Add(cboMucTon);

            ToolStripButton btnXoaLoc = new ToolStripButton("Xóa lọc");
            btnXoaLoc.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnXoaLoc.Click += delegate
            {
                if (txtTim != null) txtTim.Text = "";
                if (cboMucTon != null) cboMucTon.SelectedIndex = 0;
                ApDungLoc();
            };
            tool.Items.Add(btnXoaLoc);

            tool.Items.Add(new ToolStripSeparator());
            ToolStripButton btnXuat = new ToolStripButton("Xuất Excel (CSV)");
            btnXuat.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnXuat.TextImageRelation = TextImageRelation.ImageAboveText;
            try { btnXuat.Image = global::CuahangNongduoc.Properties.Resources.save; } catch { }
            btnXuat.Click += btnXuat_Click;
            tool.Items.Add(btnXuat);

            ToolStripButton btnThoat = new ToolStripButton("Thoát");
            btnThoat.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnThoat.TextImageRelation = TextImageRelation.ImageAboveText;
            try { btnThoat.Image = global::CuahangNongduoc.Properties.Resources.Thoat; } catch { }
            btnThoat.Click += delegate { this.Close(); };
            tool.Items.Add(btnThoat);

            this.Controls.Add(tool);
            tool.Dock = DockStyle.Top;
            tool.BringToFront();
        }

        void TaoStatus()
        {
            if (status != null) return;
            status = new StatusStrip();
            status.BackColor = Color.FromArgb(192, 255, 192);
            lblTong = new ToolStripStatusLabel("Tổng: 0");
            lblTong.Spring = false;
            lblTong.BorderSides = ToolStripStatusLabelBorderSides.Right;
            lblCanhBao = new ToolStripStatusLabel("");
            lblCanhBao.Spring = true;
            lblCanhBao.ForeColor = Color.DarkRed;
            status.Items.Add(lblTong);
            status.Items.Add(lblCanhBao);
            this.Controls.Add(status);
        }

        void TaiDuLieu()
        {
            try
            {
                IList<SoLuongTon> data = CuahangNongduoc.Controller.SanPhamController.LaySoLuongTon();
                _bangGoc = new DataTable();
                _bangGoc.Columns.Add("Mã SP", typeof(string));
                _bangGoc.Columns.Add("Tên sản phẩm", typeof(string));
                _bangGoc.Columns.Add("Đơn vị tính", typeof(string));
                _bangGoc.Columns.Add("Số lượng tồn", typeof(int));
                _bangGoc.Columns.Add("Đơn giá nhập", typeof(long));
                _bangGoc.Columns.Add("Giá bán lẻ", typeof(long));
                _bangGoc.Columns.Add("Giá bán sỉ", typeof(long));
                _bangGoc.Columns.Add("Giá trị tồn", typeof(long));

                if (data != null)
                {
                    foreach (SoLuongTon slt in data)
                    {
                        SanPham sp = slt.SanPham;
                        string ma = sp != null ? (sp.Id ?? "") : "";
                        string ten = sp != null ? (sp.TenSanPham ?? "") : "";
                        string dvt = "";
                        try
                        {
                            if (sp != null && sp.DonViTinh != null)
                                dvt = sp.DonViTinh.Ten ?? "";
                        }
                        catch { }
                        long giaNhap = 0, giaLe = 0, giaSi = 0;
                        try
                        {
                            if (sp != null)
                            {
                                giaNhap = sp.DonGiaNhap;
                                giaLe = sp.GiaBanLe;
                                giaSi = sp.GiaBanSi;
                            }
                        }
                        catch { }
                        int sl = 0;
                        try { sl = slt.SoLuong; } catch { }
                        long giaTri = giaNhap * (long)sl;
                        _bangGoc.Rows.Add(ma, ten, dvt, sl, giaNhap, giaLe, giaSi, giaTri);
                    }
                }

                ApDungLoc();

                if (_bangGoc.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Không tìm thấy dữ liệu.\nChưa có sản phẩm hoặc chưa nhập hàng.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải tồn kho:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void ApDungLoc()
        {
            if (_bangGoc == null) return;
            DataView dv = new DataView(_bangGoc);
            List<string> parts = new List<string>();

            string key = txtTim != null ? (txtTim.Text ?? "").Trim().Replace("'", "''") : "";
            if (key.Length > 0)
            {
                parts.Add(string.Format(
                    "([Mã SP] LIKE '%{0}%' OR [Tên sản phẩm] LIKE '%{0}%')", key));
            }

            int muc = cboMucTon != null ? cboMucTon.SelectedIndex : 0;
            if (muc == 1)
                parts.Add("[Số lượng tồn] > 0");
            else if (muc == 2)
                parts.Add(string.Format("[Số lượng tồn] > 0 AND [Số lượng tồn] <= {0}", MUC_SAP_HET_MAC_DINH));
            else if (muc == 3)
                parts.Add("[Số lượng tồn] = 0");

            dv.RowFilter = parts.Count > 0 ? string.Join(" AND ", parts.ToArray()) : "";
            if (grid == null) grid = TimHoacTaoLuoi();
            grid.DataSource = dv;
            DinhDangLuoi(grid);
            ToMauSapHet(grid);
            CapNhatTong(dv);
        }

        void CapNhatTong(DataView dv)
        {
            long tongSL = 0, tongGT = 0;
            int sapHet = 0, hetHang = 0;
            try
            {
                foreach (DataRowView rv in dv)
                {
                    int sl = 0;
                    long gt = 0;
                    try { sl = Convert.ToInt32(rv["Số lượng tồn"]); } catch { }
                    try { gt = Convert.ToInt64(rv["Giá trị tồn"]); } catch { }
                    tongSL += sl;
                    tongGT += gt;
                    if (sl == 0) hetHang++;
                    else if (sl <= MUC_SAP_HET_MAC_DINH) sapHet++;
                }
            }
            catch { }
            if (lblTong != null)
                lblTong.Text = string.Format("Dòng: {0} | Tổng SL: {1:N0} | Tổng giá trị tồn: {2:N0} đ",
                    dv != null ? dv.Count : 0, tongSL, tongGT);
            if (lblCanhBao != null)
            {
                if (sapHet + hetHang > 0)
                    lblCanhBao.Text = string.Format("⚠ Sắp hết: {0} | Hết hàng: {1} (mức ≤ {2})",
                        sapHet, hetHang, MUC_SAP_HET_MAC_DINH);
                else
                    lblCanhBao.Text = "Tồn ổn định.";
            }
        }

        void ToMauSapHet(DataGridView g)
        {
            try
            {
                if (g == null || g.Columns.Count == 0) return;
                string colSL = null;
                foreach (DataGridViewColumn c in g.Columns)
                    if (c.HeaderText.IndexOf("tồn", StringComparison.OrdinalIgnoreCase) >= 0
                        || c.DataPropertyName.IndexOf("tồn", StringComparison.OrdinalIgnoreCase) >= 0)
                    { colSL = c.Name; break; }
                if (colSL == null) return;
                foreach (DataGridViewRow row in g.Rows)
                {
                    if (row.IsNewRow) continue;
                    int sl = 0;
                    try
                    {
                        object v = row.Cells[colSL].Value;
                        if (v != null && v != DBNull.Value) sl = Convert.ToInt32(v);
                    }
                    catch { }
                    if (sl == 0)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    else if (sl <= MUC_SAP_HET_MAC_DINH)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 200);
                    else
                        row.DefaultCellStyle.BackColor = Color.White;
                }
            }
            catch { }
        }

        void btnXuat_Click(object sender, EventArgs e)
        {
            try
            {
                if (grid == null || grid.DataSource == null)
                {
                    MessageBox.Show("Không có dữ liệu để xuất.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "CSV (*.csv)|*.csv";
                dlg.FileName = "SoLuongTon_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                DataView dv = grid.DataSource as DataView;
                DataTable tbl = dv != null ? dv.ToTable() : (_bangGoc != null ? _bangGoc.Copy() : null);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < tbl.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(Csv(tbl.Columns[i].ColumnName));
                }
                sb.AppendLine();
                foreach (DataRow r in tbl.Rows)
                {
                    for (int i = 0; i < tbl.Columns.Count; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append(Csv(Convert.ToString(r[i])));
                    }
                    sb.AppendLine();
                }
                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Đã xuất:\n" + dlg.FileName, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xuất lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        static string Csv(string s)
        {
            if (s == null) s = "";
            if (s.IndexOfAny(new char[] { ',', '"', '\n', '\r' }) >= 0)
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        DataGridView TimHoacTaoLuoi()
        {
            foreach (Control c in this.Controls)
                if (c is DataGridView) return (DataGridView)c;

            DataGridView g = new DataGridView
            {
                Name = "gridTon",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(g);
            g.BringToFront();
            if (tool != null) tool.SendToBack();
            if (status != null) status.SendToBack();
            return g;
        }

        static void DinhDangLuoi(DataGridView g)
        {
            // Giong form danh sach: header he thong, khong xanh dam
            g.EnableHeadersVisualStyles = true;
            g.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            g.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            g.BackgroundColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 255, 232);
            g.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            g.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
            g.AllowUserToAddRows = false;
            g.ReadOnly = true;
            g.RowTemplate.Height = 24;
            try
            {
                foreach (DataGridViewColumn c in g.Columns)
                {
                    string h = c.HeaderText ?? "";
                    if (h.IndexOf("giá", StringComparison.OrdinalIgnoreCase) >= 0
                        || h.IndexOf("trị", StringComparison.OrdinalIgnoreCase) >= 0
                        || h.IndexOf("lượng", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        c.DefaultCellStyle.Format = "N0";
                        c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
            }
            catch { }
        }
    }
}
