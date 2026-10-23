using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-24: Danh sách phiếu bán sỉ — cột mở rộng (tính/hiển thị nếu có),
    /// lọc, xuất CSV/Excel, double-click, audit log file. Không bắt ALTER SQL.
    /// </summary>
    public partial class frmDanhsachPhieuBanSi : Form
    {
        PhieuBanController ctrl = new PhieuBanController();
        KhachHangController ctrlKH = new KhachHangController();
        frmBanSi BanSi = null;
        bool m_CotDaThem = false;
        ToolStripComboBox cboLocTrangThai;
        ToolStripLabel lblLocTT;


        public frmDanhsachPhieuBanSi()
        {
            InitializeComponent();
        }

        private void frmDanhsachPhieuNhap_Load(object sender, EventArgs e)
        {
            this.Text = "Danh sách phiếu bán sỉ";
            this.MaximizeBox = true;
            try { bindingNavigator.CountItemFormat = " / {0} bản ghi"; } catch { }

            dataGridView.AutoGenerateColumns = false;
            WireXemLai();
            ThemNutPM23();
            BoSungCotPM23();
            CauHinhLuoiPM23();
            TaiLai();
        }

        void ThemNutPM23()
        {
            try
            {
                if (bindingNavigator.Items["toolXuatExcel"] != null) return;
            }
            catch { }

            try
            {
                ToolStripSeparator sep = new ToolStripSeparator();
                bindingNavigator.Items.Add(sep);

                lblLocTT = new ToolStripLabel("Trạng thái:");
                bindingNavigator.Items.Add(lblLocTT);

                cboLocTrangThai = new ToolStripComboBox();
                cboLocTrangThai.Name = "cboLocTrangThai";
                cboLocTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
                cboLocTrangThai.Items.AddRange(new object[] { "Tất cả", "Đã thanh toán", "Còn nợ" });
                cboLocTrangThai.SelectedIndex = 0;
                cboLocTrangThai.Size = new Size(110, 25);
                cboLocTrangThai.SelectedIndexChanged += cboLocTrangThai_SelectedIndexChanged;
                bindingNavigator.Items.Add(cboLocTrangThai);

                ToolStripButton btnXuat = new ToolStripButton();
                btnXuat.Name = "toolXuatExcel";
                btnXuat.Text = "Xuất Excel";
                btnXuat.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                try { btnXuat.Image = global::CuahangNongduoc.Properties.Resources.printer; } catch { }
                btnXuat.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                btnXuat.TextImageRelation = TextImageRelation.ImageAboveText;
                btnXuat.Click += toolXuatExcel_Click;
                bindingNavigator.Items.Add(btnXuat);

                ToolStripButton btnAudit = new ToolStripButton();
                btnAudit.Name = "toolAudit";
                btnAudit.Text = "Nhật ký";
                btnAudit.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                try { btnAudit.Image = global::CuahangNongduoc.Properties.Resources.reload; } catch { }
                btnAudit.TextImageRelation = TextImageRelation.ImageAboveText;
                btnAudit.Click += toolAudit_Click;
                bindingNavigator.Items.Add(btnAudit);
            }
            catch { }
        }

        void BoSungCotPM23()
        {
            if (m_CotDaThem) return;
            m_CotDaThem = true;
            try
            {
                ThemCotText("colTrangThai", "Trạng thái", 90);
                ThemCotText("colHinhThucTT", "Hình thức TT", 100);
                ThemCotText("colChietKhau", "Chiết khấu", 80);
                ThemCotText("colPhiVC", "Phí VC", 80);
                ThemCotText("colVAT", "VAT", 60);
                ThemCotText("colNhanVien", "NV lập phiếu", 100);
                ThemCotText("colNguoiTao", "Người tạo", 90);
                ThemCotText("colHanMucNo", "Hạn mức nợ ĐL", 100);
                ThemCotText("colSoHDVAT", "Số HĐ VAT", 90);
            }
            catch { }
        }

        void ThemCotText(string name, string header, int width)
        {
            if (dataGridView.Columns.Contains(name)) return;
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
            c.Name = name;
            c.HeaderText = header;
            c.Width = width;
            c.ReadOnly = true;
            dataGridView.Columns.Add(c);
        }

        void CauHinhLuoiPM23()
        {
            try
            {
                dataGridView.AutoGenerateColumns = false;
                dataGridView.AllowUserToOrderColumns = true;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.MultiSelect = false;
                // Sắp xếp theo cột (khi unbound cột ảo có thể hạn chế)
                foreach (DataGridViewColumn col in dataGridView.Columns)
                {
                    try { col.SortMode = DataGridViewColumnSortMode.Automatic; } catch { }
                }
                dataGridView.CellDoubleClick -= dataGridView_CellDoubleClick;
                dataGridView.CellDoubleClick += dataGridView_CellDoubleClick;
                dataGridView.DataBindingComplete -= dataGridView_DataBindingComplete;
                dataGridView.DataBindingComplete += dataGridView_DataBindingComplete;
                // Sửa nhanh: cho phép sửa Đã trả
                try
                {
                    dataGridView.ReadOnly = false;
                    foreach (DataGridViewColumn c in dataGridView.Columns)
                        c.ReadOnly = true;
                    if (dataGridView.Columns.Contains("colDaTra"))
                        dataGridView.Columns["colDaTra"].ReadOnly = false;
                }
                catch { }
                dataGridView.CellEndEdit -= dataGridView_CellEndEdit;
                dataGridView.CellEndEdit += dataGridView_CellEndEdit;
                dataGridView.DataError -= dataGridView_DataError;
                dataGridView.DataError += dataGridView_DataError;
            }
            catch { }
        }

        private void WireXemLai()
        {
            foreach (ToolStripItem it in bindingNavigator.Items)
            {
                if (it is ToolStripButton)
                {
                    string n = (it.Name ?? "") + " " + (it.Text ?? "");
                    if (n.IndexOf("Xem", StringComparison.OrdinalIgnoreCase) >= 0
                        || n.IndexOf("reload", StringComparison.OrdinalIgnoreCase) >= 0
                        || n.IndexOf("LamMoi", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        it.Click -= toolXemLai_Click;
                        it.Click += toolXemLai_Click;
                    }
                }
            }
        }

        private void TaiLai()
        {
            try
            {
                dataGridView.AutoGenerateColumns = false;
                ctrlKH.HienthiDaiLyDataGridviewComboBox(colKhachhang);
                ctrl.HienthiPhieuBanSi(bindingNavigator, dataGridView);
                AnCotTuSinh();
                CapNhatCotAoPM23();
                try { ApDungLocTrangThai(); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải danh sách bán sỉ:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        /// <summary>Ẩn cột auto-generate trùng tên field DB (giữ cột tiếng Việt đã thêm).</summary>

        void AnCotTuSinh()
        {
            try
            {
                dataGridView.AutoGenerateColumns = false;

                // Chi giu cot thiet ke (Name bat dau "col") — xoa cot tu sinh ten field DB
                System.Collections.Generic.List<DataGridViewColumn> xoa =
                    new System.Collections.Generic.List<DataGridViewColumn>();
                foreach (DataGridViewColumn col in dataGridView.Columns)
                {
                    if (col == null) continue;
                    string n = col.Name ?? "";
                    if (n.StartsWith("col")) continue; // cot Designer / PM
                    // Cot auto-generate: Name = DataPropertyName (CHIET_KHAU, VAT...)
                    xoa.Add(col);
                }
                foreach (DataGridViewColumn col in xoa)
                {
                    try { dataGridView.Columns.Remove(col); } catch { try { col.Visible = false; } catch { } }
                }

                // Gan DataPropertyName de hien gia tri DB neu co
                GanDP("colChietKhau", "CHIET_KHAU");
                GanDP("colPhiVC", "PHI_VAN_CHUYEN");
                GanDP("colVAT", "VAT");
                GanDP("colHinhThucTT", "HINH_THUC_TT");
                GanDP("colNhanVien", "NHAN_VIEN");
                GanDP("colNguoiTao", "NGUOI_TAO");
                try { GanDP("colSoHDVAT", "SO_HD_VAT"); } catch { }
                try { GanDP("colHanMucNo", "HAN_MUC_NO"); } catch { }
            }
            catch { }
        }

        void GanDP(string colName, string field)
        {
            try
            {
                if (!dataGridView.Columns.Contains(colName)) return;
                DataGridViewColumn c = dataGridView.Columns[colName];
                if (c != null)
                {
                    c.DataPropertyName = field;
                    c.Visible = true;
                }
            }
            catch { }
        }

        void CapNhatCotAoPM23()
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataRowView rv = row.DataBoundItem as DataRowView;
                    long conNo = 0, daTra = 0, tong = 0;
                    try
                    {
                        if (rv != null)
                        {
                            if (rv.Row.Table.Columns.Contains("CON_NO"))
                                conNo = Convert.ToInt64(rv["CON_NO"] == DBNull.Value ? 0 : rv["CON_NO"]);
                            if (rv.Row.Table.Columns.Contains("DA_TRA"))
                                daTra = Convert.ToInt64(rv["DA_TRA"] == DBNull.Value ? 0 : rv["DA_TRA"]);
                            if (rv.Row.Table.Columns.Contains("TONG_TIEN"))
                                tong = Convert.ToInt64(rv["TONG_TIEN"] == DBNull.Value ? 0 : rv["TONG_TIEN"]);
                        }
                    }
                    catch { }

                    // Trạng thái suy ra từ còn nợ (không cần cột DB)
                    string tt = "Đã thanh toán";
                    if (conNo > 0) tt = "Còn nợ";
                    if (tong == 0 && daTra == 0) tt = "Nháp / trống";
                    SetCell(row, "colTrangThai", tt);

                    // Cột DB tùy chọn — chỉ hiện nếu có
                    SetCellTuData(row, rv, "colChietKhau", "CHIET_KHAU");
                    SetCellTuData(row, rv, "colPhiVC", "PHI_VAN_CHUYEN");
                    SetCellTuData(row, rv, "colVAT", "VAT");
                    SetCellTuData(row, rv, "colHinhThucTT", "HINH_THUC_TT");
                    SetCellTuData(row, rv, "colNhanVien", "NHAN_VIEN");
                    SetCellTuData(row, rv, "colNguoiTao", "NGUOI_TAO");
                    SetCellTuData(row, rv, "colSoHDVAT", "SO_HD_VAT");
                    if (GetCell(row, "colHinhThucTT") == "")
                        SetCell(row, "colHinhThucTT", conNo > 0 ? "Công nợ" : "Tiền mặt");
                    // Hạn mức công nợ đại lý (nếu có cột hoặc lấy từ KH)
                    try
                    {
                        string hm = "";
                        if (rv != null && rv.Row.Table.Columns.Contains("HAN_MUC_NO"))
                        {
                            object v = rv["HAN_MUC_NO"];
                            if (v != null && v != DBNull.Value) hm = Convert.ToString(v);
                        }
                        if (hm == "" && rv != null && rv.Row.Table.Columns.Contains("ID_KHACH_HANG"))
                        {
                            try
                            {
                                string idKH = Convert.ToString(rv["ID_KHACH_HANG"]);
                                KhachHangController ckh = new KhachHangController();
                                KhachHang kh = ckh.LayKhachHang(idKH);
                                if (kh != null)
                                {
                                    try
                                    {
                                        System.Reflection.PropertyInfo pi = kh.GetType().GetProperty("HanMucNo");
                                        if (pi == null) pi = kh.GetType().GetProperty("HanMucCongNo");
                                        if (pi != null)
                                        {
                                            object val = pi.GetValue(kh, null);
                                            if (val != null) hm = Convert.ToString(val);
                                        }
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }
                        if (hm != "") SetCell(row, "colHanMucNo", hm);
                    }
                    catch { }
                }
            }
            catch { }
        }

        void SetCellTuData(DataGridViewRow row, DataRowView rv, string colGrid, string colData)
        {
            try
            {
                if (rv == null || !rv.Row.Table.Columns.Contains(colData)) return;
                if (!dataGridView.Columns.Contains(colGrid)) return;
                object v = rv[colData];
                if (v == null || v == DBNull.Value)
                {
                    // Khong ghi chuoi rong vao cot so (Decimal/Money) — de null
                    try { row.Cells[colGrid].Value = DBNull.Value; } catch { }
                    return;
                }
                // Neu cot co DataPropertyName thi de binding hien, chi set cot ao
                DataGridViewColumn gc = dataGridView.Columns[colGrid];
                if (gc != null && !string.IsNullOrEmpty(gc.DataPropertyName))
                    return; // da bind, khong ghi de
                SetCell(row, colGrid, Convert.ToString(v));
            }
            catch { }
        }

        void SetCell(DataGridViewRow row, string col, string val)
        {
            try
            {
                if (dataGridView.Columns.Contains(col))
                    row.Cells[col].Value = val;
            }
            catch { }
        }

        string GetCell(DataGridViewRow row, string col)
        {
            try
            {
                if (dataGridView.Columns.Contains(col) && row.Cells[col].Value != null)
                    return Convert.ToString(row.Cells[col].Value);
            }
            catch { }
            return "";
        }

        void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            AnCotTuSinh();
            CapNhatCotAoPM23();
        }

        private void toolXemLai_Click(object sender, EventArgs e)
        {
            TaiLai();
            GhiAudit("Làm mới danh sách phiếu bán sỉ");
        }

        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            MoChiTiet();
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            MoChiTiet();
        }

        void MoChiTiet()
        {
            try
            {
                if (BanSi == null || BanSi.IsDisposed)
                {
                    BanSi = new frmBanSi(ctrl);
                    BanSi.Show();
                }
                else BanSi.Activate();
                GhiAudit("Mở chi tiết phiếu bán sỉ (double-click / Thêm)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            if (BanSi == null || BanSi.IsDisposed)
            {
                BanSi = new frmBanSi();
                BanSi.Show();
            }
            else BanSi.Activate();
            GhiAudit("Thêm phiếu bán sỉ");
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            try
            {
                DataRowView view = (DataRowView)bindingNavigator.BindingSource.Current;
                if (view == null) return;
                string id = Convert.ToString(view["ID"]);
                ChiTietPhieuBanController ctCtrl = new ChiTietPhieuBanController();
                System.Collections.IList ds = ctCtrl.ChiTietPhieuBan(id) as System.Collections.IList;
                if (ds != null)
                {
                    foreach (ChiTietPhieuBan ct in ds)
                    {
                        try
                        {
                            if (ct.MaSanPham != null)
                                CuahangNongduoc.DataLayer.MaSanPhanFactory.CapNhatSoLuong(ct.MaSanPham.Id, ct.SoLuong);
                        }
                        catch { }
                    }
                }
                GhiAudit("Xóa phiếu bán sỉ: " + id);
            }
            catch { }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa phiếu đang chọn?", this.Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                DataRowView view = bindingNavigator.BindingSource.Current as DataRowView;
                if (view == null) return;
                string id = Convert.ToString(view["ID"]);
                bindingNavigator.BindingSource.RemoveCurrent();
                ctrl.Save();
                GhiAudit("Xóa phiếu bán sỉ (toolbar): " + id);
                CapNhatCotAoPM23();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiLai();
            }
        }

        void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                DataGridViewColumn col = dataGridView.Columns[e.ColumnIndex];
                if (col == null) return;
                if (col.Name != "colDaTra" && col.DataPropertyName != "DA_TRA") return;

                DataRowView rv = dataGridView.Rows[e.RowIndex].DataBoundItem as DataRowView;
                if (rv == null) return;

                long daTra = 0, tong = 0;
                try { daTra = Convert.ToInt64(rv["DA_TRA"]); } catch { }
                try { tong = Convert.ToInt64(rv["TONG_TIEN"]); } catch { }
                long conNo = tong - daTra;
                if (conNo < 0) conNo = 0;
                if (rv.Row.Table.Columns.Contains("CON_NO"))
                    rv["CON_NO"] = conNo;
                ctrl.Save();
                CapNhatCotAoPM23();
                GhiAudit("Sửa nhanh Đã trả phiếu " + Convert.ToString(rv["ID"]) + " → " + daTra);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được chỉnh sửa:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void toolPrint_Click(object sender, EventArgs e)
        {
            toolIn_Click(sender, e);
        }

        private void toolIn_Click(object sender, EventArgs e)
        {
            try
            {
                string maPhieu = LayMaPhieuDangChon();
                if (maPhieu == null || maPhieu.Length == 0)
                {
                    MessageBox.Show("Chọn một phiếu để in.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                PhieuBan ph = null;
                try { ph = ctrl.LayPhieuBan(maPhieu); }
                catch (Exception exLay)
                {
                    MessageBox.Show("Không lấy được dữ liệu phiếu " + maPhieu + ":\n" + exLay.Message,
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (ph == null)
                {
                    MessageBox.Show("Không tìm thấy phiếu: " + maPhieu, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Chỉ truyền mã — form in tự nạp lại từ DB (tránh DataRow đã xóa)
                frmInPhieuBan frm = new frmInPhieuBan(ph);
                frm.Show();
                GhiAudit("In phiếu bán sỉ: " + maPhieu);
            }
            catch (Exception ex)
            {
                MessageBox.Show("In lỗi:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>Lấy mã phiếu an toàn (không đụng DataRow đã Delete).</summary>
        string LayMaPhieuDangChon()
        {
            try
            {
                if (dataGridView.CurrentRow != null && !dataGridView.CurrentRow.IsNewRow)
                {
                    if (dataGridView.Columns.Contains("colId"))
                    {
                        object v = dataGridView.CurrentRow.Cells["colId"].Value;
                        if (v != null && v != DBNull.Value)
                        {
                            string s = Convert.ToString(v);
                            if (s != null && s.Length > 0) return s;
                        }
                    }
                }
            }
            catch { }
            try
            {
                DataRowView row = bindingNavigator.BindingSource != null
                    ? bindingNavigator.BindingSource.Current as DataRowView : null;
                if (row == null) return "";
                if (row.Row.RowState == DataRowState.Deleted || row.Row.RowState == DataRowState.Detached)
                    return "";
                object id = null;
                try { id = row.Row["ID", DataRowVersion.Current]; }
                catch
                {
                    try { id = row.Row["ID", DataRowVersion.Original]; } catch { }
                }
                if (id == null || id == DBNull.Value) return "";
                return Convert.ToString(id);
            }
            catch { return ""; }
        }

        private void toolTimKiem_Click(object sender, EventArgs e)
        {
            frmTimPhieuBanLe Tim = new frmTimPhieuBanLe(true);
            Tim.StartPosition = FormStartPosition.CenterParent;
            if (Tim.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                string maKH = Tim.cmbNCC.SelectedValue != null
                    ? Tim.cmbNCC.SelectedValue.ToString() : "";
                DateTime tu = Tim.dtTuNgay != null ? Tim.dtTuNgay.Value.Date : Tim.dtNgayNhap.Value.Date;
                DateTime den = Tim.dtNgayNhap.Value.Date;
                int n = 0;
                try { n = ctrl.TimPhieuBanKhoang(maKH, tu, den); }
                catch
                {
                    if (maKH.Length == 0)
                    {
                        MessageBox.Show(
                            "Tiêu chí tìm: khoảng ngày + (tùy chọn) đại lý.\nCần cập nhật TimPhieuBanKhoang hoặc chọn đại lý.",
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    n = ctrl.TimPhieuBan(maKH, den);
                }
                if (bindingNavigator.BindingSource == null)
                {
                    try { ctrl.HienthiPhieuBanSi(bindingNavigator, dataGridView); } catch { }
                }
                CapNhatCotAoPM23();
                GhiAudit(string.Format("Tìm phiếu bán sỉ ĐL={0} từ {1:dd/MM/yyyy} đến {2:dd/MM/yyyy} → {3} dòng",
                    maKH, tu, den, n));

                if (n == 0)
                {
                    MessageBox.Show(
                        "Không tìm thấy phiếu trong khoảng đã chọn.\nDanh sách sẽ được tải lại toàn bộ.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TaiLai();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tìm lỗi:\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiLai();
            }
        }

        void toolXuatExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "CSV Excel (*.csv)|*.csv|Tất cả (*.*)|*.*";
                dlg.FileName = "DanhSachPhieuBanSi_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                StringBuilder sb = new StringBuilder();
                // Header
                for (int c = 0; c < dataGridView.Columns.Count; c++)
                {
                    if (!dataGridView.Columns[c].Visible) continue;
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(Csv(dataGridView.Columns[c].HeaderText));
                }
                sb.AppendLine();

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.IsNewRow) continue;
                    StringBuilder line = new StringBuilder();
                    for (int c = 0; c < dataGridView.Columns.Count; c++)
                    {
                        if (!dataGridView.Columns[c].Visible) continue;
                        if (line.Length > 0) line.Append(",");
                        object v = row.Cells[c].FormattedValue;
                        if (v == null) v = row.Cells[c].Value;
                        line.Append(Csv(v == null ? "" : Convert.ToString(v)));
                    }
                    sb.AppendLine(line.ToString());
                }

                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                GhiAudit("Xuất Excel/CSV: " + dlg.FileName);
                MessageBox.Show("Đã xuất file:\n" + dlg.FileName, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xuất file lỗi:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        static string Csv(string s)
        {
            if (s == null) s = "";
            if (s.IndexOfAny(new char[] { ',', '"', '\n', '\r' }) >= 0)
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        void toolAudit_Click(object sender, EventArgs e)
        {
            try
            {
                string path = LayDuongDanAudit();
                if (!File.Exists(path))
                {
                    MessageBox.Show("Chưa có nhật ký.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string noiDung = File.ReadAllText(path, Encoding.UTF8);
                Form f = new Form();
                f.Text = "Nhật ký thay đổi — Phiếu bán sỉ";
                f.Width = 640;
                f.Height = 420;
                f.StartPosition = FormStartPosition.CenterParent;
                TextBox tb = new TextBox();
                tb.Multiline = true;
                tb.ReadOnly = true;
                tb.ScrollBars = ScrollBars.Both;
                tb.Dock = DockStyle.Fill;
                tb.Font = new Font("Consolas", 9f);
                tb.Text = noiDung;
                f.Controls.Add(tb);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        void cboLocTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApDungLocTrangThai();
        }

        void ApDungLocTrangThai()
        {
            try
            {
                if (bindingNavigator.BindingSource == null) return;
                string chon = "Tất cả";
                try
                {
                    if (cboLocTrangThai != null && cboLocTrangThai.SelectedItem != null)
                        chon = cboLocTrangThai.SelectedItem.ToString();
                }
                catch { }

                if (chon == "Còn nợ")
                    bindingNavigator.BindingSource.Filter = "CON_NO > 0";
                else if (chon == "Đã thanh toán")
                    bindingNavigator.BindingSource.Filter = "CON_NO <= 0 OR CON_NO IS NULL";
                else
                    bindingNavigator.BindingSource.Filter = "";

                CapNhatCotAoPM23();
                GhiAudit("Lọc trạng thái: " + chon);
            }
            catch
            {
                try { LocThuCong(); }
                catch { }
            }
        }

        void LocThuCong()
        {
            string chon = "Tất cả";
            try
            {
                if (cboLocTrangThai != null && cboLocTrangThai.SelectedItem != null)
                    chon = cboLocTrangThai.SelectedItem.ToString();
            }
            catch { }
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                long conNo = 0;
                try
                {
                    DataRowView rv = row.DataBoundItem as DataRowView;
                    if (rv != null && rv.Row.Table.Columns.Contains("CON_NO") && rv["CON_NO"] != DBNull.Value)
                        conNo = Convert.ToInt64(rv["CON_NO"]);
                }
                catch { }
                bool hien = true;
                if (chon == "Còn nợ") hien = conNo > 0;
                else if (chon == "Đã thanh toán") hien = conNo <= 0;
                try { row.Visible = hien; } catch { }
            }
        }


        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            try { e.Cancel = true; } catch { }
        }

        void GhiAudit(string hanhDong)
        {
            try
            {
                string path = LayDuongDanAudit();
                string line = string.Format("{0:yyyy-MM-dd HH:mm:ss}\t{1}\r\n", DateTime.Now, hanhDong);
                File.AppendAllText(path, line, Encoding.UTF8);
            }
            catch { }
        }

        string LayDuongDanAudit()
        {
            string dir = Application.StartupPath;
            return Path.Combine(dir, "audit_phieu_ban_si.log");
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}
