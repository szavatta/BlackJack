using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    class SempliceStrategiaMazziere : StrategiaMazziere
    {
        public Mazziere.puntata strategy(Mazziere mazziere)
        {
            if (mazziere.Punteggio() < 17)
            {
                return Mazziere.puntata.chiama;
            }
            else
            {
                return Mazziere.puntata.stai;
            }
        }
    }
}
