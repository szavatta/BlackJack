using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    class SempliceStrategiaMazziere : StrategiaMazziere
    {
        public Mazziere.Giocata Strategy(Mazziere mazziere)
        {
            if (mazziere.Punteggio < 17)
                return Mazziere.Giocata.Chiama;
            else
                return Mazziere.Giocata.Stai;
        }
    }

    class SempliceStrategiaMazziereS17 : StrategiaMazziere
    {
        public Mazziere.Giocata Strategy(Mazziere mazziere)
        {
            if (mazziere.Punteggio < 17 ||
                mazziere.Punteggio == 17 && mazziere.Carte.Select(q => q.Numero).Contains(Carta.NumeroCarta.Asso) && mazziere.Carte.Where(q => q.Numero != Carta.NumeroCarta.Asso).Sum(q => q.Valore) == 6)
                return Mazziere.Giocata.Chiama;
            else
                return Mazziere.Giocata.Stai;
        }
    }

}
