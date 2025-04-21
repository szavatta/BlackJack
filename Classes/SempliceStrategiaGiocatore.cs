using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class SempliceStrategiaGiocatore : StrategiaGiocatore
    {
        public override Giocatore.Giocata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Giocata.Chiama;
            else
                return Giocatore.Giocata.Stai;
        }

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            return puntataBase;
        }

        public override bool Assicurazione(Giocatore giocatore, decimal conteggio)
        {
            return false;
        }
    }

    public class StrategiaPrudente : StrategiaGiocatore
    {
        public override Giocatore.Giocata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < 12)
                return Giocatore.Giocata.Chiama;
            else
                return Giocatore.Giocata.Stai;
        }

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            return puntataBase;
        }

        public override bool Assicurazione(Giocatore giocatore, decimal conteggio)
        {
            return false;
        }
    }
}
