using Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class Gioco
    {
        public List<Giocatore> Giocatori { get; set; }
        public Mazzo Mazzo { get; set; }
        public Mazziere Mazziere { get; set; }
        public Gioco(int giocatori, int numMazzi=6) 
        {
            Mazzo = new Mazzo();
            Mazzo.CreaMazzo(numMazzi);
            Giocatori = new List<Giocatore>();
            for (int i = 0; i < giocatori; i++)
            {
                Giocatori.Add(new Giocatore(this));
            }
            Mazziere = new Mazziere(this);
            
        }


    }
}
