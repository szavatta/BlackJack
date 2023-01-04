using Classes;
using NUnit.Framework;
using System;
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

            Gioco gioco = new Gioco(5);
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
                    if (giocatore.Punteggio() > 21) {
                        TestContext.WriteLine($"giocatore numero {count} ha sforato");
                    }
                }
                TestContext.WriteLine($"giocatore numero {count} ha un punteggio di {giocatore.Punteggio()}");
            }
            while (gioco.Mazziere.Punteggio() < 17)
            {
                gioco.Mazziere.Pesca();
                if (gioco.Mazziere.Punteggio() > 21)
                {
                    TestContext.WriteLine($"il mazziere ha sforato");
                }
            }
            TestContext.WriteLine($"mazziere ha un punteggio di {gioco.Mazziere.Punteggio()}");
    
            int numeroGiocatoriVincenti = gioco.Giocatori.Where(q => q.Punteggio() > gioco.Mazziere.Punteggio() && q.Punteggio() < 21).Count();
            int numeroGiocatoriPari = gioco.Giocatori.Where(q => q.Punteggio() == gioco.Mazziere.Punteggio() && q.Punteggio() < 21).Count();
            TestContext.WriteLine($"numero giocatori vincenti: {numeroGiocatoriVincenti}");
            TestContext.WriteLine($"numero giocatori perdenti: {gioco.Giocatori.Count - numeroGiocatoriVincenti - numeroGiocatoriPari}");
            TestContext.WriteLine($"numero giocatori in pareggio: {numeroGiocatoriPari}");





            Assert.Pass();
        }
    }
}