using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class BotHelper
    {
        public static string GetPayNo(string transno, string amt, string e_date, string customercd = "4750")
        {
            string y = DateTime.Today.Year.ToString().Substring(3);
            string checkcd1 = "0";
            string checkcd2 = "0";
            DateTime dt0 = DateTime.Today.AddDays(30.0);
            if (!string.IsNullOrEmpty(e_date))
            {
                string d1 = (1911 + int.Parse(e_date.Substring(0, 3))).ToString();
                string d2 = e_date.Substring(3, 2);
                string d3 = e_date.Substring(5, 2);
                dt0 = Convert.ToDateTime(d1 + "-" + d2 + "-" + d3);
                y = dt0.Year.ToString().Substring(3);
            }
            int year = dt0.Year;
            DateTime dt1 = new DateTime(year, 1, 1);
            string ddd = ((dt0.Date - dt1.Date).Days + 1).ToString().PadLeft(3, '0');
            transno = transno.PadLeft(6, '0');
            amt = amt.PadLeft(10, '0');
            string result = customercd + y + ddd + transno + amt;
            char[] charArray0 = result.ToCharArray();
            char[] charArray1 = "121212121212121212121212".ToCharArray();
            int[] sum1 = new int[24];
            for (int j = 0; j < charArray0.Length; j++)
            {
                int n = (short)(char.GetNumericValue(charArray0[j]) * (double)(short)char.GetNumericValue(charArray1[j]));
                if (n >= 10)
                {
                    int unitPlace = n / 1 % 10;
                    int tenPlace = n / 10 % 10;
                    sum1[j] = tenPlace + unitPlace;
                }
                else
                {
                    sum1[j] = n;
                }
            }
            int chk = 0;
            int[] array = sum1;
            foreach (int m in array)
            {
                chk += m;
            }
            checkcd1 = (chk / 1 % 10).ToString();
            char[] charArray2 = "137137137137137137137137".ToCharArray();
            int[] sum2 = new int[24];
            for (int i = 0; i < charArray0.Length; i++)
            {
                int l = (short)(char.GetNumericValue(charArray0[i]) * (double)(short)char.GetNumericValue(charArray2[i]));
                sum2[i] = l;
            }
            chk = 0;
            int[] array2 = sum2;
            foreach (int k in array2)
            {
                chk += k;
            }
            checkcd2 = (chk / 1 % 10).ToString();
            return customercd + checkcd1 + checkcd2 + y + ddd + transno;
        }

        public static string GetMarketNo(string yymmdd, string item = "63D")
        {
            return yymmdd.Substring(1) + item;
        }

        public static string GetMarketAmt(string uno, string amt, string payno, string yymmdd, string item = "63D")
        {
            string checkcd1 = "0";
            string checkcd2 = "0";
            string bar1 = yymmdd.Substring(1) + item.Replace("D", "4");
            string bar2 = uno + amt.PadLeft(9, '0');
            int chk = 0;
            char[] charArray = bar1.ToCharArray();
            chk = (short)(char.GetNumericValue(charArray[0]) + (double)(short)char.GetNumericValue(charArray[2]) + (double)(short)char.GetNumericValue(charArray[4]) + (double)(short)char.GetNumericValue(charArray[6]) + (double)(short)char.GetNumericValue(charArray[8]));
            charArray = payno.ToCharArray();
            chk += (short)(char.GetNumericValue(charArray[0]) + (double)(short)char.GetNumericValue(charArray[2]) + (double)(short)char.GetNumericValue(charArray[4]) + (double)(short)char.GetNumericValue(charArray[6]) + (double)(short)char.GetNumericValue(charArray[8]) + (double)(short)char.GetNumericValue(charArray[10]) + (double)(short)char.GetNumericValue(charArray[12]) + (double)(short)char.GetNumericValue(charArray[14]));
            charArray = bar2.ToCharArray();
            chk += (short)(char.GetNumericValue(charArray[0]) + (double)(short)char.GetNumericValue(charArray[2]) + (double)(short)char.GetNumericValue(charArray[4]) + (double)(short)char.GetNumericValue(charArray[6]) + (double)(short)char.GetNumericValue(charArray[8]) + (double)(short)char.GetNumericValue(charArray[10]) + (double)(short)char.GetNumericValue(charArray[12]));
            checkcd1 = (chk % 11).ToString();
            if (checkcd1 == "0")
            {
                checkcd1 = "A";
            }
            else if (checkcd1 == "10")
            {
                checkcd1 = "B";
            }
            chk = 0;
            charArray = bar1.ToCharArray();
            chk = (short)(char.GetNumericValue(charArray[1]) + (double)(short)char.GetNumericValue(charArray[3]) + (double)(short)char.GetNumericValue(charArray[5]) + (double)(short)char.GetNumericValue(charArray[7]));
            charArray = payno.ToCharArray();
            chk += (short)(char.GetNumericValue(charArray[1]) + (double)(short)char.GetNumericValue(charArray[3]) + (double)(short)char.GetNumericValue(charArray[5]) + (double)(short)char.GetNumericValue(charArray[7]) + (double)(short)char.GetNumericValue(charArray[9]) + (double)(short)char.GetNumericValue(charArray[11]) + (double)(short)char.GetNumericValue(charArray[13]) + (double)(short)char.GetNumericValue(charArray[15]));
            charArray = bar2.ToCharArray();
            chk += (short)(char.GetNumericValue(charArray[1]) + (double)(short)char.GetNumericValue(charArray[3]) + (double)(short)char.GetNumericValue(charArray[5]) + (double)(short)char.GetNumericValue(charArray[7]) + (double)(short)char.GetNumericValue(charArray[9]) + (double)(short)char.GetNumericValue(charArray[11]));
            checkcd2 = (chk % 11).ToString();
            if (checkcd2 == "0")
            {
                checkcd2 = "X";
            }
            else if (checkcd2 == "10")
            {
                checkcd2 = "Y";
            }
            return uno + checkcd1 + checkcd2 + amt.PadLeft(9, '0');
        }

        public static string GetPostNo(string transno, string amt, string e_date, string customercd = "4750")
        {
            string y = DateTime.Today.Year.ToString().Substring(3);
            string checkcd1 = "0";
            string checkcd2 = "0";
            DateTime dt0 = DateTime.Today.AddDays(30.0);
            if (!string.IsNullOrEmpty(e_date))
            {
                string d1 = (1911 + int.Parse(e_date.Substring(0, 3))).ToString();
                string d2 = e_date.Substring(3, 2);
                string d3 = e_date.Substring(5, 2);
                dt0 = Convert.ToDateTime(d1 + "-" + d2 + "-" + d3);
                y = dt0.Year.ToString().Substring(3);
            }
            int year = dt0.Year;
            DateTime dt1 = new DateTime(year, 1, 1);
            string ddd = ((dt0.Date - dt1.Date).Days + 1).ToString().PadLeft(3, '0');
            transno = transno.PadLeft(6, '0');
            int k = int.Parse(amt);
            amt = ((k <= 95) ? (k + 5) : ((k > 990) ? (k + 15) : (k + 10))).ToString().PadLeft(10, '0');
            string result = customercd + y + ddd + transno + amt;
            char[] charArray0 = result.ToCharArray();
            char[] charArray1 = "121212121212121212121212".ToCharArray();
            int[] sum1 = new int[24];
            for (int j = 0; j < charArray0.Length; j++)
            {
                int n2 = (short)(char.GetNumericValue(charArray0[j]) * (double)(short)char.GetNumericValue(charArray1[j]));
                if (n2 >= 10)
                {
                    int unitPlace = n2 / 1 % 10;
                    int tenPlace = n2 / 10 % 10;
                    sum1[j] = tenPlace + unitPlace;
                }
                else
                {
                    sum1[j] = n2;
                }
            }
            int chk = 0;
            int[] array = sum1;
            foreach (int n in array)
            {
                chk += n;
            }
            checkcd1 = (chk / 1 % 10).ToString();
            char[] charArray2 = "137137137137137137137137".ToCharArray();
            int[] sum2 = new int[24];
            for (int i = 0; i < charArray0.Length; i++)
            {
                int m = (short)(char.GetNumericValue(charArray0[i]) * (double)(short)char.GetNumericValue(charArray2[i]));
                sum2[i] = m;
            }
            chk = 0;
            int[] array2 = sum2;
            foreach (int l in array2)
            {
                chk += l;
            }
            checkcd2 = (chk / 1 % 10).ToString();
            chk = int.Parse(checkcd1 + checkcd2) + 101;
            return customercd + chk.ToString().Substring(1) + y + ddd + transno;
        }

        public static string GetPostAmt(string amt)
        {
            int i = int.Parse(amt);
            amt = ((i <= 95) ? (i + 5) : ((i > 990) ? (i + 15) : (i + 10))).ToString();
            return amt.PadLeft(6, '0');
        }

        public static int GetPostFee(string amt)
        {
            if (int.Parse(amt) <= 95)
            {
                return 5;
            }
            if (int.Parse(amt) <= 990)
            {
                return 10;
            }
            return 15;
        }
    }
}
