using System;

namespace CuahangNongduoc
{
    /// <summary>
    /// Form Khách hàng. Toàn bộ giao diện và xử lý nằm ở lớp cha frmDoiTacCoSo
    /// (REFACTOR #55 Extract Superclass + #56 Pull Up Field / #57 Pull Up Method / #59 Pull Up Constructor Body – xem frmDoiTacCoSo.cs).
    /// </summary>
    public partial class frmKhachHang : frmDoiTacCoSo
    {
        public frmKhachHang()
            : base(false, "Khách hàng", "khách hàng", "NHOM_KH")
        {
            InitializeComponent();
        }

        /// <summary>Mã khách lẻ có tiền tố "KH" (KH1, KH2...).</summary>
        protected override string TaoMaMoi(long maso)
        {
            return "KH" + maso.ToString();
        }
    }
}
