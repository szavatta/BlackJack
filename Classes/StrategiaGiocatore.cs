using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public abstract class StrategiaGiocatore
    {
        public int moltiplicatore { get; set; } = 1;
        public abstract Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere);
        public abstract int Puntata(int Conteggio);
    }
}
