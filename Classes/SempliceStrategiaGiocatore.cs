using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    class SempliceStrategiaGiocatore : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override int Puntata(int puntataMinima, int puntataBase, int Conteggio)
        {
            return puntataBase;
        }


    }
}
