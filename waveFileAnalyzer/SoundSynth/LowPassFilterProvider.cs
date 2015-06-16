using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;

namespace SoundSynth
{
    class LowPassFilterProvider : ISampleProvider
    {
        private ISampleProvider source;
        private BiQuadFilter filter;

        public LowPassFilterProvider(ISampleProvider source)
        {
            this.source = source;
            //4khz cut-off frequency and resonance parameter q to 0.7f
            filter = BiQuadFilter.LowPassFilter(source.WaveFormat.SampleRate, 4000, 0.7f);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var samples = source.Read(buffer, offset, count);

            for( var n =0; n< samples; n++)
            {
                buffer[offset + n] = filter.Transform(buffer[offset + n]);
            }

            return samples;
        }

        public WaveFormat WaveFormat { get { return source.WaveFormat; } }

    }
}
