using System;

namespace MandelbrotNetCore
{
    public static class ColorConverter
    {
        /// <summary>
        ///     Convert HSV to RGB
        ///     h is from 0-360
        ///     s,v values are 0-1
        ///     r,g,b values are 0-255
        /// </summary>
        public static void HsvToRgb(
            double hue,
            double saturation,
            double value,
            out int red,
            out int green,
            out int blue
        )
        {
            double h = hue;

            while (h < 0)
            {
                h += 360;
            }

            while (h >= 360)
            {
                h -= 360;
            }

            double r, g, b;

            if (value <= 0)
            {
                r = g = b = 0;
            }
            else if (saturation <= 0)
            {
                r = g = b = value;
            }
            else
            {
                double hf = h / 60.0;
                int i = (int) Math.Floor(hf);
                double f = hf - i;
                double pv = value * (1 - saturation);
                double qv = value * (1 - saturation * f);
                double tv = value * (1 - saturation * (1 - f));

                switch (i)
                {
                    // Red is the dominant color
                    case 0:
                        r = value;
                        g = tv;
                        b = pv;

                        break;

                    // Green is the dominant color
                    case 1:
                        r = qv;
                        g = value;
                        b = pv;

                        break;
                    case 2:
                        r = pv;
                        g = value;
                        b = tv;

                        break;

                    // Blue is the dominant color
                    case 3:
                        r = pv;
                        g = qv;
                        b = value;

                        break;
                    case 4:
                        r = tv;
                        g = pv;
                        b = value;

                        break;

                    // Red is the dominant color
                    case 5:
                        r = value;
                        g = pv;
                        b = qv;

                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                    case 6:
                        r = value;
                        g = tv;
                        b = pv;

                        break;
                    case -1:
                        r = value;
                        g = pv;
                        b = qv;

                        break;

                    // The color is not defined, we should throw an error.
                    default:
                        throw new Exception($"i Value error in Pixel conversion, Value is {i}");
                    
                    //r = g = b = value; // Just pretend its black/white

                    //break;
                }
            }

            red = Clamp((int) (r * 255.0));
            green = Clamp((int) (g * 255.0));
            blue = Clamp((int) (b * 255.0));
        }

        /// <summary>
        ///     Clamp a value to 0-255
        /// </summary>
        private static int Clamp(int i)
        {
            if (i < 0)
            {
                return 0;
            }

            if (i > 255)
            {
                return 255;
            }

            return i;
        }
    }
}
