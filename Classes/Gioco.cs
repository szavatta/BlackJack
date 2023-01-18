﻿using Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    [Serializable]
    public class Gioco : ICloneable
    {
        public List<Giocatore> Giocatori { get; set; }
        public Mazzo Mazzo { get; set; }
        public Mazziere Mazziere { get; set; }

        public int NumMazziIniziali { get; set; }
        public int PuntataMinima { get; set; }
        public int? PuntataMassima { get; set; }
        public bool Mischia { get; set; }
        public int? RandomMischiata { get; set; }
        public int? PercMischiata { get; set; }

        public int Giri { get; set; }
        public int CambiMazzi { get; set; }
        public string Nome { get; set; }
        public string Id { get; set; }
        public List<Giocatore> GiocatoriSplit { get; set; }
        public string IdGiocatoreMano { get; set; }
        public bool Iniziato { get; set; }
        DateTime DataCreazione { get; set; }

        public Gioco(int giocatori, int numMazzi=6, bool mischia=true, int? randomMischiata = null, string nome = null, int puntataMinima = 5, int? puntataMassima = null, int? percMischiata = null)
        {
            GiocatoriSplit = new List<Giocatore>();
            Mazziere = new Mazziere(this);
            Giocatori = new List<Giocatore>();
            Id = DateTime.Now.Ticks.ToString();
            DataCreazione = DateTime.Now;
            Mazzo = new Mazzo();
            Mischia = mischia;
            RandomMischiata = randomMischiata;
            PercMischiata = percMischiata;
            PuntataMinima = puntataMinima;
            PuntataMassima = puntataMassima;
            NumMazziIniziali = numMazzi;
            Mazzo.CreaMazzo(this);
            if (string.IsNullOrEmpty(nome))
                Nome = "Partita";
            else
                Nome = nome;
            for (int i = 0; i < giocatori; i++)
            {
                Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(this).build());
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();

        }

        public void Giocata()
        {
            GiocataIniziale();

            for (int i = 0; i < Giocatori.Count(); i++)
            {
                GiocataGiocatore(i);
            }
            while (Mazziere.Scelta() == Mazziere.Puntata.Chiama)
            {
                Mazziere.Pesca();
            }
            TerminaMano();
            try
            {
                if (GiocatoriVincenti().Count() + GiocatoriPerdenti().Count() + GiocatoriPari().Count() != Giocatori.Count())
                    throw new Exception("Non corrispondono i giocatori");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GiocataGiocatore(int i)
        {
            if (Giocatori[i].PuntataCorrente > 0)
            {
                while (Giocatori[i].Scelta() == GiocatoreSemplice.Puntata.Dividi)
                {
                    Dividi(i);
                }

                while (Giocatori[i].Scelta() == GiocatoreSemplice.Puntata.Chiama)
                {
                    Giocatori[i].Pesca();
                }

                if (Giocatori[i].Scelta() == GiocatoreSemplice.Puntata.Raddoppia)
                {
                    Raddoppia(i);
                }
            }
        }

        private void GiocataIniziale()
        {
            Inizializza();
            Giocatori.ForEach(q => q.Punta());
            Giocatori.Where(q => q.PuntataCorrente > 0).ToList().ForEach(q => q.Pesca());
            Mazziere.Pesca();
            Giocatori.Where(q => q.PuntataCorrente > 0).ToList().ForEach(q => q.Pesca());
            Mazziere.Pesca();
        }

        private void Raddoppia(int i)
        {
            if (Giocatori[i].Carte.Count == 2)
            {
                Giocatori[i].PuntataCorrente *= 2;
            }

            Giocatori[i].Pesca();
        }

        private void Dividi(int i)
        {
            Giocatore clone = (Giocatore)Giocatori[i].Clone();
            Giocatori[i].Carte.RemoveAt(0);
            clone.Nome += " split";
            clone.Carte.RemoveAt(1);
            clone.GiocatoreSplit ??= Giocatori[i];
            clone.SoldiTotali = 0;
            Giocatori.Insert(i + 1, clone);


            if (Giocatori[i].Carte.Count == 1)
            {
                Giocatori[i].Pesca();
            }
        }

        public void TerminaMano()
        {
            foreach (var vincente in GiocatoriVincenti())
            {
                vincente.ManiVinte++;
                double paga = vincente.HasBlackJack() ? vincente.PuntataCorrente * 3 / 2 : vincente.PuntataCorrente;
                Mazziere.SoldiTotali -= paga;
                vincente.SoldiTotali += paga;
            }

            foreach (var perdente in GiocatoriPerdenti())
            {
                perdente.ManiPerse++;
                Mazziere.SoldiTotali += perdente.PuntataCorrente;
                perdente.SoldiTotali -= perdente.PuntataCorrente;
            }

            foreach (var giocatore in Giocatori.Where(q => q.GiocatoreSplit != null))
            {
                giocatore.GiocatoreSplit.ManiVinte += giocatore.ManiVinte;
                giocatore.GiocatoreSplit.ManiPerse += giocatore.ManiPerse;
                giocatore.GiocatoreSplit.SoldiTotali += giocatore.SoldiTotali;
            }

            GiocatoriVincenti().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Vinto);
            GiocatoriPerdenti().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Perso);
            GiocatoriPari().ForEach(q => q.Risultato = Giocatore.EnumRisultato.Pari);

            GiocatoriSplit = Giocatori.Where(q => q.GiocatoreSplit != null).ToList();

        }

        public void Inizializza()
        {
            Giocatori.RemoveAll(q => q.GiocatoreSplit != null);

            if (Mazzo.Carte.Count <= (52 * NumMazziIniziali) * PercMischiata / 100)
                Mazzo.CreaMazzo(this);

            foreach (Giocatore g in Giocatori)
            {
                Mazzo.Scarti.AddRange(g.Carte);
                g.Carte = new List<Carta>();
                g.PuntataCorrente = 0;
                g.Risultato = Giocatore.EnumRisultato.Pari;
            }
            Mazzo.Scarti.AddRange(Mazziere.Carte);
            Mazziere.Carte = new List<Carta>();
            Mazziere.CartaCoperta = true;
            Iniziato = false;
            Giri++;
        }

        public void DistribuisciCarteIniziali()
        {
            foreach (Giocatore giocatore in Giocatori.Where(q => q.PuntataCorrente > 0))
            {
                giocatore.Pesca();
            }
            Mazziere.Pesca();
            foreach (Giocatore giocatore in Giocatori.Where(q => q.PuntataCorrente > 0))
            {
                giocatore.Pesca();
            }
            Mazziere.Pesca();

            if (Giocatori.Count > 0)
                IdGiocatoreMano = Giocatori[0].Id;
        }

        public void PassaMano(Giocatore giocatore)
        {
            Giocatore next = Giocatori.SkipWhile(q => q.Id != giocatore.Id).Skip(1).FirstOrDefault();
            if (next != null)
            {
                IdGiocatoreMano = next.Id;
            }
            else
            {
                IdGiocatoreMano = null;
                Mazziere.CartaCoperta = false;
                while (Mazziere.Strategia.Strategy(Mazziere) == Mazziere.Puntata.Chiama)
                {
                    Mazziere.Pesca();
                }
                TerminaMano();
            }
        }

        public List<Giocatore> GiocatoriVincenti()
        {
            var ret = Giocatori.Where(q =>
                q.PuntataCorrente > 0 && 
                q.Punteggio <= 21 && (q.Punteggio > Mazziere.Punteggio || Mazziere.Punteggio > 21) ||
                q.HasBlackJack() && !Mazziere.HasBlackJack()
                ).ToList();

            return ret;
        }

        public List<Giocatore> GiocatoriPari()
        {
            var ret = Giocatori.Where(q =>
                q.PuntataCorrente == 0 || 
                q.Punteggio == Mazziere.Punteggio && q.Punteggio <= 21 &&
                !(q.HasBlackJack() ^ Mazziere.HasBlackJack())
                ).ToList();

            return ret;
        }

        public List<Giocatore> GiocatoriPerdenti()
        {
            var ret = Giocatori.Where(q =>
                q.PuntataCorrente > 0 && 
                (q.Punteggio > 21 || 
                (q.Punteggio < Mazziere.Punteggio && Mazziere.Punteggio <= 21) ||
                !q.HasBlackJack() && Mazziere.HasBlackJack())
                ).ToList();

            return ret;
        }

    }
}
