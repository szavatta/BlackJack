using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    [Serializable]
    public class Mazziere : GiocatoreSemplice
    {
        public bool CartaCoperta { get; set; }
        [JsonIgnore]
        public StrategiaMazziere Strategia { get; set; }

        public Mazziere(Gioco gioco) : base(gioco)
        {
            Strategia = new SempliceStrategiaMazziere();
            Nome = "Mazziere";
            Carte = new List<Carta>();
        }

        public new enum Puntata
        {
            Chiama = 0,
            Stai = 1,
        }
    }
}
