using Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    public class Giocatore
    {
        public List<Carta> Carte { get; set; }
        public Gioco Gioco { get; set; }
        public String Nome { get; set; }
        public int PuntataCorrente { get; set; }
        public int SoldiTotali { get; set; }


        public Giocatore(Gioco gioco) 
        {
            Carte = new List<Carta>();
            Gioco = gioco;
            Nome = $"Giocatore {gioco.Giocatori.Count + 1}";
        }

        public Carta Pesca()
        {
            Carta carta = Gioco.Mazzo.pescaCarta();
            Carte.Add(carta);
            if (Gioco.Mazzo.Carte.Count == 0)
                Gioco.Mazzo.CreaMazzo(Gioco.NumMazziIniziali);

            return carta;
        }
        public int Punteggio()
        {
            int punt = Carte.Select(q => q.Valore).Sum();
            //TODO: Gestire il punteggio con Asso che vale 11

            return punt;
        }
        public enum puntata
    {
        chiama = 0,
        stai = 1,
        raddoppia = 2,
        dividi = 3
    }
    public override string ToString()
    {
        return $"Name: {Nome}, " +
               $"Soldi Totali: {SoldiTotali}";
    }

    }


}
