using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morse_Code_Generator
{
    public class MorseCode
    {
        public delegate void NewTimeSegmentDelegate(bool On, string dotdash);
        public event NewTimeSegmentDelegate NewTimeSegment;
        public int WPM { set; get; }
        public bool IsLoop { set; get; }

        private Dictionary<string, string> LookupTable;
        private bool isStopped;
       

        public MorseCode()
        {
            LookupTable = new Dictionary<string, string>();
            isStopped = false;
            WPM = 15;
            IsLoop = false;
            LookupTable.Add("A", ".-");
            LookupTable.Add("B", "-...");
            LookupTable.Add("C", "-.-.");
            LookupTable.Add("D", "-..");
            LookupTable.Add("E", ".");
            LookupTable.Add("F", "..-.");
            LookupTable.Add("G", "--.");
            LookupTable.Add("H", "....");
            LookupTable.Add("I", "..");
            LookupTable.Add("J", ".---");
            LookupTable.Add("K", "-.-");
            LookupTable.Add("L", ".-..");
            LookupTable.Add("M", "--");
            LookupTable.Add("N", "-.");
            LookupTable.Add("O", "---");
            LookupTable.Add("P", ".--.");
            LookupTable.Add("Q", "--.-");
            LookupTable.Add("R", ".-.");
            LookupTable.Add("S", "...");
            LookupTable.Add("T", "-");
            LookupTable.Add("U", "..-");
            LookupTable.Add("V", "...-");
            LookupTable.Add("W", ".--");
            LookupTable.Add("X", "-..-");
            LookupTable.Add("Y", "-.--");
            LookupTable.Add("Z", "--..");

            LookupTable.Add("1", ".----");
            LookupTable.Add("2", "..---");
            LookupTable.Add("3", "...--");
            LookupTable.Add("4", "....-");
            LookupTable.Add("5", ".....");
            LookupTable.Add("6", "-....");
            LookupTable.Add("7", "--...");
            LookupTable.Add("8", "---..");
            LookupTable.Add("9", "----.");
            LookupTable.Add("0", "-----");

            LookupTable.Add(".", ".-.-.-");
            LookupTable.Add(",", "--..--");
            LookupTable.Add("?", "..--..");
            LookupTable.Add("'", ".----.");
            LookupTable.Add("!", "-.-.--");
            LookupTable.Add("/", "-..-.");
            LookupTable.Add("{", "-.--.");
            LookupTable.Add("}", "-.--.-");
            LookupTable.Add("&", ".-...");
            LookupTable.Add(":", "---...");
            LookupTable.Add(";", "-.-.-.");
            LookupTable.Add("=", "-...-");
            LookupTable.Add("+", ".-.-.");
            LookupTable.Add("-", "-....-");
            LookupTable.Add("_", "..--.-");
            LookupTable.Add("\"", ".-..-.");
            LookupTable.Add("$", "...-..-");
            LookupTable.Add("@", ".--.-.");           
        }

        public Task<int> PlayAsync(string str)
        {
            return Task.Run(
                () => { return Play(str); }
                );
        }

        public int Period
        {
            get
            {
                return 20 * 60 / WPM;
            }           
        }


        public int Synthesize(string str, out int[] durations, out bool[] levels)
        {
            isStopped = false;
            int period = 20 * 60 / WPM;
            List<int> Durations = new List<int>();
            List<bool> Levels = new List<bool>();

            string morsecode = ConvertToMorseCode(str);

            for (int i = 0; i < morsecode.Length; i++)
            {
                if (morsecode[i] == '.')
                {
                    Durations.Add(period);
                    Levels.Add(true);

                    Durations.Add(period);
                    Levels.Add(false);
                }
                else if (morsecode[i] == '-')
                {
                    Durations.Add(period * 3);
                    Levels.Add(true);

                    Durations.Add(period);
                    Levels.Add(false);
                }
                else if (morsecode[i] == '+')
                {
                    Durations.Add(period * 3);
                    Levels.Add(false);
                }
                else if (morsecode[i] == ' ')
                {
                    Durations.Add(period * 7);
                    Levels.Add(false);
                }
                
            }
            Durations.Add(period*7);
            Levels.Add(false);

            durations = Durations.ToArray();
            levels = Levels.ToArray();
            return 1;
        }
        public int Play(string str)
        {
            isStopped = false;
            int period = 20 * 60 / WPM;
            

            string morsecode = ConvertToMorseCode(str);
          

            while (!isStopped)
            {
                for (int i = 0; i < morsecode.Length; i++)
                {
                    if (isStopped)
                    {
                        return 0;
                    }
                    else if (morsecode[i] == '.')
                    {
                        NewTimeSegment(true, "dot");
                        System.Threading.Tasks.Task.Delay(period).Wait();
                        NewTimeSegment(false, null);
                        System.Threading.Tasks.Task.Delay(period).Wait();
                    }
                    else if (morsecode[i] == '-')
                    {
                        NewTimeSegment(true, "dash");
                        System.Threading.Tasks.Task.Delay(period * 3).Wait();
                        NewTimeSegment(false, null);
                        System.Threading.Tasks.Task.Delay(period).Wait();
                    }
                    else if (morsecode[i] == '+')
                    {
                        System.Threading.Tasks.Task.Delay(period * 3).Wait();
                    }
                    else if (morsecode[i] == ' ')
                    {
                        System.Threading.Tasks.Task.Delay(period * 7).Wait();
                    }                  
                }
                

                if (!IsLoop)
                {
                    break;
                }
                else
                {
                    System.Threading.Tasks.Task.Delay(period * 7).Wait();
                }
            }
            return 1;
        }

        public void Stop()
        {
            isStopped = true;
        }

        public string ConvertToMorseCode(string str)
        {
            StringBuilder sb = new StringBuilder();
            string upperstring = str.ToUpper();
            for (int i = 0; i < upperstring.Length; i++)
            {
                if (upperstring[i] == ' ')
                {
                    sb.Append(' ');
                }
                else if (LookupTable.ContainsKey(upperstring[i].ToString()))
                {
                    sb.Append(LookupTable[upperstring[i].ToString()]);

                    if (i < upperstring.Length - 1)
                    {
                        sb.Append("+");
                    }
                }
            }

            return sb.Replace("+ ", " ").ToString();
        }
    }
}
