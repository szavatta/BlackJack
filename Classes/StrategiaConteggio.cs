using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class StrategiaConteggio : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override int Puntata(int Conteggio)
        {
            if (Conteggio <= -2)
                return 0;
            else if (Conteggio == -1)
                return 10;
            else if (Conteggio == 0)
                return 10;
            else if (Conteggio == 1)
                return 50;
            else if (Conteggio == 2)
                return 100;
            else if (Conteggio == 3)
                return 150;
            else if (Conteggio == 4)
                return 200;
            else //if (Conteggio >= 5)
                return 250;

        }

    }
}
