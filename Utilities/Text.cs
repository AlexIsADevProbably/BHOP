using UnityEngine;

namespace BHOP.Utilities;

public static class TextUtility
{
    /// <summary>
    /// Generates a gradient string using UnityEngine.Color.
    /// </summary>
    public static string ApplyGradient(string text, Color startColor, Color endColor)
    {
        if (string.IsNullOrEmpty(text)) return text;
        
        string result = "";
        int len = text.Length;
        
        for (int i = 0; i < len; i++)
        {
            if (text[i] == ' ') 
            {
                result += " ";
                continue;
            }

            float t = len > 1 ? (float)i / (len - 1) : 0f;
            Color lerpedColor = Color.Lerp(startColor, endColor, t);

            string hex = ColorUtility.ToHtmlStringRGB(lerpedColor);
            result += $"<color=#{hex}>{text[i]}</color>";
        }
        
        return result;
    }

    /// <summary>
    /// Generates a gradient string using Hex color codes.
    /// </summary>
    public static string ApplyGradient(string text, string hexStart, string hexEnd)
    {
        ColorUtility.TryParseHtmlString(hexStart.StartsWith("#") ? hexStart : "#" + hexStart, out Color startColor);
        ColorUtility.TryParseHtmlString(hexEnd.StartsWith("#") ? hexEnd : "#" + hexEnd, out Color endColor);
        
        return ApplyGradient(text, startColor, endColor);
    }
}