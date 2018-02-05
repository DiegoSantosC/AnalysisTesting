using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class Cluster
    {
        private int id, count, countColor, minX, minY, maxX, maxY, countColorSquare;

        public Cluster(int given_ID)
        {
            id = given_ID;
            count = 0;
            countColor = 0;
            countColorSquare = 0;
            minX = Int32.MaxValue;
            minY = Int32.MaxValue;
            maxX = 0;
            maxY = 0;
        }

        public int getId()
        {
            return id;
        }

        public void addPoint(int x, int y, int colorValue)
        {
            count++;
            countColor += colorValue;

            if (minX > x) minX = x;
            else if (maxX < x) maxX = x;

            if (minY > y) minY = y;
            else if (maxY < y) maxY = y;

            countColorSquare += colorValue * colorValue;
        }

        public int[] getBoundingBox()
        {
            int[] returnable = new int[]{ minX, minY, maxX, maxY};

            return returnable;
        }

        public int getColorAvg()
        {
            return (int)countColor / count;
        }

        public int getSize()
        {
            return count;
        }

        public int getColorVar()
        {
            return (int)(countColorSquare / count - Math.Pow(getColorAvg(), 2));
        }
    }
}
