using System;
using static System.Math;

namespace Toggl.Shared
{
    public struct Color : IEquatable<Color>
    {
        public byte Alpha { get; }
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }

        public Color(byte red, byte green, byte blue, byte alpha = 255)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public Color(uint argb)
        {
            Alpha = (byte)((argb >> 24) & 255);
            Red = (byte)((argb >> 16) & 255);
            Green = (byte)((argb >> 8) & 255);
            Blue = (byte)(argb & 255);
        }

        /// <summary>
        /// Creates a Color from a hexadecimal string. Valid formats: aarrggbb, #aarrggbb, rrggbb, #rrggbb
        /// </summary>
        public Color(string hex) : this(hexStringToInt(hex))
        {
        }

        private static uint hexStringToInt(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return 0;

            hex = hex.TrimStart('#');

            var hexLength = hex.Length;

            if (hexLength == 6)
                return 0xFF000000 + Convert.ToUInt32(hex, 16);

            if (hexLength == 8)
                return Convert.ToUInt32(hex, 16);

            throw new ArgumentException("Invalid hex string was provided. Valid formats: aarrggbb, #aarrggbb, rrggbb, #rrggbb");
        }

        public override string ToString()
        {
            return $"{{a={Alpha}, r={Red}, g={Green}, b={Blue}}}";
        }

        public override int GetHashCode()
            => HashCode.Combine(Alpha, Red, Green, Blue);

        public static bool operator ==(Color color, Color otherColor)
            => color.Red == otherColor.Red
            && color.Green == otherColor.Green
            && color.Blue == otherColor.Blue
            && color.Alpha == otherColor.Alpha;

        public static bool operator !=(Color color, Color otherColor)
            => !(color == otherColor);

        public override bool Equals(object obj)
        {
            if (obj is Color color)
                return this == color;

            return false;
        }

        public bool Equals(Color other)
            => this == other;
    }

    public static class ColorExtensions
    {
        public static string ToHexString(this Color color)
            => $"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}";

        public static Color WithAlpha(this Color color, byte alpha)
            => new Color(color.Red, color.Green, color.Blue, alpha);

        public static Color Darken(this Color color)
            => color.ChangeColorBrightness(-0.2f);

        public static Color Lighten(this Color color)
            => color.ChangeColorBrightness(0.2f);

        public static Color ChangeColorBrightness(this Color color, float correctionFactor)
        {
            float red = (float)color.Red;
            float green = (float)color.Green;
            float blue = (float)color.Blue;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return new Color((byte)red, (byte)green, (byte)blue, color.Alpha);
        }

        public static double CalculateLuminance(this Color color)
        {
            // Adjusted relative luminance
            // math based on https://www.w3.org/WAI/GL/wiki/Relative_luminance

            var rsrgb = color.Red / 255.0;
            var gsrgb = color.Green / 255.0;
            var bsrgb = color.Blue / 255.0;

            var lowGammaCoeficient = 1 / 12.92;

            var r = rsrgb <= 0.03928 ? rsrgb * lowGammaCoeficient : adjustGamma(rsrgb);
            var g = gsrgb <= 0.03928 ? gsrgb * lowGammaCoeficient : adjustGamma(gsrgb);
            var b = bsrgb <= 0.03928 ? bsrgb * lowGammaCoeficient : adjustGamma(bsrgb);

            var luma = r * 0.2126 + g * 0.7152 + b * 0.0722;
            return luma;

            double adjustGamma(double channel)
                => Pow((channel + 0.055) / 1.055, 2.4);
        }
    }
}
