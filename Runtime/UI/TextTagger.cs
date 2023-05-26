using UnityEngine;

namespace Crysc.UI
{
    public static class TextTagger
    {
        public static string AddColorTag(string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }
    }
}
