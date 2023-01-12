using Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class Giocatore : GiocatoreSemplice, ICloneable
    {
        public string Id { get; set; }
        [JsonIgnore]
        public StrategiaGiocatore Strategia { get; set; }
        public Giocatore GiocatoreSplit { get; set; }
        public int TipoStrategia { get; set; }
        public EnumRisultato Risultato { get; set; }
        public int ManiVinte { get; set; }
        public int ManiPerse { get; set; }

        public Giocatore(Gioco gioco = null, StrategiaGiocatore strategia = null, double soldi = 0, string nome = "") : base(gioco)
        {
            Nome = string.IsNullOrEmpty(nome) ? $"Giocatore { (gioco != null ? gioco.Giocatori.Count + 1 : 0) }" : nome;
            Id = DateTime.Now.Ticks.ToString();

            if (strategia is Classes.BasicStrategy)
                TipoStrategia = 0;
            else if (strategia is Classes.StrategiaConteggio)
                TipoStrategia = 1;
            else 
                TipoStrategia = 2;

            SoldiTotali = soldi;

            if (strategia == null)
                Strategia = new SempliceStrategiaGiocatore();
            else
                Strategia = strategia;
        }

        public enum EnumRisultato
        {
            Vinto = 0,
            Perso = 1,
            Pari = 2
        }

        public void Raddoppia()
        {
            PuntataCorrente *= 2;
            Pesca();
        }

        public void Stai()
        {
            Gioco.Iniziato = true;
            Gioco.PassaMano(this);
        }

        public object Clone()
        {
            Giocatore giocatore = new Giocatore(Gioco, Strategia, SoldiTotali, Nome);
            giocatore.Carte = new List<Carta>(this.Carte);
            giocatore.PuntataCorrente = PuntataCorrente;
            return giocatore;
        }

        public override Carta Pesca(int percMin = 20)
        {
            Carta carta = base.Pesca(percMin);

            if (Punteggio > 21)
                Stai();

            return carta;
        }
    }
}
