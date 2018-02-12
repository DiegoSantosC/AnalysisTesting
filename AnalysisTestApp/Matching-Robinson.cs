using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class Matching_Robinson
    {

        // Frute force best match between two given images taking into consideration their edges, found via a Robinson mask filter

        public static int[] RobinsonRepositioning(Bitmap bitmap1, Bitmap bitmap2)
        {
            int[,] edges1 = findRobinsonEdges(bitmap1);
            int[,] edges2 = findRobinsonEdges(bitmap2);

            int[] positions = findRobinsonReposition(edges1, edges2);

            return positions;
        }

        // Given certain edges of two imags, brute forthe thought them to obtain minimal ECM

        private static int[] findRobinsonReposition(int[,] e1, int[,] e2)
        {
            // set range of values to be tested

            int deplX, deplY, stepX, stepY, height, width;

            // static values based on Sprout's usual deplacements

            int min = AdvancedOptions._nMinReposition, max = AdvancedOptions._nMaxReposition;

            int[] bestPositions = new int[2];
            double bestDifference = Int32.MaxValue;

            width = Math.Min(e1.GetLength(0), e2.GetLength(0));
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
            for (int i = -deplY; i <= deplY; i += stepY)
            {
                for (int j = -deplX; j <= deplX; j += stepX)
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

                    if (diff < bestDifference) { bestDifference = diff; bestPositions[0] = j; bestPositions[1] = i; }
                }
            }

            return bestPositions;
        }

        private static int[,] findRobinsonEdges(Bitmap bmp)
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

        // Deplace an image a given number of pixels to get an optimal match

        private Bitmap executeReposition(Bitmap bmp1, int[] bestPositions, int h, int w)
        {
            Bitmap b = new Bitmap(w, h);

            System.Drawing.Color[,] pixelArray = new System.Drawing.Color[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (y < bestPositions[1] || w < bestPositions[0]) { pixelArray[x, y] = System.Drawing.Color.FromArgb(255, 255, 255); }
                    else if (y >= bestPositions[1] + bmp1.Height || x >= bestPositions[0] + bmp1.Width) { pixelArray[x, y] = System.Drawing.Color.FromArgb(255, 255, 255); }
                    else { pixelArray[x, y] = bmp1.GetPixel(x - bestPositions[0], y - bestPositions[1]); }

                    b.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return b;
        }
    }
}
