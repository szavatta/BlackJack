using Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    public abstract class GiocatoreSemplice
    {
        public List<Carta> Carte { get; set; }
        [JsonIgnore]
        public Gioco Gioco { get; set; }
        public String Nome { get; set; }
        public int PuntataCorrente { get; set; }
        public double SoldiTotali { get; set; }
        public int Punteggio => CalcolaPunteggio();
        public int ManiSballate { get; set; }


        public GiocatoreSemplice(Gioco gioco)
        {
            Carte = new List<Carta>();
            Gioco = gioco;
        }

        public virtual Carta Pesca(bool verifica21 = false)
        {
            Carta carta = Gioco.Mazzo.PescaCarta(Gioco.Mischia, Gioco.PercMischiata, Gioco.RandomMischiata);
            Gioco.Giocatori.ForEach(q => q.Strategia.Conta(carta));
            Carte.Add(carta);
            if (Punteggio > 21)
                ManiSballate += 1;

            return carta;
        }

        public int CalcolaPunteggio()
        {
            List<Carta> carte2 = new List<Carta>(Carte);
            int punt11 = carte2.Select(q => q.Valore).Sum();
            for (int i = 0; i < carte2.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count(); i++)
            {
                if (punt11 > 21)
                    punt11 -= 10;
            }

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

        public bool HasBlackJack()
        {
            bool ret = Carte.Count() == 2 
                && Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count() == 1 
                && Carte.Where(q => q.Numero >= Carta.NumeroCarta.Dieci).Count() == 1;

            return ret;
        }
    }
}

