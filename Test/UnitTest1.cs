using Classes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Gioco gioco = Giocata(new Gioco(0,1));
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy()));
            gioco.Giocatori.Add(new Giocatore(gioco));

            //gioco.Giocatori.ForEach(q => q.SoldiTotali = 100);
            //gioco.Mazziere.SoldiTotali = 100;

            for (int i = 0; i < 10000; i++)
            {
                gioco = Giocata(gioco);
                var giocatoriVincenti = gioco.Giocatori.Where(q =>
                    q.Punteggio() <= 21 && (q.Punteggio() > gioco.Mazziere.Punteggio() || gioco.Mazziere.Punteggio() > 21));

                var giocatoriPari =
                    gioco.Giocatori.Where(q => q.Punteggio() == gioco.Mazziere.Punteggio() && q.Punteggio() <= 21);

                var giocatoriPerdenti = gioco.Giocatori.Where(q =>
                    q.Punteggio() > 21 || (q.Punteggio() < gioco.Mazziere.Punteggio() && gioco.Mazziere.Punteggio() <= 21));


                TestContext.Write("vincente: [ ");
                foreach (var vincente in giocatoriVincenti)
                {
                    TestContext.Write($"{vincente}, ");
                }
                TestContext.WriteLine("]");

                TestContext.Write("perdente: [ ");
                foreach (var perdente in giocatoriPerdenti)
                {
                    TestContext.Write($"{perdente}, ");
                }
                TestContext.WriteLine("]");

                TestContext.Write("pareggio: [ ");
                foreach (var pareggio in giocatoriPari)
                {
                    TestContext.Write($"{pareggio}, ");
                }
                TestContext.WriteLine("]");

                TestContext.WriteLine($"mazziere: {gioco.Mazziere.SoldiTotali}");

            }

            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            int vinteGiocatori = 0;
            int vinteMazziere = 0;
            int totale = 0;
            Gioco gioco = new Gioco(10);

            for (int i = 0; i < 1000; i++)
            {
                gioco.Giocata();

                int numeroGiocatoriVincenti = gioco.Giocatori.Where(q => q.Punteggio() <= 21 && (q.Punteggio() > gioco.Mazziere.Punteggio() || gioco.Mazziere.Punteggio() > 21)).Count();
                int numeroGiocatoriPari = gioco.Giocatori.Where(q => q.Punteggio() == gioco.Mazziere.Punteggio() && q.Punteggio() <= 21).Count();
                int numeroGiocatoriPerdenti = gioco.Giocatori.Count - numeroGiocatoriVincenti - numeroGiocatoriPari;

                vinteGiocatori += numeroGiocatoriVincenti;
                vinteMazziere += numeroGiocatoriPerdenti;
                totale += gioco.Giocatori.Count;
            }

            TestContext.WriteLine($"Vincite mazziere: {vinteMazziere}");
            TestContext.WriteLine($"vincite giocatori: {vinteGiocatori}");
            TestContext.WriteLine($"perc. mazziere: {Math.Round((decimal)vinteMazziere * 100 / (vinteMazziere + vinteGiocatori), 0)}%");

            Assert.Pass($"Vincite mazziere: {vinteMazziere}, vincite giocatori: {vinteGiocatori}, perc. mazziere: {Math.Round((decimal)vinteMazziere*100/(vinteMazziere+vinteGiocatori),0)}%");
        }




        private Gioco Giocata(Gioco gioco)
        {
            gioco.Giocatori.ForEach(q => q.Carte = new List<Carta>());
            gioco.Mazziere.Carte = new List<Carta>();
            gioco.Giocatori.ForEach(q => q.PuntataCorrente = 5);

            foreach (Giocatore giocatore in gioco.Giocatori)
            {
                giocatore.Pesca();
            }
            gioco.Mazziere.Pesca();
            foreach (Giocatore giocatore in gioco.Giocatori)
            {
                giocatore.Pesca();
            }
            gioco.Mazziere.Pesca();
            
            foreach (Giocatore giocatore in gioco.Giocatori)
            {
                while (giocatore.Strategia.Strategy(giocatore, gioco.Mazziere) == Giocatore.Puntata.Chiama)
                {
                    giocatore.Pesca();
                }
                if (giocatore.Strategia.Strategy(giocatore, gioco.Mazziere) == Giocatore.Puntata.Raddoppia) 
                {
                    giocatore.PuntataCorrente *= 2;
                }
            }
            while (gioco.Mazziere.Strategia.Strategy(gioco.Mazziere) == Mazziere.Puntata.Chiama)
            {
                gioco.Mazziere.Pesca();
            }

            var giocatoriVincenti = gioco.Giocatori.Where(q =>
                q.Punteggio() <= 21 && (q.Punteggio() > gioco.Mazziere.Punteggio() || gioco.Mazziere.Punteggio() > 21));

            var giocatoriPari =
                gioco.Giocatori.Where(q => q.Punteggio() == gioco.Mazziere.Punteggio() && q.Punteggio() <= 21);

            var giocatoriPerdenti = gioco.Giocatori.Where(q =>
                q.Punteggio() > 21 || (q.Punteggio() < gioco.Mazziere.Punteggio() && gioco.Mazziere.Punteggio() <= 21));

            foreach (var vincente in giocatoriVincenti)
            {
                gioco.Mazziere.SoldiTotali -= vincente.PuntataCorrente;
                vincente.SoldiTotali += vincente.PuntataCorrente;
            }

            foreach (var perdente in giocatoriPerdenti)
            {
                gioco.Mazziere.SoldiTotali += perdente.PuntataCorrente;
                perdente.SoldiTotali -= perdente.PuntataCorrente;
            }

            if (giocatoriVincenti.Count() + giocatoriPerdenti.Count() + giocatoriPari.Count() != gioco.Giocatori.Count())
                throw new Exception("Non corrispondono i giocatori");

            return gioco;
        }
    }
}