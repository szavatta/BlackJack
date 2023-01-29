using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class StrategiaConteggio : StrategiaGiocatore
    {
        public int Punteggio { get; set; }

        public StrategiaConteggio(int punteggio = 17)
        {
            Punteggio = punteggio;
        }

        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, double conteggio)
        {
            if (giocatore.Punteggio < Punteggio)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override double Puntata(Giocatore giocatore, double puntataMinima, double puntataBase, double Conteggio)
        {
            if (Conteggio <= -2)
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

        public override bool Assicurazione(Giocatore giocatore, double conteggio)
        {
            return false;
        }
    }
}
