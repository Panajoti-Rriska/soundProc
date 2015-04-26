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

namespace waveFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int counter = 1;
        private int start_point = 500;
        private int end_point = 850;
        private float min_dist = 0;
        private int dim = 2;
        private int frame = 20;

        List<float> points_original = new List<float>();

        NAudio.Wave.WaveChannel32 wave;

        int k = 10;

        private void openWavButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Wave file(*.wav)|*.wav;";

            if (open.ShowDialog() != DialogResult.OK)
                return;

            counter++;

            drawChartButton.Enabled = true;

            chart1.Series.Add("wave" + counter);
            chart1.Series["wave" + counter].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series["wave" + counter].ChartArea = "ChartArea1";

            points_original.Clear();

            wave = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(open.FileName));

            byte[] buffer = new byte[16384];
            int read = 0;

            while (wave.Position < wave.Length)
            {
                read = wave.Read(buffer, 0, 16384);

                
                for (int i = 0; i < read / 4; i++)
                {
                    chart1.Series["wave" + counter].Points.Add(BitConverter.ToSingle(buffer, i * 4));
                    points_original.Add(BitConverter.ToSingle(buffer, i * 4));
                }
            }

            chart1.ChartAreas["ChartArea1"].Position.X = 1;
            chart1.ChartAreas["ChartArea1"].InnerPlotPosition.X = 1;
            chart1.Size = new Size(15000, chart1.Size.Height);

            
        }

        private void drawChartButton_Click(object sender, EventArgs e)
        {
            start_point = int.Parse(textBox3.Text);
            end_point = int.Parse(textBox2.Text);
            k = int.Parse(textBox1.Text);
            min_dist = float.Parse(textBox4.Text);
            dim = int.Parse(textBox5.Text);
            frame = int.Parse(textBox6.Text);
            
            Form2 graph = new Form2(points_original,start_point,end_point,k,min_dist, dim, frame);
            graph.Show();
        }
    }
}