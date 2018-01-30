//-------------------------------------------------------------------------------------------------
#include <math.h>
#include "General.h"
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline TEllipse::TEllipse ()
  : _dXc (0), _dYc (0), _dA (0), _dB (0), _dAngle (0), _dSin (0), _dCos (0)
{
}
//-------------------------------------------------------------------------------------------------

inline TEllipse::TEllipse (double dX, double dY, double dA, double dB, double dAngle)
{
  set (dX, dY, dA, dB, dAngle);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline void TEllipse::set (double dX, double dY, double dA, double dB, double dAngle)
{
  _dXc = dX;  _dYc = dY;  _dA = dA;  _dB = dB;  _dAngle = dAngle;
  _dSin = ::sin (_dAngle);  _dCos = ::cos (_dAngle);
}
//-------------------------------------------------------------------------------------------------

inline void TEllipse::get (double &dX, double &dY, double &dA, double &dB, double &dAngle) const
{
  dX = _dXc;  dY = _dYc;  dA = _dA;  dB = _dB;  dAngle = _dAngle;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline void TEllipse::getBoundingBox (double &dX0, double &dY0, double &dX1, double &dY1) const
{
  const double  dWidth2  = ::sqrt (square (_dA * _dCos) + square (_dB * _dSin));
  const double  dHeight2 = ::sqrt (square (_dA * _dSin) + square (_dB * _dCos));
  dX0 = _dXc - dWidth2;  dY0 = _dYc - dHeight2;  dX1 = _dXc + dWidth2;  dY1 = _dYc + dHeight2;
}
//-------------------------------------------------------------------------------------------------

inline double TEllipse::centerX () const
{
  return _dXc;
}
//-------------------------------------------------------------------------------------------------

inline void TEllipse::centerX (double dCenterX)
{
  _dXc = dCenterX;
}
//-------------------------------------------------------------------------------------------------

inline double TEllipse::centerY () const
{
  return _dYc;
}
//-------------------------------------------------------------------------------------------------

inline void TEllipse::centerY (double dCenterY)
{
  _dYc = dCenterY;
}
//-------------------------------------------------------------------------------------------------

inline double TEllipse::radiusHz () const
{
  return _dA;
}
//-------------------------------------------------------------------------------------------------

inline void TEllipse::radiusHz (double dRadiusHz)
{
  _dA = dRadiusHz;
}
//-------------------------------------------------------------------------------------------------

inline double TEllipse::radiusVt () const
{
  return _dB;
}
//-------------------------------------------------------------------------------------------------

inline void TEllipse::radiusVt (double dRadiusVt)
{
  _dB = dRadiusVt;
}
//-------------------------------------------------------------------------------------------------

inline double TEllipse::angle () const
{
  return _dAngle;
}
//-------------------------------------------------------------------------------------------------

inline void TEllipse::angle (double dAngle)
{
  _dAngle = dAngle;
  _dSin = ::sin (_dAngle);
  _dCos = ::cos (_dAngle);
}
//-------------------------------------------------------------------------------------------------

inline bool TEllipse::isInside (double dX, double dY) const
{
  return (distanceMahalanobis2 (dX, dY) <= 1);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline double TEllipse::distanceMahalanobis (double dX, double dY) const
{
  return ::sqrt (distanceMahalanobis2 (dX, dY));
}
//-------------------------------------------------------------------------------------------------

inline double TEllipse::distanceMahalanobis2 (double dX, double dY) const
{
  dX -= _dXc;  dY -= _dYc;
  return (square ((dX * _dCos - dY * _dSin) / _dA) + square ((dX * _dSin + dY * _dCos) / _dB));
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline double TEllipse::area () const
{
  return ANGLE__PI * _dA * _dB;
}
//-------------------------------------------------------------------------------------------------

inline double TEllipse::perimeter () const
{
  // The Ramanujan approximation
  return ANGLE__PI * (_dA + _dB) * (3 - ::sqrt (4 - square ((_dA - _dB) / (_dA + _dB))));
}
//-------------------------------------------------------------------------------------------------

inline double TEllipse::ratio () const
{
  return _dB / _dA;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline bool TEllipse::toImplicit (double &dAx2, double &dBxy, double &dCy2, double &dDx, double &dEy, double &dF) const
{
  // http://stackoverflow.com/questions/8201842/get-points-of-svg-xaml-arc
  dBxy = ::sin (2 * _dAngle) * (_dB * _dB - _dA * _dA);
  dAx2 = square (_dB * _dCos) + square (_dA * _dSin);
  dCy2 = square (_dA * _dCos) + square (_dB * _dSin);
  dDx = -(2 * dAx2 * _dXc + dBxy * _dYc);
  dEy = -(dBxy * _dXc + 2 * dCy2 * _dYc);
  dF = dCy2 * _dYc  * _dYc + dAx2 * _dXc * _dXc + dBxy * _dXc * _dYc - _dA * _dA * _dB * _dB;
  return true;
}
//-------------------------------------------------------------------------------------------------

inline bool TEllipse::fromImplicit (double dAx2, double dBxy, double dCy2, double dDx, double dEy, double dF)
{
  // "Some notes on ellipses", Ivo Ihrke
  // http://people.mmci.uni-saarland.de/~ihrke/software/ellipse.pdf
  dBxy /= 2;  dDx /= 2;  dEy /= 2;
  double  dDenom = dAx2 * dCy2 - dBxy * dBxy;
  bool  bRet = false;
  if (dDenom != 0)
  {
    const double  dXc = -(dCy2 * dDx - dBxy * dEy) / dDenom;
    const double  dYc = -(dAx2 * dEy - dBxy * dDx) / dDenom;
    const double  dAlpha = 0.5 * ::atan2 (2 * dBxy, dAx2 - dCy2);
    const double  dCos2 = square (::cos (dAlpha));
    const double  dSin2 = 1 - dCos2;
    dDenom = dCos2 - dSin2;
    if (dDenom != 0)
    {
      const double dCos = ::cos (dAlpha);
      const double dSin = ::sin (dAlpha);
      const double dS33 = dXc * (dXc * dAx2 + dYc * dBxy + dDx) + dYc * (dXc * dBxy + dYc * dCy2 + dEy) + dXc * dDx + dYc * dEy + dF;
      const double dA = ::sqrt (abs (dS33 / (dCos2 * dAx2 + dCos * dSin * dBxy + dCos * dSin * dBxy + dSin2 * dCy2)));
      const double dB = ::sqrt (abs (dS33 / (dSin2 * dAx2 - dSin * dCos * dBxy - dCos * dSin * dBxy + dCos2 * dCy2)));
      if (dA >= dB)
        set (dXc, dYc, dA, dB, dAlpha);
      else
        set (dXc, dYc, dB, dA, dAlpha - ANGLE__PI_OVER_2);
      bRet = true;
    }
  }
  return bRet;
}
//-------------------------------------------------------------------------------------------------

inline bool TEllipse::from5points (double dAx, double dAy, double dBx, double dBy, double dCx, double dCy, double dDx, double dDy, double dEx, double dEy)
{
  // Code to find the equation of a conic 
  // Tom Davis. April 12, 1996
  // http://mathforum.org/library/drmath/view/51735.html
  const double  p0 [3] = { dAx, dAy, 1 };
  const double  p1 [3] = { dBx, dBy, 1 };
  const double  p2 [3] = { dCx, dCy, 1 };
  const double  p3 [3] = { dDx, dDy, 1 };
  const double  p4 [3] = { dEx, dEy, 1 };
  const double  L0 [3] = { p0 [1] * p1 [2] - p0 [2] * p1 [1], p0 [2] * p1 [0] - p0 [0] * p1 [2], p0 [0] * p1 [1] - p0 [1] * p1 [0] };
  const double  L1 [3] = { p1 [1] * p2 [2] - p1 [2] * p2 [1], p1 [2] * p2 [0] - p1 [0] * p2 [2], p1 [0] * p2 [1] - p1 [1] * p2 [0] };
  const double  L2 [3] = { p2 [1] * p3 [2] - p2 [2] * p3 [1], p2 [2] * p3 [0] - p2 [0] * p3 [2], p2 [0] * p3 [1] - p2 [1] * p3 [0] };
  const double  L3 [3] = { p3 [1] * p4 [2] - p3 [2] * p4 [1], p3 [2] * p4 [0] - p3 [0] * p4 [2], p3 [0] * p4 [1] - p3 [1] * p4 [0] };
  const double  Q  [3] = { L0 [1] * L3 [2] - L0 [2] * L3 [1], L0 [2] * L3 [0] - L0 [0] * L3 [2], L0 [0] * L3 [1] - L0 [1] * L3 [0] };
  const double  y4w0 = p4 [1] * p0 [2];
  const double  w4y0 = p4 [2] * p0 [1];
  const double  w4w0 = p4 [2] * p0 [2];
  const double  y4y0 = p4 [1] * p0 [1];
  const double  x4w0 = p4 [0] * p0 [2];
  const double  w4x0 = p4 [2] * p0 [0];
  const double  x4x0 = p4 [0] * p0 [0];
  const double  y4x0 = p4 [1] * p0 [0];
  const double  x4y0 = p4 [0] * p0 [1];
  const double  a1a2 = L1 [0] * L2 [0];
  const double  a1b2 = L1 [0] * L2 [1];
  const double  a1c2 = L1 [0] * L2 [2];
  const double  b1a2 = L1 [1] * L2 [0];
  const double  b1b2 = L1 [1] * L2 [1];
  const double  b1c2 = L1 [1] * L2 [2];
  const double  c1a2 = L1 [2] * L2 [0];
  const double  c1b2 = L1 [2] * L2 [1];
  const double  c1c2 = L1 [2] * L2 [2];

  double  aa = -Q [0] * a1a2 * y4w0 + Q [0] * a1a2 * w4y0 - Q [1] * b1a2 * y4w0 - Q [1] * c1a2 * w4w0 + Q [1] * a1b2 * w4y0 + Q [1] * a1c2 * w4w0 + Q [2] * b1a2 * y4y0 + Q [2] * c1a2 * w4y0 - Q [2] * a1b2 * y4y0 - Q [2] * a1c2 * y4w0;
  double  cc =  Q [0] * c1b2 * w4w0 + Q [0] * a1b2 * x4w0 - Q [0] * b1c2 * w4w0 - Q [0] * b1a2 * w4x0 + Q [1] * b1b2 * x4w0 - Q [1] * b1b2 * w4x0 + Q [2] * b1c2 * x4w0 + Q [2] * b1a2 * x4x0 - Q [2] * c1b2 * w4x0 - Q [2] * a1b2 * x4x0;
  double  ff =  Q [0] * c1a2 * y4x0 + Q [0] * c1b2 * y4y0 - Q [0] * a1c2 * x4y0 - Q [0] * b1c2 * y4y0 - Q [1] * c1a2 * x4x0 - Q [1] * c1b2 * x4y0 + Q [1] * a1c2 * x4x0 + Q [1] * b1c2 * y4x0 - Q [2] * c1c2 * x4y0 + Q [2] * c1c2 * y4x0;
  double  bb =  Q [0] * c1a2 * w4w0 + Q [0] * a1a2 * x4w0 - Q [0] * a1b2 * y4w0 - Q [0] * a1c2 * w4w0 - Q [0] * a1a2 * w4x0 + Q [0] * b1a2 * w4y0 + Q [1] * b1a2 * x4w0 - Q [1] * b1b2 * y4w0 - Q [1] * c1b2 * w4w0 - Q [1] * a1b2 * w4x0 
               +Q [1] * b1b2 * w4y0 + Q [1] * b1c2 * w4w0 - Q [2] * b1c2 * y4w0 - Q [2] * b1a2 * x4y0 - Q [2] * b1a2 * y4x0 - Q [2] * c1a2 * w4x0 + Q [2] * c1b2 * w4y0 + Q [2] * a1b2 * x4y0 + Q [2] * a1b2 * y4x0 + Q [2] * a1c2 * x4w0;
  double  dd = -Q [0] * c1a2 * y4w0 + Q [0] * a1a2 * y4x0 + Q [0] * a1b2 * y4y0 + Q [0] * a1c2 * w4y0 - Q [0] * a1a2 * x4y0 - Q [0] * b1a2 * y4y0 + Q [1] * b1a2 * y4x0 + Q [1] * c1a2 * w4x0 + Q [1] * c1a2 * x4w0 + Q [1] * c1b2 * w4y0 
               -Q [1] * a1b2 * x4y0 - Q [1] * a1c2 * w4x0 - Q [1] * a1c2 * x4w0 - Q [1] * b1c2 * y4w0 + Q [2] * b1c2 * y4y0 + Q [2] * c1c2 * w4y0 - Q [2] * c1a2 * x4y0 - Q [2] * c1b2 * y4y0 - Q [2] * c1c2 * y4w0 + Q [2] * a1c2 * y4x0;
  double  ee = -Q [0] * c1a2 * w4x0 - Q [0] * c1b2 * y4w0 - Q [0] * c1b2 * w4y0 - Q [0] * a1b2 * x4y0 + Q [0] * a1c2 * x4w0 + Q [0] * b1c2 * y4w0 + Q [0] * b1c2 * w4y0 + Q [0] * b1a2 * y4x0 - Q [1] * b1a2 * x4x0 - Q [1] * b1b2 * x4y0
               +Q [1] * c1b2 * x4w0 + Q [1] * a1b2 * x4x0 + Q [1] * b1b2 * y4x0 - Q [1] * b1c2 * w4x0 - Q [2] * b1c2 * x4y0 + Q [2] * c1c2 * x4w0 + Q [2] * c1a2 * x4x0 + Q [2] * c1b2 * y4x0 - Q [2] * c1c2 * w4x0 - Q [2] * a1c2 * x4x0;
  if (aa != 0)
    { bb /= aa;  cc /= aa;  dd /= aa;  ee /= aa;  ff /= aa;  aa = 1; }
  else if (bb != 0)
    { cc /= bb;  dd /= bb;  ee /= bb;  ff /= bb;  bb = 1; }
  else if (cc != 0)
    { dd /= cc;  ee /= cc;  ff /= cc;  cc = 1; }
  else if (dd != 0.0)
    { ee /= dd;  ff /= dd;  dd = 1; }
  else if (ee != 0.0)
    { ff /= ee;  ee = 1; }
  else return false;

  return fromImplicit (aa, bb, cc, dd, ee, ff);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline void TEllipse::toLocalCoordinates (double dX, double dY, double &dU, double &dV) const
{
  dU = dX - _dXc;  dV = dY - _dYc;

  dX = dU * _dCos - dV * _dSin;
  dY = dU * _dSin + dV * _dCos;

  dU = ::atan2 (dY, dX);
  dV = ::sqrt ((SQUARE (dX) + SQUARE (dY)) / (square (_dA * ::cos (dU)) + square (_dB * ::sin (dU))));
}
//-------------------------------------------------------------------------------------------------

inline void TEllipse::fromLocalCoordinates (double dU, double dV, double &dX, double &dY) const
{
  const double  dCos = ::cos (dU);
  const double  dSin = ::sin (dU);
  dV *= ::sqrt (square (_dA * dCos) + square (_dB * dSin));
  dU = dV * dCos;  dV = dV * dSin;

  dX =  dU * _dCos + dV * _dSin;
  dY = -dU * _dSin + dV * _dCos;

  dX += _dXc;  dY += _dYc;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline bool isInside (const TEllipse &e, unsigned int nX, unsigned int nY)
{
  return e.isInside (nX, nY);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
