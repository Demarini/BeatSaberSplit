using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaver1
{
    public class EventsEntity
    {
        public decimal _time { get; set; }
        public int _type { get; set; }
        public int _value { get; set; }
        public int? _cutDirection { get; set; }
    }
}