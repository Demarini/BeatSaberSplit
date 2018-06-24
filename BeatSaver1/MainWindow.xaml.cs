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

namespace BeatSaver1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BeatSaverEntity bsE = new BeatSaverEntity();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            File.Text = dlg.FileName;
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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var json = System.IO.File.ReadAllText(File.Text);
            bsE = JsonConvert.DeserializeObject<BeatSaverEntity>(json);
            System.IO.StreamReader file =
    new System.IO.StreamReader("tffafnotes.txt");
            string line;
            int counter = 1;
            while ((line = file.ReadLine()) != null)
            {
                start.Text = line.Split('/')[0];
                end.Text = line.Split('/')[1];
                name.Text = "TTFAF - " + counter.ToString() + " " + line.Split('/')[2];

                List<EventsEntity> _truncatedEvents = new List<EventsEntity>();
                List<NotesEntity> _truncatedNotes = new List<NotesEntity>();
                List<ObstaclesEntity> _truncatedObstacles = new List<ObstaclesEntity>();
                decimal conversionNum = decimal.Divide(bsE._beatsPerMinute, 60);
                foreach (EventsEntity e2 in bsE._events)
                {
                    if ((e2._time / conversionNum) >= int.Parse(start.Text) - 1 && (e2._time / conversionNum) <= int.Parse(end.Text) + 1)
                    {
                        _truncatedEvents.Add(e2);
                    }
                }
                foreach (EventsEntity e3 in _truncatedEvents)
                {
                    if (decimal.Parse(start.Text) > 1)
                    {
                        e3._time = Math.Round(e3._time - (decimal.Parse(start.Text) - 1) * conversionNum + (conversionNum * 2), 2);
                    }
                    else
                    {
                        e3._time = Math.Round(e3._time - (decimal.Parse(start.Text)) * conversionNum, 2);
                    }

                }
                foreach (NotesEntity n2 in bsE._notes)
                {
                    if ((n2._time / conversionNum) >= int.Parse(start.Text) - 1 && (n2._time / conversionNum) <= int.Parse(end.Text) + 1)
                    {
                        _truncatedNotes.Add(n2);
                    }
                }
                foreach (NotesEntity n3 in _truncatedNotes)
                {
                    if (decimal.Parse(start.Text) > 1)
                    {
                        n3._time = Math.Round(n3._time - (decimal.Parse(start.Text) - 1) * conversionNum + (conversionNum * 2), 2);
                    }
                    else
                    {
                        n3._time = Math.Round(n3._time - (decimal.Parse(start.Text)) * conversionNum, 2);
                    }

                }
                foreach (ObstaclesEntity o2 in bsE._obstacles)
                {
                    if ((o2._time / conversionNum) >= int.Parse(start.Text) - 1 && (o2._time / conversionNum) <= int.Parse(end.Text) + 1)
                    {
                        _truncatedObstacles.Add(o2);
                    }
                }
                foreach (ObstaclesEntity o3 in _truncatedObstacles)
                {
                    if (decimal.Parse(start.Text) > 1)
                    {
                        o3._time = Math.Round(o3._time - (decimal.Parse(start.Text) - 1) * conversionNum + (conversionNum * 2), 2);
                    }
                    else
                    {
                        o3._time = Math.Round(o3._time - (decimal.Parse(start.Text)) * conversionNum, 2);
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
                var outputPath = output.Text + "/" + name.Text;
                System.IO.Directory.CreateDirectory(outputPath);
                System.IO.File.WriteAllText(outputPath + "\\Expert.json", newJson);
                var path = System.IO.Path.GetDirectoryName(File.Text);
                //System.IO.File.Copy(path + "\\info.json", outputPath + "\\info.json");
                var json2 = System.IO.File.ReadAllText(path + "\\info.json");
                InfoEntity iE = JsonConvert.DeserializeObject<InfoEntity>(json2);
                List<DifficultyEntity> difficulties = new List<DifficultyEntity>();
                DifficultyEntity d = new DifficultyEntity();
                d.difficulty = "Expert";
                d.difficultyRank = 4;
                d.audioPath = "TTFAF - " + counter.ToString() + " " + line.Split('/')[2] + ".ogg";
                d.jsonPath = "Expert.json";
                d.offset = 0;
                d.oldOffset = 0;
                difficulties.Add(d);
                iE.difficultyLevels = difficulties.ToArray();
                iE.songName = "TTFAF - " + counter.ToString() + " " + line.Split('/')[2];
                var newJson2 = JsonConvert.SerializeObject(iE,
                                Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                System.IO.File.WriteAllText(outputPath + "\\info.json", newJson2);
                System.IO.File.Copy(path + "\\cover.jpg", outputPath + "\\cover.jpg");

                double duration = (double.Parse(end.Text) - double.Parse(start.Text) + 4);
                if (double.Parse(start.Text) > 1)
                {
                    var result = Conversion.Split(path + "\\TTFAF.ogg", outputPath + "\\" + "TTFAF - " + counter.ToString() + " " + line.Split('/')[2] + ".ogg", TimeSpan.FromSeconds(double.Parse(start.Text) - 3), TimeSpan.FromSeconds(duration)).Start();
                }
                else
                {
                    var result = Conversion.Split(path + "\\TTFAF.ogg", outputPath + "\\" + "TTFAF - " + counter.ToString() + " " + line.Split('/')[2] + ".ogg", TimeSpan.FromSeconds(double.Parse(start.Text)), TimeSpan.FromSeconds(duration)).Start();
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            File_Copy1.Text = dlg.FileName;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var json = System.IO.File.ReadAllText(File_Copy1.Text);
            BeatSaverEntity bsE1 = JsonConvert.DeserializeObject<BeatSaverEntity>(json);
            decimal conversionNum = decimal.Divide(bsE1._beatsPerMinute, 60);
            for(int i = 0;i < bsE1._events.Count();i++)
            {
                bsE1._events[i]._time = Math.Round(bsE1._events[i]._time + decimal.Parse(SecondsOffset.Text) * conversionNum, 2);
            }
            for (int i = 0; i < bsE1._notes.Count(); i++)
            {
                bsE1._notes[i]._time = Math.Round(bsE1._notes[i]._time + decimal.Parse(SecondsOffset.Text) * conversionNum, 2);
            }
            for (int i = 0; i < bsE1._obstacles.Count(); i++)
            {
                bsE1._obstacles[i]._time = Math.Round(bsE1._obstacles[i]._time + decimal.Parse(SecondsOffset.Text) * conversionNum, 2);
            }
            var newJson2 = JsonConvert.SerializeObject(bsE1,
                                Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
            System.IO.File.WriteAllText(File_Copy1.Text, newJson2);
        }
    }
}