using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class BasicStrategy : StrategiaGiocatore
    {
        public override Giocatore.Giocata Strategy(Giocatore giocatore, Mazziere mazziere, decimal tc)
        {
            int pg= giocatore.Punteggio;
            int pm = mazziere.Carte.First().Valore;

            bool assoConDueCarte = giocatore.Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count() == 1 && giocatore.Carte.Count == 2;
            bool stesseCarte = giocatore.Carte.Count == 2 && giocatore.Carte[0].Numero == giocatore.Carte[1].Numero;

            if (stesseCarte)
            {
                if (
                    giocatore.Carte[0].Numero == Carta.NumeroCarta.Asso
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Nove && mazziere.Carte[0].Numero != Carta.NumeroCarta.Sette && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Nove
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Otto
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Sette && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette 
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Sei && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sei
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Quattro && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Cinque && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sei
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Tre && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Due && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Due && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Due && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    )
                {
                    return Giocatore.Giocata.Dividi;
                }
            }

            if (assoConDueCarte)
            {
                if (pg >= 20 //Asso+9
                    || pg == 19 && (pm != 6)
                    || pg == 18 && pm >= 7 && pm <= 8
                    )
                {
                    return Giocatore.Giocata.Stai;
                }

                if (pg == 19 && pm == 6
                        || pg == 18 && pm <= 6
                        )
                {
                    if (!giocatore.Gioco.RaddoppiaNonDisponibile)
                        return Giocatore.Giocata.Raddoppia;
                    else
                        return Giocatore.Giocata.Stai;
                }

                if (pg == 17 && pm >= 3 && pm <= 6
                        || pg == 16 && pm >= 4 && pm <= 6
                        || pg == 15 && pm >= 4 && pm <= 6
                        || pg == 14 && pm >= 5 && pm <= 6
                        || pg == 13 && pm >= 5 && pm <= 6
                        )
                {
                    if (!giocatore.Gioco.RaddoppiaNonDisponibile)
                        return Giocatore.Giocata.Raddoppia;
                    else
                        return Giocatore.Giocata.Chiama;
                }

                return Giocatore.Giocata.Chiama;
                
            }

            if (giocatore.Carte.Count == 2 && giocatore.Gioco.ArresaDisponibile)
            {
                if (pg == 16 && pm >= 9
                    || pg == 15 && pm == 10)
                    return Giocatore.Giocata.Arresa;
            }

            if (pg >= 17
                || pg == 16 && pm <= 6
                || pg == 15 && pm <= 6
                || pg == 14 && pm <= 6
                || pg == 13 && pm <= 6
                || pg == 12 && pm >= 4 && pm <= 6
                )
            {
                return Giocatore.Giocata.Stai;
            }

            if (pg == 11 
                || pg == 10 && pm <= 9
                || pg == 9 && pm >= 3 && pm <= 6
                )
            {
                return Giocatore.Giocata.Raddoppia;
            }
            
            return Giocatore.Giocata.Chiama;
            
        }

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            if (Conteggio < 1)
                return puntataMinima;
            else if (Conteggio >= 1 && Conteggio <= 4)
                return puntataBase * (decimal)Conteggio;
            else //if (Conteggio >= 5)
                return puntataBase * 5;
        }

        public override int Conta(Carta carta)
        {
            if (carta.Numero >= Carta.NumeroCarta.Due && carta.Numero <= Carta.NumeroCarta.Sei)
                Conteggio += 1;
            else if (carta.Numero >= Carta.NumeroCarta.Dieci || carta.Numero == Carta.NumeroCarta.Asso)
                Conteggio -= 1;

            return Conteggio;
        }

        public override bool Assicurazione(Giocatore giocatore, decimal truecount)
        {
            return truecount >= 3;
        }
    }

    [Serializable]
    public class BasicStrategy124 : BasicStrategy
    {
        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            if (giocatore.Risultato == Giocatore.EnumRisultato.Perso)
                return puntataBase;
            if (giocatore.Risultato == Giocatore.EnumRisultato.Pari)
                return giocatore.PuntataPrecedente == 0 ? puntataBase : giocatore.PuntataPrecedente;
            else
            {
                if (giocatore.PuntataPrecedente == puntataBase || giocatore.PuntataPrecedente == puntataBase * 2)
                    return giocatore.PuntataPrecedente * 2;
                else
                    return puntataBase;
            }
        }

    }

    [Serializable]
    public class BasicStrategyDeviation : StrategiaGiocatore
    {
        public override Giocatore.Giocata Strategy(Giocatore giocatore, Mazziere mazziere, decimal tc)
        {
            int pg = giocatore.Punteggio;
            int pm = mazziere.Carte.First().Valore;

            bool assoConDueCarte = giocatore.Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count() == 1 && giocatore.Carte.Count == 2;
            bool stesseCarte = giocatore.Carte.Count == 2 && giocatore.Carte[0].Numero == giocatore.Carte[1].Numero;

            if (stesseCarte)
            {
                if (
                    giocatore.Carte[0].Numero == Carta.NumeroCarta.Asso
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Dieci && mazziere.Carte[0].Numero == Carta.NumeroCarta.Quattro && tc >= 6
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Dieci && mazziere.Carte[0].Numero == Carta.NumeroCarta.Cinque && tc >= 5
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Dieci && mazziere.Carte[0].Numero == Carta.NumeroCarta.Sei && tc >= 4
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Nove && mazziere.Carte[0].Numero != Carta.NumeroCarta.Sette && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Nove
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Otto
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Sette && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Sei && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sei
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Quattro && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Cinque && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sei
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Tre && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Due && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Due && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Due && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    )
                {
                    return Giocatore.Giocata.Dividi;
                }
            }
            if (assoConDueCarte)
            {
                if (pg >= 20 //Asso+9
                    || pg == 19 && pm <= 3 //aggiunto
                    || pg == 19 && pm >= 7 //aggiunto
                    || pg == 19 && pm == 4 && tc < 3
                    || pg == 19 && pm == 5 && tc < 1
                    || pg == 19 && pm == 6 && tc <= 0
                    || pg == 18 && pm >= 7 && pm <= 8)
                {
                    return Giocatore.Giocata.Stai;
                }


                if (pg == 19 && pm == 4 && tc >= 3
                    || pg == 19 && pm == 5 && tc >= 1
                    || pg == 19 && pm == 6 && tc > 0
                    || pg == 18 && pm <= 6
                    )
                {
                    if (!giocatore.Gioco.RaddoppiaNonDisponibile)
                        return Giocatore.Giocata.Raddoppia;
                    else
                        return Giocatore.Giocata.Stai;
                }

                if (pg == 17 && pm >= 3 && pm <= 6
                    || pg == 17 && pm == 2 && tc >= 1
                    || pg <= 16 && pg >= 15 && pm >= 4 && pm <= 6
                    || pg <= 14 && pg >= 13 && pm >= 5 && pm <= 6
                    )
                {
                    if (!giocatore.Gioco.RaddoppiaNonDisponibile)
                        return Giocatore.Giocata.Raddoppia;
                    else
                        return Giocatore.Giocata.Chiama;
                }


                return Giocatore.Giocata.Chiama;

            }

            if (giocatore.Carte.Count == 2 && giocatore.Gioco.ArresaDisponibile)
            {
                if (pg == 16 && pm >= 9
                    || pg == 15 && pm == 10)
                    return Giocatore.Giocata.Arresa;
            }

            if (pg >= 17
                || pg == 13 && pm == 2 && tc > -1
                || pg >= 14 && pm == 2
                || pg >= 13 && pm >= 3 && pm <= 6
                || pg == 12 && pm == 2 && tc >= 3
                || pg == 12 && pm == 3 && tc >= 2
                || pg == 12 && pm == 4 && tc > 0
                || pg == 12 && pm >= 5 && pm <= 6
                || pg == 16 && pm == 9 && tc >= 4
                || pg == 16 && pm == 10 && tc >= 0
                || pg == 16 && pm == 11 && tc >= 3
                || pg == 15 && pm == 10 && tc >= 4
                || pg == 15 && pm == 11 && tc >= 5)
            {
                return Giocatore.Giocata.Stai;
            }

            if (giocatore.Carte.Count > 2
                || pg == 16 && pm == 9 && tc < 4
                || pg == 16 && pm == 10 && tc < 0
                || pg == 16 && pm == 11 && tc < 3
                || pg == 15 && pm == 10 && tc < 4
                || pg == 15 && pm == 11 && tc < 5
                || pg == 13 && pm == 2 && tc <= -1
                || pg >= 12 && pm >= 7
                || pg == 12 && pm == 2 && tc < 3
                || pg == 12 && pm == 3 && tc < 2
                || pg == 12 && pm == 4 && tc <= 0
                || pg == 10 && pm == 10 && tc < 4
                || pg == 10 && pm == 11 && tc < 3
                || pg == 9 && pm == 2 && tc < 1
                || pg == 9 && pm == 7 && tc < 3
                || pg == 9 && pm > 7
                || pg == 8 && pm == 6 && tc < 2
                || pg == 8 && pm != 6
                || pg < 8)
            {
                return Giocatore.Giocata.Chiama;
            }

            if (!giocatore.Gioco.RaddoppiaNonDisponibile)
                return Giocatore.Giocata.Raddoppia;
            else
                return Giocatore.Giocata.Chiama;

        }

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            if (Conteggio < 1)
                return puntataMinima;
            else if (Conteggio >= 1 && Conteggio <= 4)
                return puntataBase * (decimal)Conteggio;
            else //if (Conteggio >= 5)
                return puntataBase * 5;
        }

        public override int Conta(Carta carta)
        {
            if (carta.Numero >= Carta.NumeroCarta.Due && carta.Numero <= Carta.NumeroCarta.Sei)
                Conteggio += 1;
            else if (carta.Numero >= Carta.NumeroCarta.Dieci || carta.Numero == Carta.NumeroCarta.Asso)
                Conteggio -= 1;

            return Conteggio;
        }

        public override bool Assicurazione(Giocatore giocatore, decimal truecount)
        {
            return truecount >= 3;
        }
    }

    [Serializable]
    public class BasicStrategy2 : StrategiaGiocatore
    {
        public override Giocatore.Giocata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            int pg = giocatore.Punteggio;
            int pm = mazziere.Carte.First().Valore;

            bool assoConDueCarte = giocatore.Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).Count() == 1 && giocatore.Carte.Count == 2;
            bool stesseCarte = giocatore.Carte.Count == 2 && giocatore.Carte[0].Numero == giocatore.Carte[1].Numero;

            if (stesseCarte)
            {
                if (
                    giocatore.Carte[0].Numero == Carta.NumeroCarta.Asso
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Dieci && mazziere.Carte[0].Numero == Carta.NumeroCarta.Quattro && conteggio >= 6
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Dieci && mazziere.Carte[0].Numero == Carta.NumeroCarta.Cinque && conteggio >= 5
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Dieci && mazziere.Carte[0].Numero == Carta.NumeroCarta.Sei && conteggio >= 4
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Nove && mazziere.Carte[0].Numero != Carta.NumeroCarta.Sette && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Dieci
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Otto
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Sette && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Sei && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sei && mazziere.Carte[0].Numero != Carta.NumeroCarta.Due
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Tre && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Quattro && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    || giocatore.Carte[0].Numero == Carta.NumeroCarta.Due && mazziere.Carte[0].Numero >= Carta.NumeroCarta.Quattro && mazziere.Carte[0].Numero <= Carta.NumeroCarta.Sette
                    )
                {
                    return Giocatore.Giocata.Dividi;
                }

            }

            if (assoConDueCarte)
            {
                if (pg >= 20
                    || pg == 19 && pm == 4 && conteggio < 3
                    || pg == 19 && pm == 5 && conteggio < 1
                    || pg == 19 && pm == 6 && conteggio < 0
                    || pg == 18 && pm >= 7 && pm <= 8)
                {
                    return Giocatore.Giocata.Stai;
                }
                else if (
                           pg == 19 && pm == 4 && conteggio >= 3
                        || pg == 19 && pm == 5 && conteggio >= 1
                        || pg == 19 && pm == 6 && conteggio > 0
                        || pg == 18 && pm <= 6
                        || pg == 17 && pm >= 3 && pm <= 6
                        || pg == 17 && pm == 2 && conteggio >= 1
                        || pg <= 16 && pg >= 15 && pm >= 4 && pm <= 6
                        || pg <= 14 && pg >= 13 && pm >= 5 && pm <= 6
                        )
                {
                    return Giocatore.Giocata.Raddoppia;
                }
                else
                {
                    return Giocatore.Giocata.Chiama;
                }
            }

            if (pg >= 17
                || pg == 13 && pm == 2 && conteggio > -1
                || pg >= 14 && pm == 2
                || pg >= 13 && pm >= 3 && pm <= 6
                || pg == 12 && pm == 2 && conteggio >= 3
                || pg == 12 && pm == 3 && conteggio >= 2
                || pg == 12 && pm == 4 && conteggio > 0
                || pg == 12 && pm >= 5 && pm <= 6
                || pg == 16 && pm == 9 && conteggio >= 4
                || pg == 16 && pm == 10 && conteggio > 0
                || pg == 16 && pm == 11 && conteggio >= 3
                || pg == 15 && pm == 10 && conteggio >= 4
                || pg == 15 && pm == 11 && conteggio >= 5)
            {
                return Giocatore.Giocata.Stai;
            }
            if (pg == 16 && pm == 9 && conteggio < 4
                || pg == 16 && pm == 10 && conteggio < 0
                || pg == 16 && pm == 11 && conteggio < 3
                || pg == 15 && pm == 10 && conteggio < 4
                || pg == 15 && pm == 11 && conteggio < 5
                || pg == 13 && pm == 2 && conteggio <= -1
                || pg >= 12 && pm >= 7
                || pg == 12 && pm == 2 && conteggio < 3
                || pg == 12 && pm == 3 && conteggio < 2
                || pg == 12 && pm == 4 && conteggio <= 0
                || pg == 10 && pm == 10 && conteggio < 4
                || pg == 10 && pm == 11 && conteggio < 3
                || pg == 9 && pm == 2 && conteggio < 1
                || pg == 9 && pm == 7 && conteggio < 3
                || pg == 9 && pm > 7
                || pg == 8 && pm == 6 && conteggio < 2
                || pg <= 8)
            {
                return Giocatore.Giocata.Chiama;
            }
            else
            {
                return Giocatore.Giocata.Raddoppia;
            }
        }

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            if (Conteggio <= -2)
                return puntataMinima;
            else if (Conteggio == -1)
                return puntataMinima;
            else if (Conteggio == 0)
                return puntataMinima;
            else if (Conteggio >= 1 && Conteggio <= 4)
                return 50 * Conteggio;
            else //if (Conteggio >= 5)
                return 50 * 5;
        }

        public override int Conta(Carta carta)
        {
            if (carta.Numero >= Carta.NumeroCarta.Due && carta.Numero <= Carta.NumeroCarta.Sei)
                Conteggio += 1;
            else if (carta.Numero >= Carta.NumeroCarta.Dieci || carta.Numero == Carta.NumeroCarta.Asso)
                Conteggio -= 1;

            return Conteggio;
        }

        public override bool Assicurazione(Giocatore giocatore, decimal truecount)
        {
            return truecount >= 3;
        }
    }

}
