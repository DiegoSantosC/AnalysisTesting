//-------------------------------------------------------------------------------------------------
#ifndef __FEATURE_INFO__H__
#define __FEATURE_INFO__H__
//-------------------------------------------------------------------------------------------------
#include "BoundingBox2D.h"
#include "Cluster2D.h"
#include "Color.h"
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

class TFeatureInfo
{
  public:
    unsigned int  nId;
    double  dX, dY;

  protected:
    TCluster2D<unsigned long long>  _cluster;
    TBoundingBox2D<unsigned int>  _bbLimits;
    unsigned int  _nSumR, _nSumG, _nSumB;

  public:
    inline TFeatureInfo ();

    inline void  reset (unsigned int nFeatId);
    inline void  addPoint (unsigned int nX, unsigned int nY, const TColorRGB32 &clr);

    inline const TBoundingBox2D<unsigned int>  &getBoundingBox_const () const;
    inline const TBoundingBox2D<unsigned int>  &getBoundingBox () const;
    inline TBoundingBox2D<unsigned int>        &getBoundingBox ();
    inline unsigned int  X0 () const;
    inline unsigned int  Y0 () const;
    inline unsigned int  X1 () const;
    inline unsigned int  Y1 () const;

    inline const TCluster2D<unsigned long long>  &getCluster_const () const;
    inline const TCluster2D<unsigned long long>  &getCluster () const;
    inline TCluster2D<unsigned long long>        &getCluster ();
    inline unsigned int  count () const;
    inline void    set (double dAvgX, double dAvgY);
    inline double  avgX () const;
    inline double  avgY () const;

    inline bool  isInside (unsigned int nX, unsigned int nY) const;
    inline unsigned int  width () const;
    inline unsigned int  height () const;
    inline double  ratio () const;
    inline double  capacity () const;

    inline TColorRGB32  avgColor () const;
    inline TEllipse     ellipse (double dChevichev) const;
} ;
//-------------------------------------------------------------------------------------------------
inline unsigned int  featureSize (const TFeatureInfo &fi);
inline void  reset (TFeatureInfo &fi, unsigned int nId);
inline void  addPoint (TFeatureInfo &fi, unsigned int nX, unsigned int nY, const TColorRGB32 &clr);
inline void  addPoint (TFeatureInfo &fi, unsigned int nX, unsigned int nY, const TColorGrey8 &clr);
inline void  addPoint (TFeatureInfo &fi, unsigned int nX, unsigned int nY, const unsigned int &nValue);
inline bool  isInside (TFeatureInfo &fi, unsigned int nX, unsigned int nY);
//-------------------------------------------------------------------------------------------------
#include "FeatureInfo.cc"
//-------------------------------------------------------------------------------------------------
#endif
//-------------------------------------------------------------------------------------------------
