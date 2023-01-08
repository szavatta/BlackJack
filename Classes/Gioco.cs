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
            Giocatori.ForEach(q => q.PuntataCorrente =q.Strategia.Puntata(PuntataMinima, 50, 0));

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
                while (giocatore.Strategia.Strategy(giocatore, Mazziere, Mazzo.getTrueCount()) == Giocatore.Puntata.Chiama)
                {
                    giocatore.Pesca();
                }
                if (giocatore.Strategia.Strategy(giocatore, Mazziere, Mazzo.getTrueCount()) == Giocatore.Puntata.Raddoppia)
                {
                    giocatore.PuntataCorrente *= 2;
                    giocatore.Pesca();
                }
            }
            while (Mazziere.Strategia.Strategy(Mazziere) == Mazziere.Puntata.Chiama)
            {
                Mazziere.Pesca();
            }

            var giocatoriVincenti = Giocatori.Where(q =>
                q.Punteggio <= 21 && (q.Punteggio > Mazziere.Punteggio || Mazziere.Punteggio > 21)).ToList();

            var giocatoriPari =
                Giocatori.Where(q => q.Punteggio == Mazziere.Punteggio && q.Punteggio <= 21).ToList();

            var giocatoriPerdenti = Giocatori.Where(q =>
                q.Punteggio > 21 || (q.Punteggio < Mazziere.Punteggio && Mazziere.Punteggio <= 21)).ToList();

            foreach (var vincente in giocatoriVincenti)
            {
                Mazziere.SoldiTotali -= vincente.PuntataCorrente;
                vincente.SoldiTotali += vincente.PuntataCorrente;
            }

            foreach (var perdente in giocatoriPerdenti)
            {
                Mazziere.SoldiTotali += perdente.PuntataCorrente;
                perdente.SoldiTotali -= perdente.PuntataCorrente;
            }

            if (giocatoriVincenti.Count() + giocatoriPerdenti.Count() + giocatoriPari.Count() != Giocatori.Count())
                throw new Exception("Non corrispondono i giocatori");

        }

    }
}
