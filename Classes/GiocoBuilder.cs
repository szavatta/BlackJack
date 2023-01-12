using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    public class GiocoBuilder
    {
        private int NumGiocatori { get; set; }
        private Mazzo Mazzo { get; set; }
        private Mazziere Mazziere { get; set; }
        private int NumMazziIniziali { get; set; }
        private int PuntataMinima { get; set; }
        private bool Mischia { get; set; }
        private int Giri { get; set; }
        private string Nome { get; set; }
        private string Id { get; set; }
        private List<Giocatore> GiocatoriSplit { get; set; }
        private string IdGiocatoreMano { get; set; }
        private bool Iniziato { get; set; }

        private GiocoBuilder()
        {
        }

        public static GiocoBuilder Init()
        {
            return new GiocoBuilder();
        }


        public GiocoBuilder AggiungiNumeroGiocatori(int numGiocatori)
        {
            NumGiocatori = numGiocatori;
            return this;
        }

        public GiocoBuilder AggiungiNome(string nome)
        {
            Nome = nome;
            return this;
        }

        public GiocoBuilder AggiungiPuntataMinima(int puntataMinima)
        {
            PuntataMinima = puntataMinima;
            return this;
        }

        public GiocoBuilder AggiungiMischiata(bool mischia)
        {
            Mischia = mischia;
            return this;
        }

        public GiocoBuilder AggiungiMazzi(int numMazzi)
        {
            NumMazziIniziali = numMazzi;
            return this;
        }

        public Gioco build()
        {
            return new Gioco(NumGiocatori, NumMazziIniziali, Mischia, Nome, PuntataMinima);
        }

    }
}
