using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public abstract class StrategiaGiocatore
    {
        public int Conteggio { get; set; }
        public int TrueCount { get; set; }
        public int GetTrueCount(int NumCarte) => (int)(Conteggio / ((NumCarte / 52) == 0 ? 1 : (NumCarte / 52)));
        public abstract Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio);
        public abstract int Puntata(Giocatore giocatore, int puntataMinima, int puntataBase, int Conteggio);
        public virtual int Conta(Carta carta)
        {
            return 0;
        }
    }
}
