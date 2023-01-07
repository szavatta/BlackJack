﻿using Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public abstract class GiocatoreSemplice
    {
        public List<Carta> Carte { get; set; }
        [JsonIgnore]
        public Gioco Gioco { get; set; }
        public String Nome { get; set; }
        public double PuntataCorrente { get; set; }
        public double SoldiTotali { get; set; }
        public int Punteggio => CalcolaPunteggio();


        public GiocatoreSemplice(Gioco gioco)
        {
            Carte = new List<Carta>();
            Gioco = gioco;
        }

        public Carta Pesca()
        {
            Carta carta = Gioco.Mazzo.pescaCarta();
            Carte.Add(carta);
            if (Gioco.Mazzo.Carte.Count < 10)
                Gioco.Mazzo.CreaMazzo(Gioco.NumMazziIniziali);

            return carta;
        }
        public int CalcolaPunteggio()
        {
            List<Carta> carte2 = new List<Carta>(Carte);
            int punt11 = carte2.Select(q => q.Valore).Sum();
            if (punt11 > 21)
            {
                var res= carte2.Select(q => q.Valore).Sum();
                if (carte2.Select(q => q.Numero).Contains(Carta.NumeroCarta.Asso)) 
                    res -= 10;

                //io lo farei così perchè bisogna togliere 10 per ogni asso
                //res -= carte2.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count() * 10;

                return res;
            }
            else
                return punt11;
        }
        public enum Puntata
        {
            Chiama = 0,
            Stai = 1,
            Raddoppia = 2,
            Dividi = 3
        }
        public override string ToString()
        {
            return $"Nome: {Nome}, " +
                    $"Soldi Totali: {SoldiTotali}";
        }

    }
}

