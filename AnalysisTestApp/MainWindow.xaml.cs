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
        private List<List<Cluster>> blobTrack;
        private List<Cluster> lastTrack;

        public MainWindow()
        {

            InitializeComponent();

            blobTrack = new List<List<Cluster>>();

            startTracking();

            // UI elements initialization

            //System.Windows.Controls.Image firstImg = new System.Windows.Controls.Image();
            //BitmapImage src2 = new BitmapImage();
            //src2.BeginInit();
            //src2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Captures\C1.png", UriKind.Absolute);
            //src2.CacheOption = BitmapCacheOption.OnLoad;
            //src2.EndInit();
            //firstImg.Source = src2;
            //firstImg.Stretch = Stretch.Uniform;
            //sp1.Children.Add(firstImg);

            //System.Windows.Controls.Image secondImg = new System.Windows.Controls.Image();
            //BitmapImage src = new BitmapImage();
            //src.BeginInit();
            //src.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Resources\Captures\C10.png", UriKind.Absolute);
            //src.CacheOption = BitmapCacheOption.OnLoad;
            //src.EndInit();
            //secondImg.Source = src;
            //secondImg.Stretch = Stretch.Uniform;
            //sp2.Children.Add(secondImg);


            //System.Drawing.Image first = System.Drawing.Image.FromFile(@"Resources\Captures\C1.png");
            //System.Drawing.Image second = System.Drawing.Image.FromFile(@"Resources\Captures\C10.png");

            //Bitmap bitmap1 = new Bitmap(first);
            //Bitmap bitmap2 = new Bitmap(second);

            
            
            //System.Drawing.Image img3 = System.Drawing.Image.FromFile(@"Resources\Test3.png"); 
            //MemoryStream ms2 = new MemoryStream();
            //img3.Save(ms2, System.Drawing.Imaging.ImageFormat.Bmp);

            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //bi.CacheOption = BitmapCacheOption.OnLoad;
            //bi.StreamSource = ms2;
            //bi.EndInit();

            //System.Windows.Controls.Image resultImg = new System.Windows.Controls.Image();
            //resultImg.Source = bi;
            //resultImg.Stretch = Stretch.Uniform;
            //sp3.Children.Add(resultImg);

            //bmp.Save(@"Resources\Result.bmp");

        }

        private void startTracking()
        {
            Bitmap[] images = new Bitmap[11];

            for(int i=0; i<11; i++)
            {
               
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! WRONG CODE

                System.Drawing.Image img = System.Drawing.Image.FromFile(@"Resources\Captures\C1.png");
                Bitmap bmp = new Bitmap(img);

                images[i] = bmp;
            }
            
            for(int i = 1; i< 11; i++)
            {
                // Repositioning finding and executing difference between the two images

                int[] positions = RobinsonRepositioning(images[0], images[i]);

                int[,] resultDifference = getManhattan(images[0], images[i], positions);

                List<Cluster> frameBlobs = FindObjects(resultDifference);

                if (blobTrack.Count == 0) firstScan(frameBlobs);
                else
                {
                    assignBlobs(frameBlobs);
                }
            }
        }

        // Bitmap obtention from a local saved image

        private Bitmap getBitmap(int v)
        {
            string imgName = "C" + v.ToString() + ".png";
            string source = System.IO.Path.Combine(@"Resources\Captures\", imgName);
            System.Drawing.Image img = System.Drawing.Image.FromFile(source);

            Bitmap bmp = new Bitmap(img);
            return bmp; 
        }

        private List<Cluster> FindObjects(int[,] resultDifference)
        {
            ConnectedComponents cc = new ConnectedComponents(resultDifference, " ", new ROI(AdvancedOptions._nROIMargin, resultDifference), "");

            // Drawing the image will result in acquiring visual feedback of the blobs detected via their bounding boxes

            // Draw(cc.marked);

            return cc.getIdentifiedBlobs();    
        }

        // Turn int[,] containing pixel grey color values and cluster bounding boxes' contour into bitmap pixel colours

        private void Draw(int[,] marked)
        {
            Bitmap bmp = new Bitmap(marked.GetLength(0), marked.GetLength(1));

            for (int y = 0; y < marked.GetLength(1); y++)
            {
                for (int x = 0; x < marked.GetLength(0); x++)
                {
                    if(marked[x, y] == -1)
                    {
                        bmp.SetPixel(x, y, System.Drawing.Color.Green);
                    }else
                    {
                        bmp.SetPixel(x, y, getGrey(255 - marked[x, y]));
                    }
                }
            }

            bmp.Save(@"Resources\Result.bmp");
        }

        // Initial step of blob track

        private void firstScan(List<Cluster> blobs)
        {
            for(int i = 0; i < blobs.Count; i++)
            {
                List<Cluster> initialList = new List<Cluster>();
                initialList.Add(blobs.ElementAt(i));

                blobTrack.Add(initialList);

            }

            lastTrack = blobs;
        }

        // Relate obtained blobs to those already tracked

        private void assignBlobs(List<Cluster> blobs)
        {
            bool mergedChance = false, newColonyChance = false; 

            if (blobs.Count < lastTrack.Count) mergedChance = true;
            if (blobs.Count > lastTrack.Count) newColonyChance = true;

            for(int i = 0; i < blobs.Count; i++)
            {
                if (!mergedChance && !newColonyChance)
                {
                    bool match = blobCompare(lastTrack.ElementAt(i), blobs.ElementAt(i));

                    if (match)
                    {
                        int index = lastTrack.ElementAt(i).getId();
                        blobs.ElementAt(i).setIndex(index);

                        blobTrack.ElementAt(index).Add(blobs.ElementAt(i));

                    }else
                    {
                        // Try matching it with other blobs

                        int matchIndex = blobCompare(lastTrack, blobs.ElementAt(i));

                        if(matchIndex >= 0)
                        {
                            int index = lastTrack.ElementAt(matchIndex).getId();
                            blobs.ElementAt(i).setIndex(index);

                            blobTrack.ElementAt(index).Add(blobs.ElementAt(i));
                        }
                        else
                        {
                            // No match found 

                            bool merged = findPossibleMerge(lastTrack, blobs.ElementAt(i));
                            if (!merged)
                            {
                                // New colony has appeared

                                List<Cluster> newlist = new List<Cluster>();
                                blobs.ElementAt(i).setIndex(blobTrack.Count);
                                newlist.Add(blobs.ElementAt(i));
                                blobTrack.Add(newlist);

                            }
                        }
                    }
                }else
                {
                    // Try matching it with other blobs

                    int matchIndex = blobCompare(lastTrack, blobs.ElementAt(i));

                    if (matchIndex >= 0)
                    {
                        int index = lastTrack.ElementAt(matchIndex).getId();
                        blobs.ElementAt(i).setIndex(index);

                        blobTrack.ElementAt(index).Add(blobs.ElementAt(i));
                    }
                    else
                    {
                        // No match found 

                        bool merged = findPossibleMerge(lastTrack, blobs.ElementAt(i));
                        if (!merged)
                        {
                            // New colony has appeared

                            List<Cluster> newlist = new List<Cluster>();
                            blobs.ElementAt(i).setIndex(blobTrack.Count);
                            newlist.Add(blobs.ElementAt(i));
                            blobTrack.Add(newlist);

                        }
                    }
                }

                lastTrack = blobs;
            }

        }

        private bool findPossibleMerge(List<Cluster> lastTrack, Cluster blob)
        {
            for(int i = 0; i < lastTrack.Count; i++)
            {
                for (int j = 0; j < lastTrack.Count; j++)
                {
                    if(Merged(lastTrack.ElementAt(i), lastTrack.ElementAt(j), blob))
                    {
                        int indexDest, indexBrunch;

                        if (lastTrack.ElementAt(i).getSize() > lastTrack.ElementAt(j).getSize())
                        {
                            indexDest = lastTrack.ElementAt(i).getId();
                            indexBrunch = lastTrack.ElementAt(j).getId();

                            blobTrack.ElementAt(indexBrunch).ElementAt(blobTrack.ElementAt(indexBrunch).Count - 1).hasMerged();
                            blobTrack.ElementAt(indexDest).ElementAt(blobTrack.ElementAt(indexDest).Count - 1).addBranch(blobTrack.ElementAt(indexBrunch));

                            blob.setIndex(indexDest);
                            blobTrack.ElementAt(indexDest).Add(blob);
                          
                        }
                        else
                        {
                            indexDest = lastTrack.ElementAt(j).getId();
                            indexBrunch = lastTrack.ElementAt(i).getId();

                            blobTrack.ElementAt(indexBrunch).ElementAt(blobTrack.ElementAt(indexBrunch).Count - 1).hasMerged();
                            blobTrack.ElementAt(indexDest).ElementAt(blobTrack.ElementAt(indexDest).Count - 1).addBranch(blobTrack.ElementAt(indexBrunch));

                            blob.setIndex(indexDest);
                            blobTrack.ElementAt(indexDest).Add(blob);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        // Characteristics comparison that identify a merged blob

        private bool Merged(Cluster cluster1, Cluster cluster2, Cluster blob)
        {
            int possibility = 0;

            double sizeDifference = (blob.getSize() - (cluster1.getSize() + cluster2.getSize())) / blob.getSize();

            // First feature: merged blob's size must be roughly twice it's original component's size

            if (sizeDifference < AdvancedOptions._dMergingTolerance && sizeDifference > 1)
            {
                possibility++;
            }

            int[] bb1 = cluster1.getBoundingBox();
            int[] bb2 = cluster2.getBoundingBox();
            int[] bb3 = blob.getBoundingBox();

            // Second feature : bounding boxes intersect

            bool intersection = false;

            if (Math.Abs(bb1[0] - bb2[0]) < AdvancedOptions._nMergingDistance || Math.Abs(bb1[0] - bb2[2]) < AdvancedOptions._nMergingDistance || Math.Abs(bb1[2] - bb2[0]) < AdvancedOptions._nMergingDistance
                || Math.Abs(bb1[2] - bb2[2]) < AdvancedOptions._nMergingDistance) intersection = true;

            if (Math.Abs(bb1[1] - bb2[1]) < AdvancedOptions._nMergingDistance || Math.Abs(bb1[1] - bb2[3]) < AdvancedOptions._nMergingDistance || Math.Abs(bb1[3] - bb2[1]) < AdvancedOptions._nMergingDistance
                || Math.Abs(bb1[3] - bb2[3]) < AdvancedOptions._nMergingDistance) intersection = true;

            if (intersection) possibility++;

            int[] center1 = new int[2], center2 = new int[2], newCenter = new int[2];

            center1[0] = (bb1[2] - bb1[0]) / 2 + bb1[0];
            center1[1] = (bb1[3] - bb1[1]) / 2 + bb1[1];

            center2[0] = (bb2[2] - bb2[0]) / 2 + bb2[0];
            center2[1] = (bb2[3] - bb2[1]) / 2 + bb2[1];

            newCenter[0] = (bb3[2] - bb3[0]) / 2 + bb3[0];
            newCenter[1] = (bb3[3] - bb3[1]) / 2 + bb3[1];

            int[] predictedCenter = new int[2];
            predictedCenter[0] = (Math.Max(center1[0], center2[0]) - Math.Min(center1[0], center2[0])) / 2 + Math.Min(center1[0], center2[0]);
            predictedCenter[1] = (Math.Max(center1[1], center2[1]) - Math.Min(center1[1], center2[1])) / 2 + Math.Min(center1[1], center2[1]);

            if (Math.Abs(predictedCenter[0] - newCenter[0]) / Math.Sqrt(blob.getSize()) < AdvancedOptions._dMergingTolerance && Math.Abs(predictedCenter[1] - newCenter[1]) /
                Math.Sqrt(blob.getSize()) < AdvancedOptions._dMergingTolerance) possibility++;

            if (possibility > 1) return true;
            else return false;

        }

        private int blobCompare(List<Cluster> clusterList, Cluster c)
        {
            for(int i = 0; i < clusterList.Count; i++)
            {
                bool match = blobCompare(clusterList.ElementAt(i), c);

                if (match) return i;
            }

            return -1;
        }

        private bool blobCompare(Cluster c1, Cluster c2)
        {
            int[] center1 = new int[2], center2 = new int[2];
            int X0Depl, X1Depl, Y0Depl, Y1Depl;
            double boundsDepl, centerDepl;

            int[] bb1 = c1.getBoundingBox();
            int[] bb2 = c2.getBoundingBox();

            center1[0] = (bb1[2] - bb1[0]) / 2 + bb1[0];
            center1[1] = (bb1[3] - bb1[1]) / 2 + bb1[1];
        
            center2[0] = (bb2[2] - bb2[0]) / 2 + bb2[0];
            center2[1] = (bb2[3] - bb2[1]) / 2 + bb2[1];

            // The deplacement of the two centers between each other is normalized in terms of the blob's size

            centerDepl = (Math.Abs(center1[0] - center2[0]) + Math.Abs(center1[1] - center2[1]))/2 /c1.getSize();

            X0Depl = (bb2[0] - bb1[0]);
            X1Depl = (bb2[2] - bb1[2]);
            Y0Depl = (bb2[1] - bb1[1]);
            Y1Depl = (bb2[3] - bb1[3]);

            boundsDepl = (X0Depl + X1Depl + Y0Depl + Y1Depl) / c1.getSize();

            // Abnormal deplacement

            if (boundsDepl < AdvancedOptions._dBoundsDiminish || centerDepl - boundsDepl > AdvancedOptions._dGreatDeplacement) return false;

            // Abnormal growth

            if (boundsDepl > AdvancedOptions._dAbnormalGrowth && centerDepl > AdvancedOptions._dGreatDeplacement) return false;

            return true; 
        }

        // Frute force best match between two given images taking into consideration their edges, found via a Robinson mask filter

        private int[] RobinsonRepositioning(Bitmap bitmap1, Bitmap bitmap2)
        {
            int[,] edges1 = findRobinsonEdges(bitmap1);
            int[,] edges2 = findRobinsonEdges(bitmap2);

            int[] positions = findRobinsonReposition(edges1, edges2);

            return positions;
        }

        // Given certain edges of two imags, brute forthe thought them to obtain minimal ECM

        private int[] findRobinsonReposition(int[,] e1, int[,] e2)
        {
            // set range of values to be tested

            int deplX, deplY, stepX, stepY, height, width;

            // static values based on Sprout's usual deplacements

            int min = AdvancedOptions._nMinReposition, max = AdvancedOptions._nMaxReposition;

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

        private int[,] findRobinsonEdges(Bitmap bmp)
        {
            /*  Variation of Robinson mask for edges calculation used:
                    
                  0 -1  0
                 -1  2  0
                  0  0  0
            */

            // edges in fig

            Console.WriteLine(bmp.Width + " " + bmp.Height);

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

        // Difference computation via Manhattan distance

        private int[,] getManhattan(Bitmap bmp1, Bitmap bmp2, int[] positions)
        {
            int x0 = Math.Max(0, positions[0]);
            int x1 = Math.Min(bmp1.Width, bmp2.Width + positions[0]);
            int y0 = Math.Max(0, positions[1]);
            int y1 = Math.Min(bmp1.Height, bmp2.Height + positions[1]);

            int newWidth = x1 - x0, newHeight = y1 - y0;

            Bitmap bmp = new Bitmap(newWidth, newHeight);
            int[,] arrSource = new int[newWidth, newHeight];

            for (int x = x0; x < x1; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x, y);

                    System.Drawing.Color pixel2; 

                    if (((x + positions[0]) > 0) && ((y + positions[1]) > 0) && ((x + positions[0]) < bmp2.Width) && ((y + positions[1]) < bmp2.Height)) { pixel2 = bmp2.GetPixel(x + positions[0], y + positions[1]); }
                    else { pixel2 = System.Drawing.Color.FromArgb(255, 255, 255); }

                    int colorValue = (Math.Abs(pixel1.R - pixel2.R) + Math.Abs(pixel1.G - pixel2.G) + Math.Abs(pixel1.B - pixel2.B)) / 3;

                    System.Drawing.Color pixel = getGreyThreshold(colorValue);

                    bmp.SetPixel(x - x0, y - y0, pixel);

                    arrSource[x - x0, y - y0] = colorValue;
                }
            }

            return arrSource;
        }

        // Difference computation via Euclidean distance

        private Bitmap getEuclidean(Bitmap bmp1, Bitmap bmp2, int[] positions)
        {
            int x0 = Math.Max(0, positions[0]);
            int x1 = Math.Min(bmp1.Width, bmp2.Width + positions[0]);
            int y0 = Math.Max(0, positions[1]);
            int y1 = Math.Min(bmp1.Height, bmp2.Height + positions[1]);

            int newWidth = x1 - x0, newHeight = y1 - y0;

            Bitmap bmp = new Bitmap(newWidth, newHeight);

            for (int x = x0; x < x1; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    System.Drawing.Color pixel1 = bmp1.GetPixel(x, y);

                    System.Drawing.Color pixel2 = bmp2.GetPixel(x + positions[0], y + positions[1]); 

                    int colorValue = (int)Math.Round(Math.Sqrt(Math.Pow((pixel1.R - pixel2.R), 2) + Math.Pow((pixel1.G - pixel2.G),2) + Math.Pow((pixel1.B - pixel2.B),2))/3);

                    System.Drawing.Color pixel = getGreyThreshold(colorValue);

                    bmp.SetPixel(x, y, pixel);
                }
            }

            return bmp;
        }

        // Difference computation using the pearson coefficient parameter

        private Bitmap getGlobalCorrelation(Bitmap bmp1, Bitmap bmp2, int[] positions)
        {

            int[] accX = new int[3], accY = new int[3];
            long[] accXX = new long[3], accYY = new long[3], accXY = new long[3];
            double[] avgX = new double[3], avgY = new double[3], varX = new double[3], varY = new double[3], covXY = new double[3], corrIndex = new double[3];

            int x0 = Math.Max(0, positions[0]);
            int x1 = Math.Min(bmp1.Width, bmp2.Width + positions[0]);
            int y0 = Math.Max(0, positions[1]);
            int y1 = Math.Min(bmp1.Height, bmp2.Height + positions[1]);

            double epsilon = AdvancedOptions._dEpsilonValue;

            for(int i=0; i<3; i++)
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

            int n = (x1- x0)  * (y1 - y0);

            avgX[0] = accX[0] / n;
            avgX[1] = accX[1] / n;
            avgX[2] = accX[2] / n;

            avgY[0] = accY[0] / n;
            avgY[1] = accY[1] / n;
            avgY[2] = accY[2] / n;

            varX[0] = (accXX[0]/n) - Math.Pow(avgX[0], 2);
            varX[1] = (accXX[1]/n) - Math.Pow(avgX[1], 2);
            varX[2] = (accXX[2]/n) - Math.Pow(avgX[2], 2);

            varY[0] = (accYY[0]/n) - Math.Pow(avgY[0], 2);
            varY[1] = (accYY[1]/n) - Math.Pow(avgY[1], 2);
            varY[2] = (accYY[2]/n) - Math.Pow(avgY[2], 2);

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

        // Decide wether a pixel's difference is enough to be relevant or not via a fixed threshold and return absolute value

        private System.Drawing.Color getGreyThreshold(int colorValue)
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

        private System.Drawing.Color getThreshold(int colorValue)
        {
            int threshold = AdvancedOptions._nThresholdValue;

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

        // Decide wether a pixel's difference is enough to be relevant or not via a hysteresis cycle

        private System.Drawing.Color getHysteresis(int colorValue)
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

        // Get Images' size difference

        private int[] getDeplacement(Bitmap bmp1, Bitmap bmp2)
        {
            int[] d = new int[2];

            d[0] = Math.Abs((bmp1.Width-bmp2.Width));
            d[1] = Math.Abs((bmp1.Height - bmp2.Height));

            return d;
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
                    else if (y >= bestPositions[1] + bmp1.Height || x >= bestPositions[0] + bmp1.Width) { pixelArray[x, y] = System.Drawing.Color.FromArgb(255, 255, 255);}
                    else { pixelArray[x, y] = bmp1.GetPixel(x - bestPositions[0], y - bestPositions[1]);}

                    b.SetPixel(x, y, pixelArray[x, y]);
                }
            }

            return b;
        }

    }
}