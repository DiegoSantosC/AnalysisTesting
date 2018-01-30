//-------------------------------------------------------------------------------------------------
#ifndef __BOUNDING_BOX_2D__H__
#define __BOUNDING_BOX_2D__H__
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
class TBoundingBox2D
{
  public:
    TData  dtX0, dtY0, dtX1, dtY1;

  public:
    inline TBoundingBox2D ();

    inline void  reset ();
    inline void  addPoint (const TData &dtX, const TData &dtY);

    inline bool    isValid () const;
    inline bool    isInside (const TData &dtX, const TData &dtY) const;
    inline TData   width () const;
    inline TData   height () const;
    inline double  ratio () const;
} ;
//-------------------------------------------------------------------------------------------------
#include "BoundingBox2D.cc"
//-------------------------------------------------------------------------------------------------
#endif
//-------------------------------------------------------------------------------------------------
