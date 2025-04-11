using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public interface StrategiaMazziere
    {
        public Mazziere.Giocata Strategy(Mazziere mazziere);
    }
}
