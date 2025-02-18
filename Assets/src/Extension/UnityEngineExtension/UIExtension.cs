using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Cosmos.system;

namespace Cosmos.unity
{
    public static class UIExtension
    {
        /// <summary>
        /// 将X16 string转换为Color
        /// </summary>
        public static Color ToColor(this string text)
        {
            Assert.IsNotNull(text, "A NULL text can't be convert to color");
            Assert.IsTrue(text.Length == 6, "The text can't be convert to color: " + text);
            int r = (text[0].HexToDecimal() << 4) | text[1].HexToDecimal();
            int g = (text[2].HexToDecimal() << 4) | text[3].HexToDecimal();
            int b = (text[4].HexToDecimal() << 4) | text[5].HexToDecimal();

            return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
        }

        public static void ScrollToTop(this RectTransform rectTransform)
        {
            if (rectTransform != null) rectTransform.anchoredPosition = Vector2.zero;
        }

        public static void ScrollToTop(this GridLayoutGroup gridLayoutGroup)
        {
            if (gridLayoutGroup != null) ScrollToTop(gridLayoutGroup.GetComponent<RectTransform>());
        }

        public static void ScrollToTop(this VerticalLayoutGroup verticalLayoutGroup)
        {
            if (verticalLayoutGroup != null) ScrollToTop(verticalLayoutGroup.GetComponent<RectTransform>());
        }
    }
}
