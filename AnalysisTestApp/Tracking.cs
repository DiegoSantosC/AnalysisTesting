﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{
    class Tracking
    {
        private List<List<Cluster>> blobTrack;
        private List<Cluster> lastTrack;

        public Tracking()
        {
            blobTrack = new List<List<Cluster>>();
            lastTrack = new List<Cluster>();
        }

        public List<List<Cluster>> getTracking()
        {
            return blobTrack;
        }

        public bool isEmpty()
        {
            return blobTrack.Count == 0 ? true : false; 

        }

        // Initial step of blob track

        public void firstScan(List<Cluster> blobs, int step)
        {
            Console.WriteLine("First scan being performed");

            for (int i = 0; i < blobs.Count; i++)
            {
                blobs.ElementAt(i).setStep(step);
                List<Cluster> initialList = new List<Cluster>();
                initialList.Add(blobs.ElementAt(i));

                blobTrack.Add(initialList);

            }

            Console.WriteLine(blobTrack.Count + " blobs being tracked");
            
            lastTrack = blobs;
        }

        // Relate obtained blobs to those already tracked

        public void assignBlobs(List<Cluster> blobs, int step)
        {
            bool mergedChance = false, newColonyChance = false;

            if (blobs.Count < lastTrack.Count) mergedChance = true;
            if (blobs.Count > lastTrack.Count) newColonyChance = true;

            for (int i = 0; i < blobs.Count; i++)
            {
                blobs.ElementAt(i).setStep(step);

                if (!mergedChance && !newColonyChance)
                {
                    bool merged = findPossibleMerge(lastTrack, blobs.ElementAt(i));
                    if (!merged)
                    {
                        bool match = blobCompare(lastTrack.ElementAt(i), blobs.ElementAt(i));

                        if (match)
                        {
                            int index = lastTrack.ElementAt(i).getId();
                            blobs.ElementAt(i).setIndex(index);

                            blobTrack.ElementAt(index).Add(blobs.ElementAt(i));

                        }
                        else
                        {
                            // Try matching it with other blobs

                            int matchIndex = blobCompare(lastTrack, blobs.ElementAt(i));

                            if (matchIndex >= 0)
                            {
                                Console.WriteLine("Match with element " + matchIndex);

                                int index = lastTrack.ElementAt(matchIndex).getId();
                                blobs.ElementAt(i).setIndex(index);

                                blobTrack.ElementAt(index).Add(blobs.ElementAt(i));

                            }
                            else
                            {
                           
                                // New colony has appeared

                                Console.WriteLine("New colony considered to be appeared");

                                List<Cluster> newlist = new List<Cluster>();
                                blobs.ElementAt(i).setIndex(blobTrack.Count);
                                newlist.Add(blobs.ElementAt(i));
                                blobTrack.Add(newlist);

                            }
                        }
                    }
                }
                else
                {

                    bool merged = findPossibleMerge(lastTrack, blobs.ElementAt(i));
                    if (!merged)
                    {

                        // Try matching it with other blobs

                        int matchIndex = blobCompare(lastTrack, blobs.ElementAt(i));

                        if (matchIndex >= 0)
                        {
                            Console.WriteLine("Match with element " + matchIndex);

                            int index = lastTrack.ElementAt(matchIndex).getId();
                            blobs.ElementAt(i).setIndex(index);

                            blobTrack.ElementAt(index).Add(blobs.ElementAt(i));
                        }
                        else
                        {
                            // No match found 

                            Console.WriteLine("New colony considered to be appeared");

                            // New colony has appeared

                            List<Cluster> newlist = new List<Cluster>();
                            blobs.ElementAt(i).setIndex(blobTrack.Count);
                            newlist.Add(blobs.ElementAt(i));
                            blobTrack.Add(newlist);

                        }
                    }
                }
            }


            lastTrack = blobs;
        }

        private bool findPossibleMerge(List<Cluster> lastTrack, Cluster blob)
        {
            for (int i = 0; i < lastTrack.Count; i++)
            {
                for (int j = 0; j < lastTrack.Count; j++)
                {
                    if (j != i && Merged(lastTrack.ElementAt(i), lastTrack.ElementAt(j), blob))
                    {
                        Console.WriteLine("Blobs " + i + " and " + j + " have merged");

                        int indexDest, indexBrunch;

                        if (lastTrack.ElementAt(i).getSize() > lastTrack.ElementAt(j).getSize())
                        {
                            indexDest = lastTrack.ElementAt(i).getId();
                            indexBrunch = lastTrack.ElementAt(j).getId();

                            blobTrack.ElementAt(indexBrunch).ElementAt(blobTrack.ElementAt(indexBrunch).Count - 1).setFatherBranch(lastTrack.ElementAt(i).getId());
                            blobTrack.ElementAt(indexDest).ElementAt(blobTrack.ElementAt(indexDest).Count - 1).addBranch(blobTrack.ElementAt(indexBrunch));

                            blob.setIndex(indexDest);
                            blobTrack.ElementAt(indexDest).Add(blob);

                        }
                        else
                        {
                            indexDest = lastTrack.ElementAt(j).getId();
                            indexBrunch = lastTrack.ElementAt(i).getId();

                            blobTrack.ElementAt(indexBrunch).ElementAt(blobTrack.ElementAt(indexBrunch).Count - 1).setFatherBranch(lastTrack.ElementAt(j).getId());
                            blobTrack.ElementAt(indexDest).ElementAt(blobTrack.ElementAt(indexDest).Count - 1).addBranch(blobTrack.ElementAt(indexBrunch));

                            blob.setIndex(indexDest);
                            blobTrack.ElementAt(indexDest).Add(blob);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        // Characteristics comparison that identify a merged blob

        private bool Merged(Cluster cluster1, Cluster cluster2, Cluster blob)
        {
            int possibility = 0;

            double sizeDifference = (blob.getSize() - (cluster1.getSize() + cluster2.getSize())) / blob.getSize();

            // First feature: merged blob's size must be roughly twice it's original component's size

            if (sizeDifference < AdvancedOptions._dMergingTolerance && sizeDifference > 1)
            {
                Console.WriteLine(" Adequated Size Difference ");

                possibility++;
            }

            int[] bb1 = cluster1.getBoundingBox();
            int[] bb2 = cluster2.getBoundingBox();
            int[] bb3 = blob.getBoundingBox();

            // Second feature : bounding boxes intersect

            bool intersection = false;

            if (Math.Abs(bb1[0] - bb2[0]) < AdvancedOptions._nMergingDistance || Math.Abs(bb1[0] - bb2[2]) < AdvancedOptions._nMergingDistance || Math.Abs(bb1[2] - bb2[0]) < AdvancedOptions._nMergingDistance
                || Math.Abs(bb1[2] - bb2[2]) < AdvancedOptions._nMergingDistance) intersection = true;

            if (Math.Abs(bb1[1] - bb2[1]) < AdvancedOptions._nMergingDistance || Math.Abs(bb1[1] - bb2[3]) < AdvancedOptions._nMergingDistance || Math.Abs(bb1[3] - bb2[1]) < AdvancedOptions._nMergingDistance
                || Math.Abs(bb1[3] - bb2[3]) < AdvancedOptions._nMergingDistance) intersection = true;

            if (intersection) Console.WriteLine(" Blobs intersect" ); possibility++;

            int[] center1 = new int[2], center2 = new int[2], newCenter = new int[2];

            center1[0] = (bb1[2] - bb1[0]) / 2 + bb1[0];
            center1[1] = (bb1[3] - bb1[1]) / 2 + bb1[1];

            center2[0] = (bb2[2] - bb2[0]) / 2 + bb2[0];
            center2[1] = (bb2[3] - bb2[1]) / 2 + bb2[1];

            newCenter[0] = (bb3[2] - bb3[0]) / 2 + bb3[0];
            newCenter[1] = (bb3[3] - bb3[1]) / 2 + bb3[1];

            int[] predictedCenter = new int[2];
            predictedCenter[0] = (Math.Max(center1[0], center2[0]) - Math.Min(center1[0], center2[0])) / 2 + Math.Min(center1[0], center2[0]);
            predictedCenter[1] = (Math.Max(center1[1], center2[1]) - Math.Min(center1[1], center2[1])) / 2 + Math.Min(center1[1], center2[1]);

            if (Math.Abs(predictedCenter[0] - newCenter[0]) / Math.Sqrt(blob.getSize()) < AdvancedOptions._dMergingTolerance && Math.Abs(predictedCenter[1] - newCenter[1]) /
                Math.Sqrt(blob.getSize()) < AdvancedOptions._dMergingTolerance) Console.WriteLine(" Center correctly located"); possibility++;

            if (possibility > 1) return true;
            else return false;

        }

        private int blobCompare(List<Cluster> clusterList, Cluster c)
        {
            for (int i = 0; i < clusterList.Count; i++)
            {
                bool match = blobCompare(clusterList.ElementAt(i), c);

                if (match) return i;
            }

            return -1;
        }

        private bool blobCompare(Cluster c1, Cluster c2)
        {
            int[] center1 = new int[2], center2 = new int[2];
            int X0Depl, X1Depl, Y0Depl, Y1Depl;
            double boundsDepl, centerDepl;

            int[] bb1 = c1.getBoundingBox();
            int[] bb2 = c2.getBoundingBox();

            center1[0] = (bb1[2] - bb1[0]) / 2 + bb1[0];
            center1[1] = (bb1[3] - bb1[1]) / 2 + bb1[1];

            center2[0] = (bb2[2] - bb2[0]) / 2 + bb2[0];
            center2[1] = (bb2[3] - bb2[1]) / 2 + bb2[1];

            // The deplacement of the two centers between each other is normalized in terms of the blob's size

            centerDepl = (Math.Abs(center1[0] - center2[0]) + Math.Abs(center1[1] - center2[1])) / 2 / Math.Sqrt(c1.getSize());

            X0Depl = (bb1[0] - bb2[0]);
            X1Depl = (bb2[2] - bb1[2]);
            Y0Depl = (bb1[1] - bb2[1]);
            Y1Depl = (bb2[3] - bb1[3]);

            boundsDepl = (X0Depl + X1Depl + Y0Depl + Y1Depl) / Math.Sqrt(c1.getSize());

            Console.WriteLine(centerDepl + " Center depl + " + boundsDepl + " Bounds depl");

            // Abnormal deplacement

            if (boundsDepl < AdvancedOptions._dBoundsDiminish || centerDepl - boundsDepl > AdvancedOptions._dGreatDeplacement) return false;

            // Abnormal growth

            if (boundsDepl > AdvancedOptions._dAbnormalGrowth && centerDepl > AdvancedOptions._dGreatDeplacement) return false;

            Console.WriteLine("Growth");

            return true;
        }
    }
}
