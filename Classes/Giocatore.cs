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
        public Strategy Strategia { get; set; }

        public Giocatore(Gioco gioco)
        {
            Strategia = new SimpleStrategy();
            Carte = new List<Carta>();
            Gioco = gioco;
            Nome = $"Giocatore {gioco.Giocatori.Count + 1}";
        }

        public Carta Pesca()
        {
            Carta carta = Gioco.Mazzo.pescaCarta();
            Carte.Add(carta);
            if (Gioco.Mazzo.Carte.Count < 10)
                Gioco.Mazzo.CreaMazzo(Gioco.NumMazziIniziali);

            return carta;
        }
        public int Punteggio()
        {
            int punt = Carte.Select(q => q.Valore).Sum();
            List<Carta> carte2 = new List<Carta>(Carte);
            carte2.Where(q => q.Numero == Carta.NumeroCarta.Asso).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso14);
            int punt2 = carte2.Select(q => q.Valore).Sum();
            if (punt2 > punt && punt2 <= 21)
                punt = punt2;

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
