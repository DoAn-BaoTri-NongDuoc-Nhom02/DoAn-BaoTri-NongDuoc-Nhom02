using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc.BusinessObject
{
    // REFACTOR #21 Change Reference to Value – đơn vị tính (Chai, Gói, Kg...) không có "danh tính" riêng,
    // hai đối tượng cùng mã + tên là một. Bỏ setter (bất biến) và so sánh theo nội dung (Equals/GetHashCode),
    // nên có thể dùng chung một đối tượng cho nhiều sản phẩm (xem Flyweight trong DonViTinhController).
    public sealed class DonViTinh
    {
        private readonly int m_ID;
        private readonly String m_Ten;

        public DonViTinh(int id, String ten)
        {
            m_ID = id;
            m_Ten = ten ?? "";
        }

        public int Id
        {
            get { return m_ID; }
        }

        public String Ten
        {
            get { return m_Ten; }
        }

        public override bool Equals(object obj)
        {
            DonViTinh khac = obj as DonViTinh;
            return khac != null && khac.m_ID == m_ID && khac.m_Ten == m_Ten;
        }

        public override int GetHashCode()
        {
            return m_ID.GetHashCode() ^ m_Ten.GetHashCode();
        }

        public override String ToString()
        {
            return m_Ten;
        }
    }
}
