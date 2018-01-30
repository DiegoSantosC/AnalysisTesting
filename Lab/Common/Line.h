//-------------------------------------------------------------------------------------------------
#ifndef __LINE__H__
#define __LINE__H__
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

class TLine
{
  protected:
    double  _dAngle, _dOffset;
    double  _dSin, _dCos;

  public:
    inline TLine ();
    inline TLine (double dAngle, double dOffset);
    inline TLine (double dX, double dY, double dAngle);

    inline void  set (double dAngle, double dOffset);
    inline void  set (double dX, double dY, double dAngle);
    inline void  get (double &dAngle, double &dOffset) const;

    inline double  angle () const;

    inline double  getXat (double dY) const;
    inline double  getYat (double dX) const;

    inline bool    intersect (const TLine &l, double *pX=NULL, double *pY=NULL) const;

    inline double  distanceEuclidean (double dX, double dY) const;
    inline double  distanceEuclidean2 (double dX, double dY) const;

    inline bool  toImplicit (double &dAx, double &dBy, double &dC) const;
    inline bool  fromImplicit (double dAx, double dBy, double dC);
    inline bool  from2points (double dAx, double dAy, double dBx, double dBy);
} ;
//-------------------------------------------------------------------------------------------------
#include "Line.cc"
//-------------------------------------------------------------------------------------------------
#endif
//-------------------------------------------------------------------------------------------------
