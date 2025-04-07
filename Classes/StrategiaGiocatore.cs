using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Classes
{
    public abstract class StrategiaGiocatore
    {

        public int Conteggio { get; set; }
        public double TrueCount { get; set; }
        public double GetTrueCount(int NumCarte)
        {
            double ret = Math.Round((double)(Conteggio / ((NumCarte / 52) == 0 ? 1 : ((double)NumCarte / 52))),2);
            return ret;
        }


        public abstract Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, double conteggio);
        public abstract bool Assicurazione(Giocatore giocatore, double conteggio);
        public abstract double Puntata(Giocatore giocatore, double puntataMinima, double puntataBase, double Conteggio);
        public virtual int Conta(Carta carta)
        {
            return 0;
        }
    }
}
