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

        public override int Puntata(Mazzo mazzo)
        {
            var conteggio = mazzo.Conteggio;
            int ret;
            var truecount = conteggio / ((mazzo.Carte.Count / 52)+1);
            if (truecount <= -2)
                ret = 0;
            else if (truecount == -1)
                ret = 10;
            else if (truecount == 0)
                ret = 10;
            else if (truecount == 1)
                ret = 50;
            else if (truecount == 2)
                ret = 100;
            else if (truecount == 3)
                ret = 150;
            else if (truecount == 4)
                ret = 200;
            else //if (Conteggio >= 5)
                ret = 250;
            return ret;

        }

    }
}
