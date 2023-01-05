﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Classes
{
    [Serializable]
    public class Mazzo
    {
        public List<Carta> Carte { get; set; }

        public void CreaMazzo(int numMazzi = 1, bool mischia = true)
        {
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
        public Carta pescaCarta() {
            if (Carte.Count == 0)
            {
                CreaMazzo();
            }

            Carta carta = Carte.FirstOrDefault();
            Carte.RemoveAt(0);
            return carta;
        }
    }

}