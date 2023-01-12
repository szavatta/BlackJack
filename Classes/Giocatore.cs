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
        public bool CanSplit { get; set; }

        public Giocatore(Gioco gioco = null, StrategiaGiocatore strategia = null, double soldi = 0, string nome = "") : base(gioco)
        {
            Carte = new List<Carta>();
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
        public void Split()
        {
            CanSplit = false;
            Giocatore clone = (Giocatore)this.Clone();
            Carte.RemoveAt(0);
            clone.Nome += " split";
            clone.Id = DateTime.Now.Ticks.ToString();
            clone.Carte.RemoveAt(1);
            clone.GiocatoreSplit ??= this;
            clone.SoldiTotali = 0;
            for (int i = 0; i < Gioco.Giocatori.Count; i++)
            {
                if (Gioco.Giocatori[i].Id == Id)
                {
                    Gioco.Giocatori.Insert(i + 1, clone);
                    break;
                }
            }
        }

        public void Punta()
        {
            PuntataCorrente = Strategia.Puntata(Gioco.PuntataMinima, 50, Gioco.Mazzo.GetTrueCount());
            Carte = new List<Carta>();
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

            if (Carte.Count == 2 && Carte[0].Numero == Carte[1].Numero)
                CanSplit = true;
            else
                CanSplit = false;
            
            if (Punteggio > 21)
                Stai();

            return carta;
        }
    }
}
