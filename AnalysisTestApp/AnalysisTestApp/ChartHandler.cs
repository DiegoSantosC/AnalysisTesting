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
        public void barCharConstruct(double[] yData, string _saveDirectory, string[] title)
        {
            Chart ch = new Chart();
            ChartArea area1 = new ChartArea("BarChart");

            ch.ChartAreas.Add(area1);
            Series s = new Series();
            s.Points.DataBindY(yData);

            s.ChartType = SeriesChartType.Column;
            s.ChartArea = "BarChart";

            ch.Series.Add(s);
            ch.ChartAreas[0].AxisX.Title = title[2];
            ch.ChartAreas[0].AxisY.Title = title[1];
           
           
            ch.Titles.Add(new Title(title[0]));

            ch.SaveImage(Path.Combine(_saveDirectory), ChartImageFormat.Png);

        }
    }
}
