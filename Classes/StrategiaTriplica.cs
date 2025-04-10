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

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            if (giocatore.Risultato == Giocatore.EnumRisultato.Perso)
                return giocatore.PuntataPrecedente * 3;
            else if (giocatore.Risultato == Giocatore.EnumRisultato.Vinto)
                return puntataMinima;
            else
                return giocatore.PuntataPrecedente == 0 ? puntataMinima : giocatore.PuntataPrecedente;
        }


        public override bool Assicurazione(Giocatore giocatore, decimal conteggio)
        {
            return false;
        }
    }

    public class StrategiaRaddoppia : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            decimal puntata = 0;
            if (giocatore.Risultato == Giocatore.EnumRisultato.Perso)
                puntata = giocatore.PuntataPrecedente * 2;
            else if (giocatore.Risultato == Giocatore.EnumRisultato.Vinto)
                puntata = puntataMinima;
            else
                puntata = giocatore.PuntataPrecedente == 0 ? puntataMinima : giocatore.PuntataPrecedente;

            return puntata;
        }

        public override bool Assicurazione(Giocatore giocatore, decimal conteggio)
        {
            return false;
        }
    }


}
