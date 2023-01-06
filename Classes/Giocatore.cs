using Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class Giocatore : GiocatoreSemplice
    {
        public StrategiaGiocatore Strategia { get; set; }

        public Giocatore(Gioco gioco, StrategiaGiocatore strategia = null, double soldi = 0) : base(gioco)
        {
            SoldiTotali = soldi;

            if (strategia == null)
                Strategia = new SempliceStrategiaGiocatore();
            else
                Strategia = strategia;
        }
    }
}
