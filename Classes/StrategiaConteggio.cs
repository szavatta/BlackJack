using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    /// <summary>
    /// Chiama con punteggio minore di quello selezionato altrimenti Sta
    /// </summary>
    [Serializable]
    public class StrategiaPunteggio : StrategiaGiocatore
    {
        public int Punteggio { get; set; }

        public StrategiaPunteggio(int punteggio = 17)
        {
            Punteggio = punteggio;
        }

        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < Punteggio)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            if (Conteggio <= -2) //Lasciare il tavolo
                return puntataMinima;
            else if (Conteggio == -1)
                return puntataMinima;
            else if (Conteggio == 0)
                return puntataMinima;
            else if (Conteggio>=1 && Conteggio<=4)
                return puntataBase * Conteggio;
            else //if (Conteggio >= 5)
                return puntataBase*5;
        }

        public override int Conta(Carta carta)
        {
            if (carta.Numero >= Carta.NumeroCarta.Due && carta.Numero <= Carta.NumeroCarta.Sei)
                Conteggio += 1;
            else if (carta.Numero >= Carta.NumeroCarta.Dieci || carta.Numero == Carta.NumeroCarta.Asso)
                Conteggio -= 1;

            return Conteggio;
        }

        public override bool Assicurazione(Giocatore giocatore, decimal conteggio)
        {
            return false;
        }
    }
}
