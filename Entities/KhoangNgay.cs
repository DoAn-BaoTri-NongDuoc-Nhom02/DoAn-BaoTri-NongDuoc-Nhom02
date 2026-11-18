using System;
using System.Collections;
using System.Collections.Generic;

namespace CuahangNongduoc.BusinessObject
{
    /// <summary>
    /// Khoảng ngày "từ ngày ... đến ngày ..." (tính theo ngày, đã bỏ giờ), dùng cho mọi thống kê.
    /// REFACTOR #49 Introduce Parameter Object – thay cho cặp tham số (DateTime tu, DateTime den)
    /// lặp lại ở nhiều hàm, kèm luôn luật "từ ≤ đến" và phép "den + 1 ngày" mà nơi nào cũng tự viết.
    /// </summary>
    public sealed class KhoangNgay : IEnumerable<DateTime>
    {
        private readonly DateTime m_TuNgay;
        private readonly DateTime m_DenNgay;

        public KhoangNgay(DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay.Date > denNgay.Date)
                throw new ArgumentException("Từ ngày phải ≤ Đến ngày.");
            m_TuNgay = tuNgay.Date;
            m_DenNgay = denNgay.Date;
        }

        public DateTime TuNgay { get { return m_TuNgay; } }
        public DateTime DenNgay { get { return m_DenNgay; } }

        /// <summary>Mốc kết thúc không bao gồm (đầu ngày hôm sau) – dùng cho điều kiện "NGAY &lt; @den".</summary>
        public DateTime DenNgayKhongBaoGom { get { return m_DenNgay.AddDays(1); } }

        public bool Chua(DateTime ngay)
        {
            DateTime d = ngay.Date;
            return d >= m_TuNgay && d <= m_DenNgay;
        }

        public override String ToString()
        {
            return m_TuNgay.ToString("dd/MM/yyyy") + " - " + m_DenNgay.ToString("dd/MM/yyyy");
        }

        public int SoNgay
        {
            get { return (m_DenNgay - m_TuNgay).Days + 1; }
        }

        // PATTERN #16 Iterator – duyệt lần lượt từng ngày trong khoảng mà bên ngoài không cần biết cách
        // tính ngày kế tiếp (qua tháng, năm nhuận...). Dùng được trực tiếp trong foreach.
        public IEnumerator<DateTime> GetEnumerator()
        {
            return new DuyetNgay(m_TuNgay, m_DenNgay);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Iterator cụ thể: giữ vị trí hiện tại và tự tiến từng ngày.</summary>
        private sealed class DuyetNgay : IEnumerator<DateTime>
        {
            private readonly DateTime m_Dau;
            private readonly DateTime m_Cuoi;
            private DateTime m_HienTai;
            private bool m_DaBatDau;

            public DuyetNgay(DateTime dau, DateTime cuoi)
            {
                m_Dau = dau;
                m_Cuoi = cuoi;
            }

            public DateTime Current
            {
                get
                {
                    if (!m_DaBatDau) throw new InvalidOperationException("Chưa gọi MoveNext().");
                    return m_HienTai;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (!m_DaBatDau)
                {
                    m_DaBatDau = true;
                    m_HienTai = m_Dau;
                    return m_Dau <= m_Cuoi;
                }
                if (m_HienTai >= m_Cuoi) return false;
                m_HienTai = m_HienTai.AddDays(1);
                return true;
            }

            public void Reset()
            {
                m_DaBatDau = false;
            }

            public void Dispose()
            {
            }
        }
    }
}
