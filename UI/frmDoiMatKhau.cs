using System;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.Entities;

namespace CuahangNongduoc.UI
{
    /// <summary>Hộp thoại đổi mật khẩu của người đang đăng nhập (dựng bằng code, không cần file designer).</summary>
    public class frmDoiMatKhau : Form
    {
        private TextBox txtMatKhauCu;
        private TextBox txtMatKhauMoi;
        private TextBox txtXacNhan;
        private Button btnLuu;
        private Button btnHuy;

        public frmDoiMatKhau()
        {
            this.Text = "Đổi mật khẩu";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(350, 190);

            Label lblTen = new Label();
            lblTen.AutoSize = true;
            lblTen.Location = new Point(15, 15);
            NguoiDung nd = PhienDangNhap.NguoiDungHienTai;
            lblTen.Text = "Tài khoản: " + (nd != null ? nd.TenDangNhap + " - " + nd.HoTen : "");
            this.Controls.Add(lblTen);

            txtMatKhauCu = TaoDong("Mật khẩu hiện tại", 45);
            txtMatKhauMoi = TaoDong("Mật khẩu mới", 75);
            txtXacNhan = TaoDong("Nhập lại mật khẩu mới", 105);

            btnLuu = new Button();
            btnLuu.Text = "Đổi mật khẩu";
            btnLuu.Location = new Point(140, 145);
            btnLuu.Size = new Size(110, 28);
            btnLuu.Click += new EventHandler(btnLuu_Click);
            this.Controls.Add(btnLuu);

            btnHuy = new Button();
            btnHuy.Text = "Hủy";
            btnHuy.Location = new Point(260, 145);
            btnHuy.Size = new Size(75, 28);
            btnHuy.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnHuy);

            this.AcceptButton = btnLuu;
            this.CancelButton = btnHuy;
        }

        private TextBox TaoDong(string nhan, int y)
        {
            Label lbl = new Label();
            lbl.AutoSize = true;
            lbl.Location = new Point(15, y + 3);
            lbl.Text = nhan;
            this.Controls.Add(lbl);

            TextBox txt = new TextBox();
            txt.Location = new Point(155, y);
            txt.Size = new Size(180, 20);
            txt.PasswordChar = '*';
            this.Controls.Add(txt);
            return txt;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            NguoiDung nd = PhienDangNhap.NguoiDungHienTai;
            if (nd == null)
            {
                MessageBox.Show("Bạn chưa đăng nhập!", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            string ketQua = new NguoiDungService().DoiMatKhau(nd.ID, txtMatKhauCu.Text, txtMatKhauMoi.Text, txtXacNhan.Text);
            if (ketQua == "OK")
            {
                MessageBox.Show("Đổi mật khẩu thành công!", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(ketQua, "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhauCu.Focus();
            }
        }
    }
}
