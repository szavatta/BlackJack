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
        public double SoldiTotali { get; set; }
        public int Punteggio => CalcolaPunteggio();
        public int ManiSballate { get; set; }


        public GiocatoreSemplice(Gioco gioco)
        {
            Carte = new List<Carta>();
            Gioco = gioco;
        }

        public virtual Carta Chiama()
        {
            Carta carta = Gioco.Mazzo.PescaCarta();
            Gioco.Giocatori.ForEach(q => q.Strategia.Conta(carta));
            Carte.Add(carta);
            if (Punteggio > 21)
                ManiSballate += 1;

            return carta;
        }

        public int CalcolaPunteggio(bool Asso1 = false)
        {
            List<Carta> carte2 = new List<Carta>(Carte);
            int punt11 = carte2.Select(q => q.Valore).Sum();
            for (int i = 0; i < carte2.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count(); i++)
            {
                if (punt11 > 21 || Asso1)
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


        public bool HasBlackJack()
        {
            bool split = this is Giocatore ? ((Giocatore)this).GiocatoreSplit != null : false;
            bool ret = !split &&
                Carte.Count() == 2 
                && Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count() == 1 
                && Carte.Where(q => q.Numero >= Carta.NumeroCarta.Dieci).Count() == 1;

            return ret;
        }

        public bool HaSballato()
        {
            return Punteggio > 21;
        }

        public GiocatoreSemplice AggiungiCarta(Carta carta)
        {
            Carte.Add(carta);
            return this;
        }
    }
}

