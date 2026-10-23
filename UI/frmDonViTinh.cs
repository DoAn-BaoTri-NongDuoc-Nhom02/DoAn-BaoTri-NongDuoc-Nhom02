using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmDonViTinh : Form
    {
        DonViTinhController ctrl = new DonViTinhController();

        // Đổi theo hệ thống đăng nhập thực tế của nhóm
        // Ví dụ: UserContext.IsAdmin, Program.CurrentUser.Role, ...
        private bool CoQuyenSua
        {
            get
            {
                try
                {
                    // TODO: thay bằng biến đăng nhập thật
                    // return Program.NguoiDungHienTai != null && Program.NguoiDungHienTai.IsAdmin;
                    return true; // tạm: cho phép; khi có login thì siết lại
                }
                catch
                {
                    return false;
                }
            }
        }

        public frmDonViTinh()
        {
            InitializeComponent();
        }

        private void frmDonViTinh_Load(object sender, EventArgs e)
        {
            UiTheme.Apply(this);
            this.Text = "Đơn vị tính";
            this.StartPosition = FormStartPosition.CenterParent;

            // Đếm bản ghi dễ đọc
            bindingNavigator.CountItemFormat = "/ {0} bản ghi";

            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);

            // Cột Mã đơn vị (nếu chưa có trên Designer)
            DamBaoCotMa();

            ApDungPhanQuyen();
            CapNhatHuongDan();
        }

        private void DamBaoCotMa()
        {
            // colId đang ẩn: hiện thành "Mã ĐVT"
            if (colId != null)
            {
                colId.Visible = true;
                colId.HeaderText = "Mã ĐVT";
                colId.Width = 80;
                colId.ReadOnly = true; // mã do hệ thống / không sửa lung tung
                colId.DisplayIndex = 0;
            }
            if (colTenDV != null)
            {
                colTenDV.HeaderText = "Tên đơn vị";
                colTenDV.Width = 220;
                colTenDV.DisplayIndex = 1;
            }
        }

        private void ApDungPhanQuyen()
        {
            bool choPhep = CoQuyenSua;
            bindingNavigatorAddNewItem.Enabled = choPhep;
            bindingNavigatorDeleteItem.Enabled = choPhep;
            toolLuu.Enabled = choPhep;
            if (toolSua != null) toolSua.Enabled = choPhep;
            dataGridView.ReadOnly = !choPhep;
            dataGridView.AllowUserToAddRows = choPhep;
            dataGridView.AllowUserToDeleteRows = choPhep;
        }

        private void CapNhatHuongDan()
        {
            if (lblHuongDan != null)
            {
                lblHuongDan.Text = CoQuyenSua
                    ? "Hướng dẫn: Thêm / sửa trực tiếp trên lưới → bấm Lưu. Không để trống tên. Không trùng tên."
                    : "Bạn chỉ được xem. Liên hệ Admin để thêm / sửa / xóa đơn vị tính.";
            }
        }

        // ========== THÊM ==========
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            // BindingNavigator thường tự AddNew; chỉ focus cột tên
            dataGridView.Focus();
            if (dataGridView.CurrentRow != null && colTenDV != null)
                dataGridView.CurrentCell = dataGridView.CurrentRow.Cells[colTenDV.Name];
        }

        // ========== SỬA ==========
        private void toolSua_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null || dataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Hãy chọn một dòng để sửa.", "Đơn vị tính",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dataGridView.BeginEdit(true);
            if (colTenDV != null)
                dataGridView.CurrentCell = dataGridView.CurrentRow.Cells[colTenDV.Name];
        }

        // ========== XÓA (có xác nhận) ==========
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null || dataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Không có dòng nào để xóa.", "Đơn vị tính",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ten = "";
            try
            {
                object v = dataGridView.CurrentRow.Cells[colTenDV.Name].Value;
                ten = v == null ? "" : v.ToString();
            }
            catch { }

            DialogResult r = MessageBox.Show(
                "Bạn có chắc muốn xóa đơn vị tính \"" + ten + "\"?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
            {
                // Hủy thao tác xóa mặc định nếu BindingNavigator đã xóa — reload
                ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
                return;
            }

            try
            {
                // Nếu DeleteItem của navigator đã gán sẵn, có thể chỉ cần Save sau
                // Ở đây xóa dòng hiện tại rồi Lưu
                if (!dataGridView.CurrentRow.IsNewRow)
                    dataGridView.Rows.Remove(dataGridView.CurrentRow);

                ctrl.Save();
                MessageBox.Show("Đã xóa.", "Đơn vị tính",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa thất bại: " + ex.Message, "Đơn vị tính",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
            }
        }

        // ========== LƯU ==========
        private void toolLuu_Click(object sender, EventArgs e)
        {
            dataGridView.EndEdit();
            if (dataGridView.DataSource is BindingSource)
                ((BindingSource)dataGridView.DataSource).EndEdit();

            if (!KiemTraDuLieu())
                return;

            try
            {
                ctrl.Save();
                MessageBox.Show("Lưu thành công.", "Đơn vị tính",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lưu thất bại: " + ex.Message, "Đơn vị tính",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== ĐÓNG ==========
        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ========== VALIDATION ==========
        private bool KiemTraDuLieu()
        {
            // Regex: chữ cái, số, khoảng trắng, một số ký tự VN/đơn vị thường gặp
            Regex tenHopLe = new Regex(@"^[\p{L}\p{N}\s\.\-\/]+$");

            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                DataGridViewRow row = dataGridView.Rows[i];
                if (row.IsNewRow) continue;

                object val = row.Cells[colTenDV.Name].Value;
                string ten = val == null ? "" : val.ToString().Trim();

                if (ten.Length == 0)
                {
                    MessageBox.Show("Tên đơn vị không được để trống (dòng " + (i + 1) + ").",
                        "Đơn vị tính", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dataGridView.CurrentCell = row.Cells[colTenDV.Name];
                    return false;
                }

                if (!tenHopLe.IsMatch(ten))
                {
                    MessageBox.Show("Tên đơn vị có ký tự không hợp lệ (dòng " + (i + 1) + ").",
                        "Đơn vị tính", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dataGridView.CurrentCell = row.Cells[colTenDV.Name];
                    return false;
                }

                // Trùng tên
                for (int j = i + 1; j < dataGridView.Rows.Count; j++)
                {
                    DataGridViewRow row2 = dataGridView.Rows[j];
                    if (row2.IsNewRow) continue;
                    object val2 = row2.Cells[colTenDV.Name].Value;
                    string ten2 = val2 == null ? "" : val2.ToString().Trim();
                    if (string.Compare(ten, ten2, true) == 0)
                    {
                        MessageBox.Show("Tên đơn vị bị trùng: \"" + ten + "\".",
                            "Đơn vị tính", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            return true;
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}