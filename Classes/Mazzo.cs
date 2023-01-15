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
        public int Conteggio { get; set; }
        public List<Carta> Scarti { get; set; }
        public EnumRetro Retro { get; set; }

        public void CreaMazzo(int numMazzi = 1, bool mischia = true, int? random = null)
        {
            Conteggio = 0;
            if (Carte == null)
                Carte = new List<Carta>();

            Scarti = new List<Carta>();
            var rnd = new Random();
            Retro = (EnumRetro)Math.Round(rnd.NextDouble(), 0);

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
                if (random == null)
                    rnd = new Random();
                else
                    rnd = new Random(random.Value);
                Carte = Carte.OrderBy(item => rnd.Next()).ToList();
            }
        }
        public Carta PescaCarta(int percMin = 20, bool mischia=true)
        {
            if (Carte.Count <= percMin*Carte.Count/100)
                CreaMazzo(mischia:mischia);

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
