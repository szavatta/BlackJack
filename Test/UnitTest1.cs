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
        public void TestGiocate()
        {
            Gioco gioco = new Gioco(0, 1);
            gioco.Giocata();
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy()));
            gioco.Giocatori.Add(new Giocatore(gioco));

            //gioco.Giocatori.ForEach(q => q.SoldiTotali = 100);
            //gioco.Mazziere.SoldiTotali = 100;

            for (int i = 0; i < 10000; i++)
            {
                gioco.Giocata();
                var giocatoriVincenti = gioco.Giocatori.Where(q =>
                    q.Punteggio <= 21 && (q.Punteggio > gioco.Mazziere.Punteggio || gioco.Mazziere.Punteggio > 21));

                var giocatoriPari =
                    gioco.Giocatori.Where(q => q.Punteggio == gioco.Mazziere.Punteggio && q.Punteggio <= 21);

                var giocatoriPerdenti = gioco.Giocatori.Where(q =>
                    q.Punteggio > 21 || (q.Punteggio < gioco.Mazziere.Punteggio && gioco.Mazziere.Punteggio <= 21));


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
        public void TestGiocate2()
        {
            int vinteGiocatori = 0;
            int vinteMazziere = 0;
            int totale = 0;
            Gioco gioco = new Gioco(10);

            for (int i = 0; i < 1000; i++)
            {
                gioco.Giocata();

                int numeroGiocatoriVincenti = gioco.Giocatori.Where(q => q.Punteggio <= 21 && (q.Punteggio > gioco.Mazziere.Punteggio || gioco.Mazziere.Punteggio > 21)).Count();
                int numeroGiocatoriPari = gioco.Giocatori.Where(q => q.Punteggio == gioco.Mazziere.Punteggio && q.Punteggio <= 21).Count();
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

        [Test]
        public void Test1() 
        {
            Gioco gioco = new Gioco(2, 0);
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

            Assert.AreEqual(21, gioco.Mazziere.Carte.Select(q=>q.Valore).Sum());
        }

        [Test]
        public void Test2()
        {
            Gioco gioco = new Gioco(2, 0);
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

            Assert.AreEqual(21, gioco.Mazziere.Carte.Select(q => q.Valore).Sum());
            Assert.AreEqual(21, gioco.Giocatori[0].Carte.Select(q => q.Valore).Sum());
        }


        [Test]
        public void Test3()
        {
            Gioco gioco = new Gioco(2, 0);
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


    }
}