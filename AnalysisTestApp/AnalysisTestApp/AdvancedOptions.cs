using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisTestApp
{

    /// <summary>
    /// User configurable static parameters to be used during the analysis
    /// </summary>

    class AdvancedOptions
    {
        /*  Use :               ConnectedComponents, fnAreSimilar
         *  Definition :        Static grey color value threshold below which two pixels are considered to belong to the same object 
         *  Recommended value : 80 +- 20;
         */

        public static int _nSimilarityTolerance = 80;

        /*  Use :               ConnectedComponents, fnMustInclude
         *  Definition :        Static grey color value threshold below which a pixel is not considered to be meaningful
         *  Recommended value : 100 +- 30;
         */

        public static int _nRelevanceThreshold = 100;

        /*  Use :               CountAnalysis, FindObjects
         *  Definition :        Region Of Interest definition in which blobs will be tracked
         *  Recommended value : According to the receip's size so as to avoid it's borders to be considered
         */

        public static int _nROIMargin = 120;

        /*  Use :               Matching-Robinson, findRobinsonReposition
         *  Definition :        Maximun and Minimum expected pixel deplacements according to the Sprout's images usual missmatches 
         *  Recommended value : Min: 3 +-2   Max: 5+-2    We strongly recommend not to set large values due to execution efficiente purposes
         */

        public static int _nMinReposition = 3, _nMaxReposition = 5;

        /*  Use :               DifferenceComputation, GetGlobalCorrelation
         *  Definition :        Minimum value for variances to be set if they happen to be smaller or equal to zero
         *  Recommended value : 1.0e-9, or some other close to zero number
         */

        public static double _dEpsilonValue = 1.0e-9;

        /*  Use :               DifferenceComputation, getGreyThreshold getThreshold
         *  Definition :        Minimum value below which a pixel is not considered meaningful
         *  Recommended value : 200 +-50
         */

        public static int _nThresholdValue = 200;

        /*  Use :               DifferenceComputation, getHystereris
         *  Definition :        Top and bottom values of the user-defined hysteresis cycle
         *  Recommended value : Top: 220 +-20  Bottom 180 +- 20
         */

        public static int _nTopHysteresis = 220, _nBottomHysteresis = 180;

        /*  Use :               Tacking, blobCompare
         *  Definition :        Thresholds for bounds and center deplacement as well as their difference
         *  Recommended value : Bounds diminish : -0.2 +-0.1   Great deplacement : 0.3 +- 0.1
         */

        public static double _dBoundsDiminish = -0.2, _dGreatDeplacement = 0.3, _dAbnormalGrowth = 0.5;

        /*  Use :               Tacking, Merged
         *  Definition :        Tolerance to merging predicted size and center difference
         *  Recommended value : Tolerance : 0.3 +- 0.1 Distance : 30 +- 10  
         */

        public static double _dMergingTolerance = 0.3, _nMergingDistance = 40;

        /*  Use :               ConnectedComponents
         *  Definition :        Minimum size for a cluster to be considered
         *  Recommended value : 40 +- 10
         */

        public static int _nMinimumSize = 40;

        /*  Use :               Tracking
         *  Definition :        ColorDifference of two colonies to be considered of not the same type
         *  Recommended value : 0.3 +- 0.1
         */

        public static double _dMaxColorDiff = 0.3;
    }
}
