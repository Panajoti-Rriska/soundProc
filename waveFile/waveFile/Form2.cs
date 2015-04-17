using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace waveFile
{
    public partial class Form2 : Form
    {
        List<float> points_original = new List<float>();
        List<float> points_obtained = new List<float>();

        public Form2(List<float> obtained, List<float> original)
        {
            InitializeComponent();

            points_obtained = obtained;
            points_original = original;

            int begining = 300;
            float dist_x2 = (points_original.ElementAt(begining - 1) - points_original.ElementAt(begining)) * (points_original.ElementAt(begining - 1) - points_original.ElementAt(begining));
            float dist_y2 = (points_obtained.ElementAt(begining - 1) - points_obtained.ElementAt(begining)) * (points_obtained.ElementAt(begining - 1) - points_obtained.ElementAt(begining));
            double dist = Math.Sqrt(dist_x2 + dist_y2);
            double d = dist;

            for (int i = begining; i < 1000; i++)
            {
                dist_x2 = (points_original.ElementAt(i) - points_original.ElementAt(i+1)) * (points_original.ElementAt(i) - points_original.ElementAt(i+1));
                dist_y2 = (points_obtained.ElementAt(i) - points_obtained.ElementAt(i + 1)) * (points_obtained.ElementAt(i) - points_obtained.ElementAt(i + 1));
                dist = Math.Sqrt(dist_x2 + dist_y2);

                if (dist <= d)
                {
                    chart1.Series["Series1"].Points.AddXY
                                    (points_original.ElementAt(i), points_obtained.ElementAt(i));
                    chart1.Series["Series1"].ChartType =
                            SeriesChartType.FastPoint;
                    chart1.Series["Series1"].Color = Color.Red;
                }

                d = dist;
            }
        }
    }
}
