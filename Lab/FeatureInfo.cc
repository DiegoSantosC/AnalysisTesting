//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline TFeatureInfo::TFeatureInfo ()
{
  reset (NIL);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline void TFeatureInfo::reset (unsigned int nFeatId)
{
  nId = nFeatId;  dX = dY = -1;
  _cluster.reset ();  _bbLimits.reset ();
  _nSumR = _nSumG = _nSumB = 0;
}
//-------------------------------------------------------------------------------------------------

inline void TFeatureInfo::addPoint (unsigned int nX, unsigned int nY, const TColorRGB32 &clr)
{
  _cluster.addPoint (nX, nY);  _bbLimits.addPoint (nX, nY);
  _nSumR += clr.nR;  _nSumG += clr.nG;  _nSumB += clr.nB;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline const TBoundingBox2D<unsigned int> &TFeatureInfo::getBoundingBox_const () const
{
  return _bbLimits;
}
//-------------------------------------------------------------------------------------------------

inline const TBoundingBox2D<unsigned int> &TFeatureInfo::getBoundingBox () const
{
  return _bbLimits;
}
//-------------------------------------------------------------------------------------------------

inline TBoundingBox2D<unsigned int> &TFeatureInfo::getBoundingBox ()
{
  return _bbLimits;
}
//-------------------------------------------------------------------------------------------------

inline unsigned int TFeatureInfo::X0 () const
{
  return _bbLimits.dtX0;
}
//-------------------------------------------------------------------------------------------------

inline unsigned int TFeatureInfo::Y0 () const
{
  return _bbLimits.dtY0;
}
//-------------------------------------------------------------------------------------------------

inline unsigned int TFeatureInfo::X1 () const
{
  return _bbLimits.dtX1;
}
//-------------------------------------------------------------------------------------------------

inline unsigned int TFeatureInfo::Y1 () const
{
  return _bbLimits.dtY1;
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline const TCluster2D<unsigned long long> &TFeatureInfo::getCluster_const () const
{
  return _cluster;
}
//-------------------------------------------------------------------------------------------------

inline const TCluster2D<unsigned long long> &TFeatureInfo::getCluster () const
{
  return _cluster;
}
//-------------------------------------------------------------------------------------------------

inline TCluster2D<unsigned long long> &TFeatureInfo::getCluster ()
{
  return _cluster;
}
//-------------------------------------------------------------------------------------------------

inline unsigned int TFeatureInfo::count () const
{
  return _cluster.count ();
}
//-------------------------------------------------------------------------------------------------

inline void TFeatureInfo::set (double dAvgX, double dAvgY)
{
  _cluster.set (1000, dAvgX, dAvgY);
}
//-------------------------------------------------------------------------------------------------

inline double TFeatureInfo::avgX () const
{
  return _cluster.avgX ();
}
//-------------------------------------------------------------------------------------------------

inline double TFeatureInfo::avgY () const
{
  return _cluster.avgY ();
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline bool TFeatureInfo::isInside (unsigned int nX, unsigned int nY) const
{
  return _bbLimits.isInside (nX, nY);
}
//-------------------------------------------------------------------------------------------------

inline unsigned int TFeatureInfo::width () const
{
  return _bbLimits.width () + 1;
}
//-------------------------------------------------------------------------------------------------

inline unsigned int TFeatureInfo::height () const
{
  return _bbLimits.height () + 1;
}
//-------------------------------------------------------------------------------------------------

inline double TFeatureInfo::ratio () const
{
  return _bbLimits.ratio ();
}
//-------------------------------------------------------------------------------------------------

inline double TFeatureInfo::capacity () const
{
  return ((double) _cluster.count ()) / ((double) (_bbLimits.width () * _bbLimits.height ()));
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline TColorRGB32 TFeatureInfo::avgColor () const
{
  const unsigned int  nCount = max (_cluster.count (), 1U);
  return TColorRGB32 ((unsigned char) (_nSumR / nCount), (unsigned char) (_nSumG / nCount), (unsigned char) (_nSumB / nCount));
}
//-------------------------------------------------------------------------------------------------

inline TEllipse TFeatureInfo::ellipse (double dChevichev) const
{
  return _cluster.ellipse (dChevichev);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------

inline unsigned int featureSize (const TFeatureInfo &fi)
{
  return fi.count ();
}
//-------------------------------------------------------------------------------------------------

inline void reset (TFeatureInfo &fi, unsigned int nId)
{
  fi.reset (nId);
}
//-------------------------------------------------------------------------------------------------

inline void addPoint (TFeatureInfo &fi, unsigned int nX, unsigned int nY, const TColorRGB32 &clr)
{
  fi.addPoint (nX, nY, clr);
}
//-------------------------------------------------------------------------------------------------

inline void addPoint (TFeatureInfo &fi, unsigned int nX, unsigned int nY, const TColorGrey8 &clr)
{
  fi.addPoint (nX, nY, TColorRGB32 (clr.nG, clr.nG, clr.nG));
}
//-------------------------------------------------------------------------------------------------

inline void addPoint (TFeatureInfo &fi, unsigned int nX, unsigned int nY, const unsigned int &nValue)
{
  fi.addPoint (nX, nY, TColorRGB32 ((unsigned char) nValue, (unsigned char) nValue, (unsigned char) nValue));
}
//-------------------------------------------------------------------------------------------------

inline bool  isInside (const TFeatureInfo &fi, unsigned int nX, unsigned int nY)
{
  return fi.isInside (nX, nY);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
