using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace NAudioWpfDemo.AudioPlaybackDemo
{
     [Export(typeof(IVisualizationPlugin))]
    class frequencyAnalyzerVizualisation : IVisualizationPlugin
    {
         private frequencyAnalyzer frAnalyzer = new frequencyAnalyzer();
         public string Name
         {
             get { return "Frequency Analyzer"; }
         }

         public object Content
         {
             get { return frAnalyzer; }
         }

         public void OnMaxCalculated(float min, float max)
         {
             // nothing to do
         }

         public void OnFftCalculated(NAudio.Dsp.Complex[] result)
         {
             //spectrumAnalyser.Update(result);
             frAnalyzer.Update(result);
         }
    }
}
