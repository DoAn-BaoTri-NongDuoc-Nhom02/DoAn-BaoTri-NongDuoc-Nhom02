using System;
using System.Collections.Generic;
using System.Text;

namespace CuahangNongduoc
{
    public class Num2Str
    {
        public Num2Str()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        // Hàm này có thực hiện cắt các số 0
        // ví dụ 005 sẽ đọc là không trăm linh năm
        private string Group32StrX(string num)
        {
            string[] No = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string kq, tram, chuc, donvi;
            // Trăm
            if (num.Substring(0, 1) == "0")
            { tram = ""; }
            else
            { tram = No[Convert.ToByte(num.Substring(0, 1))] + " trăm "; }
            // Chục
            switch (num.Substring(1, 1))
            {
                case "0": if (num.Substring(2, 1) != "0" && num.Substring(0, 1) != "0")
                    {
                        chuc = "linh ";
                    }
                    else { chuc = ""; }; break;
                case "1": chuc = "mười "; break;
                default:
                    chuc = No[Convert.ToByte(num.Substring(1, 1))] + " mươi "; break;
            }
            // Đơn vị
            switch (num.Substring(2, 1))
            {
                case "0": donvi = ""; break;
                case "1": if ((num.Substring(1, 1) == "0") || (num.Substring(1, 1) == "1"))
                    {
                        donvi = "một";
                    }
                    else
                    {
                        donvi = "mốt";
                    }; break;
                case "5":
                    if (num.Substring(1, 1) != "0")
                    {
                        donvi = "lăm";
                    }
                    else
                    {
                        donvi = "năm";
                    }; break;
                default:
                    donvi = No[Convert.ToByte(num.Substring(2, 1))]; break;
            }
            kq = tram + chuc + donvi;
            return kq;
        }

        private string IntNum2Str(string num)
        {
            string[] Cap = { "", " nghìn ", " triệu ", " tỷ ", " nghìn tỷ ", " triệu tỷ ", " tỷ tỷ ", " nghìn tỷ tỷ " };
            string kq = "", str = num, g3, kqtg;
            int caps = 0;
            while (str.Length > 3)
            {
                g3 = str.Substring(str.Length - 3, 3);
                str = str.Substring(0, str.Length - 3);
                if (g3 != "000")
                { kqtg = Group32StrX(g3) + Cap[Convert.ToByte(caps)]; }
                else { kqtg = ""; }
                kq = kqtg + kq;
                caps++;
            }
            //Chuẩn bị trước khi sử dụng hàm Group32Str1
            while (str.Length < 3)
            { str = "0" + str; }

            if ((str == "000") && (num.Length <= 3))
            { kqtg = "không"; }
            else
            { kqtg = Group32StrX(str) + Cap[Convert.ToByte(caps)]; }
            kq = kqtg + kq;
            return kq;
        }

        private string FracNum2Str(string num)
        {
            string[] No = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string kq = "";
            for (int i = 0; i < num.Length; i++)
            {
                kq += No[Convert.ToByte(num.Substring(i, 1))] + " ";
            }
            return kq;
        }

        private string No2Str(string num)
        {
            string intstr, fracstr, am;
            // Xử lý khi là số âm
            if (num.Substring(0, 1) == "-")
            {
                am = "âm ";
                num = num.Replace("-", "");
            }
            else { am = ""; }

            string[] str = num.Split('.');
            // Số quá lớn nhiều hơn 27 chữ số
            if (str[0].Length > 28)
                throw new Exception("Số lớn quá không hiển thị được! ");

            // REFACTOR #6 Split Temporary Variable – trước đây một biến "s" lần lượt giữ 3 thứ khác nhau
            // (phần chữ của số, chuỗi có dấu "âm", phần còn lại sau chữ hoa đầu). Nay mỗi thứ một biến.
            string phanChu;
            // Xử lý phần số, nếu là có số thập phân hoặc không
            if (str.Length == 2)
            {
                intstr = IntNum2Str(str[0]);
                if (str[1].Length <= 2)
                {
                    if (str[1].Substring(0, 1) == "0")
                    {
                        fracstr = FracNum2Str(str[1]);
                    }
                    else
                    {
                        fracstr = IntNum2Str(str[1]);
                    }
                }
                else
                {
                    fracstr = FracNum2Str(str[1]);
                }
                phanChu = (intstr + " phảy " + fracstr);
            }
            else
            {
                intstr = IntNum2Str(str[0]);
                phanChu = intstr;
            }
            string coDau = am + phanChu;
            string chuHoaDau = coDau.Substring(0, 1).ToUpper();
            string phanConLai = coDau.Substring(1, coDau.Length - 1);
            return (chuHoaDau + phanConLai);
        }

        /// <summary>
        /// Chèn dấu phẩy hàng nghìn: "1234567.89" -> "1,234,567.89".
        /// REFACTOR #9 Substitute Algorithm – thay vòng lặp tự cắt từng nhóm 3 chữ số (và vá dấu "-,")
        /// bằng định dạng "#,##0" có sẵn của .NET; phần thập phân giữ nguyên.
        /// </summary>
        public string NumNormalize(string num)
        {
            string[] phan = num.Split('.');
            bool am = phan[0].StartsWith("-");
            string phanNguyen = am ? phan[0].Substring(1) : phan[0];
            decimal giaTri = decimal.Parse(phanNguyen.Length == 0 ? "0" : phanNguyen,
                System.Globalization.CultureInfo.InvariantCulture);
            string ketQua = (am ? "-" : "") + giaTri.ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture);
            if (phan.Length == 2)
                ketQua += "." + phan[1];
            return ketQua;
        }

        private static bool LaSoKhong(string chuoiSo)
        {
            return chuoiSo == "0" || chuoiSo == "";
        }

        public string NumberToString(string no)
        {
            // REFACTOR #7 Remove Assignments to Parameters – không gán lại tham số "no" (trước đây bị gán 6 lần),
            // mọi bước chuẩn hóa làm trên biến cục bộ chuoiSo.
            string chuoiSo = no;
            // xử lý trường hợp dáu phảy thay cho dấu chấm
            if (chuoiSo.IndexOf(",", 0, 1) != 0) { chuoiSo = chuoiSo.Replace(",", "."); }
            // Xoá các ký tự trắng ở đầu và cuối
            chuoiSo = chuoiSo.Trim();
            // REFACTOR #34 Consolidate Conditional Expression – hai if cùng trả "không" gộp thành một điều kiện.
            if (LaSoKhong(chuoiSo)) return "không";
            double val;
            try
            {
                val = Convert.ToDouble(chuoiSo);
            }
            catch
            {
                throw new Exception("Đây không phải là số");
            }
            // tiêu diệt các số không
            while (chuoiSo.Substring(0, 1) == "0")
            {
                chuoiSo = chuoiSo.Substring(1, chuoiSo.Length - 1);
            }

            if (chuoiSo.IndexOf(".", 0, 1) != -1)
            {
                while (chuoiSo.Substring(chuoiSo.Length - 1, 1) == "0")
                {
                    chuoiSo = chuoiSo.Substring(0, chuoiSo.Length - 1);
                }
            }
            return No2Str(chuoiSo);
        }
    }
}
