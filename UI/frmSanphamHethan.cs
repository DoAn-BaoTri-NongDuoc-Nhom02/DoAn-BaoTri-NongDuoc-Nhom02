using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmSanphamHethan : Form
    {
        DataGridView luoi;
        ComboBox cmbNhaCungCap;
        ComboBox cmbLoai;
        ComboBox cmbTrangThai;
        DateTimePicker dtTu;
        DateTimePicker dtDen;
        NumericUpDown numSapHet;
        Label lblTong;
        DataTable bangGoc;
        DataTable bangLoc;
        Dictionary<string, bool> daXuLy = new Dictionary<string, bool>();

        public frmSanphamHethan()
        {
            InitializeComponent();
        }

        private void frmSanphamHethan_Load(object sender, EventArgs e)
        {
            this.Text = "Sản phẩm hết hạn";
            this.BackColor = Color.FromArgb(236, 248, 236);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new Size(980, 560);
            if (this.ClientSize.Width < 980) this.ClientSize = new Size(1000, 620);

            SuaNhanCu();
            AnReportViewer();
            TaoBoLocThem();
            TaoLuoi();
            NapDanhMuc();

            dt.Value = DateTime.Today;
            try { dtDen.Value = DateTime.Today; } catch { }
            // Từ ngày: để xa về quá khứ để không lọc mất dữ liệu mẫu cũ
            try { dtTu.Value = new DateTime(2000, 1, 1); } catch { }

            // Mở form: xem theo "Tính đến ngày" (không giới hạn 6 tháng)
            XemTheoNgayTinh();
        }

        void SuaNhanCu()
        {
            label1.Text = "Tính đến ngày";
            btnXem.Text = "Xem";
            btnXem.TextAlign = ContentAlignment.MiddleRight;
            btnXem.ImageAlign = ContentAlignment.MiddleLeft;
            if (btnXem.Image == null) btnXem.Image = VeIcon(Color.FromArgb(46, 125, 90));
            panel1.BackColor = Color.FromArgb(236, 248, 236);
            panel1.Height = 130;
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

        static Image VeIcon(Color mau)
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
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

        void TaoBoLocThem()
        {
            // Hàng 2: khoảng ngày + trạng thái + sắp hết hạn
            Label lTu = new Label();
            lTu.Text = "Từ ngày HH";
            lTu.AutoSize = true;
            lTu.Location = new Point(12, 52);
            dtTu = new DateTimePicker();
            dtTu.Format = DateTimePickerFormat.Custom;
            dtTu.CustomFormat = "dd/MM/yyyy";
            dtTu.Location = new Point(90, 48);
            dtTu.Width = 110;

            Label lDen = new Label();
            lDen.Text = "Đến ngày HH";
            lDen.AutoSize = true;
            lDen.Location = new Point(210, 52);
            dtDen = new DateTimePicker();
            dtDen.Format = DateTimePickerFormat.Custom;
            dtDen.CustomFormat = "dd/MM/yyyy";
            dtDen.Location = new Point(295, 48);
            dtDen.Width = 110;

            Label lTt = new Label();
            lTt.Text = "Trạng thái";
            lTt.AutoSize = true;
            lTt.Location = new Point(420, 52);
            cmbTrangThai = new ComboBox();
            cmbTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTrangThai.Items.Add("Tất cả");
            cmbTrangThai.Items.Add("Đã hết hạn");
            cmbTrangThai.Items.Add("Sắp hết hạn");
            cmbTrangThai.SelectedIndex = 0;
            cmbTrangThai.Location = new Point(490, 48);
            cmbTrangThai.Width = 120;

            Label lSap = new Label();
            lSap.Text = "Sắp HH trong (ngày)";
            lSap.AutoSize = true;
            lSap.Location = new Point(620, 52);
            numSapHet = new NumericUpDown();
            numSapHet.Minimum = 1;
            numSapHet.Maximum = 365;
            numSapHet.Value = 30;
            numSapHet.Location = new Point(750, 48);
            numSapHet.Width = 50;

            // Hàng 3: NCC, loại, xuất, xử lý
            Label lNcc = new Label();
            lNcc.Text = "Nhà cung cấp";
            lNcc.AutoSize = true;
            lNcc.Location = new Point(12, 90);
            cmbNhaCungCap = new ComboBox();
            cmbNhaCungCap.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNhaCungCap.Location = new Point(100, 86);
            cmbNhaCungCap.Width = 180;

            Label lLoai = new Label();
            lLoai.Text = "Loại / ĐVT";
            lLoai.AutoSize = true;
            lLoai.Location = new Point(290, 90);
            cmbLoai = new ComboBox();
            cmbLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLoai.Location = new Point(365, 86);
            cmbLoai.Width = 140;

            Button btnLoc = Nut("Lọc", new Point(520, 84), btnLoc_Click);
            Button btnExcel = Nut("Xuất Excel", new Point(600, 84), btnExcel_Click);
            Button btnPdf = Nut("Xuất PDF", new Point(710, 84), btnPdf_Click);
            Button btnXuLy = Nut("Đánh dấu xử lý", new Point(820, 84), btnXuLy_Click);
            btnXuLy.Width = 120;

            cmbNhaCungCap.SelectedIndexChanged += BoLocDoi;
            cmbLoai.SelectedIndexChanged += BoLocDoi;
            cmbTrangThai.SelectedIndexChanged += BoLocDoi;

            panel1.Controls.Add(lTu);
            panel1.Controls.Add(dtTu);
            panel1.Controls.Add(lDen);
            panel1.Controls.Add(dtDen);
            panel1.Controls.Add(lTt);
            panel1.Controls.Add(cmbTrangThai);
            panel1.Controls.Add(lSap);
            panel1.Controls.Add(numSapHet);
            panel1.Controls.Add(lNcc);
            panel1.Controls.Add(cmbNhaCungCap);
            panel1.Controls.Add(lLoai);
            panel1.Controls.Add(cmbLoai);
            panel1.Controls.Add(btnLoc);
            panel1.Controls.Add(btnExcel);
            panel1.Controls.Add(btnPdf);
            panel1.Controls.Add(btnXuLy);
        }

        Button Nut(string text, Point p, EventHandler click)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = p;
            b.Size = new Size(100, 28);
            b.UseVisualStyleBackColor = true;
            b.Click += click;
            return b;
        }

        void TaoLuoi()
        {
            panel2.BackColor = Color.FromArgb(236, 248, 236);

            Panel tren = new Panel();
            tren.Dock = DockStyle.Top;
            tren.Height = 34;
            tren.BackColor = Color.FromArgb(198, 239, 206);

            lblTong = new Label();
            lblTong.Dock = DockStyle.Fill;
            lblTong.TextAlign = ContentAlignment.MiddleLeft;
            lblTong.Padding = new Padding(10, 0, 0, 0);
            lblTong.Font = new Font(this.Font, FontStyle.Bold);
            lblTong.Text = "Tổng SP hết hạn: 0    Tổng giá trị: 0";
            tren.Controls.Add(lblTong);

            StatusStrip ss = new StatusStrip();
            ss.BackColor = Color.FromArgb(220, 242, 220);
            ss.SizingGrip = false;
            ToolStripStatusLabel st = new ToolStripStatusLabel("Sẵn sàng. Đỏ = đã hết hạn, Vàng = sắp hết hạn.");
            st.Spring = true;
            st.TextAlign = ContentAlignment.MiddleLeft;
            st.Name = "lblTrangThai";
            ss.Items.Add(st);

            luoi = new DataGridView();
            luoi.Dock = DockStyle.Fill;
            luoi.ReadOnly = true;
            luoi.AllowUserToAddRows = false;
            luoi.AllowUserToDeleteRows = false;
            luoi.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            luoi.MultiSelect = true;
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
            luoi.CellFormatting += luoi_CellFormatting;

            panel2.Controls.Add(luoi);
            panel2.Controls.Add(tren);
            this.Controls.Add(ss);
        }

        void NapDanhMuc()
        {
            cmbNhaCungCap.Items.Clear();
            cmbNhaCungCap.Items.Add("Tất cả");
            cmbLoai.Items.Clear();
            cmbLoai.Items.Add("Tất cả");

            try
            {
                DataService ncc = new DataService();
                ncc.Load(new OleDbCommand("SELECT TEN_NHA_CUNG_CAP FROM NHA_CUNG_CAP ORDER BY TEN_NHA_CUNG_CAP"));
                foreach (DataRow r in ncc.Rows)
                    cmbNhaCungCap.Items.Add(Convert.ToString(r["TEN_NHA_CUNG_CAP"]));
            }
            catch
            {
                try
                {
                    NhaCungCapController ctrl = new NhaCungCapController();
                    // Nếu controller có Hienthi ComboBox thì bỏ qua — dùng SQL là chính
                }
                catch { }
            }

            try
            {
                DataService dvt = new DataService();
                dvt.Load(new OleDbCommand("SELECT TEN_DON_VI FROM DON_VI_TINH ORDER BY TEN_DON_VI"));
                foreach (DataRow r in dvt.Rows)
                    cmbLoai.Items.Add(Convert.ToString(r["TEN_DON_VI"]));
            }
            catch { }

            cmbNhaCungCap.SelectedIndex = 0;
            cmbLoai.SelectedIndex = 0;
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            // Nút Xem: tính đến ngày trên ô dt (logic gốc đồ án)
            XemTheoNgayTinh();
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            // Nút Lọc: theo khoảng Từ ngày – Đến ngày HH
            XemTheoKhoang();
        }

        void BoLocDoi(object sender, EventArgs e)
        {
            if (bangGoc == null) return;
            ApDungLoc();
            HienBang("Đã lọc lại");
        }

        // Tính đến ngày — SQL giống factory gốc (1 tham số, không JOIN phức tạp)
        void XemTheoNgayTinh()
        {
            DateTime den = dt.Value.Date;
            int sap = 30;
            try { sap = (int)numSapHet.Value; } catch { }
            DateTime denMoRong = den.AddDays(sap);

            try
            {
                bangGoc = DocDonGian(denMoRong, den);
            }
            catch (Exception ex)
            {
                try
                {
                    bangGoc = DocTuController(denMoRong, den);
                }
                catch (Exception ex2)
                {
                    MessageBox.Show("Không lấy được dữ liệu:\n" + ex.Message + "\n" + ex2.Message,
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try { dtDen.Value = den; } catch { }
            ApDungLoc();
            int goc = bangGoc == null ? 0 : bangGoc.Rows.Count;
            HienBang("Tính đến " + den.ToString("dd/MM/yyyy") + " (CSDL: " + goc + " dòng)");
        }

        void XemTheoKhoang()
        {
            DateTime tu = new DateTime(2000, 1, 1);
            DateTime den = DateTime.Today;
            try { tu = dtTu.Value.Date; } catch { }
            try { den = dtDen.Value.Date; } catch { }
            if (den < tu)
            {
                MessageBox.Show("Đến ngày phải ≥ Từ ngày.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bangGoc = DocDonGian(den, DateTime.Today);
                if (bangGoc != null)
                {
                    DataTable loc = TaoBangTrong();
                    foreach (DataRow r in bangGoc.Rows)
                    {
                        if (r["Ngày hết hạn"] == DBNull.Value) continue;
                        DateTime hh = Convert.ToDateTime(r["Ngày hết hạn"]).Date;
                        if (hh >= tu && hh <= den) loc.ImportRow(r);
                    }
                    bangGoc = loc;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lấy được dữ liệu:\n" + ex.Message,
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ApDungLoc();
            HienBang("Khoảng HH " + tu.ToString("dd/MM/yyyy") + " – " + den.ToString("dd/MM/yyyy"));
        }

        /// <summary>
        /// Giống factory: SELECT * FROM MA_SAN_PHAM WHERE NGAY_HET_HAN &lt;= @ngay (1 tham số).
        /// </summary>
        DataTable DocDonGian(DateTime denLay, DateTime mocTrangThai)
        {
            DataService ds = new DataService();
            OleDbCommand cmd = new OleDbCommand(
                "SELECT * FROM MA_SAN_PHAM WHERE NGAY_HET_HAN <= @ngay");
            cmd.Parameters.Add("ngay", OleDbType.Date).Value = denLay.Date;
            ds.Load(cmd);

            DataTable t = TaoBangTrong();
            int sap = 30;
            try { sap = (int)numSapHet.Value; } catch { }

            Dictionary<string, string[]> cacheSp = new Dictionary<string, string[]>();

            foreach (DataRow r in ds.Rows)
            {
                string idSp = "";
                if (r.Table.Columns.Contains("ID_SAN_PHAM") && r["ID_SAN_PHAM"] != DBNull.Value)
                    idSp = Convert.ToString(r["ID_SAN_PHAM"]);

                string ten = "", dvt = "";
                if (idSp.Length > 0)
                {
                    if (!cacheSp.ContainsKey(idSp))
                    {
                        string[] info = new string[] { "", "" };
                        try
                        {
                            DataService sp = new DataService();
                            OleDbCommand c2 = new OleDbCommand(
                                "SELECT SP.TEN_SAN_PHAM, DVT.TEN_DON_VI FROM SAN_PHAM SP " +
                                "LEFT JOIN DON_VI_TINH DVT ON SP.ID_DON_VI_TINH = DVT.ID WHERE SP.ID = @id");
                            c2.Parameters.Add("id", OleDbType.VarChar, 50).Value = idSp;
                            sp.Load(c2);
                            if (sp.Rows.Count > 0)
                            {
                                if (sp.Rows[0]["TEN_SAN_PHAM"] != DBNull.Value)
                                    info[0] = Convert.ToString(sp.Rows[0]["TEN_SAN_PHAM"]);
                                if (sp.Columns.Contains("TEN_DON_VI") && sp.Rows[0]["TEN_DON_VI"] != DBNull.Value)
                                    info[1] = Convert.ToString(sp.Rows[0]["TEN_DON_VI"]);
                            }
                        }
                        catch { }
                        cacheSp[idSp] = info;
                    }
                    ten = cacheSp[idSp][0];
                    dvt = cacheSp[idSp][1];
                }

                int soLuong = 0;
                try { soLuong = Convert.ToInt32(r["SO_LUONG"]); } catch { }
                long gia = 0;
                try
                {
                    if (r.Table.Columns.Contains("DON_GIA_NHAP") && r["DON_GIA_NHAP"] != DBNull.Value)
                        gia = Convert.ToInt64(r["DON_GIA_NHAP"]);
                }
                catch { }
                DateTime hh = DateTime.MinValue;
                try { hh = Convert.ToDateTime(r["NGAY_HET_HAN"]); } catch { }
                DateTime nn = DateTime.MinValue;
                try { nn = Convert.ToDateTime(r["NGAY_NHAP"]); } catch { }

                DataRow n = t.NewRow();
                n["Mã lô"] = Convert.ToString(r["ID"]);
                n["Mã SP"] = idSp;
                n["Tên SP"] = ten;
                n["Ngày nhập"] = nn == DateTime.MinValue ? (object)DBNull.Value : nn;
                n["Ngày hết hạn"] = hh == DateTime.MinValue ? (object)DBNull.Value : hh;
                n["Số lượng"] = soLuong;
                n["Đơn vị tính"] = dvt;
                n["Giá nhập"] = gia;
                n["Thành tiền"] = (long)soLuong * gia;
                n["Nhà cung cấp"] = "";
                n["Trạng thái"] = TrangThai(hh, mocTrangThai, sap);
                t.Rows.Add(n);
            }
            return t;
        }

        DataTable DocTuController(DateTime denLay, DateTime mocTrangThai)
        {
            IList<MaSanPham> data = MaSanPhamController.LayMaSanPhamHetHan(denLay);
            DataTable t = TaoBangTrong();
            if (data == null) return t;
            int sap = 30;
            try { sap = (int)numSapHet.Value; } catch { }

            foreach (MaSanPham m in data)
            {
                string maSp = "", ten = "", dvt = "";
                try
                {
                    if (m.SanPham != null)
                    {
                        maSp = m.SanPham.Id ?? "";
                        ten = m.SanPham.TenSanPham ?? "";
                        if (m.SanPham.DonViTinh != null)
                            dvt = m.SanPham.DonViTinh.Ten ?? "";
                    }
                }
                catch { }

                long thanhTien = (long)m.SoLuong * m.GiaNhap;
                string tt = TrangThai(m.NgayHetHan, mocTrangThai, sap);

                DataRow n = t.NewRow();
                n["Mã lô"] = m.Id ?? "";
                n["Mã SP"] = maSp;
                n["Tên SP"] = ten;
                n["Ngày nhập"] = m.NgayNhap;
                n["Ngày hết hạn"] = m.NgayHetHan;
                n["Số lượng"] = m.SoLuong;
                n["Đơn vị tính"] = dvt;
                n["Giá nhập"] = m.GiaNhap;
                n["Thành tiền"] = thanhTien;
                n["Nhà cung cấp"] = "";
                n["Trạng thái"] = tt;
                t.Rows.Add(n);
            }
            return t;
        }

        static string TrangThai(DateTime ngayHH, DateTime moc, int soNgaySap)
        {
            // moc = ngày "tính đến" (hoặc hôm nay)
            if (ngayHH.Date <= moc.Date) return "Đã hết hạn";
            if (ngayHH.Date <= moc.Date.AddDays(soNgaySap)) return "Sắp hết hạn";
            return "Còn hạn";
        }

        static DataTable TaoBangTrong()
        {
            DataTable t = new DataTable();
            t.Columns.Add("Mã lô", typeof(string));
            t.Columns.Add("Mã SP", typeof(string));
            t.Columns.Add("Tên SP", typeof(string));
            t.Columns.Add("Ngày nhập", typeof(DateTime));
            t.Columns.Add("Ngày hết hạn", typeof(DateTime));
            t.Columns.Add("Số lượng", typeof(int));
            t.Columns.Add("Đơn vị tính", typeof(string));
            t.Columns.Add("Giá nhập", typeof(long));
            t.Columns.Add("Thành tiền", typeof(long));
            t.Columns.Add("Nhà cung cấp", typeof(string));
            t.Columns.Add("Trạng thái", typeof(string));
            return t;
        }

        void ApDungLoc()
        {
            bangLoc = TaoBangTrong();
            if (bangGoc == null) return;

            string ncc = cmbNhaCungCap == null || cmbNhaCungCap.SelectedIndex <= 0 ? "" : cmbNhaCungCap.Text;
            string loai = cmbLoai == null || cmbLoai.SelectedIndex <= 0 ? "" : cmbLoai.Text;
            string tt = cmbTrangThai == null || cmbTrangThai.SelectedIndex <= 0 ? "" : cmbTrangThai.Text;

            foreach (DataRow r in bangGoc.Rows)
            {
                string maLo = Convert.ToString(r["Mã lô"]);
                if (daXuLy.ContainsKey(maLo) && daXuLy[maLo]) continue;

                if (ncc.Length > 0 && Convert.ToString(r["Nhà cung cấp"]) != ncc) continue;
                if (loai.Length > 0 && Convert.ToString(r["Đơn vị tính"]) != loai) continue;
                if (tt.Length > 0 && Convert.ToString(r["Trạng thái"]) != tt) continue;

                // Khi dùng nút Xem (tính đến ngày): chỉ hiện đã hết hạn + sắp hết hạn
                if (tt.Length == 0)
                {
                    string st = Convert.ToString(r["Trạng thái"]);
                    if (st != "Đã hết hạn" && st != "Sắp hết hạn") continue;
                }

                bangLoc.ImportRow(r);
            }
        }

        void HienBang(string tieuDe)
        {
            if (bangLoc == null) bangLoc = TaoBangTrong();
            luoi.DataSource = bangLoc;

            if (luoi.Columns.Contains("Ngày nhập"))
                luoi.Columns["Ngày nhập"].DefaultCellStyle.Format = "dd/MM/yyyy";
            if (luoi.Columns.Contains("Ngày hết hạn"))
                luoi.Columns["Ngày hết hạn"].DefaultCellStyle.Format = "dd/MM/yyyy";
            if (luoi.Columns.Contains("Giá nhập"))
            {
                luoi.Columns["Giá nhập"].DefaultCellStyle.Format = "N0";
                luoi.Columns["Giá nhập"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (luoi.Columns.Contains("Thành tiền"))
            {
                luoi.Columns["Thành tiền"].DefaultCellStyle.Format = "N0";
                luoi.Columns["Thành tiền"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (luoi.Columns.Contains("Số lượng"))
                luoi.Columns["Số lượng"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            long soSp = 0;
            long tongTien = 0;
            foreach (DataRow r in bangLoc.Rows)
            {
                soSp++;
                tongTien += Convert.ToInt64(r["Thành tiền"]);
            }
            lblTong.Text = "Tổng SP hết hạn / sắp hết: " + soSp.ToString("N0")
                + "     Tổng giá trị hàng: " + tongTien.ToString("N0");

            if (bangLoc.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy dữ liệu.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                GanTrangThai("Không tìm thấy dữ liệu.");
            }
            else
            {
                GanTrangThai(tieuDe + " — " + bangLoc.Rows.Count + " dòng. Đỏ = đã hết hạn, Vàng = sắp hết hạn.");
            }
        }

        void luoi_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || luoi.Rows[e.RowIndex].DataBoundItem == null) return;
            DataRowView drv = luoi.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;
            string tt = Convert.ToString(drv["Trạng thái"]);
            if (tt == "Đã hết hạn")
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 205, 210); // đỏ nhạt
                e.CellStyle.SelectionBackColor = Color.FromArgb(198, 40, 40);
            }
            else if (tt == "Sắp hết hạn")
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 249, 196); // vàng
                e.CellStyle.SelectionBackColor = Color.FromArgb(249, 168, 37);
            }
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

        private void btnXuLy_Click(object sender, EventArgs e)
        {
            if (luoi.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn ít nhất một dòng để đánh dấu đã xử lý.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Đánh dấu " + luoi.SelectedRows.Count + " dòng đã xử lý (ẩn khỏi danh sách)?",
                this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            foreach (DataGridViewRow row in luoi.SelectedRows)
            {
                DataRowView drv = row.DataBoundItem as DataRowView;
                if (drv == null) continue;
                string maLo = Convert.ToString(drv["Mã lô"]);
                if (maLo.Length > 0) daXuLy[maLo] = true;
            }
            ApDungLoc();
            HienBang("Đã ẩn các dòng đánh dấu xử lý");
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (bangLoc == null || bangLoc.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy dữ liệu.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel (*.xls)|*.xls";
            dlg.FileName = "SanPhamHetHan.xls";
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
            sb.AppendLine("<Worksheet ss:Name=\"HetHan\"><Table>");
            sb.Append("<Row>");
            foreach (DataColumn c in bangLoc.Columns)
                sb.Append("<Cell><Data ss:Type=\"String\">").Append(Xml(c.ColumnName)).Append("</Data></Cell>");
            sb.AppendLine("</Row>");
            foreach (DataRow r in bangLoc.Rows)
            {
                sb.Append("<Row>");
                foreach (DataColumn c in bangLoc.Columns)
                {
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
                MessageBox.Show("Không tìm thấy dữ liệu.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Drawing.Printing.PrintDocument doc = new System.Drawing.Printing.PrintDocument();
            doc.DocumentName = "Sản phẩm hết hạn";
            int dong = 0;
            doc.PrintPage += delegate(object s, System.Drawing.Printing.PrintPageEventArgs ev)
            {
                float y = 40;
                using (Font title = new Font("Segoe UI", 14f, FontStyle.Bold))
                using (Font f = new Font("Segoe UI", 9f))
                {
                    ev.Graphics.DrawString("Sản phẩm hết hạn / sắp hết hạn", title, Brushes.DarkGreen, 40, y);
                    y += 32;
                    ev.Graphics.DrawString(lblTong.Text, f, Brushes.Black, 40, y);
                    y += 28;
                    while (dong < bangLoc.Rows.Count && y < ev.MarginBounds.Bottom - 20)
                    {
                        DataRow r = bangLoc.Rows[dong];
                        string line = Convert.ToString(r["Mã SP"]) + " | " + Convert.ToString(r["Tên SP"])
                            + " | SL: " + r["Số lượng"] + " " + Convert.ToString(r["Đơn vị tính"])
                            + " | HH: " + (r["Ngày hết hạn"] == DBNull.Value ? "" : Convert.ToDateTime(r["Ngày hết hạn"]).ToString("dd/MM/yyyy"))
                            + " | " + Convert.ToString(r["Trạng thái"])
                            + " | " + Convert.ToInt64(r["Thành tiền"]).ToString("N0");
                        Color mau = Convert.ToString(r["Trạng thái"]) == "Đã hết hạn" ? Color.DarkRed : Color.DarkGoldenrod;
                        using (SolidBrush br = new SolidBrush(mau))
                            ev.Graphics.DrawString(line, f, br, 40, y);
                        y += 18;
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
            GanTrangThai("Đã mở trang in. Chọn Microsoft Print to PDF để lưu PDF.");
        }
    }
}
