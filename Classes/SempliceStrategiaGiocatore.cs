using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class SempliceStrategiaGiocatore : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, double conteggio)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override double Puntata(Giocatore giocatore, double puntataMinima, double puntataBase, double Conteggio)
        {
            return puntataBase;
        }

        public override bool Assicurazione(Giocatore giocatore, double conteggio)
        {
            return false;
        }
    }
}
