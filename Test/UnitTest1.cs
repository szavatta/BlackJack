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
            Gioco gioco = Giocata(new Gioco(10,1));

            //setto i soldi e la puntata di ogni giocatore
            foreach (var giocatore in gioco.Giocatori)
            {
                giocatore.PuntataCorrente = 5_000; //5€
                giocatore.SoldiTotali = 100_000;   //100€
            }
            for (int i = 0; i < 100; i++)
            {
                gioco = Giocata(gioco);
                var giocatoriVincenti = gioco.Giocatori.Where(q =>
                    q.Punteggio() <= 21 && (q.Punteggio() > gioco.Mazziere.Punteggio() || gioco.Mazziere.Punteggio() > 21));

                var giocatoriPari =
                    gioco.Giocatori.Where(q => q.Punteggio() == gioco.Mazziere.Punteggio() && q.Punteggio() <= 21);

                var giocatoriPerdenti = gioco.Giocatori.Where(q =>
                    q.Punteggio() > 21 || (q.Punteggio() < gioco.Mazziere.Punteggio() && gioco.Mazziere.Punteggio() < 21));


                TestContext.Write("vincente: [ ");
                foreach (var vincente in giocatoriVincenti)
                {
                    gioco.Mazziere.SoldiTotali -= vincente.PuntataCorrente;
                    vincente.SoldiTotali += vincente.PuntataCorrente;
                    TestContext.Write($"{vincente}, ");
                }
                TestContext.WriteLine("]");

                TestContext.Write("perdente: [ ");
                foreach (var perdente in giocatoriPerdenti)
                {
                    gioco.Mazziere.SoldiTotali += perdente.PuntataCorrente;
                    perdente.SoldiTotali -= perdente.PuntataCorrente;
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

                gioco.Mazziere.Carte.RemoveAll(q => true);
                gioco.Giocatori.ForEach(q => q.Carte.RemoveAll(c =>true));
            }
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            int vinteGiocatori = 0;
            int vinteMazziere = 0;
            int totale = 0;

            for (int i = 0; i < 1000; i++)
            {
                Gioco gioco = Giocata(new Gioco(10));

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
            int count = 0;
            foreach (Giocatore giocatore in gioco.Giocatori)
            {

                count++;
                while (giocatore.Strategia.strategy(giocatore, gioco.Mazziere) == Giocatore.puntata.chiama)
                {
                    giocatore.Pesca();
                    //if (giocatore.Punteggio() > 21)
                    //{
                    //    TestContext.WriteLine($"giocatore numero {count} ha sforato");
                    //}
                }
                //TestContext.WriteLine($"giocatore numero {count} ha un punteggio di {giocatore.Punteggio()}");
            }
            while (gioco.Mazziere.Strategia.strategy(gioco.Mazziere) == Mazziere.puntata.chiama)
            {
                gioco.Mazziere.Pesca();
                //if (gioco.Mazziere.Punteggio() > 21)
                //{
                //    TestContext.WriteLine($"il mazziere ha sforato");
                //}
            }
            //TestContext.WriteLine($"mazziere ha un punteggio di {gioco.Mazziere.Punteggio()}");

            return gioco;
        }
    }
}