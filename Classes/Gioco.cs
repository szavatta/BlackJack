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
        public List<Giocatore> Giocatori { get; set; }
        public Mazzo Mazzo { get; set; }
        public Mazziere Mazziere { get; set; }
        public int NumMazziIniziali { get; set; }
        public int PuntataMinima { get; set; }
        public bool Mischia { get; }
        public int Giri { get; set; }


        public Gioco(int giocatori, int numMazzi=6, bool mischia=true)
        {
            Mischia = mischia;
            PuntataMinima = 5;
            Mazzo = new Mazzo();
            Mazzo.CreaMazzo(numMazzi, mischia);
            NumMazziIniziali = numMazzi;
            Giocatori = new List<Giocatore>();
            for (int i = 0; i < giocatori; i++)
            {
                Giocatori.Add(new Giocatore(this));
            }
            Mazziere = new Mazziere(this);
            
        }

        public object Clone()
        {
            return this.MemberwiseClone();

        }

        public void Giocata()
        {
            Giocatori.RemoveAll(q => q.GiocatoreSplit != null);
            Giocatori.ForEach(q => q.Carte = new List<Carta>());
            Mazziere.Carte = new List<Carta>();
            Giocatori.ForEach(q => q.PuntataCorrente = q.Strategia.Puntata(PuntataMinima, 50, Mazzo.GetTrueCount()));

            foreach (Giocatore giocatore in Giocatori.Where(q => q.PuntataCorrente > 0))
            {
                giocatore.Pesca();
            }
            Mazziere.Pesca();
            foreach (Giocatore giocatore in Giocatori.Where(q => q.PuntataCorrente > 0))
            {
                giocatore.Pesca();
            }
            Mazziere.Pesca();

            for (int i = 0;i<Giocatori.Count(); i++)
            {
                if (Giocatori[i].Strategia.Strategy(Giocatori[i], Mazziere, Mazzo.GetTrueCount()) ==
                    Giocatore.Puntata.Dividi)
                {
                    Giocatore clone = (Giocatore)Giocatori[i].Clone();
                    Giocatori[i].Carte.RemoveAt(0);
                    clone.Carte.RemoveAt(1);
                    clone.GiocatoreSplit ??= Giocatori[i];
                    clone.SoldiTotali = 0;
                    Giocatori.Insert(i+1, clone);
                }

                if (Giocatori[i].Carte.Count == 1)
                {
                    Giocatori[i].Pesca();
                }

                while (Giocatori[i].Strategia.Strategy(Giocatori[i], Mazziere, Mazzo.GetTrueCount()) == Giocatore.Puntata.Chiama)
                {
                    Giocatori[i].Pesca();
                }
                if (Giocatori[i].Strategia.Strategy(Giocatori[i], Mazziere, Mazzo.GetTrueCount()) == Giocatore.Puntata.Raddoppia)
                {

                    if (Giocatori[i].Carte.Count == 2 && Giocatori[i].GiocatoreSplit == null)
                    {
                        Giocatori[i].PuntataCorrente *= 2;
                    }
                    Giocatori[i].Pesca();
                }
            }
            while (Mazziere.Strategia.Strategy(Mazziere) == Mazziere.Puntata.Chiama)
            {
                Mazziere.Pesca();
            }


            foreach (var vincente in GiocatoriVincenti())
            {
                Mazziere.SoldiTotali -= vincente.PuntataCorrente;
                vincente.SoldiTotali += vincente.PuntataCorrente;
                
            }

            foreach (var perdente in GiocatoriPerdenti())
            {
                Mazziere.SoldiTotali += perdente.PuntataCorrente;
                perdente.SoldiTotali -= perdente.PuntataCorrente;
            }

            foreach (var giocatore in Giocatori.Where(q => q.GiocatoreSplit != null))
            {
                giocatore.GiocatoreSplit.SoldiTotali += giocatore.SoldiTotali;
            }






            GiocatoriVincenti().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Vinto);
            GiocatoriPerdenti().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Perso);
            GiocatoriPari().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Pari);

            try
            {
                if (GiocatoriVincenti().Count() + GiocatoriPerdenti().Count() + GiocatoriPari().Count() != Giocatori.Count())
                    throw new Exception("Non corrispondono i giocatori");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Giri++;
        }

        public List<Giocatore> GiocatoriVincenti()
        {
            var ret = Giocatori.Where(q =>
                q.PuntataCorrente > 0 && q.Punteggio <= 21 && (q.Punteggio > Mazziere.Punteggio || Mazziere.Punteggio > 21) ||
                q.HasBlackJack() && !Mazziere.HasBlackJack()
                ).ToList();

            return ret;
        }

        public List<Giocatore> GiocatoriPari()
        {
            var ret = Giocatori.Where(q =>
                q.PuntataCorrente == 0 || 
                q.Punteggio == Mazziere.Punteggio && q.Punteggio <= 21 &&
                !(q.HasBlackJack() ^ Mazziere.HasBlackJack())
                ).ToList();

            return ret;
        }

        public List<Giocatore> GiocatoriPerdenti()
        {
            var ret = Giocatori.Where(q =>
                q.PuntataCorrente > 0 && 
                (q.Punteggio > 21 || 
                (q.Punteggio < Mazziere.Punteggio && Mazziere.Punteggio <= 21) ||
                !q.HasBlackJack() && Mazziere.HasBlackJack())
                ).ToList();

            return ret;
        }

    }
}
