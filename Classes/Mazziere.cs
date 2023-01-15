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

        public Puntata Scelta()
        {
            return this.Strategia.Strategy(this);
        }

        public override Carta Pesca(int percMin = 20, bool verifica21 = false)
        {
            Carta carta = base.Pesca(percMin, verifica21);
            Gioco.Giocatori.ForEach(q => q.Strategia.Conta(carta));
            return carta;
        }
    }
}
