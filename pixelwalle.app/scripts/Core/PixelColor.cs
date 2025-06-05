using System.Globalization;
using System;
namespace PixelWallE.Core;

public readonly struct PixelColor
{
    public byte Red { get; }
    public byte Green { get; }
    public byte Blue { get; }
    public byte Alpha { get; }

    public PixelColor(byte red, byte green, byte blue, byte alpha = 255)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public static bool TryParse(string colorString, out PixelColor color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(colorString)) return false;

        var normalizedString = colorString.Trim().ToLowerInvariant();

        // Nombres predefinidos
        switch (normalizedString)
        {
            case "red":
                color = new PixelColor(255, 0, 0);
                return true;
            case "green":
                color = new PixelColor(0, 255, 0);
                return true;
            case "blue":
                color = new PixelColor(0, 0, 255);
                return true;
            case "yellow":
                color = new PixelColor(255, 255, 0);
                return true;
            case "orange":
                color = new PixelColor(255, 165, 0);
                return true;
            case "purple":
                color = new PixelColor(128, 0, 128);
                return true;
            case "black":
                color = new PixelColor(0, 0, 0);
                return true;
            case "white":
                color = new PixelColor(255, 255, 255);
                return true;
            case "transparent":
                color = new PixelColor(0, 0, 0, 0);
                return true;
        }

        // Formatos Hexadecimales
        if (normalizedString.StartsWith("#"))
        {
            var hex = normalizedString.Substring(1);
            if (hex.Length == 8) // #AARRGGBB
                return TryParseHexPacked(hex, out color);
            if (hex.Length == 4) // #ARGB -> #AARRGGBB
            {
                var expandedHex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";
                return TryParseHexPacked(expandedHex, out color);
            }

            if (hex.Length == 6) // #RRGGBB
            {
                var expandedHex = $"ff{hex}";
                return TryParseHexPacked(expandedHex, out color);
            }

            if (hex.Length == 3) // #RGB -> #RRGGBB
            {
                var expandedHex = $"ff{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
                return TryParseHexPacked(expandedHex, out color);
            }
        }

        return false;
    }

    private static bool TryParseHexPacked(string hexString, out PixelColor color)
    {
        color = default;
        if (uint.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var packedValue))
        {
            byte a = 255, r, g, b; // Alfa por defecto opaco

            a = (byte)((packedValue >> 24) & 0xFF);
            r = (byte)((packedValue >> 16) & 0xFF);
            g = (byte)((packedValue >> 8) & 0xFF);
            b = (byte)(packedValue & 0xFF);

            color = new PixelColor(r, g, b, a);
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        if (Alpha == 255)
            return $"#{Red:X2}{Green:X2}{Blue:X2}";
        return $"#{Alpha:X2}{Red:X2}{Green:X2}{Blue:X2}";
    }

    public override bool Equals(object? obj)
    {
        return obj is PixelColor other &&
               Red == other.Red &&
               Green == other.Green &&
               Blue == other.Blue &&
               Alpha == other.Alpha;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Red, Green, Blue, Alpha);
    }

    public static bool operator ==(PixelColor left, PixelColor right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PixelColor left, PixelColor right)
    {
        return !(left == right);
    }
    
    public static PixelColor operator +(PixelColor left, PixelColor right)
    {
        return Mix(left, right);
    }
    private static PixelColor Mix(PixelColor left, PixelColor right)
    {
        byte a = (byte)((left.Alpha + right.Alpha) / 2);
        byte r = (byte)((left.Red + right.Red) / 2);
        byte g = (byte)((left.Green + right.Alpha) / 2);
        byte b = (byte)((left.Blue + right.Alpha) / 2);
        return new PixelColor(r, g, b, a);
    }
}