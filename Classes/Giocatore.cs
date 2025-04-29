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
        public decimal PuntataAssicurazione { get; set; }
        public decimal PuntataCorrente { get; set; }
        public decimal PuntataPrecedente { get; set; }
        public bool SceltaAssicurazione { get; set; }
        public decimal PuntataBase { get; set; }
        public string ProssimaScelta { get; set; }
        public decimal ProssimaPuntata { get; set; }


        public Giocatore(Gioco gioco = null, StrategiaGiocatore strategia = null, decimal soldi = 0, string nome = "", decimal puntataBase = 1) : base(gioco)
        {
            Carte = new List<Carta>();
            Nome = string.IsNullOrEmpty(nome) ? $"Giocatore { (gioco != null ? gioco.Giocatori.Count + 1 : 0) }" : nome;
            Id = DateTime.Now.Ticks.ToString();

            SoldiTotali = soldi;
            PuntataBase = puntataBase;

            if (strategia == null)
                Strategia = new SempliceStrategiaGiocatore();
            else
                Strategia = strategia;

            gioco.Log.AppendLine($"Aggiunto giocatore {Nome} strategia {strategia?.ToString().Replace("Classes.", "")}");
        }

        public enum EnumRisultato
        {
            Pari = 0,
            Vinto = 1,
            Perso = 2,
        }

        public Giocatore Raddoppia()
        {
            if (!Gioco.RaddoppiaNonDisponibile)
            {
                Gioco.Log.AppendLine($"{Nome} si arrende");

                PuntataCorrente *= 2;
                Chiama();
                Stai();
            }
            return this;
        }

        public Giocatore Arresa()
        {
            if (Gioco.ArresaDisponibile)
            {
                IsArreso = true;
                SoldiTotali -= PuntataCorrente / 2;
                Gioco.Mazziere.SoldiTotali += PuntataCorrente / 2;
                //PuntataCorrente = 0;
                Stai();
            }
            return this;
        }
        public Giocatore Stai()
        {
            CanSplit = false;
            Gioco.Iniziato = true;
            this.ProssimaScelta = "";
            PassaMano();
            Giocatore g = Gioco.Giocatori.Where(q => q.Id == Gioco.IdGiocatoreMano).FirstOrDefault();
            if (g != null && g.Punteggio == 21)
                g.Stai();

            ProssimaScelta = ProxScelta();

            return this;
        }

        public string ProxScelta()
        {
            if (Gioco.SecondaCartaInizialeMazziere &&
                !SceltaAssicurazione &&
                Carte.Count == 2 &&
                GiocatoreSplit == null &&
                Gioco.Mazziere.Carte.Count > 0 && Gioco.Mazziere.Carte[0].Numero == Carta.NumeroCarta.Asso)
            {
                if (Strategia.Assicurazione(this, Strategia.TrueCount))
                    return "Assicurazione SI";
                else
                    return "Assicurazione NO";
            }
            else
            {
                return Scelta().ToString();
            }
        }

        public Giocatore Punta(decimal puntata)
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
            clone.Chiama();
            if (Punteggio >= 21)
                Stai();

            ProssimaScelta = ProxScelta();
            clone.ProssimaScelta = ProxScelta();

            return this;
        }

        public Giocatore Assicurazione()
        {
            PuntataAssicurazione = PuntataCorrente / 2;

            Gioco.Log.AppendLine($"{Nome} accetta assicurazione");

            return this;
        }

        public void Punta()
        {
            PuntataCorrente = Strategia.Puntata(this, Gioco.PuntataMinima, PuntataBase, Strategia.TrueCount);
            if (Gioco.PuntataMassima.HasValue && PuntataCorrente > Gioco.PuntataMassima)
                PuntataCorrente = Gioco.PuntataMassima.Value;

            Gioco.Log.AppendLine($"{Nome} punta {PuntataCorrente}");
        }

        public Giocata Scelta()
        {
            if (Carte.Count < 2)
                return Giocata.Chiama;
            else
                return Strategia.Strategy(this, Gioco.Mazziere, Strategia.TrueCount);
        }

        public void PassaMano()
        {
            Giocatore next = Gioco.Giocatori.SkipWhile(q => q.Id != Id).Skip(1).FirstOrDefault();
            if (next != null)
            {
                Gioco.IdGiocatoreMano = next.Id;
                if (next.Carte.Count == 1)
                    next.Chiama();
            }
            else
            {
                Gioco.IdGiocatoreMano = null;
                Gioco.Mazziere.CartaCoperta = false;
                if (Gioco.SecondaCartaInizialeMazziere && Gioco.Mazziere.Carte.Count == 2)
                    Gioco.Giocatori.ForEach(q => q.Strategia.Conta(Gioco.Mazziere.Carte.Last()));
                if (Gioco.Giocatori.Where(q => q.HaSballato() == false && !q.IsArreso).Count() > 0)
                {
                    while (Gioco.Mazziere.Strategia.Strategy(Gioco.Mazziere) == Mazziere.Giocata.Chiama)
                    {
                        Gioco.Mazziere.Chiama();
                    }
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

        public override Carta Chiama(bool conta = true)
        {
            Carta carta = base.Chiama();

            if (Carte.Count == 2 && Carte[0].Valore == Carte[1].Valore)
                CanSplit = true;
            else
                CanSplit = false;

            //if (Punteggio >= 21)
            //    Stai();

            ProssimaScelta = ProxScelta();

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
