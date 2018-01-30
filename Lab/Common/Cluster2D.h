//-------------------------------------------------------------------------------------------------
#ifndef __CLUSTER_2D__H__
#define __CLUSTER_2D__H__
//-------------------------------------------------------------------------------------------------
#include "Line.h"
#include "Ellipse.h"
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
class TCluster2D
{
  protected:
    unsigned int  _nCount;
    TData  _dtSumX, _dtSumY;
    TData  _dtSumXX, _dtSumXY, _dtSumYY;

  public:
    inline TCluster2D ();
    inline void  reset ();
    inline void  set (unsigned int nCount, double dAvgX, double dAvgY);
    inline void  addPoint (const TData &dtX, const TData &dtY);

    inline unsigned int  count () const;
    inline double  avgX () const;
    inline double  avgY () const;
    inline double  varX () const;
    inline double  covXY () const;
    inline double  varY () const;

    inline double  pearson () const;
    inline double  pearson2 () const;

    inline TLine     line_lsq (double *pMSE=NULL) const;
    inline TLine     line_ev () const;
    inline TEllipse  ellipse (double dChevichev=2.5) const;
} ;
//-------------------------------------------------------------------------------------------------
#include "Cluster2D.cc"
//-------------------------------------------------------------------------------------------------
#endif
//-------------------------------------------------------------------------------------------------
