//-------------------------------------------------------------------------------------------------
#ifndef __ELLIPSE__H__
#define __ELLIPSE__H__
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

class TEllipse
{
  protected:
    double  _dXc, _dYc, _dA, _dB, _dAngle;
    double  _dSin, _dCos;

  public:
    inline TEllipse ();
    inline TEllipse (double dX, double dY, double dA, double dB, double dAngle);

    inline void    set (double dX, double dY, double dA, double dB, double dAngle);
    inline void    get (double &dX, double &dY, double &dA, double &dB, double &dAngle) const;

    inline void    getBoundingBox (double &dX0, double &dY0, double &dX1, double &dY1) const;
    inline double  centerX () const;
    inline void    centerX (double dCenterX);
    inline double  centerY () const;
    inline void    centerY (double dCenterY);
    inline double  radiusHz () const;
    inline void    radiusHz (double dRadiusHz);
    inline double  radiusVt () const;
    inline void    radiusVt (double dRadiusVt);
    inline double  angle () const;
    inline void    angle (double dAngle);
    inline bool    isInside (double dX, double dY) const;

    inline double  distanceMahalanobis (double dX, double dY) const;
    inline double  distanceMahalanobis2 (double dX, double dY) const;

    inline double  area () const;
    inline double  perimeter () const;
    inline double  ratio () const;

    inline bool  toImplicit (double &dAx2, double &dBxy, double &dCy2, double &dDx, double &dEy, double &dF) const;
    inline bool  fromImplicit (double dAx2, double dBxy, double dCy2, double dDx, double dEy, double dF);
    inline bool  from5points (double dAx, double dAy, double dBx, double dBy, double dCx, double dCy, double dDx, double dDy, double dEx, double dEy);

    inline void  toLocalCoordinates (double dX, double dY, double &dU, double &dV) const;
    inline void  fromLocalCoordinates (double dU, double dV, double &dX, double &dY) const;
} ;
//-------------------------------------------------------------------------------------------------

inline bool  isInside (const TEllipse &e, unsigned int nX, unsigned int nY);

//-------------------------------------------------------------------------------------------------
#include "Ellipse.cc"
//-------------------------------------------------------------------------------------------------
#endif
//-------------------------------------------------------------------------------------------------
