using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Math;
using System.IO;
using System.Diagnostics;

namespace task3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        NAudio.Wave.WaveChannel32 wave = null;
        NAudio.Wave.DirectSoundOut output = null;
        String waveFileName = null;
        List<double> samples = new List<double>();
        private double[][] dividedUnits;
        public int windowSize = 2 * 1024;
        String analyzedMelFile = "analyzed.txt";
        DTW dtw;

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void browse_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Wave file(*.wav)|*.wav;";

            if (open.ShowDialog() != DialogResult.OK)
                return;

            waveFileName = open.FileName;
            label2.Text = open.SafeFileName;

            byte[] buffer = new byte[16384];
            int read = 0;

            DisposeWave();
            wave = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(waveFileName));

            while (wave.Position < wave.Length)
            {
                read = wave.Read(buffer, 0, 16384);


                for (int i = 0; i < read / 4; i++)
                {
                    samples.Add(BitConverter.ToSingle(buffer, i * 4));
                }
            }
        }

        private void play_btn_Click(object sender, EventArgs e)
        {
            DisposeWave();
            wave = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(waveFileName));
            output = new NAudio.Wave.DirectSoundOut();
            output.Init(new NAudio.Wave.WaveChannel32(wave));
            output.Play();
        }

        private void DisposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Stop();
                output.Dispose();
                output = null;
            }

            if (wave != null)
            {
                wave.Dispose();
                wave = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeWave();
        }

        private void find_btn_Click(object sender, EventArgs e)
        {
            MelCepstrum cep = new MelCepstrum();
            convertToUnits();

            double[][] melCoeff = cep.getMelCepstrum(dividedUnits, 1024, 44100);
            saveMelToFile(melCoeff);
            compareWithPatterns();
        }

        private void convertToUnits()
        {
            /*
            for (int k = 1; k <= 850; k++)
            {
                samples.RemoveAt(samples.Count - 1);
            }
            */

            double[] units = new double[samples.Count / 2];
            double max = 0;

            for (int i = 0; i < samples.Count; i += 2)
            {
                if (Math.Abs(samples[i]) > max)
                {
                    max = Math.Abs(samples[i]);
                }
                units[i / 2] = samples[i];
            }

            for (int i = 0; i < units.Length; i++)
            {
                units[i] = units[i] * (1 / max);
            }

            Debug.WriteLine("Frames length: " + units.Length);

            dividedUnits = WindowFunction.sampling(units, 1024);
        }

        private void saveMelToFile(double[][] melCoeff)
        {
            String entry = "";

            for (int i = 0; i < melCoeff.Length - 1; i++)
            {
                for (int j = 0; j < melCoeff[i].Length; j++)
                {
                    entry += melCoeff[i][j];
                    if (j != melCoeff[i].Length - 1)
                    {
                        entry += '\t';
                    }
                }
                entry += '\n';
            }
            File.WriteAllText(analyzedMelFile, entry);
        }

        private void compareWithPatterns()
        {
            string[] filePaths = Directory.GetFiles(@"C:\Users\Maciek\Desktop\sound\speech_rec\models");
            double minimal = 999999;
            String bestMatch = "None.";
            Dictionary<String, Double> filePathValue = new Dictionary<String, Double>();
            foreach (string file in filePaths)
            {
                Debug.WriteLine(file);
                String modelMelFile = file;
                dtw = new DTW(modelMelFile, analyzedMelFile, false);
                dtw.calculatePath();
                if (dtw.getMinimalPath() < minimal)
                {
                    minimal = dtw.getMinimalPath();
                    bestMatch = file;
                }
                filePathValue.Add(file, dtw.getMinimalPath());
            }
            Debug.WriteLine("BEST MATCH: " + bestMatch);
        }

    }
}
