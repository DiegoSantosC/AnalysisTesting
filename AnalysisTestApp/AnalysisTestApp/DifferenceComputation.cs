using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class DifferenceComputation
    {
        private static bool drawing;

        // Difference computation via Manhattan distance

        public static Map getManhattan(Bitmap bmp1, Bitmap bmp2, int[] positions)
        {
            int x0 = Math.Max(0, positions[0]);
            int x1 = Math.Min(bmp1.Width, bmp2.Width + positions[0]);
            int y0 = Math.Max(0, positions[1]);
            int y1 = Math.Min(bmp1.Height, bmp2.Height + positions[1]);

            int newWidth = x1 - x0, newHeight = y1 - y0;

            Bitmap bmp = new Bitmap(newWidth, newHeight);
            int[,] arrSource = new int[newWidth, newHeight];
            Color[,] RGBSource = new Color[newWidth, newHeight];

            for (int x = x0; x < x1; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x, y);

                    System.Drawing.Color pixel2;

                    if (((x + positions[0]) > 0) && ((y + positions[1]) > 0) && ((x + positions[0]) < bmp2.Width) && ((y + positions[1]) < bmp2.Height)) { pixel2 = bmp2.GetPixel(x + positions[0], y + positions[1]); }
                    else { pixel2 = System.Drawing.Color.FromArgb(255, 255, 255); }

                    int colorValue = (Math.Abs(pixel1.R - pixel2.R) + Math.Abs(pixel1.G - pixel2.G) + Math.Abs(pixel1.B - pixel2.B)) / 3;

                    RGBSource[x - x0, y - y0] = pixel2;

                    System.Drawing.Color pixel = getGreyThreshold(colorValue);

                    bmp.SetPixel(x - x0, y - y0, pixel);

                    arrSource[x - x0, y - y0] = colorValue;

                }
            }

            return new Map(arrSource, RGBSource) ;
        }

        // Difference computation via Euclidean distance

        public static Map getEuclidean(Bitmap bmp1, Bitmap bmp2, int[] positions)
        {
            int x0 = Math.Max(0, positions[0]);
            int x1 = Math.Min(bmp1.Width, bmp2.Width + positions[0]);
            int y0 = Math.Max(0, positions[1]);
            int y1 = Math.Min(bmp1.Height, bmp2.Height + positions[1]);

            int newWidth = x1 - x0, newHeight = y1 - y0;

            Bitmap bmp = new Bitmap(newWidth, newHeight);

            int[,] colorMap = new int[newWidth, newHeight];
            Color[,] RGBSource = new Color[newWidth, newHeight];

            for (int x = x0; x < x1; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x, y);

                    System.Drawing.Color pixel2 = bmp2.GetPixel(x + positions[0], y + positions[1]);

                    int colorValue = (int)Math.Round(Math.Sqrt(Math.Pow((pixel1.R - pixel2.R), 2) + Math.Pow((pixel1.G - pixel2.G), 2) + Math.Pow((pixel1.B - pixel2.B), 2)) / 3);

                    colorMap[x, y] = colorValue;

                    RGBSource[x - x0, y - y0] = pixel2;

                    System.Drawing.Color pixel = getGreyThreshold(colorValue);

                    bmp.SetPixel(x, y, pixel);
                }
            }

            return new Map(colorMap, RGBSource);
        }

        // Difference computation using the pearson coefficient parameter

        private static Map getGlobalCorrelation(Bitmap bmp1, Bitmap bmp2, int[] positions)
        {

            int[] accX = new int[3], accY = new int[3];
            long[] accXX = new long[3], accYY = new long[3], accXY = new long[3];
            double[] avgX = new double[3], avgY = new double[3], varX = new double[3], varY = new double[3], covXY = new double[3], corrIndex = new double[3];

            int x0 = Math.Max(0, positions[0]);
            int x1 = Math.Min(bmp1.Width, bmp2.Width + positions[0]);
            int y0 = Math.Max(0, positions[1]);
            int y1 = Math.Min(bmp1.Height, bmp2.Height + positions[1]);

            double epsilon = AdvancedOptions._dEpsilonValue;

            for (int i = 0; i < 3; i++)
            {
                accX[i] = 0;
                accY[i] = 0;
                accXX[i] = 0;
                accYY[i] = 0;
                accXY[i] = 0;
                avgX[i] = 0;
                avgY[i] = 0;
                varX[i] = 0;
                avgY[i] = 0;
                covXY[i] = 0;
                corrIndex[i] = 0;

            }

            for (int j = y0; j < y1; j++)
            {
                for (int i = x0; i < x1; i++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(i, j);

                    System.Drawing.Color pixel2 = bmp2.GetPixel(i + positions[0], j + positions[1]);

                    accX[0] += pixel1.R;
                    accX[1] += pixel1.G;
                    accX[2] += pixel1.B;

                    accY[0] += pixel2.R;
                    accY[1] += pixel2.G;
                    accY[2] += pixel2.B;

                    accXX[0] += (int)Math.Pow(pixel1.R, 2);
                    accXX[1] += (int)Math.Pow(pixel1.G, 2);
                    accXX[2] += (int)Math.Pow(pixel1.B, 2);

                    accYY[0] += (int)Math.Pow(pixel2.R, 2);
                    accYY[1] += (int)Math.Pow(pixel2.G, 2);
                    accYY[2] += (int)Math.Pow(pixel2.B, 2);

                    accXY[0] += pixel1.R * pixel2.R;
                    accXY[1] += pixel1.G * pixel2.G;
                    accXY[2] += pixel1.B * pixel2.B;

                }
            }

            int n = (x1 - x0) * (y1 - y0);

            avgX[0] = accX[0] / n;
            avgX[1] = accX[1] / n;
            avgX[2] = accX[2] / n;

            avgY[0] = accY[0] / n;
            avgY[1] = accY[1] / n;
            avgY[2] = accY[2] / n;

            varX[0] = (accXX[0] / n) - Math.Pow(avgX[0], 2);
            varX[1] = (accXX[1] / n) - Math.Pow(avgX[1], 2);
            varX[2] = (accXX[2] / n) - Math.Pow(avgX[2], 2);

            varY[0] = (accYY[0] / n) - Math.Pow(avgY[0], 2);
            varY[1] = (accYY[1] / n) - Math.Pow(avgY[1], 2);
            varY[2] = (accYY[2] / n) - Math.Pow(avgY[2], 2);

            if (varX[0] <= 0) varX[0] = epsilon;
            if (varX[1] <= 0) varX[1] = epsilon;
            if (varX[2] <= 0) varX[2] = epsilon;

            if (varY[0] <= 0) varY[0] = epsilon;
            if (varY[1] <= 0) varY[1] = epsilon;
            if (varY[2] <= 0) varY[2] = epsilon;

            covXY[0] = (accXY[0] / n) - avgX[0] * avgY[0];
            covXY[1] = (accXY[1] / n) - avgX[1] * avgY[1];
            covXY[2] = (accXY[2] / n) - avgX[2] * avgY[2];

            corrIndex[0] = covXY[0] / Math.Sqrt(varX[0] * varY[0]);
            corrIndex[1] = covXY[1] / Math.Sqrt(varX[1] * varY[1]);
            corrIndex[2] = covXY[2] / Math.Sqrt(varX[2] * varY[2]);

            int newWidth = x1 - x0, newHeight = y1 - y0;

            Bitmap bmp = new Bitmap(newWidth, newHeight);

            int[,] colorMap = new int[newWidth, newHeight];
            Color[,] RGBSource = new Color[newWidth, newHeight];

            for (int j = 0; j < newHeight; j++)
            {
                for (int i = 0; i < newWidth; i++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(i + x0, j + y0);
                    System.Drawing.Color pixel2 = bmp2.GetPixel(i + x0 + positions[0], j + y0 + positions[1]);

                    int[] diff = new int[3];

                    diff[0] = (int)(Math.Abs(pixel1.R - pixel2.R) * corrIndex[0]);
                    diff[1] = (int)(Math.Abs(pixel1.G - pixel2.G) * corrIndex[1]);
                    diff[2] = (int)(Math.Abs(pixel1.B - pixel2.B) * corrIndex[2]);

                    colorMap[i, j] = (255 - diff[0] + 255 - diff[1] + 255 - diff[2]) / 3;

                    RGBSource[i, j] = pixel2;

                    System.Drawing.Color pixel = System.Drawing.Color.FromArgb(255 - diff[0], 255 - diff[1], 255 - diff[2]);
                    bmp.SetPixel(i, j, pixel);
                }
            }

            return new Map(colorMap, RGBSource);
        }

        public static System.Drawing.Color getGrey(int colorValue)
        {
            return (System.Drawing.Color.FromArgb(colorValue, colorValue, colorValue));
        }

        // Decide wether a pixel's difference is enough to be relevant or not via a fixed threshold and return absolute value

        private static System.Drawing.Color getGreyThreshold(int colorValue)
        {
            int threshold = AdvancedOptions._nThresholdValue;

            if (colorValue < threshold)
            {
                // return white

                return (System.Drawing.Color.FromArgb(0, 0, 0));
            }
            else
            {
                // reuturn grey value

                return (System.Drawing.Color.FromArgb(colorValue, colorValue, colorValue));
            }
        }

        // Decide wether a pixel's difference is enough to be relevant or not via a fixed threshold and return grey value

        private static System.Drawing.Color getThreshold(int colorValue)
        {
            int threshold = AdvancedOptions._nThresholdValue;

            if (colorValue < threshold)
            {
                // return white

                return (System.Drawing.Color.FromArgb(0, 0, 0));
            }
            else
            {
                // return black

                return (System.Drawing.Color.FromArgb(255, 255, 255));
            }
        }

        // Decide wether a pixel's difference is enough to be relevant or not via a hysteresis cycle

        private static System.Drawing.Color getHysteresis(int colorValue)
        {
            int downThreshold = AdvancedOptions._nBottomHysteresis, upThreshold = AdvancedOptions._nTopHysteresis;

            if (drawing)
            {
                if (colorValue < downThreshold)
                {
                    drawing = false;
                    return (System.Drawing.Color.FromArgb(0, 0, 0));
                }
                else
                {
                    return (System.Drawing.Color.FromArgb(255, 255, 255));
                }
            }
            else
            {
                if (colorValue > upThreshold)
                {
                    drawing = true;
                    return (System.Drawing.Color.FromArgb(255, 255, 255));
                }
                else
                {
                    return (System.Drawing.Color.FromArgb(0, 0, 0));
                }
            }
        }
    }
}
