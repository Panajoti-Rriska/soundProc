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

        private NAudio.Wave.WaveFileReader wavefile = null;
        private NAudio.Wave.DirectSoundOut output = null;
        private int counter = 1;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Wave file(*.wav)|*.wav;";

            if (open.ShowDialog() != DialogResult.OK)
                return;

            DisposeWave();

            wavefile = new NAudio.Wave.WaveFileReader(open.FileName);
            output = new NAudio.Wave.DirectSoundOut();
            output.Init(new NAudio.Wave.WaveChannel32(wavefile));
            output.Play();

            counter++;

            pauseButton.Enabled = true;

            chart1.Series.Add("wave" + counter);
            chart1.Series["wave" + counter].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series["wave" + counter].ChartArea = "ChartArea1";

            NAudio.Wave.WaveChannel32 wave = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(open.FileName));

            byte[] buffer = new byte[16384];
            int read = 0;
            int sizer = 0;

            List<float> points_original = new List<float>();
            List<float> points_obtained = new List<float>();
            int k = 10;

            while (wave.Position < wave.Length)
            {
                read = wave.Read(buffer, 0, 16384);
                for (int i = 0; i < read / 4; i++)
                {
                    sizer++;
                    chart1.Series["wave" + counter].Points.Add(BitConverter.ToSingle(buffer, i * 4));
                    points_original.Add(BitConverter.ToSingle(buffer, i * 4));
                }
            }

            for (int i = 0; i < points_original.Count; i++)
            {
                if (i - k >= 0)
                {
                    points_obtained.Add(points_original.ElementAt(i-k));
                }
                if (i - k < 0)
                {
                    points_obtained.Add(points_original.ElementAt(points_original.Count+(i-k)));
                }
                
            }

            chart1.ChartAreas["ChartArea1"].InnerPlotPosition.X = 2;
            chart1.Size = new Size(15000, chart1.Size.Height);

            // just make the window big enough to fit this graph...
            /*Form2 window = new Form2();
            this.Width = 500;
            this.Height = 350;
            // add 5 so the bars fit properly
            int x = 240; // the position of the X axis
            int y = 0; // the position of the Y axis
            Bitmap bmp = new Bitmap(360, 290);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawLine(new Pen(Color.Red, 2), 5, 5, 5, 250);
            g.DrawLine(new Pen(Color.Red, 2), 5, 250, 300, 250);
            // let's draw a coordinate equivalent to (20,30) (20 up, 30 across)
            g.DrawString("X", new Font("Calibri", 12), new SolidBrush(Color.Black), y + points_obtained.ElementAt(i), x - points_original.ElementAt(i));
            PictureBox display = new PictureBox();
            display.Width = 360;
            display.Height = 290;
            window.Controls.Add(display);
            display.Image = bmp;

            window.Show();*/

            /*for (int i = 0; i < points_obtained.Count; i++)
            {
               String s = "x= " + points_original.ElementAt(i) + "; y= " + points_obtained.ElementAt(i);
               Debug.WriteLine(s);
            }*/

            Form2 graph = new Form2(points_obtained,points_original);
            graph.Show();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                    output.Pause();
                else if (output.PlaybackState == NAudio.Wave.PlaybackState.Paused)
                    output.Play();
            }
        }

        private void DisposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                    output.Stop();

                output.Dispose();
                output = null;
            }

            if (wavefile != null)
            {
                wavefile.Dispose();
                wavefile = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}