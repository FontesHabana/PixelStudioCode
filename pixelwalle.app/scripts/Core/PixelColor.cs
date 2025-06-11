using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
namespace PixelWallE.Core;
 /// <summary>
    /// Represents a color with red, green, blue, and alpha components.
    /// </summary>
    public readonly struct PixelColor
    {
        /// <summary>
        /// Gets the red component of the color.
        /// </summary>
        public byte Red { get; }
        /// <summary>
        /// Gets the green component of the color.
        /// </summary>
        public byte Green { get; }
        /// <summary>
        /// Gets the blue component of the color.
        /// </summary>
        public byte Blue { get; }
        /// <summary>
        /// Gets the alpha component of the color.
        /// </summary>
        public byte Alpha { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelColor"/> struct.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The alpha component of the color. Default is 255.</param>
        public PixelColor(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// Tries to parse a string representation of a color and converts it to a <see cref="PixelColor"/>.
        /// </summary>
        /// <param name="colorString">The string to parse.</param>
        /// <param name="color">When this method returns, contains the parsed <see cref="PixelColor"/>, or default if the parsing fails.</param>
        /// <returns>True if the parsing was successful; otherwise, false.</returns>
        public static bool TryParse(string colorString, out PixelColor color)
        {
            color = default;
            if (string.IsNullOrWhiteSpace(colorString)) return false;

            string normalizedString = colorString.Trim().ToLowerInvariant();

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
                    var expandedHex = $"{hex}ff";
                    return TryParseHexPacked(expandedHex, out color);
                }

                if (hex.Length == 3) // #RGB -> #RRGGBB
                {
                    var expandedHex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}ff";
                    return TryParseHexPacked(expandedHex, out color);
                }
            }


            
            return TryParseRGB(normalizedString, out color);
            
           // return false;
        }

        /// <summary>
        /// Tries to parse a hexadecimal color string in the format #AARRGGBB and converts it to a PixelColor.
        /// </summary>
        /// <param name="hexString">The hexadecimal color string to parse.</param>
        /// <param name="color">When this method returns, contains the parsed PixelColor, or default if the parsing fails.</param>
        /// <returns>True if the parsing was successful; otherwise, false.</returns>
        private static bool TryParseHexPacked(string hexString, out PixelColor color)
        {
            color = default;
            if (uint.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var packedValue))
            {
               byte r, g, b, a;

                r = (byte)((packedValue >> 24) & 0xFF);
                g = (byte)((packedValue >> 16) & 0xFF);
                b = (byte)((packedValue >> 8) & 0xFF);
                a = (byte)(packedValue & 0xFF);


                color = new PixelColor(r, g, b, a);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a RGB color string in the format R,G,B and converts it to a PixelColor.
        /// </summary>
        /// <param name="rgbString">The RGB color string to parse.</param>
        /// <param name="color">When this method returns, contains the parsed PixelColor, or default if the parsing fails.</param>
        /// <returns>True if the parsing was successful; otherwise, false.</returns>
        private static bool TryParseRGB(string rgbString, out PixelColor color)
        {
            color = new PixelColor(0, 0, 0, 255);
            string[] separateColor = rgbString.Split(',');
            if (separateColor.Length >= 3 && separateColor.Length <= 4)
            {
                foreach (string item in separateColor)
                {
                    if (!int.TryParse(item, out int n))
                    {
                        return false;
                    }
                }
                int.TryParse(separateColor[0], out int red);
                int.TryParse(separateColor[1], out int green);
                int.TryParse(separateColor[2], out int blue);
                int value = 0;
                if (separateColor.Count() == 4)
                {
                    int.TryParse(separateColor[3], out value);
                }        
                else
                {
                    value = 255;
                }
                int alpha = value;
                color = new PixelColor((byte)NormalizeRGB(red), (byte)NormalizeRGB(green), (byte)NormalizeRGB(blue), (byte)NormalizeRGB(alpha));

                return true;

            }

            return false;
            
            



           
        }
        /// <summary>
        /// Normalizes a RGB number to be between 0 and 255.
        /// </summary>
        /// <param name="number">The number to normalize.</param>
        /// <returns>The normalized number.</returns>
        private static int NormalizeRGB(int number)
        {
            if (number > 255)
                return 255;
            if (number < 0)
                return 0;
            else
                return number;
        }

        /// <summary>
        /// Returns a string representation of the PixelColor.
        /// </summary>
        /// <returns>A string representation of the PixelColor.</returns>
        public override string ToString()
        {
            if (Alpha == 255)
                return $"#{Red:X2}{Green:X2}{Blue:X2}";
            return $"#{Alpha:X2}{Red:X2}{Green:X2}{Blue:X2}";
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is PixelColor other &&
                   Red == other.Red &&
                   Green == other.Green &&
                   Blue == other.Blue &&
                   Alpha == other.Alpha;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Red, Green, Blue, Alpha);
        }

        /// <summary>
        /// Determines whether two specified PixelColor have the same value.
        /// </summary>
        /// <param name="left">The first PixelColor to compare.</param>
        /// <param name="right">The second PixelColor to compare.</param>
        /// <returns>True if the two PixelColor have the same value; otherwise, false.</returns>
        public static bool operator ==(PixelColor left, PixelColor right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified PixelColor have different values.
        /// </summary>
        /// <param name="left">The first PixelColor to compare.</param>
        /// <param name="right">The second PixelColor to compare.</param>
        /// <returns>True if the two PixelColor have different values; otherwise, false.</returns>
        public static bool operator !=(PixelColor left, PixelColor right)
        {
            return !(left == right);
        }
        
        /// <summary>
        /// Adds two PixelColor together.
        /// </summary>
        /// <param name="left">The first PixelColor to add.</param>
        /// <param name="right">The second PixelColor to add.</param>
        /// <returns>A new PixelColor that is the result of adding the two PixelColors together.</returns>
        public static PixelColor operator +(PixelColor left, PixelColor right)
        {
            return Mix(left, right);
        }
        /// <summary>
        /// Mixes two colors together.
        /// </summary>
        /// <param name="left">The first color to mix.</param>
        /// <param name="right">The second color to mix.</param>
        /// <returns>The mixed color.</returns>
        private static PixelColor Mix(PixelColor left, PixelColor right)
        {
            byte a = (byte)((left.Alpha + right.Alpha) / 2);
            byte r = (byte)((left.Red + right.Red) / 2);
            byte g = (byte)((left.Green + right.Alpha) / 2);
            byte b = (byte)((left.Blue + right.Alpha) / 2);
            return new PixelColor(r, g, b, a);
        }
    }