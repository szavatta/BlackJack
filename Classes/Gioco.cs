using Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class Gioco : ICloneable
    {
        private Gioco() { }
        public class GiocoBuilder
        {
            private static Gioco gioco;
            private int numGiocatori;

            public static GiocoBuilder Init()
            {
                gioco = new Gioco();
                return new GiocoBuilder();
            }

            public GiocoBuilder AggiungiNumeroGiocatori(int numGiocatori)
            {
                this.numGiocatori = numGiocatori;
                return this;
            }

            public GiocoBuilder AggiungiNome(string nome)
            {
                gioco.Nome = nome;
                return this;
            }

            public GiocoBuilder AggiungiPuntataMinima(int puntataMinima)
            {
                gioco.PuntataMinima = puntataMinima;
                return this;
            }

            public GiocoBuilder AggiungiPuntataMassima(int puntataMassima)
            {
                gioco.PuntataMassima = puntataMassima;
                return this;
            }

            public GiocoBuilder AggiungiRaddoppiaNonDisponibile()
            {
                gioco.RaddoppiaNonDisponibile = true;
                return this;
            }

            public GiocoBuilder AggiungiArresaDisponibile(bool arr = true)
            {
                gioco.ArresaDisponibile = arr;
                return this;
            }

            public GiocoBuilder AggiungiMischiata(int? random = null)
            {
                gioco.Mischia = true;
                gioco.RandomMischiata = random;
                return this;
            }

            public GiocoBuilder AggiungiMazzi(int numMazzi)
            {
                gioco.NumMazziIniziali = numMazzi;
                return this;
            }

            public GiocoBuilder AggiungiPercentualeMischiata(int? perc)
            {
                gioco.PercMischiata = perc > 5 ? perc : 5;
                return this;
            }

            public GiocoBuilder AggiungiSecondaCartaInizialeMazziere(bool scm = true)
            {
                gioco.SecondaCartaInizialeMazziere = scm;
                return this;
            }

            public GiocoBuilder AggiungiVisualizzaSceltaStrategia(bool vis = false)
            {
                gioco.VisualizzaSceltaStrategia = vis;
                return this;
            }


            public Gioco build()
            {
                return new Gioco(numGiocatori, gioco.NumMazziIniziali, gioco.Mischia, gioco.RandomMischiata, gioco.Nome, gioco.PuntataMinima, gioco.PuntataMassima, gioco.PercMischiata, gioco.SecondaCartaInizialeMazziere, gioco.RaddoppiaNonDisponibile, gioco.ArresaDisponibile, gioco.VisualizzaSceltaStrategia);
            }

        }
        public List<Giocatore> Giocatori { get; set; }
        public Mazzo Mazzo { get; set; }
        public Mazziere Mazziere { get; set; }

        public int NumMazziIniziali { get; set; }
        public decimal PuntataMinima { get; set; }
        public decimal? PuntataMassima { get; set; }
        public bool Mischia { get; set; }
        public bool RaddoppiaNonDisponibile { get; set; }
        public bool ArresaDisponibile { get; set; }
        public int? RandomMischiata { get; set; }
        public int? PercMischiata { get; set; }

        public int Giri { get; set; }
        public int CambiMazzi { get; set; }
        public string Nome { get; set; }
        public string Id { get; set; }
        public List<Giocatore> GiocatoriSplit { get; set; }
        public string IdGiocatoreMano { get; set; }
        public bool Iniziato { get; set; }
        public bool SecondaCartaInizialeMazziere { get; set; }
        public bool VisualizzaSceltaStrategia { get; set; }
        DateTime DataCreazione { get; set; }
        public StringBuilder Log { get; set; }

        public Gioco(int giocatori, int numMazzi=6, bool mischia=true, int? randomMischiata = null, string nome = null, decimal puntataMinima = 5, decimal? puntataMassima = null, int? percMischiata = null, bool secondaCartaInizialeMazziere = true, bool raddoppiaNonDisponibile = false, bool arresaDisponibile = false, bool visualizzaSceltaStrategia = false)
        {
            GiocatoriSplit = new List<Giocatore>();
            Mazziere = new Mazziere(this);
            Giocatori = new List<Giocatore>();
            Id = DateTime.Now.Ticks.ToString();
            DataCreazione = DateTime.Now;
            Mazzo = new Mazzo();
            Mischia = mischia;
            RaddoppiaNonDisponibile = raddoppiaNonDisponibile;
            ArresaDisponibile = arresaDisponibile;
            RandomMischiata = randomMischiata;
            PercMischiata = percMischiata ?? 20;
            PuntataMinima = puntataMinima;
            PuntataMassima = puntataMassima;
            NumMazziIniziali = numMazzi;
            SecondaCartaInizialeMazziere = secondaCartaInizialeMazziere;
            VisualizzaSceltaStrategia = visualizzaSceltaStrategia;
            Log = new StringBuilder();
            Log.AppendLine("Inizio partita");
            Mazzo.CreaMazzo(this);
            if (string.IsNullOrEmpty(nome))
                Nome = "Partita";
            else
                Nome = nome;
            for (int i = 0; i < giocatori; i++)
            {
                Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(this).build());
            }
        }

        public object Clone()
        {
            Gioco cloned = (Gioco)this.MemberwiseClone();
            cloned.Mazzo = (Mazzo)this.Mazzo.Clone();
            return cloned;
        }

        public void Giocata()
        {
            GiocataIniziale();

            if (!Mazziere.HasBlackJack())
            {
                bool isInGioco = false;
                for (int i = 0; i < Giocatori.Count(); i++)
                {
                    GiocataGiocatore(i);
                    if (!Giocatori[i].HaSballato() && !Giocatori[i].IsArreso)
                        isInGioco = true;
                }

                if (isInGioco)
                {
                    if (!SecondaCartaInizialeMazziere)
                        Mazziere.Chiama();

                    while (Mazziere.Scelta() == Mazziere.Giocata.Chiama)
                    {
                        Mazziere.Chiama();
                    }
                }
            }
            else
                Giocatori.ForEach(q => q.Stai());

            TerminaMano();
            try
            {
                if (GiocatoriVincenti().Count() + GiocatoriPerdenti().Count() + GiocatoriPari().Count() != Giocatori.Count())
                    throw new Exception("Non corrispondono i giocatori");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GiocataGiocatore(int i)
        {
            if (Giocatori[i].PuntataCorrente > 0)
            {
                if (Giocatori[i].Carte.Count < 2)
                {
                    Giocatori[i].Chiama();
                }

                if (Giocatori[i].Carte.Count == 2 &&
                    Giocatori[i].GiocatoreSplit == null &&
                    Mazziere.Carte[0].Numero == Carta.NumeroCarta.Asso &&
                    Giocatori[i].Strategia.Assicurazione(Giocatori[i], Giocatori[i].Strategia.TrueCount)) 
                {
                    Giocatori[i].Assicurazione();
                }

                if (Giocatori[i].Scelta() == GiocatoreSemplice.Giocata.Arresa)
                    Giocatori[i].Arresa();

                while (Giocatori[i].Scelta() == GiocatoreSemplice.Giocata.Dividi)
                {
                    Dividi(i);
                }

                while (Giocatori[i].Scelta() == GiocatoreSemplice.Giocata.Chiama)
                {
                    Giocatori[i].Chiama();
                }

                if (Giocatori[i].Scelta() == GiocatoreSemplice.Giocata.Raddoppia)
                {
                    Raddoppia(i);
                }
                if (Giocatori[i].Scelta() == GiocatoreSemplice.Giocata.Stai && Giocatori[i].Punteggio <= 21)
                {
                    Log.AppendLine($"{Giocatori[i].Nome} sta");
                }

            }
        }

        public void GiocataIniziale()
        {
            Inizializza();
            Giocatori.ForEach(q => q.Punta());
            DistribuisciCarteIniziali();
        }

        private void Raddoppia(int i)
        {
            if (Giocatori[i].Carte.Count == 2)
            {
                Giocatori[i].PuntataCorrente *= 2;
            }

            Log.AppendLine($"{Giocatori[i].Nome} raddoppia");

            Giocatori[i].Chiama();
        }

        private void Dividi(int i)
        {
            Giocatore clone = (Giocatore)Giocatori[i].Clone();
            Giocatori[i].Carte.RemoveAt(0);
            clone.Nome += " split";
            clone.PuntataAssicurazione = 0;
            clone.Carte.RemoveAt(1);
            clone.GiocatoreSplit = Giocatori[i].GiocatoreSplit != null ? Giocatori[i].GiocatoreSplit : Giocatori[i];
            clone.SoldiTotali = 0;
            Giocatori.Insert(i + 1, clone);

            Log.AppendLine($"{Giocatori[i].Nome} effettua lo split");

            if (Giocatori[i].Carte.Count == 1)
            {
                Giocatori[i].Chiama();
            }
        }

        public void TerminaMano()
        {
            Log.AppendLine($"Mano terminata");

            foreach (var giocatore in GiocatoriVincenti())
            {
                giocatore.ManiVinte++;
                decimal paga = giocatore.HasBlackJack() ? giocatore.PuntataCorrente * 3 / 2 : giocatore.PuntataCorrente;
                Mazziere.SoldiTotali -= paga;
                giocatore.SoldiTotali += paga;
                Log.AppendLine($"{giocatore.Nome} vince, nuovo saldo {giocatore.SoldiTotali}");
                Log.AppendLine($"Running count: {giocatore.Strategia.Conteggio}");
                Log.AppendLine($"True count: {giocatore.Strategia.TrueCount}");
            }

            foreach (var giocatore in GiocatoriPerdenti())
            {
                giocatore.ManiPerse++;
                if (!giocatore.IsArreso)
                {
                    Mazziere.SoldiTotali += giocatore.PuntataCorrente;
                    giocatore.SoldiTotali -= giocatore.PuntataCorrente;
                }
                Log.AppendLine($"{giocatore.Nome} perde, nuovo saldo {giocatore.SoldiTotali}");
                Log.AppendLine($"Running count: {giocatore.Strategia.Conteggio}");
                Log.AppendLine($"True count: {giocatore.Strategia.TrueCount}");
            }

            foreach (var giocatore in GiocatoriPari())
            {
                Log.AppendLine($"{giocatore.Nome} pareggia, nuovo saldo {giocatore.SoldiTotali}");
                Log.AppendLine($"Running count: {giocatore.Strategia.Conteggio}");
                Log.AppendLine($"True count: {giocatore.Strategia.TrueCount}");
            }

            foreach (var giocatore in Giocatori.Where(q => q.GiocatoreSplit != null))
            {
                giocatore.GiocatoreSplit.ManiVinte += giocatore.ManiVinte;
                giocatore.GiocatoreSplit.ManiPerse += giocatore.ManiPerse;
                giocatore.GiocatoreSplit.SoldiTotali += giocatore.SoldiTotali;
            }

            foreach (var giocatore in Giocatori.Where(q => q.PuntataAssicurazione > 0))
            {
                if (Mazziere.HasBlackJack())
                {
                    giocatore.SoldiTotali += giocatore.PuntataAssicurazione * 2;
                    Mazziere.SoldiTotali -= giocatore.PuntataAssicurazione * 2;
                }
                else 
                {
                    giocatore.SoldiTotali -= giocatore.PuntataAssicurazione;
                    Mazziere.SoldiTotali += giocatore.PuntataAssicurazione;
                }
            }

            GiocatoriVincenti().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Vinto);
            GiocatoriPerdenti().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Perso);
            GiocatoriPari().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Pari);

            GiocatoriSplit = Giocatori.Where(q => q.GiocatoreSplit != null).ToList();

        }

        public void Inizializza()
        {
            Log.AppendLine("");
            Giocatori.RemoveAll(q => q.GiocatoreSplit != null);

            if (Mazzo.Carte.Count <= (Mazzo.NumCarteSingoloMazzo * NumMazziIniziali) * PercMischiata / 100)
                Mazzo.CreaMazzo(this);

            foreach (Giocatore g in Giocatori)
            {
                Mazzo.Scarti.AddRange(g.Carte);
                g.Carte = new List<Carta>();
                g.PuntataPrecedente = g.PuntataCorrente;
                g.PuntataCorrente = 0;
                g.ProssimaPuntata = g.Strategia.Puntata(g, PuntataMinima, g.PuntataBase, g.Strategia.TrueCount);
                g.PuntataAssicurazione= 0;
                g.SceltaAssicurazione = false;
                g.IsArreso = false;
                //g.Risultato = Giocatore.EnumRisultato.Pari;
            }
            Mazzo.Scarti.AddRange(Mazziere.Carte);
            Mazziere.Carte = new List<Carta>();
            Mazziere.CartaCoperta = true;
            Iniziato = false;
            Giri++;
            Log.AppendLine($"Nuova mano n.{Giri}");
        }

        public void DistribuisciCarteIniziali()
        {
            IdGiocatoreMano = Giocatori[0].Id;
            Giocatori.Where(q => q.PuntataCorrente > 0).ToList().ForEach(q => q.Chiama());
            Mazziere.Chiama();
            Giocatori.Where(q => q.PuntataCorrente > 0).ToList().ForEach(q => q.Chiama());
            if (SecondaCartaInizialeMazziere)
                Mazziere.Chiama(false);

            //if (Mazziere.HasBlackJack())
            //    Giocatori.Where(q => q.PuntataCorrente > 0).ToList().ForEach(q => q.Stai());
            //else 
            //if (Giocatori[0].Punteggio >= 21)
            //    Giocatori[0].Stai();

        }

        public List<Giocatore> GiocatoriVincenti()
        {
            var ret = Giocatori.Where(q =>
                !q.IsArreso &&
                q.PuntataCorrente > 0 && 
                q.Punteggio <= 21 && (q.Punteggio > Mazziere.Punteggio || Mazziere.Punteggio > 21) ||
                (q.HasBlackJack() && !Mazziere.HasBlackJack())
                ).ToList();

            return ret;
        }

        public List<Giocatore> GiocatoriPari()
        {
            var ret = Giocatori.Where(q =>
                !q.IsArreso && (
                q.PuntataCorrente == 0 || 
                q.Punteggio == Mazziere.Punteggio && q.Punteggio <= 21 &&
                !(q.HasBlackJack() ^ Mazziere.HasBlackJack()))
                ).ToList();

            return ret;
        }

        public List<Giocatore> GiocatoriPerdenti()
        {
            var ret = Giocatori.Where(q =>
                q.IsArreso ||
                (q.PuntataCorrente > 0 && 
                (q.Punteggio > 21 || 
                (q.Punteggio < Mazziere.Punteggio && Mazziere.Punteggio <= 21) ||
                !q.HasBlackJack() && Mazziere.HasBlackJack()))
                ).ToList();

            return ret;
        }

    }
}
