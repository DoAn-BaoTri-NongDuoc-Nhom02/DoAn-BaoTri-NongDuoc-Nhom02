using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmThongtinCuahang : Form
    {
        public frmThongtinCuahang()
        {
            InitializeComponent();
        }

        private void frmThongtinCuahang_Load(object sender, EventArgs e)
        {
            this.Text = "Thông tin cửa hàng";

            CuahangNongduoc.BusinessObject.CuaHang ch = ThamSo.LayCuaHang();
            txtTenCuaHang.Text = ch.TenCuaHang != null ? ch.TenCuaHang : "";
            txtDienThoai.Text = ch.DienThoai != null ? ch.DienThoai : "";
            txtDiaChi.Text = ch.DiaChi != null ? ch.DiaChi : "";

            // Nếu đã có Email / MST trên entity + DB thì bỏ comment:
            // txtEmail.Text = ch.Email != null ? ch.Email : "";
            // txtMaSoThue.Text = ch.MaSoThue != null ? ch.MaSoThue : "";

            SetPlaceholder(txtTenCuaHang, "Nhập tên cửa hàng");
            SetPlaceholder(txtDienThoai, "VD: 0901234567");
            SetPlaceholder(txtDiaChi, "Nhập địa chỉ đầy đủ");
            SetPlaceholder(txtEmail, "VD: cuahang@email.com");
            SetPlaceholder(txtMaSoThue, "VD: 0123456789");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string ten = GetValue(txtTenCuaHang, "Nhập tên cửa hàng");
            string dt = GetValue(txtDienThoai, "VD: 0901234567");
            string dc = GetValue(txtDiaChi, "Nhập địa chỉ đầy đủ");
            string email = GetValue(txtEmail, "VD: cuahang@email.com");
            string mst = GetValue(txtMaSoThue, "VD: 0123456789");

            if (IsEmpty(ten))
            {
                MessageBox.Show("Vui lòng nhập tên cửa hàng.", "Thông tin cửa hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenCuaHang.Focus();
                return;
            }

            if (IsEmpty(dt))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.", "Thông tin cửa hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDienThoai.Focus();
                return;
            }

            if (!Regex.IsMatch(dt, @"^(0|\+84)[0-9]{8,10}$"))
            {
                MessageBox.Show(
                    "Số điện thoại không hợp lệ.\nChỉ gồm số, bắt đầu bằng 0 hoặc +84, dài 9–11 chữ số.",
                    "Thông tin cửa hàng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtDienThoai.Focus();
                return;
            }

            if (IsEmpty(dc))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ.", "Thông tin cửa hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiaChi.Focus();
                return;
            }

            if (!IsEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không đúng định dạng.", "Thông tin cửa hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (!IsEmpty(mst) && !Regex.IsMatch(mst, @"^[0-9]{10,14}$"))
            {
                MessageBox.Show("Mã số thuế chỉ gồm 10–14 chữ số.", "Thông tin cửa hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaSoThue.Focus();
                return;
            }

            // Lưu 3 field hiện có trong ThamSo (tương thích bản cũ)
            ThamSo.GanCuaHang(ten, dc, dt);

            // Sau khi thêm cột EMAIL, MA_SO_THUE vào DB + entity:
            // ThamSo.GanCuaHang(ten, dc, dt, email, mst);

            MessageBox.Show("Đã lưu thông tin cửa hàng.", "Thông tin cửa hàng",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Thông tin cửa hàng dùng in trên tiêu đề báo cáo / phiếu.\n\n" +
                "• Tên, Điện thoại, Địa chỉ: bắt buộc.\n" +
                "• Email, Mã số thuế: nên nhập đủ để dùng trên hóa đơn.\n" +
                "• Điện thoại: 0xxxxxxxxx hoặc +84xxxxxxxx.",
                "Hướng dẫn",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void txtDienThoai_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '+')
            {
                e.Handled = true;
            }
        }

        // ================== Hàm phụ ==================

        private bool IsEmpty(string s)
        {
            return s == null || s.Trim().Length == 0;
        }

        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            if (IsEmpty(txt.Text))
            {
                txt.Text = placeholder;
                txt.ForeColor = Color.Gray;
            }

            txt.Enter += delegate (object sender, EventArgs e)
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.Leave += delegate (object sender, EventArgs e)
            {
                if (IsEmpty(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                }
            };
        }

        private string GetValue(TextBox txt, string placeholder)
        {
            if (txt.Text == placeholder)
                return "";
            return txt.Text.Trim();
        }
    }
}