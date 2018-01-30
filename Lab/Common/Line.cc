//-------------------------------------------------------------------------------------------------
#include <math.h>
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline TLine::TLine ()
  : _dAngle (0), _dOffset (0), _dSin (0), _dCos (0)
{
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline TLine::TLine (double dAngle, double dOffset)
{
  set (dAngle, dOffset);
}
//-------------------------------------------------------------------------------------------------

inline TLine::TLine (double dX, double dY, double dAngle)
{
  set (dX, dY, dAngle);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline void TLine::set (double dAngle, double dOffset)
{
  _dAngle = dAngle;
  _dSin = ::sin (_dAngle);  _dCos = ::cos (_dAngle);
  _dOffset = dOffset;
}
//-------------------------------------------------------------------------------------------------

inline void TLine::set (double dX, double dY, double dAngle)
{
  _dAngle = dAngle;
  _dSin = ::sin (_dAngle);  _dCos = ::cos (_dAngle);
  _dOffset = dX * _dSin - dY * _dCos;
}
//-------------------------------------------------------------------------------------------------

inline void TLine::get (double &dAngle, double &dOffset) const
{
  dAngle = _dAngle;  dOffset = _dOffset;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline double TLine::angle () const
{
  return _dAngle;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline double TLine::getXat (double dY) const
{
  return (dY * _dCos + _dOffset) / _dSin;
}
//-------------------------------------------------------------------------------------------------

inline double TLine::getYat (double dX) const
{
  return (dX * _dSin - _dOffset) / _dCos;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline bool TLine::intersect (const TLine &l, double *pX, double *pY) const
{
  static const double  __dEpsilon = 1.0e-6;
  double  dAux = _dCos * l._dSin - _dSin * l._dCos;  // = ::sin (l._dAngle - _dAngle);
  const bool  bRet = (::abs (dAux) > __dEpsilon);
  if (bRet && (pX || pY))
  {
    dAux = (_dSin * l._dOffset - l._dSin * _dOffset) / dAux;
    if (pX) *pX = (::abs (_dSin) > ::abs (l._dSin))? ((dAux * _dCos + _dOffset) / _dSin) : ((dAux * l._dCos + l._dOffset) / l._dSin);
    if (pY) *pY = dAux;
  }
  return bRet;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline double TLine::distanceEuclidean (double dX, double dY) const
{
  return dX * _dSin - dY * _dCos - _dOffset;
}
//-------------------------------------------------------------------------------------------------

inline double TLine::distanceEuclidean2 (double dX, double dY) const
{
  return square (distanceEuclidean (dX, dY));
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline bool TLine::toImplicit (double &dAx, double &dBy, double &dC) const
{
  dAx = _dSin;  dBy = -_dCos;  dC = -_dOffset;
}
//-------------------------------------------------------------------------------------------------

inline bool TLine::fromImplicit (double dAx, double dBy, double dC)
{
  _dAngle = ::atan2 (dAx, -dBy);
  _dSin = ::sin (_dAngle);  _dCos = ::cos (_dAngle);
  _dOffset = (::abs (dAx) >= ::abs (dBy))? -(dC / dAx) * _dSin : (dC / dBy) * _dCos;
  return true;
}
//-------------------------------------------------------------------------------------------------

inline bool TLine::from2points (double dAx, double dAy, double dBx, double dBy)
{
  _dAngle = ::atan2 (dBy - dAy, dBx - dAx);
  _dSin = ::sin (_dAngle);  _dCos = ::cos (_dAngle);
  _dOffset = dAx * _dSin - dAy * _dCos;
  return true;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
