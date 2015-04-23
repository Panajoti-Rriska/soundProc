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
        List<float> points_dim = new List<float>();
        private int start_point;
        private int end_point;
        private int k;
        private float min_dist;
        private int dim;

        public Form2(List<float> points_original, int start_point, int end_point, int k, float min_dist, int dim)
        {
            InitializeComponent();
            this.points_original = points_original;
            this.start_point = start_point;
            this.end_point = end_point;
            this.k = k;
            this.min_dist = min_dist;
            this.dim = dim;

            List<double> distances = new List<double>();

            foreach (float i in points_original)
            {
                distances.Add(0);
            }

            for (int d = 2; d <= dim; d++)
            {
                points_dim = CreateDimensionList(points_original, k*(dim-1));

                for (int i = start_point; i < end_point; i += 2)
                {
                    double dist_x = 100;
                    double dist_y = 100;

                    if (Math.Abs(points_original.ElementAt(start_point)) > Math.Abs(points_original.ElementAt(i + 2))) { dist_x = Math.Abs(points_original.ElementAt(start_point)) - Math.Abs(points_original.ElementAt(i + 2)); }
                    else { dist_x = Math.Abs(points_original.ElementAt(i + 1)) - Math.Abs(points_original.ElementAt(start_point)); }
                    double dist_x2 = dist_x * dist_x;
                    if (Math.Abs(points_dim.ElementAt(start_point)) > Math.Abs(points_dim.ElementAt(i + 2))) { dist_y = Math.Abs(points_dim.ElementAt(start_point)) - Math.Abs(points_dim.ElementAt(i + 2)); }
                    else { dist_y = Math.Abs(points_dim.ElementAt(i + 2)) - Math.Abs(points_dim.ElementAt(start_point)); }
                    double dist_y2 = dist_y * dist_y;

                    double dist = Math.Sqrt(dist_x2 + dist_y2);

                    distances[i] = Math.Sqrt((dist * dist) + (distances.ElementAt(i) * distances.ElementAt(i)));

                    if (d == dim)
                    {
                        if (distances[i] > min_dist)
                        {
                            Debug.WriteLine(dist);
                            chart1.Series["Series1"].Points.AddXY
                                            (points_original.ElementAt(i), points_dim.ElementAt(i));
                            chart1.Series["Series1"].ChartType =
                                    SeriesChartType.FastPoint;
                            chart1.Series["Series1"].Color = Color.Blue;
                        }
                        else break;
                    }
                }
                points_dim.Clear();
            }
        }

        private List<float> CreateDimensionList(List<float> points_original, int k)
        {
            List<float> output = new List<float>();
            for (int i = 0; i < points_original.Count; i++)
            {
                if(i+k<points_original.Count)
                output.Add(points_original.ElementAt(i + k));
            }
            return output;
        }
    }
}
