﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class GiocatoreBuilder
    {
        public string Id { get; set; }
        public StrategiaGiocatore Strategia { get; set; }
        public Gioco Gioco { get; set; }
        public String Nome { get; set; }
        public decimal Soldi { get; set; }
        public decimal PuntataBase { get; set; } = 1;

        public static GiocatoreBuilder Init()
        {
            return new GiocatoreBuilder();
        }

        public GiocatoreBuilder AggiungiStrategia(StrategiaGiocatore strategia)
        {
            Strategia = strategia;
            return this;
        }

        public GiocatoreBuilder AggiungiGioco(Gioco gioco)
        {
            Gioco = gioco;
            return this;
        }

        public GiocatoreBuilder AggiungiNome(string nome)
        {
            Nome = nome;
            return this;
        }

        public GiocatoreBuilder AggiungiSoldi(decimal soldi)
        {
            Soldi = soldi;
            return this;
        }

        public GiocatoreBuilder AggiungiPuntataBase(decimal puntataBase)
        {
            PuntataBase = puntataBase;
            return this;
        }

        public Giocatore build()
        {
            return new Giocatore(Gioco, Strategia, Soldi, Nome, PuntataBase);
        }
    }
}
