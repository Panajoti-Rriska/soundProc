using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace NAudioWpfDemo.AudioPlaybackDemo
{
     [Export(typeof(IVisualizationPlugin))]
    class PhaseSpaceAnalyzerVizualizations : IVisualizationPlugin
    {
         private PhaseSpace phaseSpaceAnalyzer = new PhaseSpace();

         public string Name
         {
             get { return "Phase Space Analyser"; }
         }

         public object Content
         {
             get { return phaseSpaceAnalyzer; }
         }

         public void OnMaxCalculated(float min, float max)
         {
             // nothing to do
         }

         public void OnFftCalculated(NAudio.Dsp.Complex[] result)
         {
             //spectrumAnalyser.Update(result);
         }
    }
}
