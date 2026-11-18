using System;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    /// <summary>
    /// Ô nhập số tiền (đồng): không số lẻ, có dấu phân cách hàng nghìn, tối đa 999.999.999.
    /// REFACTOR #17 Introduce Local Extension – NumericUpDown là lớp của .NET không sửa được;
    /// trước đây chỗ nào cần nhập tiền cũng phải chỉnh lại Maximum / ThousandsSeparator.
    /// </summary>
    public class TienUpDown : NumericUpDown
    {
        public const long TOI_DA = 999999999;

        public TienUpDown()
        {
            Minimum = 0;
            Maximum = TOI_DA;
            DecimalPlaces = 0;
            ThousandsSeparator = true;
        }

        public TienUpDown(long soTien)
            : this()
        {
            SoTien = soTien;
        }

        /// <summary>Số tiền dạng long (thay cho (long)Value).</summary>
        public long SoTien
        {
            get { return (long)Value; }
            set { Value = Math.Max(0, Math.Min(TOI_DA, value)); }
        }
    }
}
