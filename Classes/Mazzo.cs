using System;
using System.Collections.Generic;
using System.Linq;

namespace Classes
{
    [Serializable]
    public class Mazzo
    {
        public enum EnumRetro
        {
            blu,
            rosso
        }
        public List<Carta> Carte { get; set; }
        public List<Carta> Scarti { get; set; }
        public EnumRetro Retro { get; set; }

        public void CreaMazzo(Gioco gioco)
        {
            gioco.CambiMazzi++;
            Carte = new List<Carta>();
            Scarti = new List<Carta>();
            gioco.Giocatori.ForEach(q => q.Strategia.Conteggio = 0);

            var rnd = new Random();
            Retro = (EnumRetro)Math.Round(rnd.NextDouble(), 0);

            for (int j = 0; j < gioco.NumMazziIniziali; j++)
            {
                for (int i = 1; i <= 4; i++)
                {
                    for (int ii = 1; ii <= 13; ii++)
                    {
                        Carte.Add(new Carta((Carta.NumeroCarta)ii, (Carta.SemeCarta)i));
                    }
                }
            }

            if (gioco.Mischia)
            {
                if (gioco.RandomMischiata == null)
                    rnd = new Random();
                else
                    rnd = new Random(gioco.RandomMischiata.Value * gioco.CambiMazzi);
                Carte = Carte.OrderBy(item => rnd.Next()).ToList();
            }
        }
        public Carta PescaCarta()
        {
            Carta carta = Carte.FirstOrDefault();
            Carte.RemoveAt(0);
            //Conteggio += carta.Conteggio;

            return carta;
        }

        //public int GetTrueCount() => (int)(Conteggio / ((Carte.Count / 52) == 0 ? 1 : (Carte.Count / 52)));

        //public override string ToString()
        //{
        //    return "Conteggio: " + Conteggio;
        //}
    }

}
