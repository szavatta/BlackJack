using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    public class BasicStrategy : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere)
        {
            int pg= giocatore.Punteggio();
            int pm = mazziere.Carte.First().Valore;

            bool assoConDueCarte = giocatore.Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count() == 1 && giocatore.Carte.Count == 2;

            if (assoConDueCarte) 
            {
                if (pg >= 10 ||
                    pg == 9 && pm != 6 ||
                    pg == 8 && pm >= 7 && pm <= 8)
                {
                    return Giocatore.Puntata.Stai;
                }
                else if (pg == 9 && pm == 6 ||
                         pg == 8 && pm <= 6 ||
                         pg == 7 && pm >= 3 && pm <= 6 ||
                         pg <= 6 && pg >= 5 && pm >= 4 && pm <= 6 ||
                         pg <= 4 && pg >= 3 && pm >= 5 && pm <= 6
                        )
                {
                    return Giocatore.Puntata.Raddoppia;
                }
                else 
                {
                    return Giocatore.Puntata.Chiama;
                }
            }

            if (pg >= 17 ||
                pg >= 13 && pm <= 6 ||
                pg == 12 && pm >= 4 && pm <= 6)
            {
                return Giocatore.Puntata.Stai;
            }
            if (pg >= 12 && pm>=7 ||
                     pg ==12 && pm>=2 && pm<=3 ||
                     pg == 10 && pm >= 10 ||
                     pg == 9 && (pm==2 || pm >=7) ||
                     pg <= 8)
            {
                return Giocatore.Puntata.Chiama;
            }
            else
            {
                return Giocatore.Puntata.Raddoppia;
            }
        }
    }
}
