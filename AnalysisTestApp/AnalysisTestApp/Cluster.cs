using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class Cluster
    {
        private int id, count, countY, countX, countColor, minX, minY, maxX, maxY, step, sumR, sumG, sumB;
        private double countColorSquare;
        private string timelapse;
        private int mergedToBranch;
        private List<List<Cluster>> mergeBranches;
        private bool differentMerging;

        public Cluster(int given_ID, string time)
        {
            id = given_ID;
            count = 0;
            countColor = 0;
            countColorSquare = 0;
            minX = Int32.MaxValue;
            minY = Int32.MaxValue;
            maxX = 0;
            maxY = 0;
            countY = 0;
            countX = 0;
            timelapse = time;
            mergedToBranch = -1;
            mergeBranches = new List<List<Cluster>>();
            step = -1;
            sumR = 0;
            sumG = 0;
            sumB = 0;

            // Boolean storing wether if the colony has merged with other one of a different type

            differentMerging = false;

        }

        public int getId()
        {
            return id;
        }

        public int getStep()
        {
            return step;
        }

        public bool hasMerged()
        {
            if (mergedToBranch == -1) return true;

            else return false;
        }

        public int getFather()
        {
            return mergedToBranch;
        }

        public void setFatherBranch(int f)
        {
            mergedToBranch = f;
        }

        public int[] getCenter()
        {
            int[] c = new int[] {countX/count, countY/count};

            return c;
        }

        // Step refers to the tracking phase in which the cluster has been encountered

        public void setStep(int s)
        {
            step = s;
        }

        public void addBranch(List<Cluster> c)
        {
            mergeBranches.Add(c);
        }

        public List<List<Cluster>> getBranches()
        {
            return mergeBranches;
        }

        public void addPoint(int x, int y, int colorValue, Color RGB)
        {
            count++;
            countColor += colorValue;

            if (minX > x) minX = x;
            else if (maxX < x) maxX = x;

            if (minY > y) minY = y;
            else if (maxY < y) maxY = y;

            countColorSquare += colorValue * colorValue;
            countX += x;
            countY += y;

            sumR += RGB.R;
            sumG += RGB.G;
            sumB += RGB.B;
        }

        public void setIndex(int index)
        {
            id = index;
        }

        public int getAvgR()
        {
            return sumR / count;
        }

        public int getAvgG()
        {
            return sumG / count;
        }

        public int getAvgB()
        {
            return sumB / count;
        }

        public int[] getBoundingBox()
        {
            int[] returnable = new int[] { minX, minY, maxX, maxY };

            return returnable;
        }

        public double getColorAvg()
        {
            return (int)countColor / count;
        }

        public int getSize()
        {
            return count;
        }

        public double getColorVar()
        {
            return (double)(countColorSquare / count - Math.Pow(getColorAvg(), 2));
        }

        public double avgX()
        {
            return countX / count;
        }

        public double avgY()
        {
            return countY / count;
        }

        public void mergedWithDifferent()
        {
            differentMerging = true;
        }
        public bool hasMergedWithDifferent()
        {
            return differentMerging;
        }
    }
}