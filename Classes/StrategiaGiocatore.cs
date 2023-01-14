using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public abstract class StrategiaGiocatore
    {
        public int moltiplicatore { get; set; } = 1;
        public abstract Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio);
        public abstract int Puntata(Giocatore giocatore, int puntataMinima, int puntataBase, int Conteggio);
    }
}
