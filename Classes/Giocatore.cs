using Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    public class Giocatore : GiocatoreSemplice
    {
        public StrategiaGiocatore Strategia { get; set; }

        public Giocatore(Gioco gioco) : base(gioco)
        {
            Strategia = new SempliceStrategiaGiocatore();
        }
    }
}
