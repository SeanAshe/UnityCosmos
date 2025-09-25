using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.System
{
    public static class PinyinConverter
    {
        private static readonly Dictionary<char, string> VowelToneMap = new()
        {
            { 'a', "āáǎàa" },
            { 'e', "ēéěèe" },
            { 'i', "īíǐìi" },
            { 'o', "ōóǒòo" },
            { 'u', "ūúǔùu" },
            { 'ü', "ǖǘǚǜü" }
        };

        public static string ToFormatPinyin(this string pinyinWithNumber)
        {
            if (string.IsNullOrEmpty(pinyinWithNumber)) return "";
            pinyinWithNumber = pinyinWithNumber.ToLower().Trim();

            // 1. 分离音节和声调数字
            char lastChar = pinyinWithNumber.Last();
            if (!char.IsDigit(lastChar))
            {
                // 如果最后一位不是数字，认为是轻声或格式错误，直接返回（处理v到ü的转换）
                return pinyinWithNumber.Replace('v', 'ü');
            }

            string syllable = pinyinWithNumber[..^1];
            int tone = int.Parse(lastChar.ToString());

            // 声调数字必须在1-4之间
            if (tone < 1 || tone > 5)
            {
                return pinyinWithNumber.Replace('v', 'ü'); ; // 无效声调，返回原样
            }

            // 2. 处理特殊元音 'v' -> 'ü'
            syllable = syllable.Replace('v', 'ü');

            // 3. 确定声调应该加在哪个元音上
            // 规则：a > e > o > 其他 (i, u, ü)。有a加a，没a有e加e，没a,e有o加o。
            // 对于iu, ui等组合，声调加在后面的元音上。
            int vowelIndex = -1;

            if (syllable.Contains('a'))
            {
                vowelIndex = syllable.IndexOf('a');
            }
            else if (syllable.Contains('e'))
            {
                vowelIndex = syllable.IndexOf('e');
            }
            else if (syllable.Contains("o"))
            {
                vowelIndex = syllable.IndexOf('o');
            }
            else if (syllable.Contains("u"))
            {
                vowelIndex = syllable.IndexOf('u');
            }
            else
            {
                // 剩下的情况，声调加在最后一个元音上
                for (int i = syllable.Length - 1; i >= 0; i--)
                {
                    if ("iouü".Contains(syllable[i]))
                    {
                        vowelIndex = i;
                        break;
                    }
                }
            }

            // 如果没有找到元音（不可能的拼音），则直接返回
            if (vowelIndex == -1)
            {
                return syllable;
            }

            // 4. 替换元音为带声调的元音
            char originalVowel = syllable[vowelIndex];

            if (VowelToneMap.TryGetValue(originalVowel, out string tones))
            {
                char tonedVowel = tones[tone - 1]; // tone是1-5，索引是0-3

                // 使用StringBuilder进行高效的字符替换
                StringBuilder sb = new(syllable);
                sb[vowelIndex] = tonedVowel;
                return sb.ToString();
            }

            return syllable; // 如果找不到对应的元音（理论上不会发生）
        }
    }
}
