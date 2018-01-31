using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class ROI
    {
        private int x0, x1, y0, y1;

        public ROI(int x_0, int x_1, int y_0, int y_1)
        {
            x0 = x_0;
            x1 = x_1;
            y0 = y_0;
            y1 = y_1;
        }

        public ROI(int margin, int[,] source)
        {
            x0 = margin;
            x1 = source.GetLength(0) - margin;
            y0 = margin;
            y1 = source.GetLength(1) - margin;
        }

        public int getX0()
        {
            return x0;
        }
        public int getX1()
        {
            return x1;
        }
        public int getY0()
        {
            return y0;
        }
        public int getY1()
        {
            return y1;
        }

    }
}
