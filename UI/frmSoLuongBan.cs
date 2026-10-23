using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmSoLuongBan : Form
    {
        private readonly ChiTietPhieuBanController ctrl = new ChiTietPhieuBanController();

        DataGridView luoi;
        BieuDoBan bieuDo;
        ComboBox cmbBieuDo;
        ComboBox cmbSanPham;
        ComboBox cmbLoai;
        ComboBox cmbNhanVien;
        DateTimePicker dtTu;
        DateTimePicker dtDen;
        Label lblTong;
        DataTable bangGoc;
        DataTable bangLoc;
        string cotNhanVien;
        bool daCoNhanVien;

        public frmSoLuongBan()
        {
            InitializeComponent();
        }

        private void frmSoLuongBan_Load(object sender, EventArgs e)
        {
            this.Text = "Số lượng bán";
            this.BackColor = Color.FromArgb(236, 248, 236);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new Size(980, 620);
            if (this.ClientSize.Width < 980) this.ClientSize = new Size(980, 640);

            SuaNhanCu();
            AnReportViewer();
            TaoBoLocThem();
            TaoLuoiVaBieuDo();

            try
            {
                cmbThang.SelectedIndex = DateTime.Now.Month - 1;
                numNam.Value = DateTime.Now.Year;
            }
            catch { }

            NapDanhMuc();
            XemThang();
        }

        void SuaNhanCu()
        {
            groupBox1.Text = "Xem theo tháng/năm";
            label2.Text = "Tháng";
            label3.Text = "Năm";
            btnXemThang.Text = "Xem tháng";
            btnXemThang.TextAlign = ContentAlignment.MiddleRight;
            btnXemThang.ImageAlign = ContentAlignment.MiddleLeft;
            groupBox2.Text = "Xem theo ngày";
            label1.Text = "Ngày";
            btnXemNgay.Text = "Xem ngày";
            btnXemNgay.TextAlign = ContentAlignment.MiddleRight;
            btnXemNgay.ImageAlign = ContentAlignment.MiddleLeft;
            if (btnXemThang.Image == null) btnXemThang.Image = VeIcon(Color.SeaGreen);
            if (btnXemNgay.Image == null) btnXemNgay.Image = VeIcon(Color.SeaGreen);
            panel1.BackColor = Color.FromArgb(236, 248, 236);
            groupBox1.BackColor = Color.FromArgb(236, 248, 236);
            groupBox2.BackColor = Color.FromArgb(236, 248, 236);
        }

        void AnReportViewer()
        {
            try
            {
                reportViewer.Visible = false;
                reportViewer.ShowToolBar = false;
                reportViewer.Dock = DockStyle.None;
                reportViewer.Size = new Size(1, 1);
            }
            catch { }
        }

        void TaoBoLocThem()
        {
            panel1.Height = 198;

            GroupBox g = new GroupBox();
            g.Text = "Khoảng ngày, sản phẩm, loại, nhân viên";
            g.Location = new Point(8, 102);
            g.Size = new Size(Math.Max(panel1.Width - 16, 940), 90);
            g.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            g.BackColor = Color.FromArgb(236, 248, 236);

            Label l1 = new Label();
            l1.Text = "Từ ngày";
            l1.Location = new Point(10, 28);
            l1.AutoSize = true;
            dtTu = new DateTimePicker();
            dtTu.Format = DateTimePickerFormat.Custom;
            dtTu.CustomFormat = "dd/MM/yyyy";
            dtTu.Location = new Point(68, 24);
            dtTu.Width = 110;
            dtTu.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            Label l2 = new Label();
            l2.Text = "Đến ngày";
            l2.Location = new Point(186, 28);
            l2.AutoSize = true;
            dtDen = new DateTimePicker();
            dtDen.Format = DateTimePickerFormat.Custom;
            dtDen.CustomFormat = "dd/MM/yyyy";
            dtDen.Location = new Point(252, 24);
            dtDen.Width = 110;

            Label l3 = new Label();
            l3.Text = "Sản phẩm";
            l3.Location = new Point(372, 28);
            l3.AutoSize = true;
            cmbSanPham = new ComboBox();
            cmbSanPham.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSanPham.Location = new Point(442, 24);
            cmbSanPham.Width = 160;

            Label l4 = new Label();
            l4.Text = "Loại SP";
            l4.Location = new Point(10, 60);
            l4.AutoSize = true;
            cmbLoai = new ComboBox();
            cmbLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLoai.Location = new Point(68, 56);
            cmbLoai.Width = 160;

            Label l5 = new Label();
            l5.Text = "Nhân viên";
            l5.Location = new Point(240, 60);
            l5.AutoSize = true;
            cmbNhanVien = new ComboBox();
            cmbNhanVien.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNhanVien.Location = new Point(310, 56);
            cmbNhanVien.Width = 150;

            Button xem = Nut("Xem khoảng ngày", new Point(470, 52), btnXemKhoang_Click);
            Button excel = Nut("Xuất Excel", new Point(640, 52), btnExcel_Click);
            Button pdf = Nut("Xuất PDF", new Point(770, 52), btnPdf_Click);

            cmbSanPham.SelectedIndexChanged += BoLocDoi;
            cmbLoai.SelectedIndexChanged += BoLocDoi;
            cmbNhanVien.SelectedIndexChanged += BoLocDoi;

            g.Controls.Add(l1);
            g.Controls.Add(dtTu);
            g.Controls.Add(l2);
            g.Controls.Add(dtDen);
            g.Controls.Add(l3);
            g.Controls.Add(cmbSanPham);
            g.Controls.Add(l4);
            g.Controls.Add(cmbLoai);
            g.Controls.Add(l5);
            g.Controls.Add(cmbNhanVien);
            g.Controls.Add(xem);
            g.Controls.Add(excel);
            g.Controls.Add(pdf);
            panel1.Controls.Add(g);
        }

        Button Nut(string text, Point p, EventHandler click)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = p;
            b.Size = new Size(120, 28);
            b.Image = VeIcon(Color.FromArgb(46, 125, 90));
            b.ImageAlign = ContentAlignment.MiddleLeft;
            b.TextAlign = ContentAlignment.MiddleRight;
            b.UseVisualStyleBackColor = true;
            b.Click += click;
            return b;
        }

        static Image VeIcon(Color mau)
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (SolidBrush b = new SolidBrush(mau))
                    g.FillEllipse(b, 1, 1, 14, 14);
                using (Pen p = new Pen(Color.White, 2f))
                {
                    g.DrawLine(p, 4, 8, 7, 11);
                    g.DrawLine(p, 7, 11, 12, 4);
                }
            }
            return bmp;
        }

        void TaoLuoiVaBieuDo()
        {
            panel2.BackColor = Color.FromArgb(236, 248, 236);

            Panel tren = new Panel();
            tren.Dock = DockStyle.Top;
            tren.Height = 36;
            tren.BackColor = Color.FromArgb(198, 239, 206);

            Label lb = new Label();
            lb.Text = "Biểu đồ";
            lb.AutoSize = true;
            lb.Location = new Point(8, 10);
            cmbBieuDo = new ComboBox();
            cmbBieuDo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBieuDo.Items.Add("Cột");
            cmbBieuDo.Items.Add("Đường");
            cmbBieuDo.Items.Add("Tròn");
            cmbBieuDo.SelectedIndex = 0;
            cmbBieuDo.Location = new Point(70, 6);
            cmbBieuDo.Width = 100;
            cmbBieuDo.SelectedIndexChanged += cmbBieuDo_SelectedIndexChanged;

            lblTong = new Label();
            lblTong.AutoSize = false;
            lblTong.Location = new Point(190, 8);
            lblTong.Size = new Size(520, 22);
            lblTong.Text = "Tổng số lượng bán: 0    Tổng doanh thu: 0";
            lblTong.Font = new Font(this.Font, FontStyle.Bold);

            tren.Controls.Add(lb);
            tren.Controls.Add(cmbBieuDo);
            tren.Controls.Add(lblTong);

            StatusStrip ss = new StatusStrip();
            ss.BackColor = Color.FromArgb(220, 242, 220);
            ss.SizingGrip = false;
            ToolStripStatusLabel st = new ToolStripStatusLabel("Sẵn sàng.");
            st.Spring = true;
            st.TextAlign = ContentAlignment.MiddleLeft;
            st.Name = "lblTrangThai";
            ss.Items.Add(st);

            bieuDo = new BieuDoBan();
            bieuDo.Dock = DockStyle.Right;
            bieuDo.Width = 340;
            bieuDo.BackColor = Color.White;

            luoi = new DataGridView();
            luoi.Dock = DockStyle.Fill;
            luoi.ReadOnly = true;
            luoi.AllowUserToAddRows = false;
            luoi.AllowUserToDeleteRows = false;
            luoi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            luoi.MultiSelect = false;
            luoi.RowHeadersVisible = false;
            luoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            luoi.BackgroundColor = Color.White;
            luoi.EnableHeadersVisualStyles = false;
            luoi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 125, 90);
            luoi.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            luoi.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            luoi.ColumnHeadersHeight = 32;
            luoi.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
            luoi.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 115, 185);
            luoi.DefaultCellStyle.SelectionForeColor = Color.White;

            panel2.Controls.Add(luoi);
            panel2.Controls.Add(bieuDo);
            panel2.Controls.Add(tren);
            this.Controls.Add(ss);
        }

        void cmbBieuDo_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapNhatBieuDo();
        }

        void CapNhatBieuDo()
        {
            if (bieuDo == null) return;
            bieuDo.KieuBieuDo = cmbBieuDo == null ? 0 : cmbBieuDo.SelectedIndex;
            bieuDo.BangDuLieu = bangLoc;
            bieuDo.Invalidate();
        }

        void NapDanhMuc()
        {
            cmbSanPham.Items.Clear();
            cmbSanPham.Items.Add("Tất cả");
            cmbLoai.Items.Clear();
            cmbLoai.Items.Add("Tất cả");
            cmbNhanVien.Items.Clear();
            cmbNhanVien.Items.Add("Tất cả");

            try
            {
                DataService sp = new DataService();
                sp.Load(new OleDbCommand("SELECT ID, TEN_SAN_PHAM FROM SAN_PHAM ORDER BY TEN_SAN_PHAM"));
                foreach (DataRow r in sp.Rows)
                    cmbSanPham.Items.Add(Convert.ToString(r["TEN_SAN_PHAM"]));
            }
            catch { }

            try
            {
                DataService dvt = new DataService();
                dvt.Load(new OleDbCommand("SELECT TEN_DON_VI FROM DON_VI_TINH ORDER BY TEN_DON_VI"));
                foreach (DataRow r in dvt.Rows)
                    cmbLoai.Items.Add(Convert.ToString(r["TEN_DON_VI"]));
            }
            catch { }

            cotNhanVien = TimCotNhanVien();
            daCoNhanVien = cotNhanVien != null;
            if (!daCoNhanVien)
            {
                cmbNhanVien.Items.Clear();
                cmbNhanVien.Items.Add("Không lưu trên phiếu");
                cmbNhanVien.Enabled = false;
            }
            cmbSanPham.SelectedIndex = 0;
            cmbLoai.SelectedIndex = 0;
            if (cmbNhanVien.Items.Count > 0) cmbNhanVien.SelectedIndex = 0;
        }

        static string TimCotNhanVien()
        {
            try
            {
                DataService ds = new DataService();
                ds.Load(new OleDbCommand("SELECT TOP 1 * FROM PHIEU_BAN"));
                foreach (DataColumn c in ds.Columns)
                {
                    string n = c.ColumnName.ToUpper();
                    if (n.Contains("NHAN_VIEN") || n.Contains("NHANVIEN") || n.Contains("ID_NV") || n.Contains("NGUOI_BAN") || n == "NHAN_VIEN")
                        return c.ColumnName;
                }
            }
            catch { }
            return null;
        }

        private void btnXemNgay_Click(object sender, EventArgs e)
        {
            DateTime d = dtNgay.Value.Date;
            Nap(d, d, "Ngày " + d.ToString("dd/MM/yyyy"));
        }

        private void btnXemThang_Click(object sender, EventArgs e)
        {
            XemThang();
        }

        void XemThang()
        {
            int thang = cmbThang.SelectedIndex + 1;
            if (thang < 1) thang = DateTime.Now.Month;
            int nam = Convert.ToInt32(numNam.Value);
            DateTime tu = new DateTime(nam, thang, 1);
            DateTime den = tu.AddMonths(1).AddDays(-1);
            Nap(tu, den, "Tháng " + thang + "/" + nam);
        }

        private void btnXemKhoang_Click(object sender, EventArgs e)
        {
            DateTime tu = dtTu.Value.Date;
            DateTime den = dtDen.Value.Date;
            if (den < tu)
            {
                MessageBox.Show("Đến ngày phải lớn hơn hoặc bằng Từ ngày.", "Số lượng bán",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Nap(tu, den, "Từ " + tu.ToString("dd/MM/yyyy") + " đến " + den.ToString("dd/MM/yyyy"));
        }

        void BoLocDoi(object sender, EventArgs e)
        {
            if (bangGoc == null) return;
            ApDungLoc();
            Hien("Đã lọc lại");
        }

        void Nap(DateTime tu, DateTime den, string tieuDe)
        {
            try
            {
                bangGoc = DocDuLieu(tu, den.AddDays(1));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lấy được dữ liệu:\n" + ex.Message, "Số lượng bán",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            NapNhanVienTuBang();
            ApDungLoc();
            Hien(tieuDe);
        }

        DataTable DocDuLieu(DateTime tu, DateTime denMo)
        {
            string nv = daCoNhanVien ? ", PB.[" + cotNhanVien + "] AS NHAN_VIEN" : ", N'' AS NHAN_VIEN";
            string sql =
                "SELECT SP.ID AS MA_SP, SP.TEN_SAN_PHAM, DVT.TEN_DON_VI AS LOAI, " +
                "CT.SO_LUONG, PB.NGAY_BAN, CT.THANH_TIEN AS DOANH_THU" + nv + " " +
                "FROM CHI_TIET_PHIEU_BAN CT " +
                "INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                "LEFT JOIN MA_SAN_PHAM MSP ON CT.ID_MA_SAN_PHAM = MSP.ID " +
                "LEFT JOIN SAN_PHAM SP ON MSP.ID_SAN_PHAM = SP.ID " +
                "LEFT JOIN DON_VI_TINH DVT ON SP.ID_DON_VI_TINH = DVT.ID " +
                "WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den";

            OleDbCommand cmd = new OleDbCommand(sql);
            cmd.Parameters.Add("tu", OleDbType.Date).Value = tu;
            cmd.Parameters.Add("den", OleDbType.Date).Value = denMo;
            DataService ds = new DataService();
            ds.Load(cmd);

            DataTable t = TaoBangTrong();
            foreach (DataRow r in ds.Rows)
            {
                DataRow n = t.NewRow();
                n["Mã SP"] = r["MA_SP"] == DBNull.Value ? "" : Convert.ToString(r["MA_SP"]);
                n["Tên SP"] = r["TEN_SAN_PHAM"] == DBNull.Value ? "" : Convert.ToString(r["TEN_SAN_PHAM"]);
                n["Loại"] = r["LOAI"] == DBNull.Value ? "" : Convert.ToString(r["LOAI"]);
                n["Số lượng bán"] = r["SO_LUONG"] == DBNull.Value ? 0 : Convert.ToInt32(r["SO_LUONG"]);
                n["Ngày bán"] = r["NGAY_BAN"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDateTime(r["NGAY_BAN"]);
                n["Doanh thu"] = r["DOANH_THU"] == DBNull.Value ? 0L : Convert.ToInt64(r["DOANH_THU"]);
                n["Nhân viên"] = ds.Columns.Contains("NHAN_VIEN") && r["NHAN_VIEN"] != DBNull.Value ? Convert.ToString(r["NHAN_VIEN"]) : "";
                t.Rows.Add(n);
            }
            return t;
        }

        static DataTable TaoBangTrong()
        {
            DataTable t = new DataTable();
            t.Columns.Add("Mã SP", typeof(string));
            t.Columns.Add("Tên SP", typeof(string));
            t.Columns.Add("Loại", typeof(string));
            t.Columns.Add("Số lượng bán", typeof(int));
            t.Columns.Add("Ngày bán", typeof(DateTime));
            t.Columns.Add("Doanh thu", typeof(long));
            t.Columns.Add("Nhân viên", typeof(string));
            return t;
        }

        void NapNhanVienTuBang()
        {
            if (!daCoNhanVien || bangGoc == null) return;
            string giu = cmbNhanVien.Text;
            cmbNhanVien.SelectedIndexChanged -= BoLocDoi;
            cmbNhanVien.Items.Clear();
            cmbNhanVien.Items.Add("Tất cả");
            foreach (DataRow r in bangGoc.Rows)
            {
                string nv = Convert.ToString(r["Nhân viên"]);
                if (nv.Length > 0 && !cmbNhanVien.Items.Contains(nv))
                    cmbNhanVien.Items.Add(nv);
            }
            int i = cmbNhanVien.Items.IndexOf(giu);
            cmbNhanVien.SelectedIndex = i >= 0 ? i : 0;
            cmbNhanVien.SelectedIndexChanged += BoLocDoi;
        }

        void ApDungLoc()
        {
            bangLoc = TaoBangTrong();
            if (bangGoc == null) return;
            string sp = cmbSanPham == null || cmbSanPham.SelectedIndex <= 0 ? "" : cmbSanPham.Text;
            string loai = cmbLoai == null || cmbLoai.SelectedIndex <= 0 ? "" : cmbLoai.Text;
            string nv = (cmbNhanVien == null || !cmbNhanVien.Enabled || cmbNhanVien.SelectedIndex <= 0) ? "" : cmbNhanVien.Text;
            foreach (DataRow r in bangGoc.Rows)
            {
                if (sp.Length > 0 && Convert.ToString(r["Tên SP"]) != sp) continue;
                if (loai.Length > 0 && Convert.ToString(r["Loại"]) != loai) continue;
                if (nv.Length > 0 && Convert.ToString(r["Nhân viên"]) != nv) continue;
                bangLoc.ImportRow(r);
            }
        }

        void Hien(string tieuDe)
        {
            if (bangLoc == null) bangLoc = TaoBangTrong();
            luoi.DataSource = bangLoc;
            if (luoi.Columns.Contains("Loại")) luoi.Columns["Loại"].Visible = true;
            if (luoi.Columns.Contains("Nhân viên")) luoi.Columns["Nhân viên"].Visible = daCoNhanVien;
            if (luoi.Columns.Contains("Ngày bán")) luoi.Columns["Ngày bán"].DefaultCellStyle.Format = "dd/MM/yyyy";
            if (luoi.Columns.Contains("Doanh thu"))
            {
                luoi.Columns["Doanh thu"].DefaultCellStyle.Format = "N0";
                luoi.Columns["Doanh thu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (luoi.Columns.Contains("Số lượng bán"))
                luoi.Columns["Số lượng bán"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            long sl = 0;
            long tien = 0;
            foreach (DataRow r in bangLoc.Rows)
            {
                sl += Convert.ToInt64(r["Số lượng bán"]);
                tien += Convert.ToInt64(r["Doanh thu"]);
            }
            lblTong.Text = "Tổng số lượng bán: " + sl.ToString("N0") + "     Tổng doanh thu: " + tien.ToString("N0");

            string tb;
            if (bangLoc.Rows.Count == 0)
            {
                tb = "Không tìm thấy dữ liệu.";
                MessageBox.Show("Không tìm thấy dữ liệu.", "Số lượng bán",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                tb = tieuDe + " — " + bangLoc.Rows.Count + " dòng.";
            }
            GanTrangThai(tb);
            CapNhatBieuDo();
        }

        void GanTrangThai(string tb)
        {
            foreach (Control c in this.Controls)
            {
                StatusStrip ss = c as StatusStrip;
                if (ss == null) continue;
                foreach (ToolStripItem it in ss.Items)
                    if (it is ToolStripStatusLabel) it.Text = tb;
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (bangLoc == null || bangLoc.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy dữ liệu.", "Số lượng bán", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel (*.xls)|*.xls";
            dlg.FileName = "SoLuongBan.xls";
            if (dlg.ShowDialog(this) != DialogResult.OK) return;
            File.WriteAllText(dlg.FileName, TaoExcelXml(), Encoding.UTF8);
            GanTrangThai("Đã xuất Excel: " + dlg.FileName);
        }

        string TaoExcelXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
            sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
            sb.AppendLine("<Worksheet ss:Name=\"SoLuongBan\"><Table>");
            sb.Append("<Row>");
            foreach (DataColumn c in bangLoc.Columns)
            {
                if (c.ColumnName == "Nhân viên" && !daCoNhanVien) continue;
                sb.Append("<Cell><Data ss:Type=\"String\">").Append(Xml(c.ColumnName)).Append("</Data></Cell>");
            }
            sb.AppendLine("</Row>");
            foreach (DataRow r in bangLoc.Rows)
            {
                sb.Append("<Row>");
                foreach (DataColumn c in bangLoc.Columns)
                {
                    if (c.ColumnName == "Nhân viên" && !daCoNhanVien) continue;
                    string typ = (c.DataType == typeof(int) || c.DataType == typeof(long)) ? "Number" : "String";
                    string val = r[c] == DBNull.Value ? "" : Convert.ToString(r[c], CultureInfo.InvariantCulture);
                    if (c.DataType == typeof(DateTime) && r[c] != DBNull.Value)
                    {
                        typ = "String";
                        val = Convert.ToDateTime(r[c]).ToString("dd/MM/yyyy");
                    }
                    sb.Append("<Cell><Data ss:Type=\"").Append(typ).Append("\">").Append(Xml(val)).Append("</Data></Cell>");
                }
                sb.AppendLine("</Row>");
            }
            sb.AppendLine("</Table></Worksheet></Workbook>");
            return sb.ToString();
        }

        static string Xml(string s)
        {
            if (s == null) return "";
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            if (bangLoc == null || bangLoc.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy dữ liệu.", "Số lượng bán", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Drawing.Printing.PrintDocument doc = new System.Drawing.Printing.PrintDocument();
            doc.DocumentName = "Số lượng bán";
            int dong = 0;
            doc.PrintPage += delegate(object s, System.Drawing.Printing.PrintPageEventArgs ev)
            {
                float y = 40;
                using (Font title = new Font("Segoe UI", 14f, FontStyle.Bold))
                using (Font f = new Font("Segoe UI", 9f))
                {
                    ev.Graphics.DrawString("Số lượng bán", title, Brushes.DarkGreen, 40, y);
                    y += 36;
                    ev.Graphics.DrawString(lblTong.Text, f, Brushes.Black, 40, y);
                    y += 28;
                    while (dong < bangLoc.Rows.Count && y < ev.MarginBounds.Bottom - 20)
                    {
                        DataRow r = bangLoc.Rows[dong];
                        string line = Convert.ToString(r["Mã SP"]) + "  " + Convert.ToString(r["Tên SP"])
                            + "   SL: " + r["Số lượng bán"]
                            + "   Ngày: " + (r["Ngày bán"] == DBNull.Value ? "" : Convert.ToDateTime(r["Ngày bán"]).ToString("dd/MM/yyyy"))
                            + "   DT: " + Convert.ToInt64(r["Doanh thu"]).ToString("N0");
                        ev.Graphics.DrawString(line, f, Brushes.Black, 40, y);
                        y += 20;
                        dong++;
                    }
                    ev.HasMorePages = dong < bangLoc.Rows.Count;
                }
            };
            PrintPreviewDialog xem = new PrintPreviewDialog();
            xem.Document = doc;
            xem.Width = 900;
            xem.Height = 700;
            xem.ShowDialog(this);
            GanTrangThai("Đã mở trang in. Chọn Microsoft Print to PDF để lưu file PDF.");
        }
    }

    // Biểu đồ tự vẽ — không dùng Func<> (tương thích .NET 2.0 / VS cũ)
    public class BieuDoBan : Panel
    {
        public int KieuBieuDo;
        public DataTable BangDuLieu;

        public BieuDoBan()
        {
            this.DoubleBuffered = true;
            this.Resize += delegate { Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            DataTable t = BangDuLieu;
            if (t == null || t.Rows.Count == 0)
            {
                using (Font f = new Font("Segoe UI", 9f))
                    g.DrawString("Không tìm thấy dữ liệu.", f, Brushes.Gray, 12, 12);
                return;
            }

            Dictionary<string, long> sl = new Dictionary<string, long>();
            foreach (DataRow r in t.Rows)
            {
                string ten = Convert.ToString(r["Tên SP"]);
                if (ten.Length == 0) ten = Convert.ToString(r["Mã SP"]);
                long cong = Convert.ToInt64(r["Số lượng bán"]);
                long cu = 0;
                if (sl.ContainsKey(ten)) cu = sl[ten];
                sl[ten] = cu + cong;
            }
            if (sl.Count == 0) return;

            int kieu = KieuBieuDo;
            List<string> tenList = new List<string>();
            foreach (string k in sl.Keys) tenList.Add(k);
            long max = 1;
            foreach (long v in sl.Values) if (v > max) max = v;

            Rectangle vung = new Rectangle(36, 16, Math.Max(20, Width - 52), Math.Max(20, Height - 48));
            Color[] mau = new Color[] {
                Color.FromArgb(46, 125, 90), Color.FromArgb(33, 115, 185), Color.DarkOrange,
                Color.MediumPurple, Color.IndianRed, Color.Teal, Color.Goldenrod, Color.DimGray
            };

            if (kieu == 2)
            {
                float tong = 0;
                foreach (long v in sl.Values) tong += v;
                if (tong <= 0) tong = 1;
                float goc = -90;
                int canh = Math.Min(vung.Width, vung.Height) - 10;
                Rectangle tron = new Rectangle(vung.X + 10, vung.Y, canh, canh);
                int i = 0;
                foreach (string ten in tenList)
                {
                    float quet = (float)(sl[ten] * 360.0 / tong);
                    using (SolidBrush b = new SolidBrush(mau[i % mau.Length]))
                        g.FillPie(b, tron, goc, quet);
                    goc += quet;
                    i++;
                }
                return;
            }

            int n = tenList.Count;
            float bw = Math.Max(8, (vung.Width - 8) / (float)n);
            using (Pen truc = new Pen(Color.Silver))
                g.DrawLine(truc, vung.Left, vung.Bottom, vung.Right, vung.Bottom);

            List<PointF> diem = new List<PointF>();
            for (int i = 0; i < n; i++)
            {
                long v = sl[tenList[i]];
                float h = (float)v / max * (vung.Height - 10);
                float x = vung.Left + i * bw;
                if (kieu == 0)
                {
                    using (SolidBrush b = new SolidBrush(mau[i % mau.Length]))
                        g.FillRectangle(b, x + 2, vung.Bottom - h, Math.Max(4, bw - 6), h);
                }
                diem.Add(new PointF(x + bw / 2f, vung.Bottom - h));
            }
            if (kieu == 1 && diem.Count > 1)
            {
                using (Pen p = new Pen(Color.FromArgb(33, 115, 185), 2f))
                    g.DrawLines(p, diem.ToArray());
                foreach (PointF pt in diem)
                    g.FillEllipse(Brushes.DarkOrange, pt.X - 3, pt.Y - 3, 6, 6);
            }
        }
    }
}
