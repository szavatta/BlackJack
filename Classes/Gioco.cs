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
            Gioco copy = (Gioco)this.MemberwiseClone();

            return copy;
        }

        public void Giocata()
        {
            Giocatori.ForEach(q => q.Carte = new List<Carta>());
            Mazziere.Carte = new List<Carta>();
            Giocatori.ForEach(q => q.PuntataCorrente = q.Strategia.Puntata(PuntataMinima, 50, Mazzo.GetTrueCount()));

            foreach (Giocatore giocatore in Giocatori)
            {
                giocatore.Pesca();
            }
            Mazziere.Pesca();
            foreach (Giocatore giocatore in Giocatori)
            {
                giocatore.Pesca();
            }
            Mazziere.Pesca();

            foreach (Giocatore giocatore in Giocatori)
            {
                while (giocatore.Strategia.Strategy(giocatore, Mazziere, Mazzo.GetTrueCount()) == Giocatore.Puntata.Chiama)
                {
                    giocatore.Pesca();
                }
                if (giocatore.Strategia.Strategy(giocatore, Mazziere, Mazzo.GetTrueCount()) == Giocatore.Puntata.Raddoppia)
                {
                    giocatore.PuntataCorrente *= 2;
                    giocatore.Pesca();
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
        }

        public List<Giocatore> GiocatoriVincenti()
        {
            var ret = Giocatori.Where(q =>
                q.Punteggio <= 21 && (q.Punteggio > Mazziere.Punteggio || Mazziere.Punteggio > 21) ||
                q.HasBlackJack() && !Mazziere.HasBlackJack()
                ).ToList();

            return ret;
        }

        public List<Giocatore> GiocatoriPari()
        {
            var ret = Giocatori.Where(q => 
                q.Punteggio == Mazziere.Punteggio && q.Punteggio <= 21 &&
                !(q.HasBlackJack() ^ Mazziere.HasBlackJack())
                ).ToList();

            return ret;
        }

        public List<Giocatore> GiocatoriPerdenti()
        {
            var ret = Giocatori.Where(q =>
                q.Punteggio > 21 || 
                (q.Punteggio < Mazziere.Punteggio && Mazziere.Punteggio <= 21) ||
                !q.HasBlackJack() && Mazziere.HasBlackJack()
                ).ToList();

            return ret;
        }

    }
}
