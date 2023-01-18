﻿using Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class Giocatore : GiocatoreSemplice, ICloneable
    {
        public string Id { get; set; }
        [JsonConverter(typeof(StrategiaConverter))]
        public StrategiaGiocatore Strategia { get; set; }
        public Giocatore GiocatoreSplit { get; set; }
        public EnumRisultato Risultato { get; set; }
        public int ManiVinte { get; set; }
        public int ManiPerse { get; set; }
        public bool CanSplit { get; set; }


        public Giocatore(Gioco gioco = null, StrategiaGiocatore strategia = null, double soldi = 0, string nome = "") : base(gioco)
        {
            Carte = new List<Carta>();
            Nome = string.IsNullOrEmpty(nome) ? $"Giocatore { (gioco != null ? gioco.Giocatori.Count + 1 : 0) }" : nome;
            Id = DateTime.Now.Ticks.ToString();


            SoldiTotali = soldi;

            if (strategia == null)
                Strategia = new SempliceStrategiaGiocatore();
            else
                Strategia = strategia;
        }
        public enum EnumRisultato
        {
            Pari = 0,
            Vinto = 1,
            Perso = 2,
        }

        public Giocatore Raddoppia()
        {
            PuntataCorrente *= 2;
            Pesca();
            return this;
        }

        public Giocatore Stai()
        {
            CanSplit = false;
            Gioco.Iniziato = true;
            Gioco.PassaMano(this);
            return this;
        }

        public Giocatore Punta(int puntata)
        {
            Gioco.Iniziato = true;
            PuntataCorrente = puntata;
            return this;
        }

        public Giocatore Esci()
        {
            Gioco.Iniziato = true;
            Gioco.Giocatori.Remove(this);
            return this;
        }

        public Giocatore Split()
        {
            CanSplit = false;
            Giocatore clone = (Giocatore)this.Clone();
            Carte.RemoveAt(0);
            clone.Nome += " split";
            clone.Id = DateTime.Now.Ticks.ToString();
            clone.Carte.RemoveAt(1);
            clone.GiocatoreSplit ??= this;
            clone.SoldiTotali = 0;
            for (int i = 0; i < Gioco.Giocatori.Count; i++)
            {
                if (Gioco.Giocatori[i].Id == Id)
                {
                    Gioco.Giocatori.Insert(i + 1, clone);
                    break;
                }
            }
            return this;
        }

        public void Punta()
        {
            PuntataCorrente = Strategia.Puntata(this, Gioco.PuntataMinima, 5, Strategia.GetTrueCount(Gioco.Mazzo.Carte.Count));
            if (Gioco.PuntataMassima.HasValue && PuntataCorrente > Gioco.PuntataMassima)
                PuntataCorrente = Gioco.PuntataMassima.Value;
        }

        public Puntata Scelta()
        {
            if (Carte.Count < 2)
                return Puntata.Chiama;
            else
                return Strategia.Strategy(this, Gioco.Mazziere, Strategia.GetTrueCount(Gioco.Mazzo.Carte.Count));
        }

        public object Clone()
        {
            Giocatore giocatore = new Giocatore(Gioco, Strategia, SoldiTotali, Nome);
            giocatore.Carte = new List<Carta>(this.Carte);
            giocatore.PuntataCorrente = PuntataCorrente;
            return giocatore;
        }

        public override Carta Pesca()
        {
            Carta carta = base.Pesca();

            if (Carte.Count == 2 && Carte[0].Numero == Carte[1].Numero)
                CanSplit = true;
            else
                CanSplit = false;

            Strategia.TrueCount = Strategia.GetTrueCount(Gioco.Mazzo.Carte.Count);

            //if (Punteggio > 21 && verifica21)
            //    Stai();

            return carta;
        }
    }
}
