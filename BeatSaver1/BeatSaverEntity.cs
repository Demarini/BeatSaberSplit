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
        public decimal _beatsPerMinute { get; set; }
        public decimal _beatsPerBar { get; set; }
        public decimal _noteJumpSpeed { get; set; }
        public decimal _shuffle { get; set; }
        public decimal _shufflePeriod { get; set; }
        public EventsEntity[] _events { get; set; }
        public NotesEntity[] _notes { get; set; }
        public ObstaclesEntity[] _obstacles { get; set; }
    }
}
