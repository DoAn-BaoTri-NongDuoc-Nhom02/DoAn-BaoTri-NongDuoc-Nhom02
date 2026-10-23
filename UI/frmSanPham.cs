using CuahangNongduoc.Controller;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmSanPham : Form

    {
        private bool m_CheDoSua = false;
        private List<string> m_NhatKy = new List<string>();
        SanPhamController ctrl = new SanPhamController();
        DonViTinhController ctrlDVT = new DonViTinhController();

        // TODO: gắn đúng biến đăng nhập của nhóm
        private bool CoQuyenSua
        {
            get
            {
                try
                {
                    // return Program.CurrentUser != null && Program.CurrentUser.IdVaiTro == 1;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public frmSanPham()
        {
            InitializeComponent();
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            UiTheme.Apply(this);
            this.Text = "Sản phẩm";
            this.StartPosition = FormStartPosition.CenterParent;

            // Tự xử lý xóa + xác nhận
            bindingNavigator.DeleteItem = null;

            // Load đơn vị tính
            ctrlDVT.HienthiAutoComboBox(cmbDVT);

            // Thêm cột ĐVT (chỉ 1 lần)
            bool coCotDVT = false;
            foreach (DataGridViewColumn c in dataGridView.Columns)
            {
                if (c.Name == "colDVT" || c.DataPropertyName == "ID_DON_VI_TINH")
                {
                    coCotDVT = true;
                    break;
                }
            }
            if (!coCotDVT)
            {
                DataGridViewComboBoxColumn col = ctrlDVT.HienthiDataGridViewComboBoxColumn();
                col.Name = "colDVT";
                col.HeaderText = "Đơn vị tính";
                col.DataPropertyName = "ID_DON_VI_TINH";
                col.Width = 100;
                dataGridView.Columns.Add(col);
            }

            // Binding dữ liệu
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator,
                txtMaSanPham, txtTenSanPham, cmbDVT,
                numSoLuong, numDonGiaNhap, numGiaBanSi, numGiaBanLe);

            // Đảm bảo ĐVT (combobox + cột trong lưới) được gắn dữ liệu, và cột Trạng thái tự tính
            DamBaoBindingDVT();
            dataGridView.CellFormatting -= dataGridView_CellFormatting;
            dataGridView.CellFormatting += dataGridView_CellFormatting;
            dataGridView.DataBindingComplete -= dataGridView_DataBindingComplete;
            dataGridView.DataBindingComplete += dataGridView_DataBindingComplete;

            // Mã SP chỉ đọc
            txtMaSanPham.ReadOnly = true;
            txtMaSanPham.BackColor = Color.WhiteSmoke;

            // Ô tìm kiếm
            toolTimSanPham.Enabled = true;
            if (string.IsNullOrEmpty(toolTimSanPham.Text) || toolTimSanPham.Text.Trim() == "" || toolTimSanPham.ForeColor == Color.Gray)
            {
                toolTimSanPham.Text = "Tìm theo mã...";
                toolTimSanPham.ForeColor = Color.Gray;
            }

            // Định dạng số
            try
            {
                numDonGiaNhap.DecimalPlaces = 0;
                numGiaBanSi.DecimalPlaces = 0;
                numGiaBanLe.DecimalPlaces = 0;
                numSoLuong.DecimalPlaces = 0;

                numDonGiaNhap.ThousandsSeparator = true;
                numGiaBanSi.ThousandsSeparator = true;
                numGiaBanLe.ThousandsSeparator = true;
                numSoLuong.ThousandsSeparator = true;

                numDonGiaNhap.Maximum = 1000000000;
                numGiaBanSi.Maximum = 1000000000;
                numGiaBanLe.Maximum = 1000000000;
                numSoLuong.Maximum = 100000000;
            }
            catch { }

            ApDungPhanQuyen();
            CapNhatTongTon();
            CapNhatCotTrangThai();
        }

        // Gắn cmbDVT và cột "Đơn vị tính" vào cột ID_DON_VI_TINH của dữ liệu sản phẩm.
        private void DamBaoBindingDVT()
        {
            try
            {
                BindingSource bs = bindingNavigator.BindingSource;

                if (bs != null && cmbDVT.DataBindings["SelectedValue"] == null)
                {
                    cmbDVT.DataBindings.Add("SelectedValue", bs, "ID_DON_VI_TINH",
                        true, DataSourceUpdateMode.OnPropertyChanged);
                }

                DataGridViewComboBoxColumn col = dataGridView.Columns["colDVT"] as DataGridViewComboBoxColumn;
                if (col != null)
                {
                    col.DataPropertyName = "ID_DON_VI_TINH";
                    col.Visible = true;

                    // Nếu controller chưa nạp danh sách ĐVT cho cột thì lấy từ cmbDVT
                    // (dùng DataView riêng để chọn ở lưới không làm đổi combobox phía trên).
                    if (col.DataSource == null && cmbDVT.DataSource != null
                        && !string.IsNullOrEmpty(cmbDVT.ValueMember)
                        && !string.IsNullOrEmpty(cmbDVT.DisplayMember))
                    {
                        DataTable dt = cmbDVT.DataSource as DataTable;
                        col.DataSource = (dt != null) ? (object)new DataView(dt) : cmbDVT.DataSource;
                        col.ValueMember = cmbDVT.ValueMember;
                        col.DisplayMember = cmbDVT.DisplayMember;
                    }
                }
            }
            catch { }
        }

        // Cột "Trạng thái" không gắn dữ liệu nên tính ngay lúc vẽ ô (không bị mất khi lưới nạp lại).
        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dataGridView.Columns[e.ColumnIndex].Name != "colTrangThai") return;

            decimal sl = 0;
            object v = dataGridView.Rows[e.RowIndex].Cells["colSoLuong"].Value;
            if (v != null && v != DBNull.Value)
                decimal.TryParse(v.ToString(), out sl);

            e.Value = (sl <= 0) ? "Hết hàng" : "Còn hàng";
            e.CellStyle.ForeColor = (sl <= 0) ? Color.Red : Color.Green;
            e.FormattingApplied = true;
        }

        private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DamBaoBindingDVT();
            dataGridView.Invalidate();
        }

        private void ApDungPhanQuyen()
        {
            bool ok = CoQuyenSua;

            bindingNavigatorAddNewItem.Enabled = ok;
            bindingNavigatorDeleteItem.Enabled = ok;
            toolLuu.Enabled = ok;

            if (btnThemDVT != null)
                btnThemDVT.Enabled = ok;

            txtTenSanPham.ReadOnly = !ok;
            cmbDVT.Enabled = ok;
            numSoLuong.Enabled = ok;
            numDonGiaNhap.Enabled = ok;
            numGiaBanSi.Enabled = ok;
            numGiaBanLe.Enabled = ok;
        }

        private void CapNhatTongTon()
        {
            if (lblTongTon == null) return;

            try
            {
                decimal tongSL = 0, tongGT = 0;

                foreach (DataGridViewRow r in dataGridView.Rows)
                {
                    if (r.IsNewRow) continue;

                    decimal sl = 0, gn = 0;

                    if (r.Cells["colSoLuong"].Value != null)
                        decimal.TryParse(r.Cells["colSoLuong"].Value.ToString(), out sl);

                    if (r.Cells["colDongiaNhap"].Value != null)
                        decimal.TryParse(r.Cells["colDongiaNhap"].Value.ToString(), out gn);

                    tongSL += sl;
                    tongGT += sl * gn;
                }

                lblTongTon.Text = "Tổng SL: " + tongSL.ToString("N0")
                    + "   |   Tổng giá trị (giá nhập): " + tongGT.ToString("N0") + " đ";
            }
            catch
            {
                lblTongTon.Text = "Tổng tồn: —";
            }
        }

        private void CapNhatCotTrangThai()
        {
            if (!dataGridView.Columns.Contains("colTrangThai")) return;

            foreach (DataGridViewRow r in dataGridView.Rows)
            {
                if (r.IsNewRow) continue;

                decimal sl = 0;
                try
                {
                    if (r.Cells["colSoLuong"].Value != null)
                        decimal.TryParse(r.Cells["colSoLuong"].Value.ToString(), out sl);
                }
                catch { }

                r.Cells["colTrangThai"].Value = (sl <= 0) ? "Hết hàng" : "Còn hàng";
            }
        }

        private bool KiemTraDuLieu()
        {
            if (string.IsNullOrEmpty(txtTenSanPham.Text) || txtTenSanPham.Text.Trim() == "")
            {
                MessageBox.Show("Tên sản phẩm không được để trống.", "Sản phẩm",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSanPham.Focus();
                return false;
            }

            // Bắt buộc chọn ĐVT (sửa lỗi PM-42, PM-101)
            if (cmbDVT.SelectedValue == null || cmbDVT.SelectedIndex < 0)
            {
                MessageBox.Show("Phải chọn Đơn vị tính.", "Sản phẩm",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbDVT.Focus();
                return false;
            }

            if (numDonGiaNhap.Value < 0 || numGiaBanSi.Value < 0 || numGiaBanLe.Value < 0)
            {
                MessageBox.Show("Giá không được âm.", "Sản phẩm",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ========== LƯU ==========
        private void toolLuu_Click(object sender, EventArgs e)
        {
            if (!CoQuyenSua) return;

            bindingNavigatorPositionItem.Focus();
            this.Validate();

            if (bindingNavigator.BindingSource != null)
                bindingNavigator.BindingSource.EndEdit();

            if (!KiemTraDuLieu()) return;

            try
            {
                ctrl.Save();
                MessageBox.Show("Lưu thành công.", "Sản phẩm",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CapNhatTongTon();
                CapNhatCotTrangThai();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu thất bại: " + ex.Message, "Sản phẩm",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== THÊM ==========
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            if (!CoQuyenSua) return;

            DataRow row = ctrl.NewRow();

            // Sinh mã tự động
            long maso = ThamSo.SanPham;
            ThamSo.SanPham = maso + 1;

            row["ID"] = maso;
            row["TEN_SAN_PHAM"] = "";
            row["SO_LUONG"] = 0;
            row["DON_GIA_NHAP"] = 0;
            row["GIA_BAN_SI"] = 0;
            row["GIA_BAN_LE"] = 0;

            // Bắt buộc có ĐVT
            if (cmbDVT.SelectedValue != null)
            {
                row["ID_DON_VI_TINH"] = cmbDVT.SelectedValue;
            }
            else if (cmbDVT.Items.Count > 0)
            {
                cmbDVT.SelectedIndex = 0;
                if (cmbDVT.SelectedValue != null)
                    row["ID_DON_VI_TINH"] = cmbDVT.SelectedValue;
            }

            ctrl.Add(row);
            bindingNavigator.BindingSource.MoveLast();

            txtMaSanPham.Text = maso.ToString();
            txtTenSanPham.Focus();
        }

        // ========== XÓA ==========
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (!CoQuyenSua) return;

            if (bindingNavigator.BindingSource == null ||
                bindingNavigator.BindingSource.Current == null)
            {
                MessageBox.Show("Không có sản phẩm nào để xóa.", "Sản phẩm",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ten = txtTenSanPham.Text;
            if (string.IsNullOrEmpty(ten) || ten.Trim() == "") ten = "(không tên)";

            if (MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa sản phẩm \"" + ten + "\"?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                bindingNavigator.BindingSource.RemoveCurrent();

                MessageBox.Show("Đã xóa khỏi danh sách.\nBấm Lưu để cập nhật vào cơ sở dữ liệu.",
                    "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CapNhatTongTon();
                CapNhatCotTrangThai();
            }
            catch (Exception ex)
            {
                // PM-35: báo lỗi rõ ràng khi xóa bị chặn
                MessageBox.Show(
                    "Không xóa được sản phẩm này.\n\n" +
                    "Lý do: " + ex.Message + "\n\n" +
                    "Sản phẩm có thể đang được sử dụng trong phiếu nhập hoặc phiếu bán.",
                    "Không xóa được",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========== THOÁT ==========
        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // TẠM THỜI để tìm lỗi ĐVT: báo lỗi đầu tiên của lưới (xong việc thì xóa phần MessageBox).
        private bool _daBaoLoiLuoi = false;
        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // ĐVT không hợp lệ (ID không còn trong bảng ĐVT) thì để ô trống, không báo lỗi.
            e.Cancel = true;
        }

        // TẠM THỜI: hiện thông tin để biết vì sao ĐVT trống (xong việc thì xóa hàm này và dòng gọi nó).
        private void ChanDoanDVT()
        {
            try
            {
                DataTable dvt = cmbDVT.DataSource as DataTable;
                BindingSource bs = bindingNavigator.BindingSource;
                DataTable sp = (bs != null) ? bs.DataSource as DataTable : null;

                string kq = "";
                if (dvt == null)
                {
                    kq += "cmbDVT chưa có DataSource.\n";
                }
                else
                {
                    kq += "Số đơn vị tính nạp được: " + dvt.Rows.Count + "\n";
                    kq += "Kiểu cột DON_VI_TINH.ID: " + dvt.Columns["ID"].DataType.Name + "\n";
                    string ds = "";
                    for (int i = 0; i < dvt.Rows.Count && i < 10; i++)
                        ds += dvt.Rows[i]["ID"] + "=" + dvt.Rows[i]["TEN_DON_VI"] + "; ";
                    kq += "Danh sách: " + ds + "\n";
                }

                if (sp == null || !sp.Columns.Contains("ID_DON_VI_TINH"))
                {
                    kq += "Dữ liệu sản phẩm KHÔNG có cột ID_DON_VI_TINH.\n";
                }
                else
                {
                    kq += "Kiểu cột SAN_PHAM.ID_DON_VI_TINH: " + sp.Columns["ID_DON_VI_TINH"].DataType.Name + "\n";
                    string gt = "";
                    for (int i = 0; i < sp.Rows.Count && i < 5; i++)
                    {
                        object v = sp.Rows[i]["ID_DON_VI_TINH"];
                        gt += (v == DBNull.Value ? "NULL" : v.ToString()) + ", ";
                    }
                    kq += "ID_DON_VI_TINH của các sản phẩm đầu: " + gt + "\n";
                }

                MessageBox.Show(kq, "Chẩn đoán ĐVT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chẩn đoán lỗi: " + ex.Message);
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            CapNhatTongTon();
            CapNhatCotTrangThai();
        }

        // ========== THÊM ĐƠN VỊ TÍNH ==========
        private void btnThemDVT_Click(object sender, EventArgs e)
        {
            frmDonViTinh frm = new frmDonViTinh();
            frm.ShowDialog(this);
            ctrlDVT.HienthiAutoComboBox(cmbDVT);
        }

        // ========== TÌM KIẾM ==========
        private void toolTimMaSanPham_Click(object sender, EventArgs e)
        {
            toolTimMaSanPham.Checked = true;
            toolTimTenSanPham.Checked = false;
            toolTimSanPham.Text = "";
            toolTimSanPham.ForeColor = Color.Black;
            toolTimSanPham.Focus();
        }

        private void toolTimTenSanPham_Click(object sender, EventArgs e)
        {
            toolTimMaSanPham.Checked = false;
            toolTimTenSanPham.Checked = true;
            toolTimSanPham.Text = "";
            toolTimSanPham.ForeColor = Color.Black;
            toolTimSanPham.Focus();
        }

        private void toolTimSanPham_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                TimSanPham();
            }
        }

        private void toolTimSanPham_Leave(object sender, EventArgs e)
        {
            if (toolTimSanPham.Text.Trim() != "" && toolTimSanPham.ForeColor != Color.Gray)
            {
                TimSanPham();
            }
            else if (string.IsNullOrEmpty(toolTimSanPham.Text) || toolTimSanPham.Text.Trim() == "")
            {
                toolTimSanPham.Text = "Tìm theo mã...";
                toolTimSanPham.ForeColor = Color.Gray;
            }
        }

        private void toolTimSanPham_Enter(object sender, EventArgs e)
        {
            if (toolTimSanPham.ForeColor == Color.Gray)
            {
                toolTimSanPham.Text = "";
                toolTimSanPham.ForeColor = Color.Black;
            }
        }

        private void TimSanPham()
        {
            string key = toolTimSanPham.Text.Trim();

            if (key == "" || key == "Tìm theo mã...")
            {
                ctrl.HienthiDataGridview(dataGridView, bindingNavigator,
                    txtMaSanPham, txtTenSanPham, cmbDVT,
                    numSoLuong, numDonGiaNhap, numGiaBanSi, numGiaBanLe);

                CapNhatTongTon();
                CapNhatCotTrangThai();
                return;
            }

            if (toolTimMaSanPham.Checked)
                ctrl.TimMaSanPham(key);
            else
                ctrl.TimTenSanPham(key);

            CapNhatTongTon();
            CapNhatCotTrangThai();
        }

        private void bindingNavigatorCountItem_Click(object sender, EventArgs e)
        {

        }
        private void ThongBao(string noiDung)
        {
            // Tạm: hiện trên tiêu đề form (không cần label)
            if (!string.IsNullOrEmpty(noiDung))
                this.Text = "Sản phẩm - " + noiDung;
        }

        private void toolSua_Click(object sender, EventArgs e)
        {
            if (!CoQuyenSua)
            {
                MessageBox.Show("Bạn không có quyền sửa sản phẩm.", "Sản phẩm",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (bindingNavigator.BindingSource == null
                || bindingNavigator.BindingSource.Current == null)
            {
                MessageBox.Show("Chưa chọn sản phẩm nào để sửa.\nChọn một dòng hoặc bấm Thêm.",
                    "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Cho phép sửa các ô (trừ mã SP)
            txtMaSanPham.ReadOnly = true;
            txtTenSanPham.ReadOnly = false;
            cmbDVT.Enabled = true;
            numSoLuong.Enabled = true;
            numDonGiaNhap.Enabled = true;
            numGiaBanSi.Enabled = true;
            numGiaBanLe.Enabled = true;
            dataGridView.ReadOnly = false;

            // Focus tên để sửa
            txtTenSanPham.Focus();
            txtTenSanPham.SelectAll();

            ThongBao("Đang sửa. Chỉnh xong bấm Lưu để cập nhật.");
        }

        private void toolNhatKy_Click(object sender, EventArgs e)
        {

        }
        private void DatCheDoSua(bool sua)
        {
            m_CheDoSua = sua;

            // Mã luôn chỉ đọc
            txtMaSanPham.ReadOnly = true;

            txtTenSanPham.ReadOnly = !sua;
            cmbDVT.Enabled = sua;
            numSoLuong.Enabled = sua;
            numDonGiaNhap.Enabled = sua;
            numGiaBanSi.Enabled = sua;
            numGiaBanLe.Enabled = sua;
            dataGridView.ReadOnly = !sua;

            bindingNavigatorAddNewItem.Enabled = CoQuyenSua && true; // Thêm vẫn bật khi có quyền
            bindingNavigatorDeleteItem.Enabled = CoQuyenSua;
            toolLuu.Enabled = CoQuyenSua;

            // Label dưới form (thêm trên Design: lblCheDo)
            
        }

        private void GhiNhatKy(string noiDung)
        {
            string dong = DateTime.Now.ToString("HH:mm:ss") + " - " + noiDung;
            m_NhatKy.Insert(0, dong);
            if (m_NhatKy.Count > 100) m_NhatKy.RemoveAt(m_NhatKy.Count - 1);
            ThongBao(noiDung);
        }

       
    }
}