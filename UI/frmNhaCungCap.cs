using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmNhaCungCap : Form
    {
        private const string TIEU_DE = "Nhà cung cấp";

        // Chế độ tìm: ALL, HO_TEN, DIA_CHI, DIEN_THOAI
        private string m_CheDoTim = "ALL";
        private bool m_DangGoiY = true;       // ô tìm đang hiển thị chữ gợi ý (không phải từ khóa thật)
        private bool m_DangDatChu = false;    // đang tự đặt chữ vào ô tìm, bỏ qua TextChanged

        CuahangNongduoc.Controller.NhaCungCapController ctrl = new CuahangNongduoc.Controller.NhaCungCapController();

        public frmNhaCungCap()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------------
        // Tải dữ liệu, trạng thái
        // ---------------------------------------------------------------
        private void frmNhaCungCap_Load(object sender, EventArgs e)
        {
            UiTheme.Apply(this);
            TaiLaiDuLieu();
            HienGoiY();
        }

        private void TaiLaiDuLieu()
        {
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
            bindingNavigator.BindingSource.ListChanged += new ListChangedEventHandler(bs_ListChanged);
            ApDungLoc();
        }

        private void bs_ListChanged(object sender, ListChangedEventArgs e)
        {
            CapNhatTrangThai();
        }

        private DataTable LayBang()
        {
            BindingSource bs = bindingNavigator.BindingSource;
            return (bs == null) ? null : bs.DataSource as DataTable;
        }

        private void ThongBao(string noiDung)
        {
            lblThongBao.Text = noiDung;
        }

        private void CapNhatTrangThai()
        {
            BindingSource bs = bindingNavigator.BindingSource;
            DataTable tbl = LayBang();
            if (tbl == null) return;

            int tong = 0;
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState != DataRowState.Deleted) tong++;
            }
            int hienThi = bs.Count;

            if (hienThi == tong)
                lblTong.Text = "Tổng số: " + tong.ToString() + " nhà cung cấp";
            else
                lblTong.Text = "Hiển thị " + hienThi.ToString() + " / " + tong.ToString() + " nhà cung cấp";

            if (tong == 0)
            {
                lblChuaCoDuLieu.Text = "Chưa có dữ liệu nhà cung cấp.\nBấm \"Thêm\" để tạo mới.";
                lblChuaCoDuLieu.Visible = true;
            }
            else if (hienThi == 0)
            {
                lblChuaCoDuLieu.Text = "Không tìm thấy nhà cung cấp phù hợp.";
                lblChuaCoDuLieu.Visible = true;
            }
            else
            {
                lblChuaCoDuLieu.Visible = false;
            }
        }

        // ---------------------------------------------------------------
        // Thêm / Sửa
        // ---------------------------------------------------------------
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            XoaLoc();   // dòng mới đang trống, nếu còn bộ lọc sẽ bị ẩn mất

            long maso = ThamSo.NhaCungCap;
            ThamSo.NhaCungCap = maso + 1;

            DataRowView row = (DataRowView)bindingNavigator.BindingSource.AddNew();
            row["ID"] = maso.ToString();

            BatDauSua(colHoTen.Index);
            ThongBao("Đã thêm dòng mới (mã số " + maso.ToString() + "). Nhập thông tin rồi bấm Lưu.");
        }

        private void toolSua_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView.CurrentRow;
            if (row == null || row.IsNewRow)
            {
                MessageBox.Show("Chưa chọn nhà cung cấp nào để sửa.\nHãy chọn một dòng hoặc bấm Thêm để tạo mới.",
                    TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int cot = colHoTen.Index;
            if (dataGridView.CurrentCell != null && dataGridView.CurrentCell.ColumnIndex != colID.Index)
                cot = dataGridView.CurrentCell.ColumnIndex;
            BatDauSua(cot);
            ThongBao("Đang sửa. Nhập xong bấm Enter, rồi bấm Lưu để cập nhật.");
        }

        private void BatDauSua(int cot)
        {
            DataGridViewRow row = dataGridView.CurrentRow;
            if (row == null) return;
            dataGridView.Focus();
            dataGridView.CurrentCell = row.Cells[cot];
            dataGridView.BeginEdit(true);
        }

        // Báo lỗi ngay trên ô khi vừa nhập xong (không chặn thao tác, việc chặn nằm ở nút Lưu)
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string giaTri = Convert.ToString(cell.Value);

            if (e.ColumnIndex == colHoTen.Index)
            {
                cell.ErrorText = (giaTri.Trim().Length == 0) ? "Chưa nhập tên nhà cung cấp" : "";
            }
            else if (e.ColumnIndex == colDienThoai.Index)
            {
                bool hopLe = giaTri.Trim().Length == 0 ||
                    CuahangNongduoc.Controller.NhaCungCapController.SoDienThoaiHopLe(giaTri);
                cell.ErrorText = hopLe ? "" : "Số điện thoại không hợp lệ";
            }
        }

        // ---------------------------------------------------------------
        // Xóa có xác nhận
        // ---------------------------------------------------------------
        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string ten = Convert.ToString(e.Row.Cells[colHoTen.Index].Value);
            if (!XacNhanXoa(Convert.ToString(e.Row.Cells[colID.Index].Value), ten))
            {
                e.Cancel = true;
            }
            else
            {
                ThongBao("Đã xóa \"" + ten + "\" (chưa lưu, bấm Lưu để cập nhật cơ sở dữ liệu).");
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DataRowView dong = bindingNavigator.BindingSource.Current as DataRowView;
            if (dong == null)
            {
                MessageBox.Show("Chưa chọn nhà cung cấp nào để xóa.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string ten = Convert.ToString(dong["HO_TEN"]);
            if (XacNhanXoa(Convert.ToString(dong["ID"]), ten))
            {
                bindingNavigator.BindingSource.RemoveCurrent();
                ThongBao("Đã xóa \"" + ten + "\" (chưa lưu, bấm Lưu để cập nhật cơ sở dữ liệu).");
            }
        }

        private bool XacNhanXoa(string id, string ten)
        {
            string hienTen = (ten.Trim().Length == 0) ? "(chưa nhập tên)" : ten;

            if (id.Length > 0 && ctrl.NhaCungCapDaSuDung(id))
            {
                MessageBox.Show("Nhà cung cấp \"" + hienTen + "\" đã có phiếu nhập hàng nên không thể xóa.\n" +
                    "Bạn chỉ có thể sửa lại thông tin.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return MessageBox.Show("Bạn có chắc chắn xóa nhà cung cấp \"" + hienTen + "\" không?\n" +
                "Thao tác chỉ có hiệu lực sau khi bấm Lưu.", TIEU_DE,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        // ---------------------------------------------------------------
        // Lưu: kiểm tra dữ liệu, hỏi xác nhận, báo kết quả
        // ---------------------------------------------------------------
        private void toolLuu_Click(object sender, EventArgs e)
        {
            LuuDuLieu(true);
        }

        private bool LuuDuLieu(bool hoiXacNhan)
        {
            dataGridView.EndEdit();
            bindingNavigator.BindingSource.EndEdit();

            DataTable tbl = LayBang();
            if (tbl == null) return true;

            if (tbl.GetChanges() == null)
            {
                if (hoiXacNhan)
                    MessageBox.Show("Không có thay đổi nào để lưu.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            DataRow dongLoi;
            string cotLoi;
            string thongBaoLoi;
            if (!ctrl.KiemTraHopLe(tbl, out dongLoi, out cotLoi, out thongBaoLoi))
            {
                MessageBox.Show(thongBaoLoi, TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ChonDong(dongLoi, cotLoi);
                return false;
            }

            int them = 0, sua = 0, xoa = 0;
            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Added) them++;
                else if (r.RowState == DataRowState.Modified) sua++;
                else if (r.RowState == DataRowState.Deleted) xoa++;
            }

            if (hoiXacNhan)
            {
                string noiDung = "Bạn có chắc chắn muốn lưu các thay đổi sau?\n" +
                    "  - Thêm mới: " + them.ToString() + "\n" +
                    "  - Sửa: " + sua.ToString() + "\n" +
                    "  - Xóa: " + xoa.ToString();
                if (MessageBox.Show(noiDung, TIEU_DE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return false;
            }

            if (ctrl.Save())
            {
                TaiLaiDuLieu();
                ThongBao("Đã lưu: thêm " + them.ToString() + ", sửa " + sua.ToString() + ", xóa " + xoa.ToString() + ".");
                MessageBox.Show("Đã lưu thành công.", TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            MessageBox.Show("Lưu không thành công. Dữ liệu vẫn được giữ trên màn hình, vui lòng thử lại.",
                TIEU_DE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private void ChonDong(DataRow dong, string cot)
        {
            BindingSource bs = bindingNavigator.BindingSource;
            int viTri = TimViTri(bs, dong);
            if (viTri < 0)
            {
                XoaLoc();
                viTri = TimViTri(bs, dong);
            }
            if (viTri < 0) return;

            int chiSoCot = colHoTen.Index;
            if (cot == "DIA_CHI") chiSoCot = colDiaChi.Index;
            else if (cot == "DIEN_THOAI") chiSoCot = colDienThoai.Index;
            else if (cot == "ID") chiSoCot = colID.Index;

            bs.Position = viTri;
            dataGridView.Focus();
            dataGridView.CurrentCell = dataGridView.Rows[viTri].Cells[chiSoCot];
            dataGridView.BeginEdit(true);
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
        // Thoát: nằm tách xa nút Lưu, và hỏi nếu còn thay đổi chưa lưu
        // ---------------------------------------------------------------
        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmNhaCungCap_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataTable tbl = LayBang();
            if (tbl == null) return;

            dataGridView.EndEdit();
            bindingNavigator.BindingSource.EndEdit();
            if (tbl.GetChanges() == null) return;

            DialogResult kq = MessageBox.Show("Còn thay đổi chưa lưu. Bạn có muốn lưu trước khi đóng không?",
                TIEU_DE, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (kq == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (kq == DialogResult.Yes)
            {
                if (!LuuDuLieu(false)) e.Cancel = true;
            }
        }

        // ---------------------------------------------------------------
        // Tìm kiếm / lọc: lọc ngay khi gõ, chọn cột cần tìm ở nút kính lúp
        // ---------------------------------------------------------------
        private string TenCheDo()
        {
            switch (m_CheDoTim)
            {
                case "HO_TEN": return "Nhà cung cấp";
                case "DIA_CHI": return "Địa chỉ";
                case "DIEN_THOAI": return "Điện thoại";
                default: return "tất cả các cột";
            }
        }

        private void HienGoiY()
        {
            m_DangDatChu = true;
            toolTimNhaCungCap.Text = "Tìm theo " + TenCheDo() + "...";
            toolTimNhaCungCap.ForeColor = Color.Gray;
            m_DangGoiY = true;
            m_DangDatChu = false;
        }

        private void toolTimNhaCungCap_Enter(object sender, EventArgs e)
        {
            if (m_DangGoiY)
            {
                m_DangDatChu = true;
                toolTimNhaCungCap.Text = "";
                toolTimNhaCungCap.ForeColor = Color.Black;
                m_DangGoiY = false;
                m_DangDatChu = false;
            }
        }

        private void toolTimNhaCungCap_Leave(object sender, EventArgs e)
        {
            if (toolTimNhaCungCap.Text.Trim().Length == 0)
            {
                HienGoiY();
            }
        }

        private void toolTimNhaCungCap_TextChanged(object sender, EventArgs e)
        {
            if (m_DangDatChu) return;
            ApDungLoc();
        }

        private void toolTimNhaCungCap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                XoaLoc();
                e.SuppressKeyPress = true;
            }
        }

        private void toolXoaLoc_Click(object sender, EventArgs e)
        {
            XoaLoc();
        }

        private void XoaLoc()
        {
            m_DangDatChu = true;
            toolTimNhaCungCap.Text = "";
            m_DangDatChu = false;
            ApDungLoc();
            if (!toolTimNhaCungCap.Focused) HienGoiY();
        }

        private void DatCheDoTim(string cheDo)
        {
            m_CheDoTim = cheDo;
            toolTimTatCa.Checked = (cheDo == "ALL");
            toolTimHoTen.Checked = (cheDo == "HO_TEN");
            toolTimDiaChi.Checked = (cheDo == "DIA_CHI");
            toolTimDienThoai.Checked = (cheDo == "DIEN_THOAI");

            if (m_DangGoiY) HienGoiY();
            ApDungLoc();
            dataGridView.Focus();
        }

        private void toolTimTatCa_Click(object sender, EventArgs e) { DatCheDoTim("ALL"); }
        private void toolTimHoTen_Click(object sender, EventArgs e) { DatCheDoTim("HO_TEN"); }
        private void toolTimDiaChi_Click(object sender, EventArgs e) { DatCheDoTim("DIA_CHI"); }
        private void toolTimDienThoai_Click(object sender, EventArgs e) { DatCheDoTim("DIEN_THOAI"); }

        private void ApDungLoc()
        {
            BindingSource bs = bindingNavigator.BindingSource;
            if (bs == null) return;

            string tuKhoa = m_DangGoiY ? "" : toolTimNhaCungCap.Text.Trim();
            if (tuKhoa.Length == 0)
                bs.Filter = null;
            else
                bs.Filter = TaoBieuThucLoc(tuKhoa);
            CapNhatTrangThai();
        }

        private string TaoBieuThucLoc(string tuKhoa)
        {
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
            string like = " LIKE '%" + sb.ToString() + "%'";

            switch (m_CheDoTim)
            {
                case "HO_TEN": return "HO_TEN" + like;
                case "DIA_CHI": return "DIA_CHI" + like;
                case "DIEN_THOAI": return "DIEN_THOAI" + like;
                default:
                    return "HO_TEN" + like + " OR DIA_CHI" + like + " OR DIEN_THOAI" + like +
                           " OR Convert(ID, 'System.String')" + like;
            }
        }
    }
}