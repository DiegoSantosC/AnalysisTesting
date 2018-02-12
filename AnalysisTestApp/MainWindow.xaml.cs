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
        public MainWindow()
        {

            InitializeComponent();

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

            images[0] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C1.png"));
            images[1] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C2.png"));
            images[2] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\MergingTest.png"));
            images[3] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C4.png"));
            images[4] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C5.png"));
            images[5] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C6.png"));
            images[6] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C7.png"));
            images[7] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C8.png"));
            images[8] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C9.png"));
            images[9] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C10.png"));
            images[10] = new Bitmap(System.Drawing.Image.FromFile(@"Resources\Captures\C11.png"));

            Tracking t = new Tracking();

            for (int i = 1; i< 11; i++)
            {
                // Repositioning finding and executing difference between the two images

                int[] positions = Matching_Robinson.RobinsonRepositioning(images[0], images[i]);

                int[,] resultDifference = DifferenceComputation.getManhattan(images[0], images[i], positions);

                List<Cluster> frameBlobs = FindObjects(resultDifference);

                Console.WriteLine("Tracking " + i + ": " + frameBlobs.Count + " blobs encountered");

                if (t.isEmpty()) t.firstScan(frameBlobs, i);
                else
                {
                    t.assignBlobs(frameBlobs, i);
                }

                Console.WriteLine();
            }

            List<List<Cluster>> results = t.getTracking();

            // Tracking results siso

            Console.WriteLine();
            Console.WriteLine(results.Count + " different blobs being tracked");

            for(int i=0; i<results.Count; i++)
            {
                double[] data = new double[results.ElementAt(i).Count];

                string dir = (System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CharTest", "ClusterN" + i));
                Directory.CreateDirectory(dir);

                string p = "ClusterTrack_" + i.ToString() + ".txt";

                FileStream f = File.Create(System.IO.Path.Combine(dir, p));

                f.Close();
                File.AppendAllText(System.IO.Path.Combine(dir, p), "Blob " + i + ": " + results.ElementAt(i).Count + " steps tracked" + Environment.NewLine);
                File.AppendAllText(System.IO.Path.Combine(dir, p), Environment.NewLine);

                for (int j = 0; j < results.ElementAt(i).Count; j++)
                {
                    data[j] = results.ElementAt(i).ElementAt(j).getSize();

                    File.AppendAllText(System.IO.Path.Combine(dir, p), "Track " + j + Environment.NewLine);
                    File.AppendAllText(System.IO.Path.Combine(dir, p), "    ID = " + results.ElementAt(i).ElementAt(j).getId() + Environment.NewLine);
                    File.AppendAllText(System.IO.Path.Combine(dir, p), "    Size = " + results.ElementAt(i).ElementAt(j).getSize() + Environment.NewLine);
                    File.AppendAllText(System.IO.Path.Combine(dir, p), "    Encounter step = " + results.ElementAt(i).ElementAt(j).getStep() + Environment.NewLine);
                    File.AppendAllText(System.IO.Path.Combine(dir, p), Environment.NewLine);
                }

                ChartHandler ch = new ChartHandler();
                ch.barCharConstruct(data, i, dir);   
            }

            this.Hide();                  

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
                        bmp.SetPixel(x, y, DifferenceComputation.getGrey(255 - marked[x, y]));
                    }
                }
            }

            bmp.Save(@"Resources\Result.bmp");
        }
    }
}