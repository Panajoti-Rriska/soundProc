using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private int start_point;
        private int end_point;
        private int k;
        private float min_dist;
        private int dim;

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

        public Form2(List<float> points_obtained, List<float> points_original, int start_point, int end_point, int k, float min_dist, int dim)
        {
            InitializeComponent();

            this.points_obtained = points_obtained;
            this.points_original = points_original;
            this.start_point = start_point;
            this.end_point = end_point;
            this.k = k;
            this.min_dist = min_dist;
            this.dim = dim;

            for (int i = start_point; i < end_point; i+=2)
            {
                float dist_x = 0;
                float dist_y = 0;

                if (Math.Abs(points_original.ElementAt(i)) > Math.Abs(points_original.ElementAt(i + 2))) { dist_x = Math.Abs(points_original.ElementAt(i)) - Math.Abs(points_original.ElementAt(i + 2)); }
                else { dist_x = Math.Abs(points_original.ElementAt(i+1)) - Math.Abs(points_original.ElementAt(i)); }
                float dist_x2 = dist_x * dist_x;
                if (Math.Abs(points_obtained.ElementAt(i)) > Math.Abs(points_obtained.ElementAt(i + 2))) { dist_y = Math.Abs(points_obtained.ElementAt(i)) - Math.Abs(points_obtained.ElementAt(i + 2)); }
                else { dist_y = Math.Abs(points_obtained.ElementAt(i + 2)) - Math.Abs(points_obtained.ElementAt(i)); }
                float dist_y2 = dist_y * dist_y;
                double dist = Math.Sqrt(dist_x2 + dist_y2);

                //Debug.WriteLine(points_original.ElementAt(i) + " " + points_original.ElementAt(i+2));

                if (dist>min_dist)
                {
                    chart1.Series["Series1"].Points.AddXY
                                    (points_original.ElementAt(i), points_obtained.ElementAt(i));
                    chart1.Series["Series1"].ChartType =
                            SeriesChartType.FastPoint;
                    chart1.Series["Series1"].Color = Color.Blue;
                }
                else break;
            }
        }
    }
}
