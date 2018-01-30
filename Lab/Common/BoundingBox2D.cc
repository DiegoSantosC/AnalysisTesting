//-------------------------------------------------------------------------------------------------
#include <limits>
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
inline TBoundingBox2D<TData>::TBoundingBox2D ()
{
  reset ();
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
inline void TBoundingBox2D<TData>::reset ()
{
  dtX0 = dtY0 = std::numeric_limits<TData>::max ();
  dtX1 = dtY1 = std::numeric_limits<TData>::lowest ();
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline void TBoundingBox2D<TData>::addPoint (const TData &dtX, const TData &dtY)
{
  if (dtX < dtX0) dtX0 = dtX;
  if (dtY < dtY0) dtY0 = dtY;
  if (dtX > dtX1) dtX1 = dtX;
  if (dtY > dtY1) dtY1 = dtY;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

template <class TData>
inline bool TBoundingBox2D<TData>::isValid () const
{
  return (dtX0 <= dtX1) && (dtY0 <= dtY1);
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline bool TBoundingBox2D<TData>::isInside (const TData &dtX, const TData &dtY) const
{
  return (dtX >= dtX0) && (dtX <= dtX1) && (dtY >= dtY0) && (dtY <= dtY1);
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline TData TBoundingBox2D<TData>::width () const
{
  return dtX1 - dtX0;
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline TData TBoundingBox2D<TData>::height () const
{
  return dtY1 - dtY0;
}
//-------------------------------------------------------------------------------------------------

template <class TData>
inline double TBoundingBox2D<TData>::ratio () const
{
  return ((double) width ()) / ((double) height ());
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
