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
        [JsonConverter(typeof(StrategiaConverter))]
        public StrategiaGiocatore Strategia { get; set; }
        public Giocatore GiocatoreSplit { get; set; }
        public EnumRisultato Risultato { get; set; }
        public int ManiVinte { get; set; }
        public int ManiPerse { get; set; }
        public bool CanSplit { get; set; }
        public double PuntataAssicurazione { get; set; }
        public double PuntataCorrente { get; set; }
        public double PuntataPrecedente { get; set; }


        public Giocatore(Gioco gioco = null, StrategiaGiocatore strategia = null, double soldi = 0, string nome = "") : base(gioco)
        {
            Carte = new List<Carta>();
            Nome = string.IsNullOrEmpty(nome) ? $"Giocatore { (gioco != null ? gioco.Giocatori.Count + 1 : 0) }" : nome;
            Id = DateTime.Now.Ticks.ToString();


            SoldiTotali = soldi;

            if (strategia == null)
                Strategia = new SempliceStrategiaGiocatore();
            else
                Strategia = strategia;
        }
        public enum EnumRisultato
        {
            Pari = 0,
            Vinto = 1,
            Perso = 2,
        }

        public Giocatore Raddoppia()
        {
            PuntataCorrente *= 2;
            Chiama();
            return this;
        }

        public Giocatore Stai()
        {
            CanSplit = false;
            Gioco.Iniziato = true;
            PassaMano();
            return this;
        }

        public Giocatore Punta(double puntata)
        {
            Gioco.Iniziato = true;
            PuntataCorrente = puntata;
            return this;
        }

        public Giocatore Esci()
        {
            Gioco.Iniziato = true;
            Gioco.Giocatori.Remove(this);
            return this;
        }

        public Giocatore Split()
        {
            if (!CanSplit)
                throw new Exception("Split non ammesso");
            CanSplit = false;
            Giocatore clone = (Giocatore)this.Clone();
            Carte.RemoveAt(0);
            clone.Nome += " split";
            clone.Id = DateTime.Now.Ticks.ToString();
            clone.Carte.RemoveAt(1);
            clone.GiocatoreSplit = GiocatoreSplit != null ? GiocatoreSplit : this;
            clone.SoldiTotali = 0;
            for (int i = 0; i < Gioco.Giocatori.Count; i++)
            {
                if (Gioco.Giocatori[i].Id == Id)
                {
                    Gioco.Giocatori.Insert(i + 1, clone);
                    break;
                }
            }
            Chiama();

            return this;
        }

        public void Punta()
        {
            PuntataCorrente = Strategia.Puntata(this, Gioco.PuntataMinima, 5, Strategia.GetTrueCount(Gioco.Mazzo.Carte.Count));
            if (Gioco.PuntataMassima.HasValue && PuntataCorrente > Gioco.PuntataMassima)
                PuntataCorrente = Gioco.PuntataMassima.Value;
        }

        public Puntata Scelta()
        {
            if (Carte.Count < 2)
                return Puntata.Chiama;
            else
                return Strategia.Strategy(this, Gioco.Mazziere, Strategia.GetTrueCount(Gioco.Mazzo.Carte.Count));
        }

        public void PassaMano()
        {
            Giocatore next = Gioco.Giocatori.SkipWhile(q => q.Id != Id).Skip(1).FirstOrDefault();
            if (next != null)
            {
                Gioco.IdGiocatoreMano = next.Id;
                if (next.Carte.Count == 1)
                    next.Chiama();
                if (Punteggio >= 21)
                    Stai();
            }
            else
            {
                Gioco.IdGiocatoreMano = null;
                Gioco.Mazziere.CartaCoperta = false;
                while (Gioco.Mazziere.Strategia.Strategy(Gioco.Mazziere) == Mazziere.Puntata.Chiama)
                {
                    Gioco.Mazziere.Chiama();
                }
                Gioco.TerminaMano();
            }

        }

        public object Clone()
        {
            Giocatore giocatore = new Giocatore(Gioco, Strategia, SoldiTotali, Nome);
            giocatore.Carte = new List<Carta>(this.Carte);
            giocatore.PuntataCorrente = PuntataCorrente;
            return giocatore;
        }

        public override Carta Chiama()
        {
            Carta carta = base.Chiama();

            if (Carte.Count == 2 && Carte[0].Valore == Carte[1].Valore)
                CanSplit = true;
            else
                CanSplit = false;

            Strategia.TrueCount = Strategia.GetTrueCount(Gioco.Mazzo.Carte.Count);

            return carta;
        }

        public override string ToString()
        {
            return $"Nome: {Nome}" +
                    $", Soldi Totali: {SoldiTotali}" +
                    $", Puntata: {PuntataCorrente}";
        }
    }
}
