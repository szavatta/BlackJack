using System;
using System.Collections.Generic;
using System.Linq;

namespace Classes
{
    [Serializable]
    public class Mazzo : ICloneable
    {
        public enum EnumRetro
        {
            blu,
            rosso
        }
        public List<Carta> Carte { get; set; }
        public List<Carta> Scarti { get; set; }
        public EnumRetro Retro { get; set; }
        public int NumCarte => Carte?.Count ?? 0;
        public int NumScarti => Scarti?.Count ?? 0;
        public const int NumCarteSingoloMazzo = 52;

        public void CreaMazzo(Gioco gioco)
        {
            gioco.CambiMazzi++;
            Carte = new List<Carta>();
            Scarti = new List<Carta>();
            gioco.Giocatori.ForEach(q => q.Strategia.Conteggio = 0);

            var rnd = new Random();
            Retro = (EnumRetro)Math.Round(rnd.NextDouble(), 0);

            for (int mazzo = 0; mazzo < gioco.NumMazziIniziali; mazzo++)
            {
                for (int seme = 1; seme <= 4; seme++)
                {
                    for (int numero = 1; numero <= 13; numero++)
                    {
                        Carte.Add(new Carta((Carta.NumeroCarta)numero, (Carta.SemeCarta)seme));
                    }
                }
            }

            if (gioco.Mischia)
            {
                if (gioco.RandomMischiata == null)
                    rnd = new Random();
                else
                    rnd = new Random(gioco.RandomMischiata.Value + gioco.CambiMazzi);
                Carte = Carte.OrderBy(item => rnd.Next()).ToList();
            }

            gioco.Log.AppendLine($"Nuovo mazzo di {Carte.Count} carte{(gioco.Mischia ? " mischiate" : "")}");
        }

        public Carta PescaCarta()
        {
            Carta carta = Carte.FirstOrDefault();
            Carte.RemoveAt(0);
            //Conteggio += carta.Conteggio;

            return carta;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //public int GetTrueCount() => (int)(Conteggio / ((Carte.Count / 52) == 0 ? 1 : (Carte.Count / 52)));

        //public override string ToString()
        //{
        //    return "Conteggio: " + Conteggio;
        //}
    }

}
