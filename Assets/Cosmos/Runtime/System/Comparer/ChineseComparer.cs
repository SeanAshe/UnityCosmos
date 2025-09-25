using TinyPinyin;
using System.Collections.Generic;
using System.Globalization;

namespace Cosmos.System
{
    public class ChineseComparer : IComparer<string>
    {
        private static readonly CultureInfo cultureInfo = new ("zh-CN");
        public int Compare(string x, string y)
        {
            string pinyinX = GetPinYin(x);
            string pinyinY = GetPinYin(y);
            return string.Compare(pinyinX, pinyinY, cultureInfo, CompareOptions.IgnoreCase);
        }
        private string GetPinYin(string input)
        {
            var firstChar = input[0];
            if (PinyinHelper.IsChinese(firstChar))
            {
                return PinyinHelper.GetPinyin(firstChar);
            }
            else
            {
                return input;
            }
        }
    }
}
