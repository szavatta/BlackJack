using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class StrategiaConteggio : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override int Puntata(Giocatore giocatore, int puntataMinima, int puntataBase, int Conteggio)
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

    }
}
