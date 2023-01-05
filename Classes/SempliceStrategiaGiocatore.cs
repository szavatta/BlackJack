using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    class SempliceStrategiaGiocatore : StrategiaGiocatore
    {
        public override Giocatore.puntata strategy(Giocatore giocatore, Mazziere mazziere)
        {

            if (giocatore.Punteggio() < 17)
            {
                return Giocatore.puntata.chiama;
            }
            else
            {
                return Giocatore.puntata.stai;
            }
        }
    }
}
