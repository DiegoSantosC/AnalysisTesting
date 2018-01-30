//-------------------------------------------------------------------------------------------------
#include <math.h>
#include "General.h"
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
inline TCluster2D<TData>::TCluster2D ()
{
  reset ();
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
inline void TCluster2D<TData>::reset ()
{
  _nCount = 0;
  _dtSumX = _dtSumY = 0;
  _dtSumXX = _dtSumXY = _dtSumYY = 0;
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline void TCluster2D<TData>::set (unsigned int nCount, double dAvgX, double dAvgY)
{
  _nCount = nCount;
  _dtSumX = (TData) (nCount * dAvgX);
  _dtSumY = (TData) (nCount * dAvgY);
  _dtSumXX = _dtSumXY = _dtSumYY = 0;
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline void TCluster2D<TData>::addPoint (const TData &dtX, const TData &dtY)
{
  ++_nCount;
  _dtSumX += dtX;  _dtSumY += dtY;
  _dtSumXX += dtX * dtX;  _dtSumXY += dtX * dtY;  _dtSumYY += dtY * dtY;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
inline unsigned int TCluster2D<TData>::count () const
{
  return _nCount;
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline double TCluster2D<TData>::avgX () const
{
  return ((double) _dtSumX) / ((double) _nCount);
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline double TCluster2D<TData>::avgY () const
{
  return ((double) _dtSumY) / ((double) _nCount);
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline double TCluster2D<TData>::varX () const
{
  return ((double) _dtSumXX) / ((double) _nCount) - square (avgX ());
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline double TCluster2D<TData>::covXY () const
{
  return ((double) _dtSumXY) / ((double) _nCount) - avgX () * avgY ();
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline double TCluster2D<TData>::varY () const
{
  return ((double) _dtSumYY) / ((double) _nCount) - square (avgY ());
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
inline double TCluster2D<TData>::pearson () const
{
  static const double  __dEpsilon = 1.0e-12;
  const double  dFactor = max (varX () * varY (), __dEpsilon);
  return covXY () / ::sqrt (dFactor);
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline double TCluster2D<TData>::pearson2 () const
{
  static const double  __dEpsilon = 1.0e-12;
  const double  dFactor = max (varX () * varY (), __dEpsilon);
  return square (covXY ()) / dFactor;
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline TLine TCluster2D<TData>::line_lsq (double *pMSE) const
{
  const double  dVarX  = varX ();
  const double  dCovXY = covXY ();
  const double  dVarY  = varY ();
  const double  dAngle = ::atan2 (2 * dCovXY, dVarX - dVarY) / 2;
  if (pMSE)
  {
    const double  dSin = ::sin (dAngle);
    const double  dCos = ::cos (dAngle);
    *pMSE = dVarX * dSin * dSin + dVarY * dCos * dCos - 2 * dCovXY * dSin * dCos;
  }
  return TLine (avgX (), avgY (), dAngle);
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline TLine TCluster2D<TData>::line_ev () const
{
  const double  dVarX  = varX ();
  const double  dCovXY = covXY ();
  const double  dVarY  = varY ();
  const double  dEigenValue = ((dVarX + dVarY) - ::sqrt (square (dVarX + dVarY) - 4 * (dVarX * dVarY - dCovXY * dCovXY))) / 2;
  return TLine (avgX (), avgY (), ::atan2 (-dCovXY, dEigenValue - dVarX));
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline TEllipse TCluster2D<TData>::ellipse (double dChevichev) const
{
  const double  dVarX  = varX ();
  const double  dCovXY = covXY ();
  const double  dVarY  = varY ();
  const double  dAux = ::sqrt (square (dVarX + dVarY) - 4 * (dVarX * dVarY - square (dCovXY)));
  return TEllipse (avgX (), avgY (), dChevichev * ::sqrt (((dVarX + dVarY) + dAux) / 2), dChevichev * ::sqrt (((dVarX + dVarY) - dAux) / 2), -::atan2 (2 * dCovXY, dVarX - dVarY) / 2);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
