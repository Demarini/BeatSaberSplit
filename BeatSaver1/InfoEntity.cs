using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaver1
{
    public class InfoEntity
    {
        public string songName { get; set; }
        public string songSubName { get; set; }
        public string authorName { get; set; }
        public decimal beatsPerMinute { get; set; }
        public decimal previewStartTime { get; set; }
        public decimal previewDuration { get; set; }
        public string coverImagePath { get; set; }
        public string environmentName { get; set; }
        public DifficultyEntity[] difficultyLevels { get; set; }
    }
}
