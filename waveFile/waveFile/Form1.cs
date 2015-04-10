using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
namespace waveFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private Mp3FileReader wavefile = null;
        private WaveOut output = null;
        private int counter = 1;

        private void button1_Click(object sender, EventArgs e)
        {
            //Opening file
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "mp3 file(*.mp3)|*.mp3;";

            if (open.ShowDialog() != DialogResult.OK)
                return;

            DisposeWave();

            //Initialize
            output = new WaveOut();
            output.NumberOfBuffers = 2;
            output.DesiredLatency = 100;
            output.Volume = (float)volumeBar.Value/100;
            

            wavefile = new Mp3FileReader(open.FileName);
            output.Init(wavefile);
            

            counter++;

            pauseButton.Enabled = true;

            //waveViewer1.SamplesPerPixel = 400;
            //waveViewer1.StartPosition = 40000;
            //waveViewer1.WaveStream = new NAudio.Wave.WaveFileReader(open.FileName);


            //Frequency visualizer
            
            chart1.Series.Add("wave"+counter);
            chart1.Series["wave"+counter].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series["wave"+counter].ChartArea = "ChartArea1";

            WaveChannel32 wave = new WaveChannel32(new Mp3FileReader(open.FileName));

            byte[] buffer = new byte[8384];
            int read = 0;

            while(wave.Position < wave.Length)
            {
                read = wave.Read(buffer, 0, 8384);

                for (int i = 0; i < read / 4; i++)
                {
                    chart1.Series["wave"+counter].Points.Add(BitConverter.ToSingle(buffer, i * 4));
                }
            }

            //Sound output
            output.Play();
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

            if(wavefile != null)
            {
                wavefile.Dispose();
                wavefile = null;
            }
        }

        private void volumeBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (output != null)
            {
                output.Volume = (float)volumeBar.Value/100;
            }
        }


    }
}
