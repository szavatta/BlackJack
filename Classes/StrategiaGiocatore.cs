using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Classes
{
    public abstract class StrategiaGiocatore
    {

        public int Conteggio { get; set; }
        public decimal TrueCount { get; set; }
        public decimal GetTrueCount(int NumCarte)
        {
            decimal ret = Math.Round((decimal)(Conteggio / ((NumCarte / 52) == 0 ? 1 : ((decimal)NumCarte / 52))),2);
            return ret;
        }


        public abstract Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio);
        public abstract bool Assicurazione(Giocatore giocatore, decimal conteggio);
        public abstract decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio);
        public virtual int Conta(Carta carta)
        {
            return 0;
        }
    }
}
