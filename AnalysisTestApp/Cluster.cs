using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class Cluster
    {
        private int id, count, countY, countX, countColor, minX, minY, maxX, maxY;
        private double countYY, countXX, countXY, countColorSquare;
        private string timelapse;
        private bool mergedBranch;
        private List<List<Cluster>> mergeBranches;

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
            countXX = 0;
            countYY = 0;
            countXY = 0;
            timelapse = time;
            mergedBranch = false;
            mergeBranches = new List<List<Cluster>>();

        }

        public int getId()
        {
            return id;
        }

        public void hasMerged()
        {
            mergedBranch = true;
        }

        public void addBranch(List<Cluster> c)
        {
            mergeBranches.Add(c);
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
            countX += x;
            countY += y;
            countXX += x * x;
            countYY += y * y;
            countXY += x * y;
        }

        public void setIndex(int index)
        {
            id = index;
        }

        public int[] getBoundingBox()
        {
            int[] returnable = new int[]{ minX, minY, maxX, maxY};

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

        public double getXVar()
        {
            return countXX / count - avgX() * avgX();
        }

        public double getYVar()
        {
            return countYY / count - avgY() * avgY();
        }

        public double avgX()
        {
            return countX / count;
        }

        public double avgY()
        {
            return countY / count;
        }

        public double covXY()
        {
            return countXY / count - avgX() * avgY();
        }

        public double pearson()
        {
            const double Epsilon = 1.0e-9;

            double dFactor = Math.Max(getXVar() * getYVar(), Epsilon);

            return covXY() / Math.Sqrt(dFactor);
        }

        public Line getLine_ev()
        {
            double dEigenValue = (getXVar() + getYVar()) - Math.Sqrt(Math.Pow((getYVar() + getXVar()), 2)) - 4 * (getXVar() * getYVar() - covXY() * covXY()) / 2;    
            return new Line(avgX() , avgY(), Math.Atan2(-covXY() , dEigenValue - getXVar()));
        }
    }
}
