using Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    public class Giocatore
    {
        List<Carta> Carte { get; set; }
        Gioco Gioco { get; set; }
        public String Nome { get; set; }


        public Giocatore(Gioco gioco) 
        {
            Carte = new List<Carta>();
            Gioco = gioco;
        }

        public Carta Pesca()
        {
            Carta carta = Gioco.Mazzo.pescaCarta();
            Carte.Add(carta);
            if (Gioco.Mazzo.Carte.Count == 0)
                Gioco.Mazzo.CreaMazzo(Gioco.NumMazziIniziali);

            return carta;
        }

        public int Punteggio() => Carte.Select(q => q.Valore).Sum();
    }
}
