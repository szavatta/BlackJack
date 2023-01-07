using Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class Giocatore : GiocatoreSemplice
    {
        [JsonIgnore]
        public StrategiaGiocatore Strategia { get; set; }

        public Giocatore(Gioco gioco, StrategiaGiocatore strategia = null, double soldi = 0, string nome = "") : base(gioco)
        {
            Nome = string.IsNullOrEmpty(nome) ? $"Giocatore { (gioco != null ? gioco.Giocatori.Count + 1 : 0) }" : nome;

            SoldiTotali = soldi;

            if (strategia == null)
                Strategia = new SempliceStrategiaGiocatore();
            else
                Strategia = strategia;
        }
    }
}
