using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public abstract class Strategy
    {
        public int moltiplicatore { get; set; } = 1;
       public abstract Giocatore.puntata strategy(Giocatore giocatore, Mazziere mazziere);
    }
}
