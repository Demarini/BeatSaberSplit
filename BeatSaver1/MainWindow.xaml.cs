using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Model;
using System.IO;
using Declarations;
using Implementation;
using Declarations.Media;
using Declarations.Players;
using Vlc.DotNet.Core;

namespace BeatSaver1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IAudioPlayer player;
        BeatSaverEntity bsE = new BeatSaverEntity();
        string[] files;
        
        public MainWindow()
        {
            InitializeComponent();
            chartTypes.Items.Add("Easy");
            chartTypes.Items.Add("Medium");
            chartTypes.Items.Add("Hard");
            chartTypes.Items.Add("Expert");
            chartTypes.Items.Add("ExpertPlus");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            File.Text = dlg.SelectedPath;
            files = Directory.GetFiles(File.Text);
            string ogg = GetOgg(files);

            IMediaPlayerFactory factory = new MediaPlayerFactory();
            IMedia media = factory.CreateMedia<IMedia>(ogg);
            player = factory.CreatePlayer<IAudioPlayer>();
            player.Open(media);
            player.Events.PlayerPlaying += new EventHandler(Playing);
            //player.Events.MediaEnded += new EventHandler(Events_MediaEnded);
            //player.Events.TimeChanged += new EventHandler<TimeChangedEventArgs>(Events_TimeChanged);
            player.Play();
            //player.
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            //IConversionResult result = await Conversion.Split()
            try
            {
                var json = System.IO.File.ReadAllText(File.Text);
                bsE = JsonConvert.DeserializeObject<BeatSaverEntity>(json);
                System.Windows.MessageBox.Show("Success!");
            }
            catch
            {
                System.Windows.MessageBox.Show("Error!");
            }

        }
        private string GetOgg(string[] files)
        {
            foreach(string s in files)
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
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string[] files = Directory.GetFiles(File.Text);
            
            string ogg = GetOgg(files);
            string chart = GetChart(files, "Expert");
            string info = GetInfo(files);
            string directoryName = new DirectoryInfo(File.Text).Name;
            var json = System.IO.File.ReadAllText(chart);
            bsE = JsonConvert.DeserializeObject<BeatSaverEntity>(json);
            System.IO.StreamReader file =
    new System.IO.StreamReader("tffafnotes.txt");
            string line;
            int counter = 1;
            while ((line = file.ReadLine()) != null)
            {
                string start = line.Split('/')[0];
                string end = line.Split('/')[1];
                string name = directoryName + " - " + counter.ToString() + " " + line.Split('/')[2];

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
                var outputPath = output.Text + "/" + name;
                System.IO.Directory.CreateDirectory(outputPath);
                System.IO.File.WriteAllText(outputPath + "\\" + chartTypes.SelectedValue + ".json", newJson);
                var path = System.IO.Path.GetDirectoryName(File.Text);
                //System.IO.File.Copy(path + "\\info.json", outputPath + "\\info.json");
                var json2 = System.IO.File.ReadAllText(File.Text + "\\info.json");
                InfoEntity iE = JsonConvert.DeserializeObject<InfoEntity>(json2);
                List<DifficultyEntity> difficulties = new List<DifficultyEntity>();
                DifficultyEntity d = new DifficultyEntity();
                d.difficulty = "Expert";
                d.difficultyRank = 4;
                d.audioPath = directoryName +" - " + counter.ToString() + " " + line.Split('/')[2] + ".ogg";
                d.jsonPath = chartTypes.SelectedValue + ".json";
                d.offset = 0;
                d.oldOffset = 0;
                difficulties.Add(d);
                iE.difficultyLevels = difficulties.ToArray();
                iE.songName = directoryName + " - " + counter.ToString() + " " + line.Split('/')[2];
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
                    var result = Conversion.Split(ogg, outputPath + "\\" + directoryName + " - " + counter.ToString() + " " + line.Split('/')[2] + ".ogg", TimeSpan.FromSeconds(double.Parse(start) - 3), TimeSpan.FromSeconds(duration)).Start();
                }
                else
                {
                    var result = Conversion.Split(ogg, outputPath + "\\" + directoryName + " - " + counter.ToString() + " " + line.Split('/')[2] + ".ogg", TimeSpan.FromSeconds(double.Parse(start)), TimeSpan.FromSeconds(duration)).Start();
                }
                counter++;
            }
            file.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.ShowDialog();
                output.Text = dlg.SelectedPath;
            }
            catch
            {
                System.Windows.MessageBox.Show("Error!");
            }
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            //player.Play()
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Playing(object sender, EventArgs e)
        {

        }
        private void Slider_DragEnter(object sender, System.Windows.DragEventArgs e)
        {

        }
        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
        //    dlg.ShowDialog();
        //    File_Copy1.Text = dlg.FileName;
        //}

        //private void Button_Click_4(object sender, RoutedEventArgs e)
        //{
        //    var json = System.IO.File.ReadAllText(File_Copy1.Text);
        //    BeatSaverEntity bsE1 = JsonConvert.DeserializeObject<BeatSaverEntity>(json);
        //    decimal conversionNum = decimal.Divide(bsE1._beatsPerMinute, 60);
        //    for(int i = 0;i < bsE1._events.Count();i++)
        //    {
        //        bsE1._events[i]._time = Math.Round(bsE1._events[i]._time + decimal.Parse(SecondsOffset.Text) * conversionNum, 2);
        //    }
        //    for (int i = 0; i < bsE1._notes.Count(); i++)
        //    {
        //        bsE1._notes[i]._time = Math.Round(bsE1._notes[i]._time + decimal.Parse(SecondsOffset.Text) * conversionNum, 2);
        //    }
        //    for (int i = 0; i < bsE1._obstacles.Count(); i++)
        //    {
        //        bsE1._obstacles[i]._time = Math.Round(bsE1._obstacles[i]._time + decimal.Parse(SecondsOffset.Text) * conversionNum, 2);
        //    }
        //    var newJson2 = JsonConvert.SerializeObject(bsE1,
        //                        Newtonsoft.Json.Formatting.None,
        //                        new JsonSerializerSettings
        //                        {
        //                            NullValueHandling = NullValueHandling.Ignore
        //                        });
        //    System.IO.File.WriteAllText(File_Copy1.Text, newJson2);
        //}
    }
}