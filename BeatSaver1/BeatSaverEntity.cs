using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaver1
{
    public class BeatSaverEntity
    {
        public string _version { get; set; }
        public int _beatsPerMinute { get; set; }
        public int _beatsPerBar { get; set; }
        public int _noteJumpSpeed { get; set; }
        public int _shuffle { get; set; }
        public decimal _shufflePeriod { get; set; }
        public EventsEntity[] _events { get; set; }
        public NotesEntity[] _notes { get; set; }
        public ObstaclesEntity[] _obstacles { get; set; }
    }
}
