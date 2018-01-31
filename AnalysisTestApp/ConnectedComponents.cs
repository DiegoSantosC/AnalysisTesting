﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class ConnectedComponents
    {
        int[,] arrTagMap;

        public ConnectedComponents(int [,] arrSrc, string Timelapse, ROI region)
        {
            // Initializing

            int NIL = -1;

            int nMaxX = arrSrc.GetLength(0);
            int nMaxY = arrSrc.GetLength(1);

            int nSize = arrSrc.GetLength(0) * arrSrc.GetLength(1);

            arrTagMap = new int[nMaxX, nMaxY];

            List<int> tagRemap = new List<int>();

            int[,] currTag = new int[nMaxX, nMaxY];
            int tagCounter = 0;

            for(int i = 0; i <nMaxY; i++)
            {
                for(int j = 0; j<nMaxX; j++)
                {
                    currTag[j, i] = NIL;
                }
            }

            // First point (0,0) is managed

            if ((region != null || isInside(region, 0, 0)) && (fnMustInclude(arrSrc[0, 0])))
            {
                currTag[0, 0] = tagCounter;
                tagRemap.Add(0);

            } else tagCounter++;

            // First row is managed

            for (int x = 1; x < nMaxX; x++)
            {
                int nLeftTag = currTag[x - 1, 0];

                if ((region != null || isInside(region, x, 0)) && (fnMustInclude(arrSrc[x, 0])))
                {
                    if (!((nLeftTag != NIL) && fnAreSimilar(arrSrc[x - 1, 0], arrSrc[x, 0])))
                    {
                        tagRemap.Add(tagCounter);
                        nLeftTag++; tagCounter++;
                    }

                }
                else nLeftTag = NIL;

                currTag[x, 0] = nLeftTag;
            }

            for (int y = 1; y < nMaxY; y++)
            {
                // First column is managed

                int nDownTag = currTag[0, y - 1];

                if ((region != null || isInside(region, 0, y)) && (fnMustInclude(arrSrc[0, y])))
                {
                    if (!((nDownTag != NIL) && fnAreSimilar(arrSrc[0, y - 1], arrSrc[0, y])))
                    {
                        tagRemap.Add(tagCounter);
                        nDownTag++; tagCounter++;
                    }
                }
                else nDownTag = NIL;

                currTag[0 , y] = nDownTag;


                for (int x = 1; x < nMaxX; x++)
                {
                    int nLeftTag = currTag[x - 1 , y];

                    if ((region != null || isInside(region, x, y)) && (fnMustInclude(arrSrc[x, y])))
                    {
                        if ((nLeftTag != NIL) && fnAreSimilar(arrSrc[x - 1, y], arrSrc[x, y]))
                        {
                            currTag[x, y] = nLeftTag;

                            if((nDownTag != NIL) && (tagRemap.ElementAt(nDownTag) != tagRemap.ElementAt(nLeftTag)) && fnAreSimilar(arrSrc[x, y - 1], arrSrc[x, y]))
                            {
                                int nOrig = tagRemap.ElementAt(nLeftTag);
                                int nDest = tagRemap.ElementAt(nDownTag);

                                if(nOrig > nDest)
                                {
                                    int aux = nOrig;
                                    nOrig = nDest;
                                    nDest = aux;
                                }

                                for (int i = 0; i<tagRemap.Count(); i++)
                                {
                                    if (tagRemap.ElementAt(i) == nDest)
                                    {
                                        tagRemap.RemoveAt(i);

                                        if (tagRemap.Count > nDest)
                                        {
                                            tagRemap.Insert(i, nDest);
                                        }
                                        else
                                        {
                                            tagRemap.Add(nDest);
                                        }

                                    }
                                }
                            }

                        }
                        else if ((nDownTag != NIL) && fnAreSimilar(arrSrc[0, y - 1], arrSrc[0, y])) currTag[x, y] = nDownTag;
                        else
                        {
                            tagRemap.Add(tagCounter);
                            currTag[x, y] = tagCounter; tagCounter++;
                        }
                    }
                    else { nLeftTag = NIL; currTag[x, y] = nLeftTag; }
                }
            }

            List<Cluster> clusterList = new List<Cluster>();
            List<int> takenIndexes = new List<int>();

            // Execute remapping
            for (int y = 0; y < nMaxY; y++)
            {
                for (int x = 0; x < nMaxX; x++)
                {
                    if (currTag[x, y] != NIL)
                    {
                        arrTagMap[x, y] = tagRemap.ElementAt(currTag[x, y]);

                        if (takenIndexes.Contains(tagRemap.ElementAt(currTag[x, y])))
                        {
                            //clusterList.ElementAt(arrTagMap[x, y]).addPoint(x, y, arrSrc[x, y]);
                        }
                        else
                        {
                            clusterList.Add(new Cluster(arrTagMap[x, y]));
                            takenIndexes.Add(arrTagMap[x, y]);
                        }
                    }
                    else arrTagMap[x, y] = NIL;
                }
            }

            Console.WriteLine(tagRemap.Count);

            for (int i=0; i<tagRemap.Count; i++)
            {
                Console.Write(tagRemap.ElementAt(i) +" ");
            }

            Console.WriteLine(tagCounter);
            Console.WriteLine(clusterList.Count);

        }

        // Decide wether if two pixels belong to the same cluster

        private bool fnAreSimilar(int value1, int value2)
        {
            // At first we will only demand that their difference is not bigger than a certain threshold

            if (Math.Abs(value1 - value2) < 50) return true;
            else return false;

        }

        // Decide wether if a pixel value is worth being considered

        private bool fnMustInclude(int currValue)
        {
            // At first we will only demand it not to be under a certain threshold

            if (currValue > 100) return true;
            else return false;
        }

        // Point is inside the taken region

        private bool isInside(ROI region, int x, int y)
        {
            if (x > region.getX0() && x < region.getY0() && y > region.getY0() && y < region.getY1()) return true;
            else return false;
        }


    }
}
