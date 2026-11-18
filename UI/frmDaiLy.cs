using System;

namespace CuahangNongduoc
{
    /// <summary>
    /// Form Quản lý Đại lý. Toàn bộ giao diện và xử lý nằm ở lớp cha frmDoiTacCoSo
    /// (REFACTOR #55 Extract Superclass + #56 Pull Up Field / #57 Pull Up Method / #59 Pull Up Constructor Body – xem frmDoiTacCoSo.cs).
    /// </summary>
    public partial class frmDaiLy : frmDoiTacCoSo
    {
        public frmDaiLy()
            : base(true, "Quản lý Đại lý", "đại lý", "CAP_DAI_LY")
        {
            InitializeComponent();
        }

        /// <summary>Mã đại lý là số thứ tự (1, 2, 3...). REFACTOR #61 Push Down Method – chuyển từ frmDoiTacCoSo xuống.</summary>
        protected override string TaoMaMoi(long maso)
        {
            return maso.ToString();
        }
    }
}
