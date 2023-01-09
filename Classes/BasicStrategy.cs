using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class BasicStrategy : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            int pg= giocatore.Punteggio;
            int pm = mazziere.Carte.First().Valore;

            bool assoConDueCarte = giocatore.Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count() == 1 && giocatore.Carte.Count == 2;
            bool stesseCarte = giocatore.Carte.Count == 2 && giocatore.Carte[0].Numero == giocatore.Carte[1].Numero;

            if (stesseCarte)
            {
                if (giocatore.Carte[0].Numero == Carta.NumeroCarta.Asso
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Nove && mazziere.Carte[0].Numero != Carta.NumeroCarta.Sette && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Dieci
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Otto
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Sette && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette 
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Sei && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sei && mazziere.Carte[0].Numero != Carta.NumeroCarta.Due
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Tre && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Quattro && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Due && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Quattro && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    )
                {
                    return Giocatore.Puntata.Dividi;
                }

            }

            if (assoConDueCarte) 
            {
                if (pg >= 20 ||
                    pg == 19 && pm != 6 ||
                    pg == 18 && pm >= 7 && pm <= 8)
                {
                    return Giocatore.Puntata.Stai;
                }
                else if (pg == 19 && pm == 6 ||
                         pg == 18 && pm <= 6 ||
                         pg == 17 && pm >= 3 && pm <= 6 ||
                         pg <= 16 && pg >= 15 && pm >= 4 && pm <= 6 ||
                         pg <= 14 && pg >= 13 && pm >= 5 && pm <= 6
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

        public override int Puntata(int puntataMinima, int puntataBase, int Conteggio)
        {
            if (Conteggio <= -2)
                return puntataMinima;
            else if (Conteggio == -1)
                return puntataMinima;
            else if (Conteggio == 0)
                return puntataMinima;
            else if (Conteggio >= 1 && Conteggio <= 4)
                return puntataBase * Conteggio;
            else //if (Conteggio >= 5)
                return puntataBase * 5;
        }

    }
}
