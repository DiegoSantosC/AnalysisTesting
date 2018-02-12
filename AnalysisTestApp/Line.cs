using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class Line
    {
        private double _dAngle, _dOffset, _dSin, _dCos;

        public Line()
        {
            _dAngle = 0;
            _dOffset = 0;
            _dSin = 0;
            _dCos = 0;
        }

        public Line(double dX, double dY, double dAngle)
        {
            _dAngle = dAngle;
            _dSin = Math.Sin(_dAngle);
            _dCos = Math.Cos(_dAngle);

            _dOffset = dX * _dSin - dY * _dCos;
        }

        public double getAngle()
        {
            return _dAngle;
        }

        public double getOffset()
        {
            return _dOffset;
        }

        public double getSin()
        {
            return _dSin;
        }

        public double getCos()
        {
            return _dCos;
        }
    }
}
