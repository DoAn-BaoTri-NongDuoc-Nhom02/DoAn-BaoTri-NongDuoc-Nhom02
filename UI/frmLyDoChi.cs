using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmLyDoChi : Form
    {
        private const string TIEU_DE = "Lý do chi";

        CuahangNongduoc.Controller.LyDoChiController ctrl = new CuahangNongduoc.Controller.LyDoChiController();

        public frmLyDoChi()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------------
        // Tải dữ liệu
        // ---------------------------------------------------------------
        private void frmLyDoChi_Load(object sender, EventArgs e)
        {
            UiTheme.Apply(this);
            // Mục 1: bảng trống thì nạp sẵn vài lý do chi mẫu để người dùng hình dung
            ctrl.KhoiTaoDuLieuMau();
            TaiLaiDuLieu();
        }

        private void TaiLaiDuLieu()
        {
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
            bindingNavigator.BindingSource.ListChanged += new ListChangedEventHandler(bs_ListChanged);
            ApDungLoc();
        }

        private void bs_ListChanged(object sender, ListChangedEventArgs e)
        {
            CapNhatKetQua();
        }

        // ---------------------------------------------------------------
        // Mục 2: cột "Mã lý do" hiển thị dạng LD001, LD002... (không đổi CSDL)
        // ---------------------------------------------------------------
        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colID.Index && e.Value != null && e.Value != DBNull.Value)
            {
                long id;
                if (long.TryParse(Convert.ToString(e.Value), out id))
                {
                    e.Value = "LD" + id.ToString("000");
                    e.FormattingApplied = true;
                }
            }
        }

        // ---------------------------------------------------------------
        // Mục 4: Thêm / Sửa
        // ---------------------------------------------------------------
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            // Bỏ lọc trước khi thêm, nếu không dòng mới (đang trống) sẽ bị lọc ẩn mất
            txtTim.Text = "";
            bindingNavigator.BindingSource.AddNew();
            BatDauSuaDongHienTai();
        }

        private void toolSua_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView.CurrentRow;
            if (row == null || row.IsNewRow)
            {
                MessageBox.Show("Chưa chọn lý do chi nào để sửa.\nHãy chọn một dòng hoặc bấm Thêm để tạo mới.",
                    TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            BatDauSuaDongHienTai();
        }

        private void BatDauSuaDongHienTai()
        {
            DataGridViewRow row = dataGridView.CurrentRow;
            if (row == null) return;
            dataGridView.Focus();
            dataGridView.CurrentCell = row.Cells[colLyDo.Index];
            dataGridView.BeginEdit(true);
        }

        // ---------------------------------------------------------------
        // Mục 6: Xóa có xác nhận (cả nút Xóa lẫn phím Delete trên lưới)
        // ---------------------------------------------------------------
        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            object lyDo = e.Row.Cells[colLyDo.Index].Value;
            if (!XacNhanXoa(e.Row.Cells[colID.Index].Value, Convert.ToString(lyDo)))
            {
                e.Cancel = true;
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DataRowView dong = bindingNavigator.BindingSource.Current as DataRowView;
            if (dong == null)
            {
                MessageBox.Show("Chưa chọn lý do chi nào để xóa.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (XacNhanXoa(dong["ID"], Convert.ToString(dong["LY_DO"])))
            {
                bindingNavigator.BindingSource.RemoveCurrent();
            }
        }

        private bool XacNhanXoa(object id, string lyDo)
        {
            long maSo;
            if (id != null && id != DBNull.Value && long.TryParse(Convert.ToString(id), out maSo) && ctrl.LyDoDaSuDung(maSo))
            {
                MessageBox.Show("Lý do chi \"" + lyDo + "\" đã được dùng trong phiếu chi nên không thể xóa.\n" +
                    "Bạn chỉ có thể sửa lại tên lý do này.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string ten = string.IsNullOrEmpty(lyDo) ? "(chưa nhập)" : lyDo;
            return MessageBox.Show("Bạn có chắc chắn xóa lý do chi \"" + ten + "\" không?\n" +
                "Thao tác chỉ có hiệu lực sau khi bấm Lưu.", TIEU_DE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        // ---------------------------------------------------------------
        // Mục 3: Lưu có kiểm tra dữ liệu
        // ---------------------------------------------------------------
        private void toolLuu_Click(object sender, EventArgs e)
        {
            dataGridView.EndEdit();
            bindingNavigator.BindingSource.EndEdit();

            DataTable tbl = bindingNavigator.BindingSource.DataSource as DataTable;
            if (tbl == null) return;

            if (tbl.GetChanges() == null)
            {
                MessageBox.Show("Không có thay đổi nào để lưu.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataRow dongLoi;
            string thongBao;
            if (!ctrl.KiemTraHopLe(tbl, out dongLoi, out thongBao))
            {
                MessageBox.Show(thongBao, TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ChonDong(dongLoi);
                return;
            }

            if (ctrl.Save())
            {
                TaiLaiDuLieu();
                if (txtTim.Text.Trim().Length == 0)
                {
                    bindingNavigator.BindingSource.MoveLast();
                }
                MessageBox.Show("Đã lưu thành công.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lưu không thành công. Dữ liệu vẫn được giữ trên màn hình, vui lòng thử lại.",
                    TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChonDong(DataRow dong)
        {
            BindingSource bs = bindingNavigator.BindingSource;
            int viTri = TimViTri(bs, dong);
            if (viTri < 0 && txtTim.Text.Length > 0)
            {
                txtTim.Text = "";           // dòng lỗi đang bị bộ lọc ẩn
                viTri = TimViTri(bs, dong);
            }
            if (viTri >= 0)
            {
                bs.Position = viTri;
                dataGridView.Focus();
                dataGridView.CurrentCell = dataGridView.Rows[viTri].Cells[colLyDo.Index];
                dataGridView.BeginEdit(true);
            }
        }

        private static int TimViTri(BindingSource bs, DataRow dong)
        {
            for (int i = 0; i < bs.Count; i++)
            {
                DataRowView v = bs[i] as DataRowView;
                if (v != null && v.Row == dong) return i;
            }
            return -1;
        }

        // ---------------------------------------------------------------
        // Mục 5: Tìm kiếm / lọc theo lý do chi hoặc mã (LD001)
        // ---------------------------------------------------------------
        private void txtTim_TextChanged(object sender, EventArgs e)
        {
            ApDungLoc();
        }

        private void txtTim_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                txtTim.Text = "";
                e.SuppressKeyPress = true;
            }
        }

        private void ApDungLoc()
        {
            BindingSource bs = bindingNavigator.BindingSource;
            if (bs == null) return;

            string tuKhoa = txtTim.Text.Trim();
            if (tuKhoa.Length == 0)
                bs.Filter = null;
            else
                bs.Filter = TaoBieuThucLoc(tuKhoa);
            CapNhatKetQua();
        }

        private static string TaoBieuThucLoc(string tuKhoa)
        {
            // Gõ "LD003" hoặc "ld3" thì lọc theo mã
            string hoa = tuKhoa.ToUpper();
            if (hoa.StartsWith("LD") && hoa.Length > 2)
            {
                long so;
                if (long.TryParse(hoa.Substring(2), out so))
                {
                    return "Convert(ID, 'System.String') = '" + so.ToString() + "'";
                }
            }

            // Ngược lại lọc theo nội dung lý do (escape ký tự đặc biệt của RowFilter)
            StringBuilder sb = new StringBuilder();
            foreach (char c in tuKhoa)
            {
                if (c == '[' || c == ']' || c == '*' || c == '%')
                    sb.Append('[').Append(c).Append(']');
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return "LY_DO LIKE '%" + sb.ToString() + "%'";
        }

        private void CapNhatKetQua()
        {
            BindingSource bs = bindingNavigator.BindingSource;
            DataTable tbl = (bs == null) ? null : bs.DataSource as DataTable;
            if (tbl == null)
            {
                lblKetQua.Text = "";
                return;
            }
            int tong = 0;
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState != DataRowState.Deleted) tong++;
            }
            lblKetQua.Text = "Hiển thị " + bs.Count.ToString() + " / " + tong.ToString() + " lý do";
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}