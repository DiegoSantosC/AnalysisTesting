using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace AnalysisTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool drawing;

        public MainWindow()
        {

            InitializeComponent();

            System.Windows.Controls.Image firstImg = new System.Windows.Controls.Image();
            BitmapImage src2 = new BitmapImage();
            src2.BeginInit();
            src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Test3.png", UriKind.Absolute);
            src2.CacheOption = BitmapCacheOption.OnLoad;
            src2.EndInit();
            firstImg.Source = src2;
            firstImg.Stretch = Stretch.Uniform;
            sp1.Children.Add(firstImg);

            System.Windows.Controls.Image secondImg = new System.Windows.Controls.Image();
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Test4.png", UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            secondImg.Source = src;
            secondImg.Stretch = Stretch.Uniform;
            sp2.Children.Add(secondImg);


            System.Drawing.Image first = System.Drawing.Image.FromFile(@"Resources\Test3.png");

            System.Drawing.Image second = System.Drawing.Image.FromFile(@"Resources\Test4.png");

            Bitmap bitmap1 = new Bitmap(first);
            Bitmap bitmap2 = new Bitmap(second);

            int[] positions = RobinsonRepositioning(bitmap1, bitmap2);

            Console.WriteLine(positions[0] + " " + positions[1]);

            Bitmap bmp = getGlobalCorrelation(bitmap1, bitmap2, positions);

            System.Drawing.Image img3 = bmp;
            MemoryStream ms2 = new MemoryStream();
            img3.Save(ms2, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = ms2;
            bi.EndInit();

            System.Windows.Controls.Image resultImg = new System.Windows.Controls.Image();
            resultImg.Source = bi;
            resultImg.Stretch = Stretch.Uniform;
            sp3.Children.Add(resultImg);

            bmp.Save(@"Resources\Result.bmp");

        }

        private int[] RobinsonRepositioning(Bitmap bitmap1, Bitmap bitmap2)
        {
            int[,] edges1 = findRobinsonEdges(bitmap1);
            int[,] edges2 = findRobinsonEdges(bitmap2);

            int[] positions = findRobinsonReposition(edges1, edges2);

            return positions;
        }

        private int[] findRobinsonReposition(int[,] e1, int[,] e2)
        {
            // set range of values to be tested

            int deplX, deplY, stepX, stepY, height, width;

            // static values based on Sprout's usual deplacements

            int min = 3, max = 5;

            int[] bestPositions = new int[2];
            double bestDifference = Int32.MaxValue;

            width = Math.Min (e1.GetLength(0), e2.GetLength(0));
            stepX = 1;
            deplX = Math.Max(e1.GetLength(0), e2.GetLength(0)) - width;
            if (deplX < min) deplX = min;
            if (deplX > max) { stepX = deplX / max; }

            height = Math.Min(e1.GetLength(1), e2.GetLength(1));
            stepY = 1;
            deplY = Math.Max(e1.GetLength(1), e2.GetLength(1)) - height;
            if (deplY < min) deplY = min;
            if (deplY > max) { stepY = deplY / max; }


            // deplace 2 in relation to 1
            for (int i = -deplY; i <= deplY; i+=stepY)
            {
                for (int j = -deplX; j <= deplX; j+=stepX)
                {
                    double diff = 0;
                    int count = 0;

                    // match pixel by pixel 
                    for (int h = 0; h < height; h++)
                    {
                        for (int w = 0; w < width; w++)
                        {
                            if ((h + i >= 0) && (h + i < e2.GetLength(1)) && (w + j >= 0) && (w + j < e2.GetLength(0)))
                            {
                                diff += Math.Abs(e2[w + j, h + i] - e1[w, h]);
                                count++;
                            }
                        }
                    }
                    diff /= count;

                    Console.WriteLine(diff);

                    if (diff < bestDifference) { bestDifference = diff; bestPositions[0] = j; bestPositions[1] = i; Console.WriteLine(j + " " + i); }
                }

                Console.WriteLine();
            }

            return bestPositions;
        }

        private int[,] findRobinsonEdges(Bitmap bmp)
        {
            /*  Variation of Robinson mask for edges calculation used:
                    
                  0 -1  0
                 -1  2  0
                  0  0  0
            */

            // edges in fig

            int[,] edgeGreyScale = new int[bmp.Width, bmp.Height];

            for (int x = 1; x < bmp.Width; x++)
            {
                for (int y = 1; y < bmp.Height; y++)
                {
                        int edgyness = 0;

                        System.Drawing.Color center = bmp.GetPixel(x, y);
                        System.Drawing.Color L = bmp.GetPixel(x - 1, y);
                        System.Drawing.Color U = bmp.GetPixel(x, y - 1);
                        //System.Drawing.Color R = bmp.GetPixel(x + 1, y);
                        //System.Drawing.Color D = bmp.GetPixel(x, y + 1);

                        edgyness += (Math.Abs(center.R - L.R) + Math.Abs(center.G - L.G) + Math.Abs(center.B - L.B));
                        edgyness += (Math.Abs(center.R - U.R) + Math.Abs(center.G - U.G) + Math.Abs(center.B - U.B));
                        //edgyness -= (Math.Abs(center.R - D.R) + Math.Abs(center.G - D.G) + Math.Abs(center.B - D.B));
                        //edgyness -= (Math.Abs(center.R - R.R) + Math.Abs(center.G - R.G) + Math.Abs(center.B - R.B));

                        // threshold to be consider an edge is put to 255/3 * 3 * 2, this is (RGB MAX value)/3 * |RGB| * |Directions considered|

                        edgeGreyScale[x, y] = 255 - (edgyness / 6);

                        //int greyColor = (center.R + center.G + center.B) / 3;

                        //if (edgyness > ((255 / 10) * 3 * 2)) edgeGreyScale[x, y] = greyColor;
                        //else edgeGreyScale[x, y] = 255;

                    

                    // Borders are not fittable for being edges
                }
            }

            return edgeGreyScale;
        }

        private Bitmap getManhattan(Bitmap bmp1, Bitmap bmp2, int[] positions)
        {
            int width = bmp1.Width, height = bmp1.Height;

            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[width, height];

            Bitmap bmp = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x, y);

                    // The positions deplacement will always be applied to the second taken image

                    System.Drawing.Color pixel2; 

                    if (((x + positions[0]) > 0) && ((y + positions[1]) > 0) && ((x + positions[0]) < bmp2.Width) && ((y + positions[1]) < bmp2.Height)) { pixel2 = bmp2.GetPixel(x + positions[0], y + positions[1]); }
                    else { pixel2 = System.Drawing.Color.FromArgb(255, 255, 255); }

                    int colorValue = (Math.Abs(pixel1.R - pixel2.R) + Math.Abs(pixel1.G - pixel2.G) + Math.Abs(pixel1.B - pixel2.B)) / 3;

                    pixelArray[x, y] = getGreyThreshold(Math.Abs(255 - colorValue));

                    bmp.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return bmp;
        }

        private Bitmap getEuclidean(Bitmap bmp1, Bitmap bmp2, int[] positions)
        {
            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[bmp1.Width, bmp1.Height];

            Bitmap bmp = new Bitmap(bmp1.Width, bmp1.Height);

            for (int x = 0; x < bmp1.Width; x++)
            {
                for (int y = 0; y < bmp1.Height; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x, y);

                    // The positions deplacement will always be applied to the second taken image

                    System.Drawing.Color pixel2;

                    if (((x + positions[0]) > 0) && ((y + positions[1]) > 0) && ((x + positions[0]) < bmp2.Width) && ((y + positions[1]) < bmp2.Height)) { pixel2 = bmp2.GetPixel(x + positions[0], y + positions[1]); }
                    else { pixel2 = System.Drawing.Color.FromArgb(255, 255, 255); }


                    int colorValue = (int)Math.Round(Math.Sqrt(Math.Pow((pixel1.R - pixel2.R), 2) + Math.Pow((pixel1.G - pixel2.G),2) + Math.Pow((pixel1.B - pixel2.B),2))/3);

                    pixelArray[x, y] = getGreyThreshold(255 - colorValue);

                    bmp.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return bmp;
        }

        private Bitmap getGlobalCorrelation(Bitmap bmp1, Bitmap bmp2,int[] positions)
        {
            int width = Math.Min(bmp1.Width, bmp2.Width), height = Math.Min(bmp1.Height, bmp2.Height);      

            int[] accX = new int[3], accY = new int[3];
            long[] accXX = new long[3], accYY = new long[3], accXY = new long[3];
            double[] avgX = new double[3], avgY = new double[3], varX = new double[3], varY = new double[3], covXY = new double[3], corrIndex = new double[3];

            int x0 = Math.Max(0, positions[0]);
            int x1 = Math.Min(bmp1.Width, bmp2.Width + positions[0]);
            int y0 = Math.Max(0, positions[1]);
            int y1 = Math.Min(bmp1.Height, bmp2.Height + positions[1]);

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

            int n = (x1- x0)  * (y1 - y0);

            avgX[0] = accX[0] / n;
            avgX[1] = accX[1] / n;
            avgX[2] = accX[2] / n;

            avgY[0] = accY[0] / n;
            avgY[1] = accY[1] / n;
            avgY[2] = accY[2] / n;

            Console.WriteLine(accXX[0] + " " + accXY[1] + " " + accYY[2]);
            Console.WriteLine(varY[0] + " " + varY[1] + " " + varY[2]);

            varX[0] = (accXX[0]/n) - Math.Pow(avgX[0], 2);
            varX[1] = (accXX[1]/n) - Math.Pow(avgX[1], 2);
            varX[2] = (accXX[2]/n) - Math.Pow(avgX[2], 2);

            varY[0] = (accYY[0]/n) - Math.Pow(avgY[0], 2);
            varY[1] = (accYY[1]/n) - Math.Pow(avgY[1], 2);
            varY[2] = (accYY[2]/n) - Math.Pow(avgY[2], 2);

            if (varX[0] < 0) varX[0] = 1.0e-9;
            if (varX[1] < 0) varX[1] = 1.0e-9;
            if (varX[2] < 0) varX[2] = 1.0e-9;

            if (varY[0] < 0) varY[0] = 1.0e-9;
            if (varY[1] < 0) varY[1] = 1.0e-9;
            if (varY[2] < 0) varY[2] = 1.0e-9;

            covXY[0] = (accXY[0] / n) - avgX[0] * avgY[0];
            covXY[1] = (accXY[1] / n) - avgX[1] * avgY[1];
            covXY[2] = (accXY[2] / n) - avgX[2] * avgY[2];

            corrIndex[0] = covXY[0] / Math.Sqrt(varX[0] * varY[0]);
            corrIndex[1] = covXY[1] / Math.Sqrt(varX[1] * varY[1]);
            corrIndex[2] = covXY[2] / Math.Sqrt(varX[2] * varY[2]);

            int newWidth = x1 - x0, newHeight = y1 - y0;

            Bitmap bmp = new Bitmap(newWidth, newHeight);

            for (int j = 0; j < newHeight; j++)
            {
                for(int i = 0; i<newWidth; i++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(i +x0 , j +y0);
                    System.Drawing.Color pixel2 = bmp2.GetPixel(i + x0+ positions[0], j +y0 + positions[1]);

                    int[] diff = new int[3];

                    diff[0] = (int)(Math.Abs(pixel1.R - pixel2.R) * corrIndex[0]);
                    diff[1] = (int)(Math.Abs(pixel1.G - pixel2.G) * corrIndex[1]);
                    diff[2] = (int)(Math.Abs(pixel1.B - pixel2.B) * corrIndex[2]);

                    System.Drawing.Color pixel = System.Drawing.Color.FromArgb(255 - diff[0],255 - diff[1],255 - diff[2]);
                    bmp.SetPixel(i, j, pixel);
                }
            }

            return bmp;
        }

        private System.Drawing.Color getGrey(int colorValue)
        {
            return (System.Drawing.Color.FromArgb(colorValue, colorValue, colorValue));
        }

        private System.Drawing.Color getGreyThreshold(int colorValue)
        {
            int threshold = 200;

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

        private System.Drawing.Color getThreshold(int colorValue)
        {
            int threshold = 200;

            if (colorValue < threshold)
            {
                // return white

                return (System.Drawing.Color.FromArgb(0, 0, 0));
            }else
            {
                // return black

                return (System.Drawing.Color.FromArgb(255, 255, 255));
            }
        }

        private System.Drawing.Color getHysteresis(int colorValue)
        {
            int downThreshold = 180, upThreshold = 220;

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

        private int[] getDeplacement(Bitmap bmp1, Bitmap bmp2)
        {
            int[] d = new int[2];

            d[0] = Math.Abs((bmp1.Width-bmp2.Width));
            d[1] = Math.Abs((bmp1.Height - bmp2.Height));

            return d;
        }

        private Bitmap executeReposition(Bitmap bmp1, int[] bestPositions, int h, int w)
        {
            Console.WriteLine(bestPositions[0] + " " + bestPositions[1]);

            Bitmap b = new Bitmap(w, h);

            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (y < bestPositions[1] || w < bestPositions[0]) { pixelArray[x, y] = System.Drawing.Color.FromArgb(255, 255, 255); }
                    else if (y >= bestPositions[1] + bmp1.Height || x >= bestPositions[0] + bmp1.Width) { pixelArray[x, y] = System.Drawing.Color.FromArgb(255, 255, 255);}
                    else { pixelArray[x, y] = bmp1.GetPixel(x - bestPositions[0], y - bestPositions[1]);}

                    b.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return b;
        }

    }
}