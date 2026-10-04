using CuahangNongduoc.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc.UI
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {

        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHienMatKhau.Checked)
                txtMatKhau.PasswordChar = '\0';
            else
                txtMatKhau.PasswordChar = '*';
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn thoát không?",
                                          "Xác nhận",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;

            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Gọi hàm đăng nhập
            BLL.Service.NguoiDungService service = new BLL.Service.NguoiDungService();
            NguoiDung nd = service.DangNhap(tenDangNhap, matKhau);

            if (nd == null)
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!",
                                "Đăng nhập thất bại",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                txtMatKhau.Clear();
                txtMatKhau.Focus();
            }
            else
            {
                MessageBox.Show("Đăng nhập thành công!\nXin chào: " + nd.HoTen,
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                // Mở form chính
                frmMain frm = new frmMain(nd);
                this.Hide();
                frm.ShowDialog();
                this.Close();
            }
        }
    }
}
