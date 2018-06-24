using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaver1
{
    public class DifficultyEntity
    {
        public string difficulty { get; set; }
        public int difficultyRank { get; set; }
        public string audioPath { get; set; }
        public string jsonPath { get; set; }
        public int offset { get; set; }
        public int oldOffset { get; set; }
    }
}
