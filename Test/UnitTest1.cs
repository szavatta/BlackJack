using Classes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Test
{
    public class Tests1
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGiocate()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(0).AggiungiMazzi(6).AggiungiMischiata(true).build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy())
                .build());

            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new StrategiaConteggio())
                .build());

            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new SempliceStrategiaGiocatore())
                .build());

            //gioco.Giocatori.ForEach(q => q.SoldiTotali = 100);
            //gioco.Mazziere.SoldiTotali = 100;

            for (int i = 0; i < 10000; i++)
            {
                gioco.Giocata();

                TestContext.Write("vincente: [ ");
                foreach (var vincente in gioco.GiocatoriVincenti())
                {
                    TestContext.Write($"{vincente}, ");
                }
                TestContext.WriteLine("]");

                TestContext.Write("perdente: [ ");
                foreach (var perdente in gioco.GiocatoriPerdenti())
                {
                    TestContext.Write($"{perdente}, ");
                }
                TestContext.WriteLine("]");

                TestContext.Write("pareggio: [ ");
                foreach (var pareggio in gioco.GiocatoriPari())
                {
                    TestContext.Write($"{pareggio}, ");
                }
                TestContext.WriteLine("]");

                TestContext.WriteLine($"mazziere: {gioco.Mazziere.SoldiTotali}");
                TestContext.WriteLine($"truecount: {gioco.Mazzo.GetTrueCount()}");

            }

            Assert.Pass();
        }

        [Test]
        public void TestGiocate2()
        {
            int vinteGiocatori = 0;
            int vinteMazziere = 0;
            int totale = 0;
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(10).build();

            for (int i = 0; i < 1000; i++)
            {
                gioco.Giocata();

                vinteGiocatori += gioco.GiocatoriVincenti().Count();
                vinteMazziere += gioco.GiocatoriPerdenti().Count();
                totale += gioco.Giocatori.Count;
            }

            TestContext.WriteLine($"Vincite mazziere: {vinteMazziere}");
            TestContext.WriteLine($"vincite giocatori: {vinteGiocatori}");
            TestContext.WriteLine($"perc. mazziere: {Math.Round((decimal)vinteMazziere * 100 / (vinteMazziere + vinteGiocatori), 0)}%");

            Assert.Pass($"Vincite mazziere: {vinteMazziere}, vincite giocatori: {vinteGiocatori}, perc. mazziere: {Math.Round((decimal)vinteMazziere * 100 / (vinteMazziere + vinteGiocatori), 0)}%");
        }

        [Test]
        public void TestGiocateRaddoppio()
        {
            int vinteGiocatori = 0;
            int vinteMazziere = 0;
            int totale = 0;
            Gioco gioco = GiocoBuilder.Init()
                .AggiungiMazzi(6)
                .AggiungiPuntataMinima(5)
                .AggiungiPuntataMassima(2000)
                .build();

            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new StrategiaTriplica()).build());
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new StrategiaDuplica()).build());
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new BasicStrategy()).build());
            int puntatamassima = 0;
            int perseconsecutive = 0;
            int perseconsecutivemax = 0;
            int puntatamassima2 = 0;
            int perseconsecutive2 = 0;
            int perseconsecutivemax2 = 0;

            int mani = 100;
            for (int i = 0; i < mani; i++)
            {
                gioco.Giocata();

                Giocatore giocatore = gioco.Giocatori[0];
                if (giocatore.PuntataCorrente > puntatamassima)
                    puntatamassima = giocatore.PuntataCorrente;

                if (giocatore.Risultato == Giocatore.EnumRisultato.Perso)
                {
                    perseconsecutive++;
                    if (perseconsecutive > perseconsecutivemax)
                        perseconsecutivemax = perseconsecutive;
                }
                else if (giocatore.Risultato == Giocatore.EnumRisultato.Vinto)
                {
                    perseconsecutive = 0;
                }

                giocatore = gioco.Giocatori[1];
                if (giocatore.PuntataCorrente > puntatamassima2)
                    puntatamassima2 = giocatore.PuntataCorrente;

                if (giocatore.Risultato == Giocatore.EnumRisultato.Perso)
                {
                    perseconsecutive2++;
                    if (perseconsecutive2 > perseconsecutivemax2)
                        perseconsecutivemax2 = perseconsecutive2;
                }
                else if (giocatore.Risultato == Giocatore.EnumRisultato.Vinto)
                {
                    perseconsecutive2 = 0;
                }


                vinteGiocatori += gioco.GiocatoriVincenti().Count();
                vinteMazziere += gioco.GiocatoriPerdenti().Count();
                totale += gioco.Giocatori.Count;
            }

            TestContext.WriteLine($"Mani: {gioco.Giri}");
            TestContext.WriteLine($"Puntata massima: {gioco.PuntataMassima}");
            TestContext.WriteLine($"Vincita giocatore Triplica: {gioco.Giocatori[0].SoldiTotali}");
            TestContext.WriteLine($"Puntata massima: {puntatamassima}");
            TestContext.WriteLine($"Perdite consecutive: {perseconsecutivemax}");
            TestContext.WriteLine($"Vincita giocatore Raddoppio: {gioco.Giocatori[1].SoldiTotali}");
            TestContext.WriteLine($"Puntata massima: {puntatamassima2}");
            TestContext.WriteLine($"Perdite consecutive: {perseconsecutivemax2}");
            TestContext.WriteLine($"Vincita giocatore 3: {gioco.Giocatori[2].SoldiTotali}");

        }


    }

    public class Tests2
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(2).AggiungiMazzi(0).build();
            for (int i = 0; i < 4; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Picche));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Cuori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Cuori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Quadri));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Picche));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Picche));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Cuori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori));
            }

            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
        }

        [Test]
        public void Test2()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(2).AggiungiMazzi(0).build();
            for (int i = 0; i < 4; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Picche));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Cuori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Quadri));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Cuori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori));

            }

            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
            Assert.AreEqual(21, gioco.Giocatori[0].Punteggio);
        }

        [Test]
        public void TestConteggio()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiMazzi(1).build();
            for (int i = 0; i < 52; i++)
            {
                gioco.Giocatori[0].Pesca(0);
            }
            Assert.AreEqual(0, gioco.Mazzo.Conteggio);
        }

        [Test]
        public void Test3()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(2).AggiungiMazzi(0).build();
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori));
            }

            gioco.Giocata();

            Assert.AreEqual(19, gioco.Giocatori[0].Punteggio);
        }

        [Test]
        public void TestBlackJack()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiMazzi(0).build();
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori));
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori));
            }
            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
            Assert.AreEqual(21, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.IsTrue(gioco.Giocatori[0].HasBlackJack());

        }

        [Test]
        public void TestBlackJack2()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiMazzi(0).build();
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori));
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori));
            }
            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
            Assert.AreEqual(21, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.IsTrue(gioco.Giocatori[0].HasBlackJack());
            Assert.IsTrue(gioco.GiocatoriVincenti().Count() == 0);
            Assert.IsTrue(gioco.GiocatoriPerdenti().Count() == 0);
            Assert.IsTrue(gioco.GiocatoriPari().Count() == 1);
        }

        [Test]
        public void TestBlackJack3()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiMazzi(0).build();
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri));
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori));
            }
            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
            Assert.AreEqual(21, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.IsFalse(gioco.Giocatori[0].HasBlackJack());
            Assert.IsTrue(gioco.GiocatoriVincenti().Count() == 0);
            Assert.IsTrue(gioco.GiocatoriPerdenti().Count() == 1);
            Assert.IsTrue(gioco.GiocatoriPari().Count() == 0);
        }
        [Test]
        public void TestCount()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(0).AggiungiMazzi(1).AggiungiMischiata(false).build();
            
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new BasicStrategy()).build());

            gioco.Giocata();
            Assert.AreEqual(4, gioco.Mazzo.Conteggio);
            gioco.Giocata();
            Assert.AreEqual(2, gioco.Mazzo.Conteggio);
            gioco.Giocata();
            Assert.AreEqual(2, gioco.Mazzo.Conteggio);
        }

        [Test]
        public void TestSplit()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(0).AggiungiMazzi(0).AggiungiMischiata(false).build();
            
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new BasicStrategy()).build());
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new SempliceStrategiaGiocatore()).build());
            
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Cuori)); 

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Cuori));

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Cuori));

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Cuori));
            gioco.Giocata();

            Assert.AreEqual(0,gioco.Giocatori[0].SoldiTotali);
            Assert.AreEqual(0, gioco.Giocatori[1].SoldiTotali);
            Assert.AreEqual(0, gioco.Mazziere.SoldiTotali);

        }

        [Test]
        public void TestDueAssi()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiMazzi(0).build();
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Cuori));
            }
            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
            Assert.AreEqual(21, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.False(gioco.Giocatori[0].HasBlackJack());

        }
    }
}