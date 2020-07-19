using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Lights;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Morse_Code_Generator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MorseCode morsecode;
        Lamp lamp;

        public MainPage()
        {
            this.InitializeComponent();
            morsecode = new MorseCode();
            morsecode.NewTimeSegment += Morsecode_NewTimeSegment;      


        }

     

        private void Morsecode_NewTimeSegment(bool On, string dotdash)
        {
            
                try
                {
                    if (On)
                    {
                        this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           MainGrid.Background = new SolidColorBrush(Windows.UI.Colors.White);
                           btnStop.Background = new SolidColorBrush(Windows.UI.Colors.White);
                           btnStop.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                       });

                        if (lamp!=null)
                        {
                             lamp.IsEnabled = true;
                        }
                    }
                    else
                    {
                        this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           MainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Black);
                           btnStop.Background = new SolidColorBrush(Windows.UI.Colors.Black);
                           btnStop.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                       });
                        if (lamp != null)
                        {
                                 lamp.IsEnabled = false;
                        }
                }
                }
                catch { }
           
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (btnGenerate.Content.ToString()=="Play")
            {
                Generate();
            }
            else
            {
                Player.Stop();
                btnGenerate.Content = "Play";
                btnSave.Visibility = Visibility.Visible;
            }        
        
        }

        private async void Generate()
        {
            morsecode.WPM = (int)WPMSlider.Value;
            morsecode.IsLoop = IsLoop.IsOn;

            if (SoundSignal.IsSelected)
            {
                Player.Stop();
                btnGenerate.Content = "Stop";
                btnSave.Visibility = Visibility.Collapsed;
                Player.IsLooping = IsLoop.IsOn;
                int[] durations = null;
                bool[] levels = null;
                morsecode.Synthesize(textbox1.Text, out durations, out levels);

                StorageFolder tempfolder = ApplicationData.Current.TemporaryFolder;
                int j = await Sound.SynthesizeAsync(durations, levels, tempfolder.Path.ToString() + "\\MORSECODE.wav");


                Windows.Storage.StorageFile file = await tempfolder.GetFileAsync("MORSECODE.wav");
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                Player.SetSource(stream, file.ContentType);
            }
            else
            {
                if (lamp == null)
                {
                    lamp = await Lamp.GetDefaultAsync();
                }
                Player.Stop();
                ButtonsStackPanel.Visibility = Visibility.Collapsed;
                textbox1.Visibility = Visibility.Collapsed;
                ControlsStackPanel.Visibility = Visibility.Collapsed;
                TitleTextBlock.Visibility = Visibility.Collapsed;
                btnStop.Visibility = Visibility.Visible;
                int i = await morsecode.PlayAsync(textbox1.Text);
                CleanUP();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            morsecode.Stop();
            Player.Stop();
            CleanUP();
            if (lamp != null)
            {
                lamp.IsEnabled = false;
            }
        }

        private void CleanUP()
        {
            ButtonsStackPanel.Visibility = Visibility.Visible;
            textbox1.Visibility = Visibility.Visible;           
            ControlsStackPanel.Visibility = Visibility.Visible;
            TitleTextBlock.Visibility = Visibility.Visible;           
            btnStop.Visibility = Visibility.Collapsed;
            SolidColorBrush brush = Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;
            MainGrid.Background = brush;
          
        }

        private void SignalType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LightSignal.IsSelected && btnSave!=null)
            {
                btnSave.Visibility = Visibility.Collapsed;
            }
            else if (btnSave != null)
            {
                btnSave.Visibility = Visibility.Visible;
            }
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            btnGenerate.Content = "Play";
            btnSave.Visibility = Visibility.Visible;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;      
            savePicker.FileTypeChoices.Add("WAV file", new List<string>() { ".wav" });            

            StorageFile destinationfile = await savePicker.PickSaveFileAsync();
            if (destinationfile != null)
            {
                
                int[] durations = null;
                bool[] levels = null;
                morsecode.Synthesize(textbox1.Text, out durations, out levels);

                StorageFolder tempfolder = ApplicationData.Current.TemporaryFolder;
                int j = await Sound.SynthesizeAsync(durations, levels, tempfolder.Path.ToString() + "\\MORSECODE.wav");


                StorageFile originalfile = await tempfolder.GetFileAsync("MORSECODE.wav");
                await originalfile.CopyAndReplaceAsync(destinationfile);
            }
          
        }
    }
}
