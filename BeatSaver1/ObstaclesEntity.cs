using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaver1
{
    public class ObstaclesEntity
    {
        public decimal _time { get; set; }
        public int _lineIndex { get; set; }
        public int _type { get; set; }
        public decimal _duration { get; set; }
        public int _width { get; set; }
    }
}
