namespace CuahangNongduoc
{
    partial class frmInDunoKhachHang
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(240, 60);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmInDunoKhachHang";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "In dư nợ khách hàng";
            this.Load += new System.EventHandler(this.frmInDunoKhachHang_Load);
            this.ResumeLayout(false);
        }
    }
}
