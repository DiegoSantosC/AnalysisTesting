using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.IO;

namespace AnalysisTestApp
{
    class ChartHandler
    {
        public void barCharConstruct(double[] yData, int index, string _saveDirectory)
        {

            Chart ch = new Chart();
            ChartArea area1 = new ChartArea("BarChart");

            ch.ChartAreas.Add(area1);
            Series s = new Series();
            s.Points.DataBindY(yData);

            s.ChartType = SeriesChartType.Column;
            s.ChartArea = "BarChart";

            ch.Series.Add(s);

            string p = "Chart_" + index.ToString() + ".png";

            ch.SaveImage(Path.Combine(_saveDirectory, p), ChartImageFormat.Png);

        }
    }
}
