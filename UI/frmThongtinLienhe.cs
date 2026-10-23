using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public partial class frmThongtinLienhe : Form
    {
        public frmThongtinLienhe()
        {
            InitializeComponent();
        }

        private void frmThongtinLienhe_Load(object sender, EventArgs e)
        {
            AnStatusStrip();

            this.Text = "Thông tin liên hệ";
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Tuỳ chọn: bấm label email / web để mở
        private void lnkEmail1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MoEmail("lanhue@email.com");
        }

        private void lnkEmail2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MoEmail("tinhoc@agu.edu.vn");
        }

        private void MoEmail(string email)
        {
            try
            {
                Process.Start("mailto:" + email);
            }
            catch
            {
                MessageBox.Show("Không mở được ứng dụng email.\nĐịa chỉ: " + email,
                    "Thông tin liên hệ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void frmThongtinLienhe_Load_1(object sender, EventArgs e)
        {

        }
        private void AnStatusStrip()
        {
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (this.Controls[i] is StatusStrip)
                    this.Controls.RemoveAt(i);
            }
        }

    }
}