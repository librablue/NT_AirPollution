using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class ChineseMoneyConverter
    {
        private static readonly string[] ChineseDigits = { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖" };
        private static readonly string[] Units = { "", "拾", "佰", "仟" };
        private static readonly string[] BigUnits = { "", "萬", "億", "兆" };

        public string ToChineseUpper(double amount)
        {
            if (amount == 0) return "零元整";

            StringBuilder result = new StringBuilder();
            long integerPart = (long)Math.Floor(amount);
            int fractionPart = (int)((amount - integerPart) * 100);

            // 處理整數部分
            string integerStr = ConvertIntegerPart(integerPart);
            result.Append(integerStr + "元");

            // 處理小數部分
            if (fractionPart == 0)
            {
                result.Append("整");
            }
            else
            {
                int jiao = fractionPart / 10;
                int fen = fractionPart % 10;
                if (jiao > 0)
                    result.Append(ChineseDigits[jiao] + "角");
                if (fen > 0)
                    result.Append(ChineseDigits[fen] + "分");
            }

            return result.ToString();
        }

        private string ConvertIntegerPart(long number)
        {
            if (number == 0) return "零";

            StringBuilder sb = new StringBuilder();
            int bigUnitIndex = 0;
            bool needZero = false; // 是否需要補零

            while (number > 0)
            {
                int section = (int)(number % 10000);
                if (section != 0)
                {
                    string sectionChinese = ConvertSection(section);
                    if (needZero)
                    {
                        sb.Insert(0, "零");
                        needZero = false;
                    }

                    if (bigUnitIndex > 0)
                        sectionChinese += BigUnits[bigUnitIndex];

                    sb.Insert(0, sectionChinese);
                }
                else
                {
                    if (sb.Length > 0)
                        needZero = true; // 下一段若有值就補零
                }

                number /= 10000;
                bigUnitIndex++;
            }

            return sb.ToString().TrimEnd('零');
        }

        private string ConvertSection(int section)
        {
            StringBuilder sb = new StringBuilder();
            bool zeroFlag = false;
            bool hasValue = false; // 段中是否已有非零數字

            for (int i = 0; i < 4; i++)
            {
                int digit = section % 10;
                if (digit == 0)
                {
                    if (hasValue)
                    {
                        zeroFlag = true; // 只有在段中已有值時才考慮補零
                    }
                }
                else
                {
                    if (zeroFlag)
                    {
                        sb.Insert(0, "零");
                        zeroFlag = false;
                    }
                    sb.Insert(0, ChineseDigits[digit] + Units[i]);
                    hasValue = true;
                }
                section /= 10;
            }

            return sb.ToString();
        }

    }
}
