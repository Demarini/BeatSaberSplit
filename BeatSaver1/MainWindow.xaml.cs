using LibVLC.NET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Xabe.FFmpeg;
//using Declarations;
//using Implementation;
//using Declarations.Media;
//using Declarations.Players;
//using Vlc.DotNet.Core;

namespace BeatSaver1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int section1 = 0;
        int section2 = 0;
        Stopwatch st = new Stopwatch();
        int initialMilli = 0;
        int currentMilli = 0;
        static bool is64BitProcess = (IntPtr.Size == 8);
        static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();
        string[] files;
        LibVLC.NET.MediaPlayer vlc = new LibVLC.NET.MediaPlayer();
        Thread t;
        int currentTime = 0;
        bool runThread = false;
        List<CutEntity> cuts = new List<CutEntity>();
        public MainWindow()
        {
            InitializeComponent();
            t = new Thread(PollVlcControl);
        }

        private void AudioShit2(string path)
        {
            LibVLCLibrary library = LibVLCLibrary.Load(null);
            IntPtr inst, mp, m;

            inst = library.libvlc_new();
            // Load the VLC engine 
            m = library.libvlc_media_new_location(inst, "f.mp4"); // Create a new item 
            mp = library.libvlc_media_player_new_from_media(m);               // Create a media player playing environement 
            library.libvlc_media_release(m);                                  // No need to keep the media now 
            library.libvlc_media_player_play(mp);                             // play the media_player 
            Thread.Sleep(10000);                                              // Let it play a bit 
            library.libvlc_media_player_stop(mp);                             // Stop playing 
            library.libvlc_media_player_release(mp);                          // Free the media_player 
            library.libvlc_release(inst);

            LibVLCLibrary.Free(library);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            runThread = false;
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            File.Text = dlg.SelectedPath;
            if (File.Text != "")
            {
                files = Directory.GetFiles(File.Text);
                string ogg = GetOgg(files);
                if(GetChart(files, "Easy") != null)
                {
                    chartTypes.Items.Add("Easy");
                }
                if (GetChart(files, "Medium") != null)
                {
                    chartTypes.Items.Add("Medium");
                }
                if (GetChart(files, "Hard") != null)
                {
                    chartTypes.Items.Add("Hard");
                }
                if (GetChart(files, "Expert") != null)
                {
                    chartTypes.Items.Add("Expert");
                }
                if (GetChart(files, "ExpertPlus") != null)
                {
                    chartTypes.Items.Add("ExpertPlus");
                }
                chartTypes.IsEnabled = true;
                AudioShit3(ogg);
                CheckCriteriaForEnable();
            }
        }
        private void AudioShit3(string path)
        {
            vlc.Location = new Uri(path);
            runThread = true;
            DoubleCollection dc = new DoubleCollection();

            int loops = 0;
            while (vlc.Length.TotalSeconds == 0 && loops < 30)
            {
                vlc.Volume = 0;
                vlc.Play();
                vlc.Pause();
                var test = vlc.Length.TotalSeconds;
                loops++;
                Thread.Sleep(1000);
            }
            for (int i = 0; i <= vlc.Length.TotalSeconds; i++)
            {
                dc.Add(i);
            }
            Slider.TickFrequency = 1;
            Slider.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.None;
            Slider.Maximum = vlc.Length.TotalSeconds;
            if (t.IsAlive)
            {
                t.Abort();
            }
            t.Start();
            st.Stop();
            st.Reset();
            vlc.Stop();
            cuts.Clear();
            Sections.Items.Clear();
        }
        private void UpdateSlider()
        {
            while (true)
            {
                Dispatcher.Invoke(() =>
                {
                    if (vlc.Length.TotalSeconds != 0)
                    {
                        DoubleCollection dc = new DoubleCollection();
                        for (int i = 0; i <= vlc.Length.TotalSeconds; i++)
                        {
                            dc.Add(i);
                        }
                        Slider.Ticks = dc;
                        Slider.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft;
                        Slider.TickFrequency = 10;
                        Slider.Maximum = vlc.Length.TotalSeconds;
                        int loops = 0;
                        while (vlc.Length.TotalSeconds == 0 && loops < 30)
                        {
                            vlc.Pause();
                            var test = vlc.Length.TotalSeconds;
                            loops++;
                            Thread.Sleep(1000);
                        }
                    }
                });
            }
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            //IConversionResult result = await Conversion.Split()
            try
            {
                var json = System.IO.File.ReadAllText(File.Text);
                System.Windows.MessageBox.Show("Success!");
            }
            catch
            {
                System.Windows.MessageBox.Show("Error!");
            }

        }
        private string GetOgg(string[] files)
        {
            foreach (string s in files)
            {
                string s2 = System.IO.Path.GetExtension(s).ToUpper();
                if (System.IO.Path.GetExtension(s).ToUpper() == ".OGG")
                {
                    return s;
                }
            }
            return null;
        }
        private string GetChart(string[] files, string chartType)
        {
            foreach (string s in files)
            {
                string s2 = System.IO.Path.GetExtension(s).ToUpper();
                if (System.IO.Path.GetFileName(s).ToUpper() == chartType.ToUpper() + ".JSON")
                {
                    return s;
                }
                string s3 = System.IO.Path.GetFileName(s).ToUpper();
            }
            return null;
        }
        private string GetInfo(string[] files)
        {
            foreach (string s in files)
            {
                string s2 = System.IO.Path.GetExtension(s).ToUpper();
                if (System.IO.Path.GetFileName(s).ToUpper() == "INFO.JSON")
                {
                    return s;
                }
            }
            return null;
        }
        private void CreateChart(string json, string directoryName, string ogg)
        {
            BeatSaverEntity bsE = JsonConvert.DeserializeObject<BeatSaverEntity>(json);
            int counter = 1;
            int i = 0;
            while (i < cuts.Count)
            {
                string counterString = "";
                if (counter < 10)
                {
                    counterString = "00" + counter.ToString();
                }
                else if (counter > 9 && counter < 100)
                {
                    counterString = "0" + counter.ToString();
                }
                else
                {
                    counterString = counter.ToString();
                }
                string start = (cuts[i].StartTime / 1000).ToString();
                string end = (cuts[i].EndTime / 1000).ToString();
                string name = directoryName + " - " + counterString + " " + cuts[i].Name;

                List<EventsEntity> _truncatedEvents = new List<EventsEntity>();
                List<NotesEntity> _truncatedNotes = new List<NotesEntity>();
                List<ObstaclesEntity> _truncatedObstacles = new List<ObstaclesEntity>();
                decimal conversionNum = decimal.Divide(bsE._beatsPerMinute, 60);
                foreach (EventsEntity e2 in bsE._events)
                {
                    if ((e2._time / conversionNum) >= int.Parse(start) - 1 && (e2._time / conversionNum) <= int.Parse(end) + 1)
                    {
                        _truncatedEvents.Add(e2);
                    }
                }
                foreach (EventsEntity e3 in _truncatedEvents)
                {
                    if (decimal.Parse(start) > 1)
                    {
                        e3._time = Math.Round(e3._time - (decimal.Parse(start) - 1) * conversionNum + (conversionNum * 2), 2);
                    }
                    else
                    {
                        e3._time = Math.Round(e3._time - (decimal.Parse(start)) * conversionNum, 2);
                    }

                }
                foreach (NotesEntity n2 in bsE._notes)
                {
                    if ((n2._time / conversionNum) >= int.Parse(start) - 1 && (n2._time / conversionNum) <= int.Parse(end) + 1)
                    {
                        _truncatedNotes.Add(n2);
                    }
                }
                foreach (NotesEntity n3 in _truncatedNotes)
                {
                    if (decimal.Parse(start) > 1)
                    {
                        n3._time = Math.Round(n3._time - (decimal.Parse(start) - 1) * conversionNum + (conversionNum * 2), 2);
                    }
                    else
                    {
                        n3._time = Math.Round(n3._time - (decimal.Parse(start)) * conversionNum, 2);
                    }

                }
                foreach (ObstaclesEntity o2 in bsE._obstacles)
                {
                    if ((o2._time / conversionNum) >= int.Parse(start) - 1 && (o2._time / conversionNum) <= int.Parse(end) + 1)
                    {
                        _truncatedObstacles.Add(o2);
                    }
                }
                foreach (ObstaclesEntity o3 in _truncatedObstacles)
                {
                    if (decimal.Parse(start) > 1)
                    {
                        o3._time = Math.Round(o3._time - (decimal.Parse(start) - 1) * conversionNum + (conversionNum * 2), 2);
                    }
                    else
                    {
                        o3._time = Math.Round(o3._time - (decimal.Parse(start)) * conversionNum, 2);
                    }

                }
                BeatSaverEntity bseNew = new BeatSaverEntity();
                bseNew._beatsPerBar = bsE._beatsPerBar;
                bseNew._beatsPerMinute = bsE._beatsPerMinute;
                bseNew._noteJumpSpeed = bsE._noteJumpSpeed;
                bseNew._shuffle = bsE._shuffle;
                bseNew._shufflePeriod = bsE._shufflePeriod;
                bseNew._version = bsE._version;
                bseNew._events = _truncatedEvents.ToArray();
                bseNew._notes = _truncatedNotes.ToArray();
                JsonSerializer _jsonWriter = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                var newJson = JsonConvert.SerializeObject(bseNew,
                                Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                var outputPath = output.Text + "\\" + name;
                System.IO.Directory.CreateDirectory(outputPath);
                System.IO.File.WriteAllText(outputPath + "\\" + chartTypes.SelectedValue + ".json", newJson);

                //System.IO.File.Copy(path + "\\info.json", outputPath + "\\info.json");
                var json2 = System.IO.File.ReadAllText(File.Text + "\\info.json");
                InfoEntity iE = JsonConvert.DeserializeObject<InfoEntity>(json2);
                List<DifficultyEntity> difficulties = new List<DifficultyEntity>();
                DifficultyEntity d = new DifficultyEntity();
                d.difficulty = chartTypes.SelectedValue.ToString();
                d.difficultyRank = 4;
                foreach (DifficultyEntity d2 in iE.difficultyLevels)
                {
                    if (d2.difficulty.ToUpper() == chartTypes.SelectedValue.ToString().ToUpper())
                    {
                        d.difficultyRank = d2.difficultyRank;
                    }
                }

                d.audioPath = directoryName + " - " + counterString + " " + cuts[i].Name + ".ogg";
                d.jsonPath = chartTypes.SelectedValue + ".json";
                d.offset = 0;
                d.oldOffset = 0;
                difficulties.Add(d);
                iE.difficultyLevels = difficulties.ToArray();
                iE.songName = directoryName + " - " + counterString + " " + cuts[i].Name;
                var newJson2 = JsonConvert.SerializeObject(iE,
                                Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                System.IO.File.WriteAllText(outputPath + "\\info.json", newJson2);
                //System.IO.File.Copy(path + "\\cover.jpg", outputPath + "\\cover.jpg");

                double duration = (double.Parse(end) - double.Parse(start) + 4);
                
                if (double.Parse(start) > 1)
                {
                    var result = Conversion.Split(ogg, outputPath + "\\" + directoryName + " - " + counterString + " " + cuts[i].Name + ".ogg", TimeSpan.FromSeconds(double.Parse(start) - 3), TimeSpan.FromSeconds(duration)).Start();
                }
                else
                {
                    var result = Conversion.Split(ogg, outputPath + "\\" + directoryName + " - " + counterString + " " + cuts[i].Name + ".ogg", TimeSpan.FromSeconds(double.Parse(start)), TimeSpan.FromSeconds(duration)).Start();
                }
                counter++;
                i++;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vlc.Stop();
            string[] files = Directory.GetFiles(File.Text);

            string ogg = GetOgg(files);
            string chart = GetChart(files, "Expert");
            string info = GetInfo(files);
            string directoryName = new DirectoryInfo(File.Text).Name;
            var json = System.IO.File.ReadAllText(chart);
            CreateChart(json, directoryName, ogg);
            vlc = new LibVLC.NET.MediaPlayer();
            Sections.Items.Clear();
            cuts.Clear();
            Slider.Value = 0;
            output_Copy1.Text = "0:00";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.ShowDialog();
                output.Text = dlg.SelectedPath;
                CheckCriteriaForEnable();
            }
            catch
            {
                System.Windows.MessageBox.Show("Error!");
            }
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            //player.Play();
            vlc.Volume = 100;
            runThread = true;
            if (!t.IsAlive)
            {
                t = new Thread(PollVlcControl);
                t.Start();
            }
            initialMilli = (int)(Slider.Value * 1000);
            st.Start();
            vlc.Play();

            currentMilli = (int)(Slider.Value * 1000);
            //TimeSpan time = TimeSpan.FromMilliseconds(currentMilli);
            //TimeSpan ts = TimeSpan.FromMilliseconds(60 * currentMilli / 1000);
            //TimeSpan ts2 = time.Subtract(ts);
            //vlc.Time = ts2;
            //int test = vlc.Delay;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            //player.Stop();
            //currentTime = (int)vlc.Time.TotalSeconds;
            //Slider.Value = currentTime;
            runThread = false;
            //t.Abort();
            st.Stop();
            st.Reset();
            vlc.Pause();
            //Slider.Value = (int)(currentMilli / 1000);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            //player.Play()\
            
            st.Stop();
            st.Reset();
            vlc.Stop();
            Slider.Value = 0;
            runThread = false;
            Sections.Items.Clear();
            cuts.Clear();
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            section2 = currentMilli;

            TimeSpan ts = TimeSpan.FromMilliseconds(section1);

            string secondsVal = "";
            if (ts.Seconds.ToString().Length == 1)
            {
                secondsVal = "0" + ts.Seconds.ToString();
            }
            else
            {
                secondsVal = ts.Seconds.ToString();
            }

            TimeSpan ts2 = TimeSpan.FromMilliseconds(section2);
            string secondsVal2 = "";
            if (ts2.Seconds.ToString().Length == 1)
            {
                secondsVal2 = "0" + ts2.Seconds.ToString();
            }
            else
            {
                secondsVal2 = ts2.Seconds.ToString();
            }


            Sections.Items.Add(output_Copy.Text + " - " + ts.Minutes.ToString() + ":" + secondsVal + "/" + ts2.Minutes.ToString() + ":" + secondsVal2);
            Create.IsEnabled = true;
            CutEntity c = new CutEntity();
            c.StartTime = section1;
            c.EndTime = section2;
            c.Name = output_Copy.Text;
            cuts.Add(c);
            section1 = section2;
            foreach (CutEntity c2 in cuts)
            {
                if (c2.Name == output_Copy.Text)
                {
                    Cut.IsEnabled = false;
                }
                else
                {
                    Cut.IsEnabled = true;
                }
            }
        }

        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
        
        private void PollVlcControl()
        {
            try
            {
                while (runThread)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (vlc.State == MediaPlayerState.Playing)
                        {
                            currentMilli = (int)(initialMilli + st.ElapsedMilliseconds);
                            currentTime = (int)(currentMilli / 1000);
                            Slider.Value = currentTime;
                            string secondsVal = "";
                            if (vlc.Time.Seconds.ToString().Length == 1)
                            {
                                secondsVal = "0" + vlc.Time.Seconds.ToString();
                            }
                            else
                            {
                                secondsVal = vlc.Time.Seconds.ToString();
                            }
                            //output_Copy2.Text = vlc.Time.TotalMilliseconds.ToString();
                        }
                    });
                    Thread.Sleep(100);
                }
            }
            catch(Exception ex)
            {
                //do this eventualyl
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vlc.State == MediaPlayerState.Playing)
            {
                TimeSpan time = TimeSpan.FromMilliseconds(currentMilli);
                string secondsVal = "";
                if (time.Seconds.ToString().Length == 1)
                {
                    secondsVal = "0" + time.Seconds.ToString();
                }
                else
                {
                    secondsVal = time.Seconds.ToString();
                }
                output_Copy1.Text = time.Minutes + ":" + secondsVal;

                //vlc.Time = time;
            }
            else
            {
                currentMilli = (int)(Slider.Value * 1000);
                TimeSpan time = TimeSpan.FromMilliseconds(currentMilli);
                string secondsVal = "";
                if (time.Seconds.ToString().Length == 1)
                {
                    secondsVal = "0" + time.Seconds.ToString();
                }
                else
                {
                    secondsVal = time.Seconds.ToString();
                }
                output_Copy1.Text = time.Minutes.ToString() + ":" + secondsVal;
                
            }
        }
        public bool CheckCriteriaForEnable()
        {
            if (File.Text != "" && output.Text != "" && chartTypes.SelectedItem!= null)
            {
                Play.IsEnabled = true;
                Pause.IsEnabled = true;
                Stop.IsEnabled = true;
                Cut.IsEnabled = false;
                output_Copy1.IsEnabled = true;
                Sections.IsEnabled = true;
                output_Copy.IsEnabled = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            HowToUse h = new HowToUse();
            h.ShowDialog();
        }
        private void output_Copy_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (output_Copy.Text == "")
            {
                Cut.IsEnabled = false;
            }
            else
            {
                Cut.IsEnabled = true;
            }
            foreach(CutEntity c in cuts)
            {
                if(c.Name == output_Copy.Text)
                {
                    Cut.IsEnabled = false;
                }
                else
                {
                    Cut.IsEnabled = true;
                }
            }
        }
        private void chartTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CheckCriteriaForEnable();
        }
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
        [In] IntPtr hProcess,
        [Out] out bool wow64Process
    );

        private void output_Copy_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        { 
            char[] invalids = Path.GetInvalidFileNameChars();
            char[] invalids2 = Path.GetInvalidPathChars();
            foreach (char c in e.Text)
            {
                if(!char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '_' && !char.IsNumber(c))
                {
                    e.Handled = true;
                }
            }
        }

        private void Cut_Copy_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.me/demarini71691");
        }
    }

}