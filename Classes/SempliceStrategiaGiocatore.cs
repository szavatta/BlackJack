﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class SempliceStrategiaGiocatore : StrategiaGiocatore
    {
        public override Giocatore.Puntata Strategy(Giocatore giocatore, Mazziere mazziere, decimal conteggio)
        {
            if (giocatore.Punteggio < 17)
                return Giocatore.Puntata.Chiama;
            else
                return Giocatore.Puntata.Stai;
        }

        public override decimal Puntata(Giocatore giocatore, decimal puntataMinima, decimal puntataBase, decimal Conteggio)
        {
            return puntataBase;
        }

        public override bool Assicurazione(Giocatore giocatore, decimal conteggio)
        {
            return false;
        }
    }
}
