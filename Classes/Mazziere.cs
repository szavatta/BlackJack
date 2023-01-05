﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class Mazziere : GiocatoreSemplice
    {
        public StrategiaMazziere Strategia { get; set; }

        public Mazziere(Gioco gioco) : base(gioco)
        {
            Strategia = new SempliceStrategiaMazziere();
        }

        public new enum Puntata
        {
            Chiama = 0,
            Stai = 1,
        }
    }
}