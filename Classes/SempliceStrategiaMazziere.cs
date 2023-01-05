using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    class SempliceStrategiaMazziere : StrategiaMazziere
    {
        public Mazziere.Puntata Strategy(Mazziere mazziere)
        {
            if (mazziere.Punteggio() < 17)
                return Mazziere.Puntata.Chiama;
            else
                return Mazziere.Puntata.Stai;
        }
    }
}
