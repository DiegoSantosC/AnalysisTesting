// Function connectComponents()
//   This function will create a labeling/tag map for all connected components in a 2D array
//   Final labeling map will be set in arrTagMap. Each label can be seen as an index to one detected feature returned in the bufFeatures list
//
// Template params:
//   TData       Data-type/class of each cell in the map (or pixel on an image)
//   TFeature    Data-type/class of the detected features
//   TROI        Data-type/class of the Region-Of-Interest definition
//
// Function parameters:
//   arrSrc             [input]    Source data map
//   arrTagMap          [output]   Map of labels (pixel correspondence to any detected feature/blob)
//   bufFeatures        [output]   List of detected features/blobs
//   fnMustInclude      [input]    Function to be used to decide if a pixel must be included in a feature/blob
//   pParamMustInclude  [input]    Pointer to any parameter wanted to be passed additionally to the function fnMustInclude when called (can be NULL)
//   fnAreSimilar       [input]    Function to be used to decide if two contiguous pixels are similar
//   pParamAreSimilar   [input]    Pointer to any parameter that want be passed additionally to the function fnAreSimilar when called (can be NULL)
//   pROI               [input]    Pointer to a Region-Of-Interest object. Only those pixels in arcSrc that ar inside this Region-Of-Interest are processed (can be NULL if no ROI is defined, so the whole source map will be processed)
template <class TData, class TFeature, class TROI>
void connectComponents (const DynamicArray2D<TData> &arrSrc, DynamicArray2D<unsigned int> &arrTagMap, Buffer<TFeature> &bufFeatures, bool (*fnMustInclude) (const TData &, void *), void *pParamMustInclude, bool (*fnAreSimilar) (const TData &, const TData &, void *), void *pParamAreSimilar, const TROI *pROI)
{
  const int  nMaxX = arrSrc.colCount () - 1;
  const int  nMaxY = arrSrc.rowCount () - 1;
  const unsigned int  nSize = arrSrc.elemCount ();

  // Resize Tag map
  arrTagMap.resize (arrSrc.rowCount (), arrSrc.colCount ());

  // Initialize remapping array
  unsigned int  *pRemap = (unsigned int*) ::malloc (nSize * sizeof (unsigned int));
  for (unsigned int n = 0; n < nSize; ++n)
    pRemap [n] = n;

  // Connecting components
  const TData   *pCurrData = arrSrc.getRow_const (0);
  unsigned int  *pCurrTag = arrTagMap.getRawData ();
  unsigned int   nTagCounter = 0;
  *pCurrTag = NIL;

  if ((!pROI || isInside (*pROI, 0, 0)) && (!fnMustInclude || fnMustInclude (pCurrData [0], pParamMustInclude)))
    *pCurrTag = nTagCounter++;

  for (int x = 1; x <= nMaxX; ++x)
  {
    const unsigned int  nLeftTag = *pCurrTag;
    *(++pCurrTag) = NIL;
    if ((!pROI || isInside (*pROI, x, 0)) && (!fnMustInclude || fnMustInclude (pCurrData [x], pParamMustInclude)))
      *pCurrTag = ((nLeftTag != NIL) && (!fnAreSimilar || fnAreSimilar (pCurrData [x], pCurrData [x - 1], pParamAreSimilar))) ? nLeftTag : nTagCounter++;
  }

  for (int y = 1; y <= nMaxY; ++y)
  {
    const unsigned int  *pDownTag = arrTagMap.getRow_const (y - 1);
    const TData  *pDownData = pCurrData;
    pCurrData = arrSrc.getRow_const (y);

    *(++pCurrTag) = NIL;
    if ((!pROI || isInside (*pROI, 0, y)) && (!fnMustInclude || fnMustInclude (pCurrData [0], pParamMustInclude)))
      *pCurrTag = ((pDownTag [0] != NIL) && (!fnAreSimilar || fnAreSimilar (pCurrData [0], pDownData [0], pParamAreSimilar))) ? pDownTag [0] : nTagCounter++;

    for (int x = 1; x <= nMaxX; ++x)
    {
      const unsigned int  nLeftTag = *pCurrTag;
      *(++pCurrTag) = NIL;
      if ((!pROI || isInside (*pROI, x, y)) && (!fnMustInclude || fnMustInclude (pCurrData [x], pParamMustInclude)))
      {
        if ((nLeftTag != NIL) && (!fnAreSimilar || fnAreSimilar (pCurrData [x], pCurrData [x - 1], pParamAreSimilar)))
        {
          *pCurrTag = nLeftTag;
          if ((pDownTag [x] != NIL) && (pRemap [pDownTag [x]] != pRemap [nLeftTag]) && (!fnAreSimilar || fnAreSimilar (pCurrData [x], pDownData [x], pParamAreSimilar)))
          {
            unsigned int  nOrig = pRemap [nLeftTag];
            unsigned int  nDest = pRemap [pDownTag [x]];
            if (nOrig > nDest) { nOrig = nDest;  nDest = pRemap [nLeftTag]; }
            for (unsigned int i = nDest; i < nTagCounter; ++i)
              if (pRemap [i] == nDest) pRemap [i] = nOrig;
          }
        }
        else *pCurrTag = ((pDownTag [x] != NIL) && (!fnAreSimilar || fnAreSimilar (pCurrData [x], pDownData [x], pParamAreSimilar))) ? pDownTag [x] : nTagCounter++;
      }
    }
  }

  // Initialize feature buffer
  bufFeatures.resize (nTagCounter, false);
  for (unsigned int n = 0; n < nTagCounter; ++n)
    reset (bufFeatures (n), n);

  // Set final labels and fill bufFeatures buffer
  pCurrTag = arrTagMap.getRawData ();
  for (int y = 0; y <= nMaxY; ++y)
  {
    pCurrData = arrSrc.getRow_const (y);
    for (int x = 0; x <= nMaxX; ++x, ++pCurrTag)
      if (*pCurrTag != NIL)
      {
        *pCurrTag = pRemap [*pCurrTag];
        /*if (*pCurrTag != NIL)*/ addPoint (bufFeatures (*pCurrTag), x, y, pCurrData [x]);
      }
  }

  // Finalize feature computation
  ::qsort (bufFeatures.getRawData (), bufFeatures.elemCount (), sizeof (TFeature), &cmpFeatureSize<TFeature>);
  for (unsigned int n = 0; n < bufFeatures.elemCount (); ++n)
    if (!featureSize (bufFeatures.get_const (n)))
      { bufFeatures.resize (n, true);  break; }

  // Free up reserved memory
  ::free (pRemap);
}
//-------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------
