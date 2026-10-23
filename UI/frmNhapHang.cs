using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    public partial class frmNhapHang : Form
    {
        SanPhamController ctrlSanPham = new SanPhamController();
        PhieuNhapController ctrl = new PhieuNhapController();
        MaSanPhamController ctrlMaSP = new MaSanPhamController();
        DataTable m_ChiTiet = null;                         // toàn bộ MA_SAN_PHAM, sửa trực tiếp trên bảng này
        BindingSource m_BsChiTiet = new BindingSource();    // lọc ra các dòng của phiếu đang xem
        NhaCungCapController ctrlNCC = new NhaCungCapController();

        string m_TenNhanVien = Environment.UserName;
        const string TIEU_DE = "Nhập hàng";
        Controll status = Controll.Normal;

        bool m_MoPhieuMoi = false;      // mở form là vào thẳng chế độ tạo phiếu mới
        bool m_DangNap = false;         // đang tải danh sách, bỏ qua CurrentChanged
        bool m_DangHoanVi = false;      // đang trả con trỏ về chỗ cũ, tránh gọi lồng nhau
        string m_IdDangSua = "";        // mã phiếu đang sửa
        int m_ViTriCu = -1;             // vị trí phiếu trước khi bắt đầu nhập / sửa

        // Hình chụp phiếu lúc bắt đầu sửa, để biết đã đổi gì (cho nhật ký và hỏi lưu khi đóng)
        string m_SnapNcc = "";
        string m_SnapTenNcc = "";
        decimal m_SnapDaTra = 0;
        decimal m_SnapTongTien = 0;
        Dictionary<string, string> m_DongGoc = new Dictionary<string, string>();

        public frmNhapHang()
        {
            InitializeComponent();
        }

        // Mở thẳng ở chế độ tạo phiếu mới
        public frmNhapHang(bool phieuMoi)
            : this()
        {
            m_MoPhieuMoi = phieuMoi;
        }

        // Mở từ form danh sách: đứng đúng phiếu đang chọn bên đó
        public frmNhapHang(PhieuNhapController ctrlPN)
            : this()
        {
            if (ctrlPN != null) this.ctrl = ctrlPN;
        }

        // ---------- Tải dữ liệu ----------

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            this.Text = TIEU_DE;
            dataGridView.AutoGenerateColumns = false;

            // Nút Hủy phải có hình thì mới hiện trên thanh công cụ (nút không có hình và kiểu chỉ-hình sẽ vô hình)
            try
            {
                toolHuy.Image = Properties.Resources.undo;
                toolHuy.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                toolHuy.TextImageRelation = TextImageRelation.ImageAboveText;
                toolHuy.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                toolHuy.Visible = true;
                toolHuy.AutoSize = true;
            }
            catch (Exception) { }

            try
            {
                bindingNavigator.CountItemFormat = "/ {0}";

            // PM-15: nut Tim kiem tren form Nhap hang
            ThemNutTimKiem();
            ThemNutIn();

            }
            catch (Exception) { }

            KhoiTaoDanhMuc();

            string idChon = "";
            try { idChon = ctrl.IdHienTai; }
            catch (Exception) { }

            try
            {
                TaiDanhSach(idChon);
            }
            catch (Exception ex)
            {
                // Lỗi trong Load thường bị Windows nuốt mất khiến form trống, nên báo ra cho biết nguyên nhân
                BaoLoiHeThong("Không tải được danh sách phiếu nhập", ex);
            }

            if (m_MoPhieuMoi) BatDauPhieuMoi();
            else XemPhieuHienTai();
        }

        private void KhoiTaoDanhMuc()
        {
            try
            {
                ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
                ctrlSanPham.HienthiDataGridViewComboBoxColumn(colSanPham);
                ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không tải được danh sách sản phẩm / nhà cung cấp", ex);
            }
        }

        private void TaiDanhSach(string idChon)
        {
            m_ChiTiet = null;       // nạp lại chi tiết từ CSDL (bỏ mọi dòng thêm / xóa chưa lưu)
            m_DangNap = true;
            try
            {
                ctrl.TaiDanhSach(bindingNavigator, idChon);
                bindingNavigator.BindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                bindingNavigator.BindingSource.CurrentChanged += BindingSource_CurrentChanged;
            }
            finally
            {
                m_DangNap = false;
            }
        }

        // Chuyển phiếu bằng thanh điều hướng: hiện phiếu mới chọn. Đang nhập / sửa thì không cho chuyển.
        void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (m_DangNap || m_DangHoanVi) return;

            if (status != Controll.Normal)
            {
                m_DangHoanVi = true;
                try { ctrl.DatViTri(m_ViTriCu); }
                finally { m_DangHoanVi = false; }
                ThongBao("Đang nhập / sửa phiếu. Hãy Lưu hoặc Hủy trước khi chuyển sang phiếu khác.");
                return;
            }

            HienPhieu();
            ThongBao("Đang xem phiếu " + txtMaPhieu.Text + ".");
        }

        // ---------- Hiển thị phiếu ----------

        private void XemPhieuHienTai()
        {
            status = Controll.Normal;
            if (errProvider != null) errProvider.Clear();

            if (ctrl.SoPhieu == 0)
            {
                HienTrong();
                return;
            }

            HienPhieu();
            Allow(false);
            ThongBao("Đang xem phiếu " + txtMaPhieu.Text + ". Bấm Thêm để tạo phiếu mới, Chỉnh sửa để sửa phiếu này.");
        }

        private void HienTrong()
        {
            status = Controll.Normal;
            txtMaPhieu.Text = "";
            cmbNhaCungCap.SelectedIndex = -1;
            numTongTien.Value = 0;
            numDaTra.Value = 0;
            numConNo.Value = 0;
            dataGridView.DataSource = null;
            Allow(false);
            CapNhatTongDong();
            ThongBao("Chưa có phiếu nhập nào. Bấm Thêm để tạo phiếu nhập đầu tiên.");
        }

        static decimal KepSo(object val, NumericUpDown num)
        {
            decimal d = ToDecimal(val);
            if (d < num.Minimum) d = num.Minimum;
            if (d > num.Maximum) d = num.Maximum;
            return d;
        }

        static DateTime KepNgay(object val)
        {
            if (val == null || val == DBNull.Value) return DateTime.Today;
            DateTime d;
            try { d = Convert.ToDateTime(val); }
            catch (Exception) { return DateTime.Today; }
            if (d < new DateTime(1753, 1, 1)) return DateTime.Today;
            return d;
        }

        // Điền các ô thông tin phiếu và danh sách sản phẩm từ phiếu đang đứng
        private void HienPhieu()
        {
            DataRowView r = ctrl.HienTai;
            if (r == null)
            {
                HienTrong();
                return;
            }

            string id = Convert.ToString(r["ID"]);
            txtMaPhieu.Text = id;

            object ncc = r["ID_NHA_CUNG_CAP"];
            if (ncc == null || ncc == DBNull.Value) cmbNhaCungCap.SelectedIndex = -1;
            else cmbNhaCungCap.SelectedValue = ncc;

            dtNgayNhap.Value = KepNgay(r["NGAY_NHAP"]);
            numTongTien.Value = KepSo(r["TONG_TIEN"], numTongTien);
            numDaTra.Value = KepSo(r["DA_TRA"], numDaTra);
            numConNo.Value = KepSo(r["CON_NO"], numConNo);
            if (errProvider != null) errProvider.Clear();

            try
            {
                HienChiTiet(id);
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không tải được các sản phẩm của phiếu " + id, ex);
            }
            CapNhatTongDong();
        }

        // ---------- Thông báo / trạng thái (giống Khách hàng) ----------

        private void ThongBao(string noiDung)
        {
            if (lblThongBao != null)
                lblThongBao.Text = noiDung ?? "";
        }

        private void BaoLoiHeThong(string viec, Exception ex)
        {
            MessageBox.Show(viec + ":\n" + ex.Message, TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            ThongBao(viec + ": " + ex.Message);
        }

        private void CapNhatTongDong()
        {
            int n = 0;
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (!r.IsNewRow) n++;
            }
            if (lblTong != null)
                lblTong.Text = "Tổng dòng: " + n;
        }

        private void CapNhatCheDo(bool dangSua)
        {
            if (lblCheDo == null) return;
            if (status == Controll.AddNew) lblCheDo.Text = "Chế độ THÊM MỚI";
            else if (status == Controll.Edit) lblCheDo.Text = "Chế độ SỬA";
            else lblCheDo.Text = "Chế độ xem";
            lblCheDo.ForeColor = dangSua ? Color.Firebrick : SystemColors.ControlText;
        }

        // Nút nào bấm được tùy theo đang xem hay đang nhập / sửa
        private void CapNhatNut()
        {
            bool xem = (status == Controll.Normal);
            bool coPhieu = ctrl.SoPhieu > 0;

            toolLuuThem.Enabled = xem;
            toolChinhsua.Enabled = xem && coPhieu;
            toolXoa.Enabled = xem && coPhieu;
            toolHuy.Enabled = !xem;
            toolLuuThoat.Enabled = true;
            toolNhatKy.Enabled = true;
            toolThoat.Enabled = true;
        }

        // ---------- Ngày tháng ----------

        void KhoiTaoGiaTriMacDinh()
        {
            dtNgayNhap.Value = DateTime.Today;
            dtNgaySanXuat.MaxDate = DateTime.Today;
            dtNgaySanXuat.Value = DateTime.Today;
            dtNgayHetHan.Value = DateTime.Today.AddYears(1);

            cmbNhaCungCap.SelectedIndex = -1;
            cmbSanPham.SelectedIndex = -1;
            txtMaSo.Clear();
            numGiaNhap.Value = 0;
            numSoLuong.Value = 0;
            numThanhTien.Value = 0;
            numTongTien.Value = 0;
            numDaTra.Value = 0;
            numConNo.Value = 0;

            if (errProvider != null) errProvider.Clear();
            KiemTraNgay();
        }

        void dtNgay_ValueChanged(object sender, EventArgs e)
        {
            KiemTraNgay();
        }

        bool KiemTraNgay()
        {
            bool hopLe = true;
            DateTime nhap = dtNgayNhap.Value.Date;
            DateTime sx = dtNgaySanXuat.Value.Date;
            DateTime hh = dtNgayHetHan.Value.Date;

            if (errProvider == null) return hopLe;

            if (sx > nhap)
            {
                errProvider.SetError(dtNgaySanXuat, "Ngày sản xuất không được sau ngày nhập");
                hopLe = false;
            }
            else
                errProvider.SetError(dtNgaySanXuat, "");

            if (hh <= sx)
            {
                errProvider.SetError(dtNgayHetHan, "Ngày hết hạn phải lớn hơn ngày sản xuất");
                hopLe = false;
            }
            else if (hh <= nhap)
            {
                errProvider.SetError(dtNgayHetHan, "Ngày hết hạn phải sau ngày nhập");
                hopLe = false;
            }
            else
                errProvider.SetError(dtNgayHetHan, "");

            return hopLe;
        }

        // ---------- Validation ----------

        bool ChonNhaCungCap()
        {
            return cmbNhaCungCap.SelectedValue != null
                && Convert.ToString(cmbNhaCungCap.SelectedValue).Trim().Length > 0;
        }

        bool ChonSanPham()
        {
            return cmbSanPham.SelectedValue != null
                && Convert.ToString(cmbSanPham.SelectedValue).Trim().Length > 0;
        }

        void BaoLoi(string noiDung, Control focus)
        {
            MessageBox.Show(noiDung, TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (focus != null) focus.Focus();
        }

        bool KiemTraDong(string maSo)
        {
            if (!ChonNhaCungCap())
            {
                BaoLoi("Vui lòng chọn Nhà cung cấp!", cmbNhaCungCap);
                return false;
            }
            if (!ChonSanPham())
            {
                BaoLoi("Vui lòng chọn Sản phẩm!", cmbSanPham);
                return false;
            }
            if (maSo.Length <= 0)
            {
                BaoLoi("Vui lòng nhập Mã số!", txtMaSo);
                return false;
            }
            if (numGiaNhap.Value < 0)
            {
                BaoLoi("Giá nhập không được âm!", numGiaNhap);
                return false;
            }
            if (numGiaNhap.Value == 0)
            {
                BaoLoi("Vui lòng nhập Đơn giá!", numGiaNhap);
                return false;
            }
            if (numSoLuong.Value < 0)
            {
                BaoLoi("Số lượng không được âm!", numSoLuong);
                return false;
            }
            if (numSoLuong.Value == 0)
            {
                BaoLoi("Vui lòng nhập Số lượng!", numSoLuong);
                return false;
            }
            if (dtNgaySanXuat.Value.Date > dtNgayNhap.Value.Date)
            {
                BaoLoi("Ngày sản xuất không được sau ngày nhập!", dtNgaySanXuat);
                return false;
            }
            if (dtNgayHetHan.Value.Date <= dtNgaySanXuat.Value.Date)
            {
                BaoLoi("Ngày hết hạn phải lớn hơn ngày sản xuất!", dtNgayHetHan);
                return false;
            }
            if (dtNgayHetHan.Value.Date <= dtNgayNhap.Value.Date)
            {
                BaoLoi("Ngày hết hạn phải sau ngày nhập!", dtNgayHetHan);
                return false;
            }
            return true;
        }

        bool KiemTraPhieu()
        {
            if (!ChonNhaCungCap())
            {
                BaoLoi("Vui lòng chọn Nhà cung cấp!", cmbNhaCungCap);
                return false;
            }
            if (DemDong() == 0)
            {
                BaoLoi("Phiếu nhập chưa có sản phẩm nào!\n(Muốn bỏ cả phiếu thì dùng nút Xóa.)", cmbSanPham);
                return false;
            }
            if (numDaTra.Value > numTongTien.Value)
            {
                BaoLoi("Số tiền đã trả không được lớn hơn tổng tiền!", numDaTra);
                return false;
            }
            return true;
        }

        // ---------- Thêm / xóa dòng sản phẩm trong phiếu ----------

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ThemDong();
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không thêm được dòng vào phiếu", ex);
            }
        }

        private void ThemDong()
        {
            if (status == Controll.Normal) return;

            string maSo = txtMaSo.Text.Trim();
            if (!KiemTraDong(maSo)) return;

            MaSanPhamController ctrlMa = new MaSanPhamController();
            if (ctrlMa.LayMaSanPham(maSo) != null)
            {
                BaoLoi("Mã sản phẩm này đã tồn tại! Vui lòng nhập lại!", txtMaSo);
                return;
            }

            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow) continue;
                if (maSo.Equals(Convert.ToString(view.Cells["colMaSanPham"].Value)))
                {
                    BaoLoi("Mã này đã có trong danh sách phiếu!", txtMaSo);
                    return;
                }
            }

            if (m_ChiTiet == null) NapChiTiet();
            if (!m_ChiTiet.Columns.Contains("ID_SAN_PHAM"))
                throw new Exception("Không đọc được cấu trúc bảng MA_SAN_PHAM trong cơ sở dữ liệu.");

            DataRow row = m_ChiTiet.NewRow();
            row["ID_SAN_PHAM"] = cmbSanPham.SelectedValue;
            row["ID_PHIEU_NHAP"] = txtMaPhieu.Text;
            row["ID"] = maSo;
            row["DON_GIA_NHAP"] = numGiaNhap.Value;
            row["SO_LUONG"] = numSoLuong.Value;
            row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
            row["NGAY_SAN_XUAT"] = dtNgaySanXuat.Value.Date;
            row["NGAY_HET_HAN"] = dtNgayHetHan.Value.Date;
            m_ChiTiet.Rows.Add(row);

            TinhTongTien();
            CapNhatTongDong();
            ThongBao("Đã thêm mã " + maSo + " vào phiếu.");

            txtMaSo.Clear();
            txtMaSo.Focus();
        }

        // ---------- Chi tiết phiếu: nạp cả bảng MA_SAN_PHAM rồi lọc theo phiếu (giống form Khách hàng / Đại lý) ----------

        private void NapChiTiet()
        {
            m_ChiTiet = ctrlMaSP.TaiBangChiTiet();      // lỗi đọc bảng sẽ ném ra kèm nguyên nhân
            m_BsChiTiet.DataSource = m_ChiTiet;
        }

        // Chỉ hiện các dòng của phiếu idPhieu. Convert sang chuỗi để lọc đúng dù cột ID_PHIEU_NHAP là số hay chuỗi.
        private void HienChiTiet(string idPhieu)
        {
            if (m_ChiTiet == null) NapChiTiet();

            m_BsChiTiet.Filter = "Convert(ID_PHIEU_NHAP, 'System.String') = '" + idPhieu.Replace("'", "''") + "'";
            if (!object.ReferenceEquals(dataGridView.DataSource, m_BsChiTiet))
                dataGridView.DataSource = m_BsChiTiet;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (status == Controll.Normal) return;

            if (dataGridView.CurrentRow == null)
            {
                MessageBox.Show("Chọn dòng sản phẩm cần xóa trong danh sách!", TIEU_DE,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Xóa dòng sản phẩm đang chọn?", TIEU_DE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                dataGridView.Rows.Remove(dataGridView.CurrentRow);
                TinhTongTien();
                CapNhatTongDong();
                ThongBao("Đã xóa dòng khỏi danh sách (chưa lưu CSDL nếu chưa bấm Lưu).");
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không xóa được dòng", ex);
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Xóa dòng sản phẩm này?", TIEU_DE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        private void dataGridView_RowsChanged(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                TinhTongTien();
                CapNhatTongDong();
            }
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string col = dataGridView.Columns[e.ColumnIndex].Name;
            if (col == "colNhaCungCap")
            {
                e.Value = cmbNhaCungCap.Text;
                e.FormattingApplied = true;
            }
            else if (col == "colNhanVien")
            {
                e.Value = m_TenNhanVien;
                e.FormattingApplied = true;
            }
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        // ---------- Tính toán ----------

        static decimal ToDecimal(object val)
        {
            if (val == null || val == DBNull.Value) return 0;
            decimal d;
            return decimal.TryParse(Convert.ToString(val, CultureInfo.InvariantCulture),
                NumberStyles.Number, CultureInfo.InvariantCulture, out d) ? d : 0;
        }

        private void numGiaNhap_ValueChanged(object sender, EventArgs e)
        {
            numThanhTien.Value = Math.Min(numGiaNhap.Value * numSoLuong.Value, numThanhTien.Maximum);
        }

        void TinhTongTien()
        {
            decimal tong = 0;
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (r.IsNewRow) continue;
                tong += ToDecimal(r.Cells["colDonGiaNhap"].Value)
                      * ToDecimal(r.Cells["colSoLuong"].Value);
            }
            numTongTien.Value = Math.Min(tong, numTongTien.Maximum);
            TinhConNo(numDaTra.Value);
        }

        void TinhConNo(decimal daTra)
        {
            decimal conNo = numTongTien.Value - daTra;
            if (conNo < 0)
            {
                if (errProvider != null)
                    errProvider.SetError(numDaTra, "Đã trả không được lớn hơn tổng tiền");
                conNo = 0;
            }
            else if (errProvider != null)
                errProvider.SetError(numDaTra, "");

            numConNo.Value = Math.Min(conNo, numConNo.Maximum);
        }

        private void numDaTra_ValueChanged(object sender, EventArgs e)
        {
            TinhConNo(numDaTra.Value);
        }

        private void numDaTra_TextChanged(object sender, EventArgs e)
        {
            decimal v;
            if (decimal.TryParse(numDaTra.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out v)
                && v >= numDaTra.Minimum && v <= numDaTra.Maximum)
                TinhConNo(v);
        }

        // ---------- Chụp hình phiếu, đếm dòng, mô tả thay đổi ----------

        int DemDong()
        {
            int n = 0;
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (!r.IsNewRow) n++;
            }
            return n;
        }

        // Các dòng sản phẩm đang có trong lưới: mã số -> mô tả (dùng cho nhật ký)
        Dictionary<string, string> DocDong()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (r.IsNewRow) continue;
                string ma = Convert.ToString(r.Cells["colMaSanPham"].Value);
                string sp = Convert.ToString(r.Cells["colSanPham"].FormattedValue);
                string mota = sp + " (mã " + ma
                    + ", SL " + ToDecimal(r.Cells["colSoLuong"].Value).ToString("N0")
                    + ", giá " + ToDecimal(r.Cells["colDonGiaNhap"].Value).ToString("N0") + ")";
                d[ma] = mota;
            }
            return d;
        }

        // Có gì chưa lưu không (để hỏi lưu khi hủy / đóng form)
        bool CoThayDoi()
        {
            if (status == Controll.AddNew)
                return DemDong() > 0 || numDaTra.Value > 0;

            if (status == Controll.Edit)
            {
                if (Convert.ToString(cmbNhaCungCap.SelectedValue) != m_SnapNcc) return true;
                if (numDaTra.Value != m_SnapDaTra) return true;

                Dictionary<string, string> hienTai = DocDong();
                if (hienTai.Count != m_DongGoc.Count) return true;
                foreach (KeyValuePair<string, string> kv in hienTai)
                {
                    if (!m_DongGoc.ContainsKey(kv.Key)) return true;
                }
            }
            return false;
        }

        static void Them(StringBuilder sb, string dong)
        {
            if (sb.Length > 0) sb.Append("\n");
            sb.Append(dong);
        }

        string TomTatPhieu()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Nhà cung cấp: ").Append(cmbNhaCungCap.Text);
            sb.Append("; Ngày nhập: ").Append(dtNgayNhap.Value.ToString("dd/MM/yyyy"));
            sb.Append("; Tổng tiền: ").Append(numTongTien.Value.ToString("N0"));
            sb.Append("; Đã trả: ").Append(numDaTra.Value.ToString("N0"));
            sb.Append("; Còn nợ: ").Append(numConNo.Value.ToString("N0"));

            Dictionary<string, string> d = DocDong();
            sb.Append("; Số dòng: ").Append(d.Count);
            foreach (KeyValuePair<string, string> kv in d)
                sb.Append("\n  - ").Append(kv.Value);
            return sb.ToString();
        }

        string MoTaThayDoi()
        {
            StringBuilder sb = new StringBuilder();
            if (Convert.ToString(cmbNhaCungCap.SelectedValue) != m_SnapNcc)
                Them(sb, "Nhà cung cấp: " + m_SnapTenNcc + " → " + cmbNhaCungCap.Text);
            if (numTongTien.Value != m_SnapTongTien)
                Them(sb, "Tổng tiền: " + m_SnapTongTien.ToString("N0") + " → " + numTongTien.Value.ToString("N0"));
            if (numDaTra.Value != m_SnapDaTra)
                Them(sb, "Đã trả: " + m_SnapDaTra.ToString("N0") + " → " + numDaTra.Value.ToString("N0"));

            Dictionary<string, string> hienTai = DocDong();
            foreach (KeyValuePair<string, string> kv in hienTai)
            {
                if (!m_DongGoc.ContainsKey(kv.Key)) Them(sb, "Thêm dòng: " + kv.Value);
            }
            foreach (KeyValuePair<string, string> kv in m_DongGoc)
            {
                if (!hienTai.ContainsKey(kv.Key)) Them(sb, "Xóa dòng: " + kv.Value);
            }
            return sb.Length == 0 ? "(không đổi nội dung)" : sb.ToString();
        }

        // Ghi nhật ký; lỗi ghi nhật ký không làm hỏng việc đã lưu, chỉ trả về câu cảnh báo
        string GhiNhatKy(string hanhDong, string id, string tenNcc, string noiDung)
        {
            try
            {
                ctrl.GhiNhatKy(hanhDong, id, tenNcc, noiDung, m_TenNhanVien);
                return "";
            }
            catch (Exception ex)
            {
                return "Lưu ý: chưa ghi được nhật ký thay đổi (" + ex.Message + ").\n"
                     + "Hãy chạy CapNhat_CSDL_PhieuNhap.sql để tạo bảng NHAT_KY_PHIEU_NHAP.";
            }
        }

        // ---------- Thêm phiếu / Chỉnh sửa / Hủy ----------

        private void toolLuuThem_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal) return;
            BatDauPhieuMoi();
        }

        private void BatDauPhieuMoi()
        {
            try
            {
                m_ViTriCu = ctrl.ViTri;
                m_IdDangSua = "";
                status = Controll.AddNew;

                txtMaPhieu.Text = ThamSo.LayMaPhieuNhap().ToString();
                KhoiTaoGiaTriMacDinh();
                HienChiTiet(txtMaPhieu.Text);
                m_DongGoc = DocDong();

                Allow(true);
                CapNhatTongDong();
                ThongBao("Phiếu mới " + txtMaPhieu.Text + ". Chọn nhà cung cấp, thêm sản phẩm vào phiếu rồi bấm Lưu.");
            }
            catch (Exception ex)
            {
                status = Controll.Normal;
                BaoLoiHeThong("Không tạo được phiếu mới", ex);
                XemPhieuHienTai();
            }
        }

        private void toolChinhsua_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal) return;

            string id = ctrl.IdHienTai;
            if (id.Length == 0)
            {
                MessageBox.Show("Chưa có phiếu nhập nào để sửa.\nHãy bấm Thêm để tạo phiếu mới.", TIEU_DE,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            m_IdDangSua = id;
            m_ViTriCu = ctrl.ViTri;
            status = Controll.Edit;

            m_SnapNcc = Convert.ToString(cmbNhaCungCap.SelectedValue);
            m_SnapTenNcc = cmbNhaCungCap.Text;
            m_SnapDaTra = numDaTra.Value;
            m_SnapTongTien = numTongTien.Value;
            m_DongGoc = DocDong();

            // Ô nhập dòng mới: ngày sản xuất không được sau ngày nhập của phiếu
            DateTime nhap = dtNgayNhap.Value.Date;
            dtNgaySanXuat.MaxDate = DateTime.Today;
            dtNgaySanXuat.Value = (nhap < DateTime.Today) ? nhap : DateTime.Today;
            dtNgayHetHan.Value = nhap.AddYears(1);
            cmbSanPham.SelectedIndex = -1;
            txtMaSo.Clear();
            numGiaNhap.Value = 0;
            numSoLuong.Value = 0;
            KiemTraNgay();

            Allow(true);
            ThongBao("Đang sửa phiếu " + id + ". Xong bấm Lưu, hoặc Hủy để bỏ thay đổi.");
        }

        private void toolHuy_Click(object sender, EventArgs e)
        {
            if (status == Controll.Normal) return;

            if (CoThayDoi() && MessageBox.Show("Bỏ các thay đổi chưa lưu của phiếu này?", TIEU_DE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            HuyThayDoi();
        }

        // Tải lại từ CSDL: các dòng đã thêm / xóa trong lưới nhưng chưa lưu sẽ trở về như cũ
        private void HuyThayDoi()
        {
            string id = (status == Controll.Edit) ? m_IdDangSua : ctrl.IdHienTai;
            status = Controll.Normal;
            try
            {
                TaiDanhSach(id);
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không tải lại được danh sách phiếu nhập", ex);
            }
            XemPhieuHienTai();
        }

        // ---------- Lưu ----------

        private void toolLuuThoat_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();

            if (status == Controll.Normal)
            {
                MessageBox.Show("Không có thay đổi nào để lưu.\nBấm Thêm hoặc Chỉnh sửa trước.", TIEU_DE,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Luu();
        }

        bool Luu()
        {
            if (status == Controll.Normal) return true;

            dataGridView.EndEdit();
            this.ValidateChildren();
            TinhTongTien();
            if (!KiemTraPhieu()) return false;

            try
            {
                return (status == Controll.AddNew) ? ThemMoi() : CapNhat();
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Lưu không thành công", ex);
                return false;
            }
        }

        bool CapNhat()
        {
            string id = m_IdDangSua;

            if (!CoThayDoi())
            {
                status = Controll.Normal;
                TaiDanhSach(id);
                XemPhieuHienTai();
                ThongBao("Không có thay đổi nào để lưu.");
                return true;
            }

            string idNcc = Convert.ToString(cmbNhaCungCap.SelectedValue);
            string tenNcc = cmbNhaCungCap.Text;
            string mota = MoTaThayDoi();

            // Dòng thêm mới trong lần sửa này: cần cập nhật giá nhập sản phẩm như khi thêm phiếu
            List<string[]> dongMoi = new List<string[]>();
            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow) continue;
                string ma = Convert.ToString(view.Cells["colMaSanPham"].Value);
                if (m_DongGoc.ContainsKey(ma)) continue;
                dongMoi.Add(new string[] {
                    Convert.ToString(view.Cells["colSanPham"].Value),
                    Convert.ToString(view.Cells["colDonGiaNhap"].Value),
                    Convert.ToString(view.Cells["colSoLuong"].Value) });
            }

            LuuChiTiet();
            ctrl.CapNhatPhieu(id, idNcc, numTongTien.Value, numDaTra.Value, numConNo.Value);

            if (dongMoi.Count > 0)
            {
                SanPhamController ctrlSP = new SanPhamController();
                foreach (string[] d in dongMoi)
                {
                    ctrlSP.CapNhatGiaNhap(d[0],
                        Convert.ToInt64(ToDecimal(d[1])),
                        Convert.ToInt64(ToDecimal(d[2])));
                }
            }

            string canhBao = GhiNhatKy("Sửa", id, tenNcc, mota);

            status = Controll.Normal;
            TaiDanhSach(id);
            XemPhieuHienTai();
            ThongBao("Đã lưu thay đổi của phiếu " + id + ".");
            if (canhBao.Length > 0)
                MessageBox.Show(canhBao, TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return true;
        }

        // Lưu các dòng sản phẩm của phiếu. DataService nuốt lỗi và chỉ trả về false,
        // nên phải kiểm tra kết quả và báo nguyên nhân thật thay vì báo "Đã lưu" khi chưa lưu được.
        void LuuChiTiet()
        {
            if (m_ChiTiet == null || !ctrlMaSP.CoThayDoi()) return;

            bool ok = false;
            string loi = "";
            try { ok = ctrlMaSP.Save(); }
            catch (Exception ex) { loi = ex.Message; }

            if (ok) return;
            if (loi.Length == 0) loi = ctrlMaSP.LastError;

            throw new Exception("Không lưu được các dòng sản phẩm của phiếu."
                + (loi.Length > 0 ? "\n\nChi tiết: " + loi : "")
                + "\n\nNếu lỗi nói về kiểu dữ liệu: cột ID_PHIEU_NHAP của bảng MA_SAN_PHAM trong file cuahang.dll "
                + "phải cùng kiểu Text với cột ID của bảng PHIEU_NHAP.");
        }

        bool ThemMoi()
        {
            string id = txtMaPhieu.Text.Trim();
            if (id.Length == 0)
            {
                BaoLoi("Vui lòng nhập mã phiếu!", txtMaPhieu);
                return false;
            }

            PhieuNhapController ctrlKT = new PhieuNhapController();
            if (ctrlKT.LayPhieuNhap(id) != null)
            {
                BaoLoi("Mã phiếu nhập này đã tồn tại!", txtMaPhieu);
                return false;
            }

            string tenNcc = cmbNhaCungCap.Text;
            string mota = TomTatPhieu();

            // Mã phiếu có thể bị sửa sau khi đã thêm dòng: cho các dòng mới trỏ về đúng mã phiếu sắp lưu
            foreach (DataRow r in m_ChiTiet.Rows)
            {
                if (r.RowState == DataRowState.Added && Convert.ToString(r["ID_PHIEU_NHAP"]) != id)
                    r["ID_PHIEU_NHAP"] = id;
            }

            PhieuNhapController ctrlMoi = new PhieuNhapController();
            DataRow row = ctrlMoi.NewRow();
            row["ID"] = id;
            row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
            row["TONG_TIEN"] = numTongTien.Value;
            row["ID_NHA_CUNG_CAP"] = cmbNhaCungCap.SelectedValue;
            row["DA_TRA"] = numDaTra.Value;
            row["CON_NO"] = numConNo.Value;
            ctrlMoi.Add(row);
            ctrlMoi.Save();

            try
            {
                LuuChiTiet();
            }
            catch (Exception)
            {
                // Lưu sản phẩm lỗi thì bỏ luôn phần đầu phiếu vừa lưu, tránh để lại phiếu trống
                try { ctrlMoi.XoaPhieu(id); }
                catch (Exception) { }
                throw;
            }

            if (ThamSo.LaSoNguyen(id))
            {
                long so = Convert.ToInt64(id);
                if (so >= ThamSo.LayMaPhieuNhap())
                    ThamSo.GanMaPhieuNhap(so + 1);
            }

            SanPhamController ctrlSP = new SanPhamController();
            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow) continue;
                ctrlSP.CapNhatGiaNhap(
                    Convert.ToString(view.Cells["colSanPham"].Value),
                    Convert.ToInt64(view.Cells["colDonGiaNhap"].Value),
                    Convert.ToInt64(view.Cells["colSoLuong"].Value));
            }

            string canhBao = GhiNhatKy("Thêm", id, tenNcc, mota);

            status = Controll.Normal;
            TaiDanhSach(id);
            XemPhieuHienTai();
            ThongBao("Đã lưu phiếu " + id + ".");
            if (canhBao.Length > 0)
                MessageBox.Show(canhBao, TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return true;
        }

        // ---------- Xóa phiếu ----------

        private void toolXoa_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal) return;

            string id = ctrl.IdHienTai;
            if (id.Length == 0)
            {
                MessageBox.Show("Chưa có phiếu nhập nào để xóa.", TIEU_DE,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string tenNcc = cmbNhaCungCap.Text;
            if (MessageBox.Show("Bạn có chắc chắn xóa phiếu nhập " + id + " không?\n" +
                "  - Nhà cung cấp: " + tenNcc + "\n" +
                "  - Tổng tiền: " + numTongTien.Value.ToString("N0") + " đ\n" +
                "  - Số dòng sản phẩm: " + DemDong().ToString() + "\n\n" +
                "Các dòng sản phẩm của phiếu cũng bị xóa và không thể hoàn tác.",
                TIEU_DE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            string mota = TomTatPhieu();
            try
            {
                // Sản phẩm của phiếu phải xóa trước vì chúng tham chiếu tới phiếu
                for (int i = dataGridView.Rows.Count - 1; i >= 0; i--)
                {
                    if (!dataGridView.Rows[i].IsNewRow) dataGridView.Rows.RemoveAt(i);
                }
                LuuChiTiet();
                ctrl.XoaPhieu(id);

                string canhBao = GhiNhatKy("Xóa", id, tenNcc, mota);

                TaiDanhSach("");
                XemPhieuHienTai();
                ThongBao("Đã xóa phiếu " + id + ".");
                if (canhBao.Length > 0)
                    MessageBox.Show(canhBao, TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không xóa được phiếu " + id + ":\n" + ex.Message + "\n\n" +
                    "Có thể sản phẩm của phiếu này đã được bán hoặc đang được dùng ở nơi khác.",
                    TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { TaiDanhSach(id); }
                catch (Exception) { }
                XemPhieuHienTai();
            }
        }

        // ---------- Nhật ký / Thoát ----------

        private void toolNhatKy_Click(object sender, EventArgs e)
        {
            string id = "";
            if (status == Controll.Edit) id = m_IdDangSua;
            else if (status == Controll.Normal) id = ctrl.IdHienTai;

            using (frmNhatKy f = new frmNhatKy(id, cmbNhaCungCap.Text, frmNhatKy.NGUON_PHIEU_NHAP))
            {
                f.ShowDialog(this);
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ---------- Menu chuột phải + đóng form ----------

        private void mnuInPhieu_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại phiếu nhập hiện tại!", TIEU_DE,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string id = ctrl.IdHienTai;
            if (id.Length == 0)
            {
                MessageBox.Show("Chưa có phiếu nhập nào để in.", TIEU_DE,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                PhieuNhapController ctrlPN = new PhieuNhapController();
                PhieuNhap ph = ctrlPN.LayPhieuNhap(id);
                if (ph == null)
                {
                    MessageBox.Show("Không tìm thấy phiếu nhập để in (mã: " + id + ").",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                frmInPhieuNhap f = new frmInPhieuNhap(ph);
                f.Show();
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không in được phiếu", ex);
            }
        }

        private void mnuXemLai_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Đang nhập / sửa phiếu. Hãy Lưu hoặc Hủy trước khi tải lại.", TIEU_DE,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                TaiDanhSach(ctrl.IdHienTai);
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không tải lại được dữ liệu", ex);
            }
            XemPhieuHienTai();
        }

        private void frmNhapHang_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (status == Controll.Normal
                || e.CloseReason == CloseReason.WindowsShutDown
                || e.CloseReason == CloseReason.TaskManagerClosing)
                return;

            // Chưa nhập / sửa gì thì đóng luôn, không hỏi
            if (!CoThayDoi()) return;

            DialogResult r = MessageBox.Show(
                "Bạn có muốn lưu lại phiếu nhập này không?",
                TIEU_DE, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (r == DialogResult.Cancel)
                e.Cancel = true;
            else if (r == DialogResult.Yes)
            {
                if (!Luu()) e.Cancel = true;
            }
        }

        // ---------- Thêm mới sản phẩm / nhà cung cấp ngay trong phiếu ----------

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            object dangChon = cmbSanPham.SelectedValue;
            frmSanPham f = new frmSanPham();
            f.ShowDialog(this);
            try
            {
                ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
                ctrlSanPham.HienthiDataGridViewComboBoxColumn(colSanPham);
                if (dangChon != null) cmbSanPham.SelectedValue = dangChon;
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không tải lại được danh sách sản phẩm", ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object dangChon = cmbNhaCungCap.SelectedValue;
            frmNhaCungCap f = new frmNhaCungCap();
            f.ShowDialog(this);
            try
            {
                ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);
                if (dangChon != null) cmbNhaCungCap.SelectedValue = dangChon;
            }
            catch (Exception ex)
            {
                BaoLoiHeThong("Không tải lại được danh sách nhà cung cấp", ex);
            }
        }

        // Ngày nhập, Tổng tiền, Còn nợ luôn chỉ đọc
        void Allow(bool val)
        {
            txtMaPhieu.Enabled = val && status == Controll.AddNew;
            cmbNhaCungCap.Enabled = val;
            btnThemNCC.Enabled = val;
            numDaTra.Enabled = val;

            cmbSanPham.Enabled = val;
            btnThemSanPham.Enabled = val;
            txtMaSo.Enabled = val;
            numGiaNhap.Enabled = val;
            numSoLuong.Enabled = val;
            dtNgaySanXuat.Enabled = val;
            dtNgayHetHan.Enabled = val;

            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dataGridView.Enabled = true;

            CapNhatCheDo(val);
            CapNhatNut();
        }

        // ===== PM-15: Tim phieu nhap ngay tren form Nhap hang =====
        private ToolStripButton toolTimKiem;

        
        int TimViTriChenTruocThoat()
        {
            for (int i = 0; i < bindingNavigator.Items.Count; i++)
            {
                string n = bindingNavigator.Items[i].Name ?? "";
                string tx = bindingNavigator.Items[i].Text ?? "";
                if (n.Contains("Thoat") || tx == "Thoát")
                    return i;
            }
            return bindingNavigator.Items.Count;
        }

        void ThemNutTimKiem()
        {
            try
            {
                if (toolTimKiem != null && bindingNavigator.Items.Contains(toolTimKiem))
                    return;
                // da co san ten toolTimKiem trong designer?
                foreach (ToolStripItem it in bindingNavigator.Items)
                {
                    if (it.Name == "toolTimKiem" || it.Text == "Tìm kiếm")
                    {
                        toolTimKiem = it as ToolStripButton;
                        if (toolTimKiem != null)
                        {
                            toolTimKiem.Click -= toolTimKiem_Click;
                            DongBoStyleNut(toolTimKiem);
                toolTimKiem.Click += toolTimKiem_Click;
                            return;
                        }
                    }
                }
                toolTimKiem = new ToolStripButton();
                toolTimKiem.Name = "toolTimKiem";
                toolTimKiem.Text = "Tìm kiếm";
                toolTimKiem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                toolTimKiem.TextImageRelation = TextImageRelation.ImageAboveText;
                try
                {
                    toolTimKiem.Image = global::CuahangNongduoc.Properties.Resources.find;
                }
                catch { }
                toolTimKiem.Click += toolTimKiem_Click;
                // chen truoc Thoat / Huy neu co
                int idx = -1;
                for (int i = 0; i < bindingNavigator.Items.Count; i++)
                {
                    string n = bindingNavigator.Items[i].Name ?? "";
                    string t = bindingNavigator.Items[i].Text ?? "";
                    if (n.Contains("Thoat") || t.Contains("Thoát") || n.Contains("Huy") || t.Contains("Hủy"))
                    {
                        idx = i;
                        break;
                    }
                }
                bindingNavigator.Items.Insert(TimViTriChenTruocThoat(), toolTimKiem);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ThemNutTimKiem: " + ex.Message);
            }
        }

        private void toolTimKiem_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Đang nhập / sửa phiếu. Hãy Lưu hoặc Hủy trước khi tìm.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                frmTimPhieuNhap TimPhieu = new frmTimPhieuNhap();
                if (TimPhieu.ShowDialog(this) != DialogResult.OK)
                    return;

                string idNcc = "";
                try { idNcc = TimPhieu.IdNhaCungCap; } catch { }
                if (string.IsNullOrEmpty(idNcc))
                {
                    try
                    {
                        if (TimPhieu.cmbNCC != null && TimPhieu.cmbNCC.SelectedValue != null)
                            idNcc = Convert.ToString(TimPhieu.cmbNCC.SelectedValue);
                    }
                    catch { }
                }

                DateTime? ngay = null;
                bool boQuaNgay = false;
                try { boQuaNgay = TimPhieu.BoQuaNgay; } catch { }
                if (!boQuaNgay)
                {
                    try { ngay = TimPhieu.dtNgayNhap.Value.Date; } catch { ngay = DateTime.Today; }
                }

                decimal? tongTu = null, tongDen = null;
                try
                {
                    if (TimPhieu.LocTongTien)
                    {
                        tongTu = TimPhieu.TongTienTu;
                        tongDen = TimPhieu.TongTienDen;
                    }
                }
                catch { }

                string maPhieu = "";
                try { maPhieu = TimPhieu.MaPhieu; } catch { }
                string nguoiLap = "";
                try { nguoiLap = TimPhieu.NguoiLap; } catch { }
                string kho = "";
                try { kho = TimPhieu.KhoNhap; } catch { }

                int n = 0;
                try
                {
                    n = ctrl.TimPhieuNhapNangCao(maPhieu, idNcc, ngay, tongTu, tongDen, nguoiLap, kho);
                }
                catch
                {
                    // Fallback: chi NCC + ngay (API cu)
                    if (!string.IsNullOrEmpty(idNcc) && ngay.HasValue)
                    {
                        ctrl.TimPhieuNhap(idNcc, ngay.Value);
                        n = ctrl.SoPhieu;
                    }
                    else
                    {
                        MessageBox.Show("Không gọi được tìm nâng cao. Chọn Nhà cung cấp và Ngày rồi thử lại.",
                            this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Dam bao navigator dung bs cua controller
                try
                {
                    if (bindingNavigator.BindingSource == null)
                        bindingNavigator.BindingSource = null; // no-op
                    // Tai lai binding event
                    if (bindingNavigator.BindingSource != null)
                    {
                        bindingNavigator.BindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                        bindingNavigator.BindingSource.CurrentChanged += BindingSource_CurrentChanged;
                    }
                }
                catch { }

                if (n == 0)
                {
                    MessageBox.Show("Không tìm thấy phiếu nhập phù hợp.", this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // giu danh sach cu: tai lai toan bo
                    try { TaiDanhSach(null); XemPhieuHienTai(); } catch { }
                    return;
                }

                XemPhieuHienTai();
                ThongBao("Tìm thấy " + n + " phiếu. Đang xem phiếu " + txtMaPhieu.Text + ".");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm phiếu nhập:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ===== PM-17: Nut In thong nhat tren thanh cong cu (cung style Tim / Luu / Xoa) =====
        private ToolStripButton toolInPhieu;

        void ThemNutIn()
        {
            try
            {
                foreach (ToolStripItem it in bindingNavigator.Items)
                {
                    if (it.Name == "toolInPhieu" || it.Name == "toolIn" || it.Text == "In")
                    {
                        toolInPhieu = it as ToolStripButton;
                        if (toolInPhieu != null)
                        {
                            toolInPhieu.Click -= toolInPhieu_Click;
                            toolInPhieu.Click += toolInPhieu_Click;
                            DongBoStyleNut(toolInPhieu);
                            return;
                        }
                    }
                }

                toolInPhieu = new ToolStripButton();
                toolInPhieu.Name = "toolInPhieu";
                toolInPhieu.Text = "In";
                toolInPhieu.ToolTipText = "In phiếu nhập hiện tại";
                DongBoStyleNut(toolInPhieu);
                try { toolInPhieu.Image = global::CuahangNongduoc.Properties.Resources.printer; }
                catch
                {
                    try { toolInPhieu.Image = global::CuahangNongduoc.Properties.Resources.find; } catch { }
                }
                toolInPhieu.Click += toolInPhieu_Click;

                // Chen sau Tim kiem, truoc Luu / Xoa / Thoat
                int idx = -1;
                for (int i = 0; i < bindingNavigator.Items.Count; i++)
                {
                    string n = bindingNavigator.Items[i].Name ?? "";
                    string tx = bindingNavigator.Items[i].Text ?? "";
                    if (n == "toolTimKiem" || tx == "Tìm kiếm")
                    {
                        idx = i + 1;
                        break;
                    }
                }
                if (idx < 0)
                {
                    for (int i = 0; i < bindingNavigator.Items.Count; i++)
                    {
                        string n = bindingNavigator.Items[i].Name ?? "";
                        string tx = bindingNavigator.Items[i].Text ?? "";
                        if (n.Contains("Luu") || tx == "Lưu" || n.Contains("Thoat") || tx.Contains("Thoát"))
                        {
                            idx = i;
                            break;
                        }
                    }
                }
                bindingNavigator.Items.Insert(TimViTriChenTruocThoat(), toolInPhieu);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ThemNutIn: " + ex.Message);
            }
        }

        /// <summary>Style chung: chu + icon tren/duoi, giong Thêm / Lưu / Xóa.</summary>
        void DongBoStyleNut(ToolStripButton b)
        {
            if (b == null) return;
            b.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            b.TextImageRelation = TextImageRelation.ImageAboveText;
            b.ImageScaling = ToolStripItemImageScaling.SizeToFit;
            b.AutoSize = true;
        }

        private void toolInPhieu_Click(object sender, EventArgs e)
        {
            // Goi chung logic in (giong menu chuot phai)
            try { mnuInPhieu_Click(sender, e); }
            catch
            {
                // Neu khong co mnuInPhieu_Click (ten khac) – in truc tiep
                InPhieuHienTai();
            }
        }

        private void InPhieuHienTai()
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại phiếu nhập hiện tại trước khi in!",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string id = "";
            try { id = ctrl.IdHienTai; } catch { }
            if (string.IsNullOrEmpty(id))
            {
                try { id = txtMaPhieu.Text.Trim(); } catch { }
            }
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Chưa có phiếu nhập nào để in.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                PhieuNhapController ctrlPN = new PhieuNhapController();
                CuahangNongduoc.BusinessObject.PhieuNhap ph = ctrlPN.LayPhieuNhap(id);
                if (ph == null)
                {
                    MessageBox.Show("Không tìm thấy phiếu nhập \"" + id + "\" để in.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                frmInPhieuNhap f = new frmInPhieuNhap(ph);
                f.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được phiếu:\n" + ex.Message,
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
