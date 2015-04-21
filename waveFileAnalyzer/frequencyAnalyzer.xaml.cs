using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NAudio.Dsp;
using System.Windows.Media; // Pen

using System.IO;
using Microsoft.Research.DynamicDataDisplay; // Core functionality
using Microsoft.Research.DynamicDataDisplay.DataSources; // EnumerableDataSource
using Microsoft.Research.DynamicDataDisplay.PointMarkers; // CirclePointMarker


namespace NAudioWpfDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class frequencyAnalyzer : UserControl
    {
        private double xScale = 200;
        private int bins = 512; // guess a 1024 size FFT, bins is half FFT size
        private const int binsPerPoint = 2;
        private int updateCount;
        private List<double> dbListX = new List<double>();
        private List<double> dbListY = new List<double>();

        EnumerableDataSource<double> _edsSPP ;
        private bool didIt = false;
        public frequencyAnalyzer()
        {
            IPointDataSource _eds = null;
            //ah weird one...should consider y axis as well
            _edsSPP = new EnumerableDataSource<double>(dbListX);

            InitializeComponent();
            CalculateXScale();
            this.SizeChanged += SpectrumAnalyser_SizeChanged;
        }

        //x scale of the graph
        private void CalculateXScale()
        {
            this.xScale = this.ActualWidth / (bins / binsPerPoint);
        }

        void SpectrumAnalyser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateXScale();
        }

        private double calculateFrequency(int bin)
        {
            if (bin == 0) return 0;
            return bin * xScale; 
        }

        private double calculateDB(Complex c)
        {
            //Magnitude 
            double intensityDB = 10 * Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));

            double minDB = -90;

            //Checking if its below minimum
            if (intensityDB < minDB)
            {
                intensityDB = minDB;
            }

            double percent = intensityDB / minDB;
            // we want 0dB to be at the top (i.e. yPos = 0)
            double yPos = percent * this.ActualHeight;
            return yPos;    
        }

        private void addResultsToGraph(int index, double power)
        {

            
            Point p = new Point(calculateFrequency(index), power);
            dbListX.Add(p.X);
            dbListY.Add(p.Y);
        }


        //When update count is 42 it ends.
        public void Update(Complex[] fftResults)
        {
            // no need to repaint too many frames per second
            if (updateCount++ % 2 == 0)
            {
                return;
            }

            if (fftResults.Length / 2 != bins)
            {
                this.bins = fftResults.Length / 2;
                CalculateXScale();
            }

            for (int n = 0; n < fftResults.Length / 2; n += binsPerPoint)
            {
                // averaging out bins
                double db = 0;
                for (int b = 0; b < binsPerPoint; b++)
                {
                    db += calculateDB(fftResults[n + b]);
                }
                addResultsToGraph(n / binsPerPoint, db / binsPerPoint);
            }

            //End of song
            if (updateCount >= 42)
            {
                var frequencyDataSource = new EnumerableDataSource<double>(dbListX);
                frequencyDataSource.SetXMapping(x => x);

                var decibelOpenDataSource = new EnumerableDataSource<double>(dbListY);
                decibelOpenDataSource.SetYMapping(x => x);

                CompositeDataSource compositeDataSource1 = new
                 CompositeDataSource(frequencyDataSource, decibelOpenDataSource);

                frequencyChart.AddLineGraph(compositeDataSource1,
                 new Pen(Brushes.Blue, 1),
                 new CirclePointMarker { Size = 0.5, Fill = Brushes.Blue },
                 new PenDescription("Line"));

                frequencyChart.FitToView();
                updateCount = 0;
            }
        }
    }
}
