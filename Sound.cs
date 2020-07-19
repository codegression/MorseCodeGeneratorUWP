using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Morse_Code_Generator
{
    public class Sound
    {
        public static Task<int> SynthesizeAsync(int[] durations, bool[] level, string filename, int Frequency = 1100, int Amplitude = 500)
        {
            return Task.Run(
               () => { return Synthesize(durations, level, filename, Frequency, Amplitude); }
               );
        }
        public static int Synthesize(int[] durations, bool[] level, string filename, int Frequency = 1100, int Amplitude = 500)
        {
            double A = ((Amplitude * (System.Math.Pow(2, 15))) / 1000) - 1;
            double DeltaFT = 2 * Math.PI * Frequency / 44100.0;

            int Duration = durations.Sum();
            int Samples = 441 * Duration / 10;
            int Bytes = Samples * 4;
            int[] Hdr = { 0X46464952, 36 + Bytes, 0X45564157, 0X20746D66, 16, 0X20001, 44100, 176400, 0X100004, 0X61746164, Bytes };

            using (BinaryWriter BW = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                for (int I = 0; I < Hdr.Length; I++)
                {
                    BW.Write(Hdr[I]);
                }


                for (int i = 0; i < durations.Length; i++)
                {
                    int samples = 441 * durations[i] / 10;
                    for (int T = 0; T < samples; T++)
                    {
                        short Sample = level[i] ? System.Convert.ToInt16(A * Math.Sin(DeltaFT * T)) : System.Convert.ToInt16(0);
                        BW.Write(Sample);
                        BW.Write(Sample);
                    }
                }               

            }

            return 0;
        }

    }
}
