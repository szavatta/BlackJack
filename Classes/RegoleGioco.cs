using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    class RegoleGioco
    {
        public int NumMazziIniziali { get; set; }
        public int PuntataMinima { get; set; }
        public int? PuntataMassima { get; set; }
        public bool Mischia { get; set; }
        public int? RandomMischiata { get; set; }
        public int? PercMischiata { get; set; }
    }
}
