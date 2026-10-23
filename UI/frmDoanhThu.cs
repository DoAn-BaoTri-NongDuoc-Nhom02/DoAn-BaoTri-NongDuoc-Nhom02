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

namespace CuahangNongduoc
{
    /// <summary>
    /// Form Doanh thu – đủ ticket [36]: lọc khoảng ngày / KH / SP / kênh,
    /// bảng + biểu đồ, tổng DT / lãi gộp / chi phí / lãi ròng, xuất Excel / PDF, drill-down.
    /// Không phụ thuộc ReportViewer / ThongKeDoanhThu.
    /// </summary>
    public partial class frmDoanhThu : Form
    {
        DataTable _data;
        DataGridView dgv;
        Panel panelFilter;
        Panel panelBottom;
        DateTimePicker dtTuNgay;
        DateTimePicker dtDenNgay;
        ComboBox cmbKhachHang;
        ComboBox cmbSanPham;
        ComboBox cmbKenh;
        ComboBox cmbBieuDo;
        Label lblTongHop;
        BieuDoDoanhThu bieuDo;
        Button btnXem;
        Button btnXuatExcel;
        Button btnPdf;
        Button btnThoat;

        public frmDoanhThu()
        {
            InitializeComponent();
        }

        private void frmDoanhThu_Load(object sender, EventArgs e)
        {
            this.Text = "Doanh thu";
            this.BackColor = Color.FromArgb(236, 248, 236);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new Size(980, 560);
            if (this.ClientSize.Width < 980) this.ClientSize = new Size(1000, 640);

            AnDieuKhienCu();
            XayGiaoDienMoi();
            NapDanhMuc();

            // Mặc định tháng hiện tại
            dtTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtDenNgay.Value = DateTime.Now.Date;
            try
            {
                if (comboBox1 != null) comboBox1.SelectedIndex = DateTime.Now.Month - 1;
                if (numericUpDown1 != null) numericUpDown1.Value = DateTime.Now.Year;
            }
            catch { }

            // Nối nút Designer cũ (tháng / năm / 1 ngày) nếu còn
            NoiNutCu();

            TaiDuLieu();
        }

        void AnDieuKhienCu()
        {
            try
            {
                if (reportViewer != null)
                {
                    reportViewer.Visible = false;
                    reportViewer.ShowToolBar = false;
                    reportViewer.Dock = DockStyle.None;
                    reportViewer.Size = new Size(1, 1);
                }
            }
            catch { }
            try { if (panel1 != null) panel1.Visible = false; } catch { }
            try { if (panel2 != null) { panel2.Visible = true; panel2.Controls.Clear(); } } catch { }
        }

        void NoiNutCu()
        {
            try
            {
                if (button1 != null) button1.Click += delegate { // Xem 1 ngày
                    dateTimePicker1.Value = dateTimePicker1.Value.Date;
                    dtTuNgay.Value = dateTimePicker1.Value.Date;
                    dtDenNgay.Value = dateTimePicker1.Value.Date;
                    TaiDuLieu();
                };
            }
            catch { }
            try
            {
                if (button2 != null) button2.Click += delegate { // Xem tháng
                    int th = comboBox1.SelectedIndex + 1;
                    if (th < 1) th = DateTime.Now.Month;
                    int nam = Convert.ToInt32(numericUpDown1.Value);
                    dtTuNgay.Value = new DateTime(nam, th, 1);
                    dtDenNgay.Value = new DateTime(nam, th, 1).AddMonths(1).AddDays(-1);
                    TaiDuLieu();
                };
            }
            catch { }
            try
            {
                if (button3 != null) button3.Click += delegate { // Xem năm
                    int nam = Convert.ToInt32(numericUpDown1.Value);
                    dtTuNgay.Value = new DateTime(nam, 1, 1);
                    dtDenNgay.Value = new DateTime(nam, 12, 31);
                    TaiDuLieu();
                };
            }
            catch { }
        }

        void XayGiaoDienMoi()
        {
            panelFilter = new Panel();
            panelFilter.Dock = DockStyle.Top;
            panelFilter.Height = 108;
            panelFilter.BackColor = Color.FromArgb(232, 245, 233);

            Label title = new Label();
            title.Text = "BÁO CÁO DOANH THU";
            title.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(27, 94, 32);
            title.AutoSize = true;
            title.Location = new Point(12, 8);

            Label l1 = new Label(); l1.Text = "Từ ngày"; l1.AutoSize = true; l1.Location = new Point(12, 42);
            dtTuNgay = new DateTimePicker();
            dtTuNgay.Format = DateTimePickerFormat.Custom;
            dtTuNgay.CustomFormat = "dd/MM/yyyy";
            dtTuNgay.Location = new Point(70, 38);
            dtTuNgay.Width = 110;

            Label l2 = new Label(); l2.Text = "Đến ngày"; l2.AutoSize = true; l2.Location = new Point(190, 42);
            dtDenNgay = new DateTimePicker();
            dtDenNgay.Format = DateTimePickerFormat.Custom;
            dtDenNgay.CustomFormat = "dd/MM/yyyy";
            dtDenNgay.Location = new Point(255, 38);
            dtDenNgay.Width = 110;

            Label l3 = new Label(); l3.Text = "Khách hàng"; l3.AutoSize = true; l3.Location = new Point(380, 42);
            cmbKhachHang = new ComboBox();
            cmbKhachHang.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKhachHang.Location = new Point(460, 38);
            cmbKhachHang.Width = 180;

            Label l4 = new Label(); l4.Text = "Kênh"; l4.AutoSize = true; l4.Location = new Point(655, 42);
            cmbKenh = new ComboBox();
            cmbKenh.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKenh.Items.Add("Tất cả");
            cmbKenh.Items.Add("Bán sỉ");
            cmbKenh.Items.Add("Bán lẻ");
            cmbKenh.SelectedIndex = 0;
            cmbKenh.Location = new Point(695, 38);
            cmbKenh.Width = 100;

            Label l5 = new Label(); l5.Text = "Sản phẩm"; l5.AutoSize = true; l5.Location = new Point(12, 74);
            cmbSanPham = new ComboBox();
            cmbSanPham.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSanPham.Location = new Point(80, 70);
            cmbSanPham.Width = 200;

            Label l6 = new Label(); l6.Text = "Biểu đồ"; l6.AutoSize = true; l6.Location = new Point(300, 74);
            cmbBieuDo = new ComboBox();
            cmbBieuDo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBieuDo.Items.Add("Cột");
            cmbBieuDo.Items.Add("Đường");
            cmbBieuDo.Items.Add("Tròn");
            cmbBieuDo.SelectedIndex = 0;
            cmbBieuDo.Location = new Point(355, 70);
            cmbBieuDo.Width = 90;
            cmbBieuDo.SelectedIndexChanged += delegate { CapNhatBieuDo(); };

            btnXem = TaoNut("Xem", new Point(460, 68), Color.FromArgb(46, 125, 50));
            btnXem.Click += btnXem_Click;
            btnXuatExcel = TaoNut("Xuất Excel", new Point(555, 68), Color.FromArgb(21, 101, 192));
            btnXuatExcel.Click += btnXuatExcel_Click;
            btnPdf = TaoNut("Xuất PDF", new Point(665, 68), Color.FromArgb(123, 31, 162));
            btnPdf.Click += btnPdf_Click;
            btnThoat = TaoNut("Thoát", new Point(775, 68), Color.FromArgb(97, 97, 97));
            btnThoat.Click += delegate { this.Close(); };

            panelFilter.Controls.Add(title);
            panelFilter.Controls.Add(l1); panelFilter.Controls.Add(dtTuNgay);
            panelFilter.Controls.Add(l2); panelFilter.Controls.Add(dtDenNgay);
            panelFilter.Controls.Add(l3); panelFilter.Controls.Add(cmbKhachHang);
            panelFilter.Controls.Add(l4); panelFilter.Controls.Add(cmbKenh);
            panelFilter.Controls.Add(l5); panelFilter.Controls.Add(cmbSanPham);
            panelFilter.Controls.Add(l6); panelFilter.Controls.Add(cmbBieuDo);
            panelFilter.Controls.Add(btnXem);
            panelFilter.Controls.Add(btnXuatExcel);
            panelFilter.Controls.Add(btnPdf);
            panelFilter.Controls.Add(btnThoat);

            panelBottom = new Panel();
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 40;
            panelBottom.BackColor = Color.FromArgb(255, 243, 224);
            lblTongHop = new Label();
            lblTongHop.Dock = DockStyle.Fill;
            lblTongHop.TextAlign = ContentAlignment.MiddleLeft;
            lblTongHop.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblTongHop.ForeColor = Color.FromArgb(191, 54, 12);
            lblTongHop.Padding = new Padding(10, 0, 0, 0);
            lblTongHop.Text = "Tổng DT: 0 | Lãi gộp: 0 | Chi phí: 0 | Lãi ròng: 0 | Số phiếu: 0";
            panelBottom.Controls.Add(lblTongHop);

            bieuDo = new BieuDoDoanhThu();
            bieuDo.Dock = DockStyle.Right;
            bieuDo.Width = 320;
            bieuDo.BackColor = Color.White;

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 125, 90);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 32;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
            dgv.CellDoubleClick += dgv_CellDoubleClick;

            StatusStrip ss = new StatusStrip();
            ss.BackColor = Color.FromArgb(220, 242, 220);
            ss.SizingGrip = false;
            ToolStripStatusLabel st = new ToolStripStatusLabel("Sẵn sàng. Double-click dòng để xem chi tiết phiếu.");
            st.Spring = true;
            st.TextAlign = ContentAlignment.MiddleLeft;
            ss.Items.Add(st);

            // panel2 nếu còn thì dùng làm host; không thì form
            Control host = null;
            try { host = panel2; } catch { }
            if (host == null) host = this;

            if (host != this)
            {
                host.Controls.Clear();
                host.Controls.Add(dgv);
                host.Controls.Add(bieuDo);
            }
            else
            {
                this.Controls.Add(dgv);
                this.Controls.Add(bieuDo);
            }

            this.Controls.Add(panelBottom);
            this.Controls.Add(panelFilter);
            this.Controls.Add(ss);
            panelFilter.BringToFront();
        }

        static Button TaoNut(string text, Point p, Color bg)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = p;
            b.Size = new Size(100, 28);
            b.BackColor = bg;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        void NapDanhMuc()
        {
            // Khách hàng
            try
            {
                DataService ds = new DataService();
                ds.Load(new OleDbCommand("SELECT ID, HO_TEN FROM KHACH_HANG ORDER BY HO_TEN"));
                DataTable tbl = ds.Copy();
                DataRow all = tbl.NewRow();
                all["ID"] = "";
                all["HO_TEN"] = "-- Tất cả --";
                tbl.Rows.InsertAt(all, 0);
                cmbKhachHang.DisplayMember = "HO_TEN";
                cmbKhachHang.ValueMember = "ID";
                cmbKhachHang.DataSource = tbl;
            }
            catch
            {
                cmbKhachHang.DataSource = null;
                cmbKhachHang.Items.Clear();
                cmbKhachHang.Items.Add("-- Tất cả --");
                cmbKhachHang.SelectedIndex = 0;
            }

            // Sản phẩm
            try
            {
                DataService ds = new DataService();
                ds.Load(new OleDbCommand("SELECT ID, TEN_SAN_PHAM FROM SAN_PHAM ORDER BY TEN_SAN_PHAM"));
                DataTable tbl = ds.Copy();
                DataRow all = tbl.NewRow();
                all["ID"] = "";
                all["TEN_SAN_PHAM"] = "-- Tất cả --";
                tbl.Rows.InsertAt(all, 0);
                cmbSanPham.DisplayMember = "TEN_SAN_PHAM";
                cmbSanPham.ValueMember = "ID";
                cmbSanPham.DataSource = tbl;
            }
            catch
            {
                cmbSanPham.DataSource = null;
                cmbSanPham.Items.Clear();
                cmbSanPham.Items.Add("-- Tất cả --");
                cmbSanPham.SelectedIndex = 0;
            }
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            TaiDuLieu();
        }

        void TaiDuLieu()
        {
            if (dtTuNgay.Value.Date > dtDenNgay.Value.Date)
            {
                MessageBox.Show("Từ ngày phải ≤ Đến ngày.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _data = DocDoanhThu(dtTuNgay.Value.Date, dtDenNgay.Value.Date);
                dgv.DataSource = _data;
                DatTenCot();
                CapNhatTong();
                CapNhatBieuDo();

                if (_data == null || _data.Rows.Count == 0)
                    MessageBox.Show("Không tìm thấy dữ liệu.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải doanh thu:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        DataTable DocDoanhThu(DateTime tu, DateTime den)
        {
            // 1 tham số ngày tối đa → tránh lỗi OleDb; lọc trong bộ nhớ cho KH/SP/kênh
            DataService ds = new DataService();
            string sql =
                "SELECT PB.ID, PB.NGAY_BAN, PB.TONG_TIEN, PB.DA_TRA, PB.CON_NO, " +
                "PB.ID_KHACH_HANG, KH.HO_TEN, KH.LOAI_KH " +
                "FROM PHIEU_BAN PB " +
                "LEFT JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID " +
                "WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den";

            // den exclusive = den+1 ngày
            OleDbCommand cmd = new OleDbCommand(sql);
            cmd.Parameters.Add("tu", OleDbType.Date).Value = tu;
            cmd.Parameters.Add("den", OleDbType.Date).Value = den.AddDays(1);

            try
            {
                ds.Load(cmd);
            }
            catch
            {
                // Fallback 1 tham số: lấy đến den rồi lọc từ ngày trong RAM
                ds = new DataService();
                OleDbCommand cmd2 = new OleDbCommand(
                    "SELECT PB.ID, PB.NGAY_BAN, PB.TONG_TIEN, PB.DA_TRA, PB.CON_NO, " +
                    "PB.ID_KHACH_HANG, KH.HO_TEN, KH.LOAI_KH " +
                    "FROM PHIEU_BAN PB LEFT JOIN KHACH_HANG KH ON PB.ID_KHACH_HANG = KH.ID " +
                    "WHERE PB.NGAY_BAN <= @den");
                cmd2.Parameters.Add("den", OleDbType.Date).Value = den.AddDays(1).AddSeconds(-1);
                ds.Load(cmd2);
            }

            string idKh = LayGiaTriCombo(cmbKhachHang);
            string idSp = LayGiaTriCombo(cmbSanPham);
            int kenh = cmbKenh == null ? 0 : cmbKenh.SelectedIndex; // 0 all, 1 sỉ, 2 lẻ

            // Chi phí (phiếu chi) trong khoảng
            long tongChi = LayTongChi(tu, den);

            // Cache giá vốn theo phiếu (nếu lọc SP thì chỉ tính SP đó)
            Dictionary<string, long[]> vonVaSp = TinhGiaVonVaLocSp(tu, den, idSp);

            DataTable t = new DataTable();
            t.Columns.Add("MaPhieu", typeof(string));
            t.Columns.Add("NgayBan", typeof(DateTime));
            t.Columns.Add("TenKhachHang", typeof(string));
            t.Columns.Add("Kenh", typeof(string));
            t.Columns.Add("TongTien", typeof(long));
            t.Columns.Add("DaTra", typeof(long));
            t.Columns.Add("ConNo", typeof(long));
            t.Columns.Add("GiaVon", typeof(long));
            t.Columns.Add("LaiGop", typeof(long));

            foreach (DataRow r in ds.Rows)
            {
                DateTime ngay = r["NGAY_BAN"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["NGAY_BAN"]).Date;
                if (ngay < tu || ngay > den) continue;

                string maKh = r["ID_KHACH_HANG"] == DBNull.Value ? "" : Convert.ToString(r["ID_KHACH_HANG"]);
                if (idKh.Length > 0 && maKh != idKh) continue;

                bool laDaiLy = false;
                try
                {
                    if (r["LOAI_KH"] != DBNull.Value)
                        laDaiLy = Convert.ToBoolean(r["LOAI_KH"]);
                }
                catch
                {
                    try { laDaiLy = Convert.ToInt32(r["LOAI_KH"]) != 0; } catch { }
                }
                if (kenh == 1 && !laDaiLy) continue; // bán sỉ = đại lý
                if (kenh == 2 && laDaiLy) continue;

                string maPhieu = Convert.ToString(r["ID"]);
                long giaVon = 0;
                bool coSp = true;
                if (vonVaSp.ContainsKey(maPhieu))
                {
                    giaVon = vonVaSp[maPhieu][0];
                    coSp = vonVaSp[maPhieu][1] > 0;
                }
                else if (idSp.Length > 0)
                {
                    coSp = false; // lọc SP mà phiếu không có SP đó
                }
                if (!coSp) continue;

                long tong = r["TONG_TIEN"] == DBNull.Value ? 0L : Convert.ToInt64(r["TONG_TIEN"]);
                // Nếu lọc theo SP: doanh thu = thành tiền chi tiết SP (ước lượng qua vonVaSp[2] nếu có)
                if (idSp.Length > 0 && vonVaSp.ContainsKey(maPhieu) && vonVaSp[maPhieu].Length > 2)
                    tong = vonVaSp[maPhieu][2];

                long daTra = r["DA_TRA"] == DBNull.Value ? 0L : Convert.ToInt64(r["DA_TRA"]);
                long conNo = r["CON_NO"] == DBNull.Value ? 0L : Convert.ToInt64(r["CON_NO"]);
                long lai = tong - giaVon;

                DataRow n = t.NewRow();
                n["MaPhieu"] = maPhieu;
                n["NgayBan"] = ngay;
                n["TenKhachHang"] = r["HO_TEN"] == DBNull.Value ? "" : Convert.ToString(r["HO_TEN"]);
                n["Kenh"] = laDaiLy ? "Bán sỉ" : "Bán lẻ";
                n["TongTien"] = tong;
                n["DaTra"] = daTra;
                n["ConNo"] = conNo;
                n["GiaVon"] = giaVon;
                n["LaiGop"] = lai;
                t.Rows.Add(n);
            }

            // Gắn chi phí tổng vào Tag để CapNhatTong dùng
            t.ExtendedProperties["TongChi"] = tongChi;
            return t;
        }

        /// <summary>
        /// Trả về dictionary: mã phiếu → [giaVon, coSanPhamLoc(1/0), thanhTienSp]
        /// </summary>
        Dictionary<string, long[]> TinhGiaVonVaLocSp(DateTime tu, DateTime den, string idSp)
        {
            Dictionary<string, long[]> map = new Dictionary<string, long[]>();
            try
            {
                DataService ds = new DataService();
                string sql =
                    "SELECT CT.ID_PHIEU_BAN, CT.SO_LUONG, CT.DON_GIA, CT.THANH_TIEN, " +
                    "MSP.DON_GIA_NHAP, MSP.ID_SAN_PHAM, PB.NGAY_BAN " +
                    "FROM CHI_TIET_PHIEU_BAN CT " +
                    "INNER JOIN PHIEU_BAN PB ON CT.ID_PHIEU_BAN = PB.ID " +
                    "LEFT JOIN MA_SAN_PHAM MSP ON CT.ID_MA_SAN_PHAM = MSP.ID " +
                    "WHERE PB.NGAY_BAN >= @tu AND PB.NGAY_BAN < @den";
                OleDbCommand cmd = new OleDbCommand(sql);
                cmd.Parameters.Add("tu", OleDbType.Date).Value = tu;
                cmd.Parameters.Add("den", OleDbType.Date).Value = den.AddDays(1);
                try { ds.Load(cmd); }
                catch
                {
                    ds = new DataService();
                    OleDbCommand cmd2 = new OleDbCommand(
                        "SELECT CT.ID_PHIEU_BAN, CT.SO_LUONG, CT.DON_GIA, CT.THANH_TIEN, " +
                        "MSP.DON_GIA_NHAP, MSP.ID_SAN_PHAM " +
                        "FROM CHI_TIET_PHIEU_BAN CT " +
                        "LEFT JOIN MA_SAN_PHAM MSP ON CT.ID_MA_SAN_PHAM = MSP.ID");
                    ds.Load(cmd2);
                }

                foreach (DataRow r in ds.Rows)
                {
                    string ma = Convert.ToString(r["ID_PHIEU_BAN"]);
                    string sp = r.Table.Columns.Contains("ID_SAN_PHAM") && r["ID_SAN_PHAM"] != DBNull.Value
                        ? Convert.ToString(r["ID_SAN_PHAM"]) : "";
                    if (idSp.Length > 0 && sp != idSp) continue;

                    int sl = 0;
                    try { sl = Convert.ToInt32(r["SO_LUONG"]); } catch { }
                    long giaNhap = 0;
                    try
                    {
                        if (r["DON_GIA_NHAP"] != DBNull.Value) giaNhap = Convert.ToInt64(r["DON_GIA_NHAP"]);
                    }
                    catch { }
                    long thanhTien = 0;
                    try
                    {
                        if (r["THANH_TIEN"] != DBNull.Value) thanhTien = Convert.ToInt64(r["THANH_TIEN"]);
                        else if (r["DON_GIA"] != DBNull.Value) thanhTien = Convert.ToInt64(r["DON_GIA"]) * sl;
                    }
                    catch { }

                    long von = giaNhap * sl;
                    if (!map.ContainsKey(ma))
                        map[ma] = new long[] { 0, 0, 0 };
                    map[ma][0] += von;
                    map[ma][1] = 1;
                    map[ma][2] += thanhTien;
                }
            }
            catch { }
            return map;
        }

        long LayTongChi(DateTime tu, DateTime den)
        {
            try
            {
                DataService ds = new DataService();
                OleDbCommand cmd = new OleDbCommand(
                    "SELECT TONG_TIEN, NGAY_CHI FROM PHIEU_CHI WHERE NGAY_CHI >= @tu AND NGAY_CHI < @den");
                cmd.Parameters.Add("tu", OleDbType.Date).Value = tu;
                cmd.Parameters.Add("den", OleDbType.Date).Value = den.AddDays(1);
                try { ds.Load(cmd); }
                catch
                {
                    ds = new DataService();
                    ds.Load(new OleDbCommand("SELECT TONG_TIEN, NGAY_CHI FROM PHIEU_CHI"));
                }
                long tong = 0;
                foreach (DataRow r in ds.Rows)
                {
                    try
                    {
                        if (r.Table.Columns.Contains("NGAY_CHI") && r["NGAY_CHI"] != DBNull.Value)
                        {
                            DateTime n = Convert.ToDateTime(r["NGAY_CHI"]).Date;
                            if (n < tu || n > den) continue;
                        }
                        if (r["TONG_TIEN"] != DBNull.Value) tong += Convert.ToInt64(r["TONG_TIEN"]);
                    }
                    catch { }
                }
                return tong;
            }
            catch { return 0; }
        }

        static string LayGiaTriCombo(ComboBox cmb)
        {
            if (cmb == null) return "";
            try
            {
                object v = cmb.SelectedValue;
                if (v != null && v != DBNull.Value)
                {
                    string s = Convert.ToString(v);
                    if (s != null && s.Length > 0) return s;
                }
            }
            catch { }
            return "";
        }

        void DatTenCot()
        {
            if (dgv.Columns.Count == 0) return;
            string[][] map = {
                new[]{"MaPhieu","Mã phiếu"},
                new[]{"NgayBan","Ngày bán"},
                new[]{"TenKhachHang","Khách hàng"},
                new[]{"Kenh","Kênh"},
                new[]{"TongTien","Doanh thu"},
                new[]{"DaTra","Đã thu"},
                new[]{"ConNo","Còn nợ"},
                new[]{"GiaVon","Giá vốn"},
                new[]{"LaiGop","Lãi gộp"}
            };
            foreach (string[] m in map)
                if (dgv.Columns.Contains(m[0])) dgv.Columns[m[0]].HeaderText = m[1];
            if (dgv.Columns.Contains("NgayBan"))
                dgv.Columns["NgayBan"].DefaultCellStyle.Format = "dd/MM/yyyy";
            foreach (string c in new[] { "TongTien", "DaTra", "ConNo", "GiaVon", "LaiGop" })
            {
                if (dgv.Columns.Contains(c))
                {
                    dgv.Columns[c].DefaultCellStyle.Format = "N0";
                    dgv.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }

        void CapNhatTong()
        {
            long tong = 0, daTra = 0, conNo = 0, giaVon = 0, laiGop = 0;
            int so = 0;
            if (_data != null)
            {
                so = _data.Rows.Count;
                foreach (DataRow r in _data.Rows)
                {
                    try { tong += Convert.ToInt64(r["TongTien"]); } catch { }
                    try { daTra += Convert.ToInt64(r["DaTra"]); } catch { }
                    try { conNo += Convert.ToInt64(r["ConNo"]); } catch { }
                    try { giaVon += Convert.ToInt64(r["GiaVon"]); } catch { }
                    try { laiGop += Convert.ToInt64(r["LaiGop"]); } catch { }
                }
            }
            long chi = 0;
            try
            {
                if (_data != null && _data.ExtendedProperties.Contains("TongChi"))
                    chi = Convert.ToInt64(_data.ExtendedProperties["TongChi"]);
            }
            catch { }
            long laiRong = laiGop - chi;

            lblTongHop.Text = string.Format(
                "Doanh thu: {0:N0}  |  Giá vốn: {1:N0}  |  Lãi gộp: {2:N0}  |  Chi phí: {3:N0}  |  Lãi ròng: {4:N0}  |  Đã thu: {5:N0}  |  Còn nợ: {6:N0}  |  Phiếu: {7}",
                tong, giaVon, laiGop, chi, laiRong, daTra, conNo, so);
        }

        void CapNhatBieuDo()
        {
            if (bieuDo == null) return;
            bieuDo.Kieu = cmbBieuDo == null ? 0 : cmbBieuDo.SelectedIndex;
            bieuDo.Bang = _data;
            bieuDo.Invalidate();
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _data == null) return;
            try
            {
                string ma = Convert.ToString(dgv.Rows[e.RowIndex].Cells["MaPhieu"].Value);
                string kh = Convert.ToString(dgv.Rows[e.RowIndex].Cells["TenKhachHang"].Value);
                string kenh = Convert.ToString(dgv.Rows[e.RowIndex].Cells["Kenh"].Value);
                string ngay = "";
                try { ngay = Convert.ToDateTime(dgv.Rows[e.RowIndex].Cells["NgayBan"].Value).ToString("dd/MM/yyyy"); } catch { }
                long tt = 0, cn = 0, lg = 0;
                try { tt = Convert.ToInt64(dgv.Rows[e.RowIndex].Cells["TongTien"].Value); } catch { }
                try { cn = Convert.ToInt64(dgv.Rows[e.RowIndex].Cells["ConNo"].Value); } catch { }
                try { lg = Convert.ToInt64(dgv.Rows[e.RowIndex].Cells["LaiGop"].Value); } catch { }

                // Chi tiết dòng hàng
                StringBuilder ct = new StringBuilder();
                try
                {
                    DataService ds = new DataService();
                    OleDbCommand cmd = new OleDbCommand(
                        "SELECT CT.ID_MA_SAN_PHAM, CT.SO_LUONG, CT.DON_GIA, CT.THANH_TIEN, SP.TEN_SAN_PHAM " +
                        "FROM CHI_TIET_PHIEU_BAN CT " +
                        "LEFT JOIN MA_SAN_PHAM MSP ON CT.ID_MA_SAN_PHAM = MSP.ID " +
                        "LEFT JOIN SAN_PHAM SP ON MSP.ID_SAN_PHAM = SP.ID " +
                        "WHERE CT.ID_PHIEU_BAN = @id");
                    cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = ma;
                    ds.Load(cmd);
                    foreach (DataRow r in ds.Rows)
                    {
                        string ten = r.Table.Columns.Contains("TEN_SAN_PHAM") && r["TEN_SAN_PHAM"] != DBNull.Value
                            ? Convert.ToString(r["TEN_SAN_PHAM"]) : Convert.ToString(r["ID_MA_SAN_PHAM"]);
                        ct.AppendLine(string.Format("  • {0}  SL={1}  ĐG={2:N0}  TT={3:N0}",
                            ten, r["SO_LUONG"], r["DON_GIA"], r["THANH_TIEN"]));
                    }
                }
                catch (Exception ex)
                {
                    ct.AppendLine("(Không đọc được chi tiết: " + ex.Message + ")");
                }

                MessageBox.Show(
                    "Phiếu: " + ma + "\nNgày: " + ngay + "\nKhách: " + kh + "\nKênh: " + kenh +
                    "\nDoanh thu: " + tt.ToString("N0") + "\nCòn nợ: " + cn.ToString("N0") +
                    "\nLãi gộp: " + lg.ToString("N0") + "\n\nChi tiết hàng:\n" + ct.ToString(),
                    "Chi tiết phiếu bán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (_data == null || _data.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu. Hãy nhấn Xem trước.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel (*.xls)|*.xls";
            dlg.FileName = string.Format("DoanhThu_{0:yyyyMMdd}_{1:yyyyMMdd}.xls", dtTuNgay.Value, dtDenNgay.Value);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                File.WriteAllText(dlg.FileName, TaoExcelXml(), Encoding.UTF8);
                MessageBox.Show("Đã xuất: " + dlg.FileName, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xuất lỗi: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        string TaoExcelXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
            sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
            sb.AppendLine("<Worksheet ss:Name=\"DoanhThu\"><Table>");
            sb.Append("<Row>");
            foreach (DataColumn c in _data.Columns)
                sb.Append("<Cell><Data ss:Type=\"String\">").Append(Xml(c.ColumnName)).Append("</Data></Cell>");
            sb.AppendLine("</Row>");
            foreach (DataRow r in _data.Rows)
            {
                sb.Append("<Row>");
                foreach (DataColumn c in _data.Columns)
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
            if (_data == null || _data.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để in / PDF.", this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Drawing.Printing.PrintDocument doc = new System.Drawing.Printing.PrintDocument();
            doc.DocumentName = "Doanh thu";
            int dong = 0;
            string tieu = string.Format("Doanh thu {0:dd/MM/yyyy} – {1:dd/MM/yyyy} | {2}",
                dtTuNgay.Value, dtDenNgay.Value, cmbKenh.Text);
            string tong = lblTongHop.Text;
            doc.PrintPage += delegate(object s, System.Drawing.Printing.PrintPageEventArgs ev)
            {
                float y = 40;
                using (Font title = new Font("Segoe UI", 12f, FontStyle.Bold))
                using (Font f = new Font("Segoe UI", 8.5f))
                {
                    ev.Graphics.DrawString(tieu, title, Brushes.DarkGreen, 40, y);
                    y += 28;
                    ev.Graphics.DrawString(tong, f, Brushes.Black, 40, y);
                    y += 24;
                    while (dong < _data.Rows.Count && y < ev.MarginBounds.Bottom - 20)
                    {
                        DataRow r = _data.Rows[dong];
                        string line = string.Format("{0}  {1:dd/MM/yyyy}  {2}  {3}  DT:{4:N0}  Lãi:{5:N0}",
                            r["MaPhieu"],
                            r["NgayBan"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["NgayBan"]),
                            Convert.ToString(r["TenKhachHang"]),
                            Convert.ToString(r["Kenh"]),
                            r["TongTien"], r["LaiGop"]);
                        ev.Graphics.DrawString(line, f, Brushes.Black, 40, y);
                        y += 18;
                        dong++;
                    }
                    ev.HasMorePages = dong < _data.Rows.Count;
                }
            };
            PrintPreviewDialog xem = new PrintPreviewDialog();
            xem.Document = doc;
            xem.Width = 900;
            xem.Height = 700;
            xem.ShowDialog(this);
        }
    }

    /// <summary>Biểu đồ doanh thu theo ngày – không dùng Func&lt;&gt;.</summary>
    public class BieuDoDoanhThu : Panel
    {
        public int Kieu;
        public DataTable Bang;

        public BieuDoDoanhThu()
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

            if (Bang == null || Bang.Rows.Count == 0)
            {
                using (Font f = new Font("Segoe UI", 9f))
                    g.DrawString("Không có dữ liệu biểu đồ.", f, Brushes.Gray, 12, 12);
                return;
            }

            // Gom doanh thu theo ngày
            SortedDictionary<string, long> theoNgay = new SortedDictionary<string, long>();
            foreach (DataRow r in Bang.Rows)
            {
                string key = "";
                try { key = Convert.ToDateTime(r["NgayBan"]).ToString("dd/MM"); } catch { key = "?"; }
                long v = 0;
                try { v = Convert.ToInt64(r["TongTien"]); } catch { }
                if (!theoNgay.ContainsKey(key)) theoNgay[key] = 0;
                theoNgay[key] += v;
            }
            if (theoNgay.Count == 0) return;

            List<string> keys = new List<string>();
            foreach (string k in theoNgay.Keys) keys.Add(k);
            long max = 1;
            foreach (long v in theoNgay.Values) if (v > max) max = v;

            Rectangle vung = new Rectangle(36, 20, Math.Max(20, Width - 52), Math.Max(20, Height - 50));
            Color[] mau = new Color[] {
                Color.FromArgb(46, 125, 90), Color.FromArgb(33, 115, 185), Color.DarkOrange,
                Color.MediumPurple, Color.IndianRed, Color.Teal
            };

            using (Font f = new Font("Segoe UI", 8f, FontStyle.Bold))
                g.DrawString("Doanh thu theo ngày", f, Brushes.DarkGreen, 8, 4);

            if (Kieu == 2)
            {
                float tong = 0;
                foreach (long v in theoNgay.Values) tong += v;
                if (tong <= 0) tong = 1;
                float goc = -90;
                int canh = Math.Min(vung.Width, vung.Height) - 10;
                Rectangle tron = new Rectangle(vung.X + 10, vung.Y, canh, canh);
                int i = 0;
                foreach (string k in keys)
                {
                    float quet = (float)(theoNgay[k] * 360.0 / tong);
                    using (SolidBrush b = new SolidBrush(mau[i % mau.Length]))
                        g.FillPie(b, tron, goc, quet);
                    goc += quet;
                    i++;
                }
                return;
            }

            int n = keys.Count;
            float bw = Math.Max(6, (vung.Width - 8) / (float)n);
            using (Pen truc = new Pen(Color.Silver))
                g.DrawLine(truc, vung.Left, vung.Bottom, vung.Right, vung.Bottom);

            List<PointF> diem = new List<PointF>();
            for (int i = 0; i < n; i++)
            {
                long v = theoNgay[keys[i]];
                float h = (float)v / max * (vung.Height - 10);
                float x = vung.Left + i * bw;
                if (Kieu == 0)
                {
                    using (SolidBrush b = new SolidBrush(mau[i % mau.Length]))
                        g.FillRectangle(b, x + 2, vung.Bottom - h, Math.Max(4, bw - 5), h);
                }
                diem.Add(new PointF(x + bw / 2f, vung.Bottom - h));
            }
            if (Kieu == 1 && diem.Count > 1)
            {
                using (Pen p = new Pen(Color.FromArgb(33, 115, 185), 2f))
                    g.DrawLines(p, diem.ToArray());
                foreach (PointF pt in diem)
                    g.FillEllipse(Brushes.DarkOrange, pt.X - 3, pt.Y - 3, 6, 6);
            }
        }
    }
}
