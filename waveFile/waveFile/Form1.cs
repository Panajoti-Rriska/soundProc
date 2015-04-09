using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            //waveViewer1.SamplesPerPixel = 400;
            //waveViewer1.StartPosition = 40000;
            //waveViewer1.WaveStream = new NAudio.Wave.WaveFileReader(open.FileName);

            chart1.Series.Add("wave"+counter);
            chart1.Series["wave"+counter].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series["wave"+counter].ChartArea = "ChartArea1";

            NAudio.Wave.WaveChannel32 wave = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(open.FileName));

            byte[] buffer = new byte[16384];
            int read = 0;

            while(wave.Position < wave.Length)
            {
                read = wave.Read(buffer, 0, 16384);

                for (int i = 0; i < read / 4; i++)
                {
                    chart1.Series["wave"+counter].Points.Add(BitConverter.ToSingle(buffer, i * 4));
                }
            }
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
