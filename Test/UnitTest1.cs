using Classes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
            Gioco gioco = Giocata(10);
    
            int numeroGiocatoriVincenti = gioco.Giocatori.Where(q => q.Punteggio() <= 21 && (q.Punteggio() > gioco.Mazziere.Punteggio() || gioco.Mazziere.Punteggio() > 21)).Count();
            int numeroGiocatoriPari = gioco.Giocatori.Where(q => q.Punteggio() == gioco.Mazziere.Punteggio() && q.Punteggio() <= 21).Count();
            //TestContext.WriteLine($"numero giocatori vincenti: {numeroGiocatoriVincenti}");
            //TestContext.WriteLine($"numero giocatori perdenti: {gioco.Giocatori.Count - numeroGiocatoriVincenti - numeroGiocatoriPari}");
            //TestContext.WriteLine($"numero giocatori in pareggio: {numeroGiocatoriPari}");


            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            int vinteGiocatori = 0;
            int vinteMazziere = 0;

            for (int i = 0; i < 10; i++)
            {
                Gioco gioco = Giocata(100);

                int numeroGiocatoriVincenti = gioco.Giocatori.Where(q => q.Punteggio() <= 21 && (q.Punteggio() > gioco.Mazziere.Punteggio() || gioco.Mazziere.Punteggio() > 21)).Count();
                int numeroGiocatoriPari = gioco.Giocatori.Where(q => q.Punteggio() == gioco.Mazziere.Punteggio() && q.Punteggio() <= 21).Count();
                int numeroGiocatoriPerdenti = gioco.Giocatori.Count - numeroGiocatoriVincenti - numeroGiocatoriPari;

                vinteGiocatori += numeroGiocatoriVincenti;
                vinteMazziere += numeroGiocatoriPerdenti;
            }


            Assert.Pass($"Vincite mazziere: {vinteMazziere}, vincite giocatori: {vinteGiocatori}");
        }




        private Gioco Giocata(int numGiocatori)
        {
            Gioco gioco = new Gioco(numGiocatori);
            //Gioco gioco2 = (Gioco)gioco.Clone();
            ////ripartire da qui per ripetere la stessa situazione
            //gioco = (Gioco)gioco2.Clone();
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
                while (giocatore.Punteggio() < 17)
                {
                    giocatore.Pesca();
                    //if (giocatore.Punteggio() > 21)
                    //{
                    //    TestContext.WriteLine($"giocatore numero {count} ha sforato");
                    //}
                }
                //TestContext.WriteLine($"giocatore numero {count} ha un punteggio di {giocatore.Punteggio()}");
            }
            while (gioco.Mazziere.Punteggio() < 17)
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