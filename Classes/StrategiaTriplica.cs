using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class StrategiaTriplica : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override double Puntata(Giocatore giocatore, double puntataMinima, double puntataBase, int Conteggio)
        {
            if (giocatore.PuntataCorrente == 0)
                giocatore.PuntataCorrente = puntataBase;
            if (giocatore.Risultato == Giocatore.EnumRisultato.Perso)
                return giocatore.PuntataCorrente * 3;
            else if (giocatore.Risultato == Giocatore.EnumRisultato.Vinto)
                return puntataBase;
            else
                return giocatore.PuntataCorrente;
        }

    }

    public class StrategiaDuplica : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override double Puntata(Giocatore giocatore, double puntataMinima, double puntataBase, int Conteggio)
        {
            if (giocatore.PuntataCorrente == 0)
                giocatore.PuntataCorrente = puntataBase;
            if (giocatore.Risultato == Giocatore.EnumRisultato.Perso)
                return giocatore.PuntataCorrente * 2;
            else if (giocatore.Risultato == Giocatore.EnumRisultato.Vinto)
                return puntataBase;
            else
                return giocatore.PuntataCorrente;
        }


    }


}
