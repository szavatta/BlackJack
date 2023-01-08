using System;
using System.Collections.Generic;
using System.Linq;

namespace Classes
{
    [Serializable]
    public class Mazzo
    {
        public List<Carta> Carte { get; set; }
        public int Conteggio { get; set; }

        public void CreaMazzo(int numMazzi = 1, bool mischia = true)
        {
            Conteggio = 0;
            if (Carte == null)
                Carte = new List<Carta>();

            for (int j = 0; j < numMazzi; j++)
            {
                for (int i = 1; i <= 4; i++)
                {
                    for (int ii = 1; ii <= 13; ii++)
                    {
                        Carte.Add(new Carta((Carta.NumeroCarta)ii, (Carta.SemeCarta)i));
                    }
                }
            }

            if (mischia)
            {
                var rnd = new Random();
                Carte = Carte.OrderBy(item => rnd.Next()).ToList();
            }
        }
        public Carta PescaCarta(int percMin = 20, bool mischia=true)
        {
            if (Carte.Count <= percMin*Carte.Count/100)
                CreaMazzo(mischia:mischia);

            Carta carta = Carte.FirstOrDefault();
            Carte.RemoveAt(0);
            Conteggio += carta.Conteggio;

            return carta;
        }

        public int getTrueCount() => (int)(Conteggio / ((Carte.Count / 52) == 0 ? 1 : (Carte.Count / 52)));

        public override string ToString()
        {
            return "Conteggio: " + Conteggio;
        }
    }

}
