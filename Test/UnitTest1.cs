using Classes;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;

namespace Test
{
    public class Simulations
    {
        [SetUp]
        public void Setup()
        {
        }

        class Result
        {
            public decimal Cassa { get; set; }
            public decimal Puntata { get; set; }
            public string Risultato { get; set; }
            public int PunteggioGiocatore { get; set; }
            public int PunteggioMazziere { get; set; }
            public string CarteGiocatore { get; set; }
            public string CarteMazziere { get; set; }
            public int Conteggio { get; set; }
            public decimal TrueCount { get; set; }
            public int NumCarte { get; set; }
        }

        [Test]
        public void TestGiocate()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init()
                .AggiungiNumeroGiocatori(0)
                .AggiungiMazzi(6)
                .AggiungiMischiata(1)
                .AggiungiSecondaCartaInizialeMazziere()
                .AggiungiPuntataMinima(10)
                .AggiungiPuntataMassima(1000)
                .AggiungiPercentualeMischiata(20)
                //.AggiungiRaddoppiaNonDisponibile()
                .build();

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiPuntataBase(10)
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new BasicStrategy())
            //    .build());

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiPuntataBase(10)
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new BasicStrategy124())
            //    .build());

            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiPuntataBase(100)
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategyDeviation())
                .build());

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiPuntataBase(100)
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new StrategiaRaddoppia())
            //    .build());

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new BasicStrategyS17())
            //    .build());

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new BasicStrategy2())
            //    .build());

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new StrategiaPunteggio(16))
            //    .build());

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new StrategiaConteggio(17))
            //    .build());

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new StrategiaConteggio(18))
            //    .build());

            //gioco.Giocatori.Add(GiocatoreBuilder.Init()
            //    .AggiungiGioco(gioco)
            //    .AggiungiStrategia(new SempliceStrategiaGiocatore())
            //    .build());

            //gioco.Giocatori.ForEach(q => q.SoldiTotali = 100);
            //gioco.Mazziere.SoldiTotali = 100;
            List<decimal> max = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> min = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> vmax = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> pmax = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> vcons = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> pcons = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> nblackjack = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> ppair = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> cpair = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            List<decimal> mpair = new List<decimal> { 0, 0, 0, 0, 0, 0, 0 };
            int[] puntiMazziere = new int[30];
            int maxsplit = 0;
            int numass = 0;
            int nblackjackmazziere = 0;
            decimal vass = 0;
            decimal pass = 0;
            List<Result> report = new List<Result>();

            for (int i = 0; i < 1000; i++)
            {
                gioco.Giocata();

                for (int x = 0; x < gioco.Giocatori.Count(q => q.GiocatoreSplit == null); x++)
                {
                    if (gioco.Giocatori[x].SoldiTotali > max[x]) max[x] = gioco.Giocatori[x].SoldiTotali;
                    if (gioco.Giocatori[x].SoldiTotali < min[x]) min[x] = gioco.Giocatori[x].SoldiTotali;
                    if (gioco.Giocatori[x].Risultato == Giocatore.EnumRisultato.Vinto)
                    {
                        vcons[x]++;
                        pcons[x] = 0;
                        if (vcons[x] > vmax[x])
                            vmax[x] = vcons[x];
                    }
                    else if (gioco.Giocatori[x].Risultato == Giocatore.EnumRisultato.Perso)
                    {
                        pcons[x]++;
                        vcons[x] = 0;
                        if (pcons[x] > pmax[x])
                            pmax[x] = pcons[x];
                    }

                    if (gioco.Giocatori[x].HasBlackJack())
                        nblackjack[x]++;

                    if (gioco.Giocatori[x].Carte[0].Numero == gioco.Giocatori[x].Carte[1].Numero)
                    {
                        if (gioco.Giocatori[x].Carte[0].Seme == gioco.Giocatori[x].Carte[1].Seme)
                            ppair[x]++;
                        else if (gioco.Giocatori[x].Carte[0].GetColoreSeme() == gioco.Giocatori[x].Carte[1].GetColoreSeme())
                            cpair[x]++;
                        else
                            mpair[x]++;
                    }
                }

                if (gioco.Giocatori.Where(q => q.GiocatoreSplit != null).Count() > maxsplit)
                    maxsplit = gioco.Giocatori.Where(q => q.GiocatoreSplit != null).Count();

                if (gioco.Giocatori[0].PuntataAssicurazione > 0)
                {
                    numass++;
                    if (gioco.Mazziere.HasBlackJack())
                        vass += gioco.Giocatori[0].PuntataAssicurazione * 2;
                    else
                        pass += gioco.Giocatori[0].PuntataAssicurazione;
                }
                puntiMazziere[gioco.Mazziere.Punteggio]++;
                if (gioco.Mazziere.HasBlackJack())
                    nblackjackmazziere++;

                report.Add(new Result
                {
                    Cassa = gioco.Giocatori[0].SoldiTotali,
                    Puntata = gioco.Giocatori[0].PuntataCorrente,
                    Risultato = gioco.Giocatori[0].Risultato.ToString(),
                    PunteggioGiocatore = gioco.Giocatori[0].Punteggio,
                    PunteggioMazziere = gioco.Mazziere.Punteggio,
                    CarteGiocatore = string.Join(",", gioco.Giocatori[0].Carte),
                    CarteMazziere = string.Join(",", gioco.Mazziere.Carte),
                    Conteggio = gioco.Giocatori[0].Strategia.Conteggio,
                    TrueCount = gioco.Giocatori[0].Strategia.GetTrueCount(gioco.Mazzo.Carte.Count),
                    NumCarte = gioco.Mazzo.Carte.Count
                });

                //Assert.AreEqual(Math.Abs(gioco.Mazziere.SoldiTotali), Math.Abs(gioco.Giocatori.Where(q => q.GiocatoreSplit == null).Sum(q => q.SoldiTotali)));
            }
            var dt = Utils.GetDataTable(report);
            TestContext.WriteLine($"Mani: {gioco.Giri}");
            gioco.Log.AppendLine("");
            gioco.Log.AppendLine("Partita finita");
            gioco.Log.AppendLine("");
            string riga = "";

            for (int x = 0; x < gioco.Giocatori.Count(q => q.GiocatoreSplit == null); x++)
            {
                riga = $"{gioco.Giocatori[x].Nome}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = gioco.Giocatori[x].Strategia.ToString().Replace("Classes.", "")
                    + (gioco.Giocatori[x].Strategia is StrategiaPunteggio ? " " + ((StrategiaPunteggio)gioco.Giocatori[x].Strategia).Punteggio.ToString() : "");
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Vincita finale: {gioco.Giocatori[x].SoldiTotali}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Mani vinte: {gioco.Giocatori[x].ManiVinte}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Mani perse: {gioco.Giocatori[x].ManiPerse}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Mani sballate: {gioco.Giocatori[x].ManiSballate}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Vincita massima: {max[x]}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Perdita massima: {min[x]}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Vincite consecutive: {vmax[x]}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Perdite consecutive: {pmax[x]}";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Black Jack: {nblackjack[x]} ({Math.Round((decimal)nblackjack[x] / gioco.Giri * 100, 1)}%)";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Perfect pair: {ppair[x]} ({Math.Round((decimal)ppair[x] / gioco.Giri * 100, 1)}%)";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Color pair: {cpair[x]} ({Math.Round((decimal)cpair[x] / gioco.Giri * 100, 1)}%)";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                riga = $"   Mixed pair: {mpair[x]} ({Math.Round((decimal)mpair[x] / gioco.Giri * 100, 1)}%)";
                gioco.Log.AppendLine(riga);
                TestContext.WriteLine(riga);
                if (x == 0)
                {
                    riga = $"   Num assicurazioni: {numass}";
                    gioco.Log.AppendLine(riga);
                    TestContext.WriteLine(riga);
                    riga = $"   Vincite assicurazioni: {vass}";
                    gioco.Log.AppendLine(riga);
                    TestContext.WriteLine(riga);
                    riga = $"   Perdite assicurazioni: {pass}";
                    gioco.Log.AppendLine(riga);
                    TestContext.WriteLine(riga);
                }
            }

            riga = "Mazziere";
            gioco.Log.AppendLine(riga);
            TestContext.WriteLine(riga);
            riga = $"   Vincita finale: {gioco.Mazziere.SoldiTotali}";
            gioco.Log.AppendLine(riga);
            TestContext.WriteLine(riga);
            riga = $"   Mani sballate: {gioco.Mazziere.ManiSballate}";
            gioco.Log.AppendLine(riga);
            TestContext.WriteLine(riga);
            for (int i = 17; i < puntiMazziere.Length; i++)
            {
                if (puntiMazziere[i] > 0)
                {
                    riga = $"   Punti: {i}: {puntiMazziere[i]} ({Math.Round((decimal)puntiMazziere[i] / gioco.Giri * 100, 1)}%)";
                    gioco.Log.AppendLine(riga);
                    TestContext.WriteLine(riga);
                }
            }
            riga = $"   Black Jack: {nblackjackmazziere} ({Math.Round((decimal)nblackjackmazziere / gioco.Giri * 100, 1)}%)";
            gioco.Log.AppendLine(riga);
            TestContext.WriteLine(riga);

            riga = $"Max split: {maxsplit}";
            gioco.Log.AppendLine(riga);
            TestContext.WriteLine(riga);

        }

        [Test]
        public void TestGiocate2()
        {
            int vinteGiocatori = 0;
            int vinteMazziere = 0;
            int totale = 0;
            Gioco gioco = Gioco.GiocoBuilder.Init()
                .AggiungiNumeroGiocatori(10)
                .AggiungiMazzi(6)
                .build();

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
            Gioco gioco = Gioco.GiocoBuilder.Init()
                .AggiungiMazzi(6)
                .AggiungiPuntataMinima(5)
                .AggiungiPuntataMassima(2000)
                .build();

            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new StrategiaTriplica()).build());
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new StrategiaRaddoppia()).build());
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new BasicStrategy()).build());
            decimal puntatamassima = 0;
            int perseconsecutive = 0;
            int perseconsecutivemax = 0;
            decimal puntatamassima2 = 0;
            int perseconsecutive2 = 0;
            int perseconsecutivemax2 = 0;

            int mani = 1000;
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
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(2).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();
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
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(2).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();
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
        public void TestAssicurazione()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiPuntataMinima(5).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();

            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiStrategia(new BasicStrategy()).AggiungiGioco(gioco).build());

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
            for (int i = 0; i < 48; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri));
            }

            gioco.Giocatori[0].Strategia.Conteggio = 8;
            gioco.Giocata();

            Assert.AreEqual(5, gioco.Giocatori[0].SoldiTotali);
        }

        [Test]
        public void TestAssicurazione2()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiPuntataMinima(5).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();

            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiStrategia(new BasicStrategy()).AggiungiGioco(gioco).build());

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
            for (int i = 0; i < 48; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri));
            }

            gioco.Giocatori[0].Strategia.Conteggio = 8;
            gioco.Giocata();

            Assert.AreEqual(0, gioco.Giocatori[0].SoldiTotali);
        }

        [Test]
        public void TestAssicurazione3()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiPuntataMinima(5).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();

            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiStrategia(new BasicStrategy()).AggiungiGioco(gioco).build());

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri));
            for (int i = 0; i < 48; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri));
            }

            gioco.Giocatori[0].Strategia.Conteggio = 4;
            gioco.Giocata();

            Assert.AreEqual(4, gioco.Giocatori[0].SoldiTotali);
        }

        [Test]
        public void TestConteggio()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init()
                .AggiungiNumeroGiocatori(0)
                .AggiungiMazzi(1)
                .AggiungiMischiata(2)
                //.AggiungiPercentualeMischiata(0)
                .build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new StrategiaPunteggio(17)).build());
            
            for (int i = 0; i < gioco.Mazzo.NumCarte; i++)
            {
                gioco.Giocatori[0].Chiama();
            }
            Assert.AreEqual(0, gioco.Giocatori[0].Strategia.Conteggio);
        }

        [Test]
        public void TestConteggio2()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init()
                .AggiungiNumeroGiocatori(0)
                .AggiungiMazzi(1)
                .AggiungiSecondaCartaInizialeMazziere()
                .AggiungiMischiata(1)
                //.AggiungiPercentualeMischiata(0)
                .build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new StrategiaPunteggio(17)).build());
            for (int i = 0; i < 10; i++)
            {
                gioco.Giocatori[0].Chiama();
            }
            Assert.AreEqual(-2, gioco.Giocatori[0].Strategia.Conteggio);
            for (int i = 0; i < 10; i++)
            {
                gioco.Giocatori[0].Chiama();
            }
            Assert.AreEqual(4, gioco.Giocatori[0].Strategia.Conteggio);
        }

        [Test]
        public void TestConteggio3()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init()
                .AggiungiNumeroGiocatori(0)
                .AggiungiSecondaCartaInizialeMazziere()
                .AggiungiMazzi(6)
                .AggiungiMischiata(1)
                //.AggiungiPercentualeMischiata(0)
                .build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new StrategiaPunteggio(17)).build());
            int maxConteggio = 0;
            int minConteggio = 0;
            decimal minTrueCount = 0;
            decimal maxTrueCount = 0;
            int numCarte = gioco.Mazzo.Carte.Count;
            for (int i = 0; i < numCarte; i++)
            {
                gioco.Giocatori[0].Chiama();
                if (gioco.Giocatori[0].Strategia.Conteggio > maxConteggio)
                    maxConteggio = gioco.Giocatori[0].Strategia.Conteggio;
                if (gioco.Giocatori[0].Strategia.Conteggio < minConteggio)
                    minConteggio = gioco.Giocatori[0].Strategia.Conteggio;
                if (gioco.Giocatori[0].Strategia.GetTrueCount(gioco.Mazzo.Carte.Count) > maxTrueCount)
                    maxTrueCount = gioco.Giocatori[0].Strategia.GetTrueCount(gioco.Mazzo.Carte.Count);
                if (gioco.Giocatori[0].Strategia.GetTrueCount(gioco.Mazzo.Carte.Count) < minTrueCount)
                    minTrueCount = gioco.Giocatori[0].Strategia.GetTrueCount(gioco.Mazzo.Carte.Count);
            }
            Assert.AreEqual(0, gioco.Giocatori[0].Strategia.Conteggio);
            Assert.AreEqual(3, maxConteggio);
            Assert.AreEqual(-13, minConteggio);
        }

        [Test]
        public void Test3()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(2).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Picche));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Picche));
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
            Assert.AreEqual(20, gioco.Giocatori[1].Punteggio);
        }

        [Test]
        public void TestBlackJack()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();
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
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();
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
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();
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
            Assert.AreEqual(13, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.IsFalse(gioco.Giocatori[0].HasBlackJack());
            Assert.IsTrue(gioco.GiocatoriVincenti().Count() == 0);
            Assert.IsTrue(gioco.GiocatoriPerdenti().Count() == 1);
            Assert.IsTrue(gioco.GiocatoriPari().Count() == 0);
        }

        [Test]
        public void TestBlackJackMazziere()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).AggiungiPuntataMinima(5).build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy())
                .build());
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche)); //mazziere
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori)); //mazziere
            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
            Assert.AreEqual(20, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.IsTrue(!gioco.Giocatori[0].HasBlackJack());
            Assert.AreEqual(Giocatore.EnumRisultato.Perso, gioco.Giocatori[0].Risultato);
        }

        [Test]
        public void TestBlackJackMazziere2()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).AggiungiPuntataMinima(5).build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy())
                .build());
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Picche)); //mazziere
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori)); //mazziere
            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
            Assert.AreEqual(12, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.IsTrue(!gioco.Giocatori[0].HasBlackJack());
            Assert.AreEqual(Giocatore.EnumRisultato.Perso, gioco.Giocatori[0].Risultato);
        }

        [Test]
        public void TestBlackJackMazziere3()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).AggiungiPuntataMinima(5).build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy())
                .build());
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Picche)); //mazziere
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori)); //mazziere
            gioco.Giocata();

            Assert.AreEqual(21, gioco.Mazziere.Punteggio);
            Assert.AreEqual(21, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.IsTrue(gioco.Giocatori[0].HasBlackJack());
            Assert.AreEqual(Giocatore.EnumRisultato.Pari, gioco.Giocatori[0].Risultato);
        }

        [Test]
        public void TestBlackJack4()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).AggiungiPuntataMinima(5).build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy())
                .build());
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Picche)); //mazziere
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori)); //mazziere
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori));
            }
            gioco.Giocata();

            Assert.AreEqual(17, gioco.Mazziere.Punteggio);
            Assert.AreEqual(21, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(!gioco.Mazziere.HasBlackJack());
            Assert.IsTrue(gioco.Giocatori[0].HasBlackJack());
            Assert.IsTrue(gioco.Giocatori[0].SoldiTotali == (decimal)7.5);
        }


        [Test]
        public void Test21Giocatore()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).AggiungiPuntataMinima(5).build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy())
                .build());
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Picche)); //mazziere
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Quadri)); //giocatore
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori)); //mazziere
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Quadri)); //giocatore
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori));
            }
            gioco.GiocataIniziale();
            gioco.Giocatori[0].Chiama();

            Assert.AreEqual(17, gioco.Mazziere.Punteggio);
            Assert.AreEqual(21, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(!gioco.Mazziere.HasBlackJack());
            Assert.IsTrue(!gioco.Giocatori[0].HasBlackJack());
        }

        [Test]
        public void TestCount()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(0).AggiungiPuntataMinima(5).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(1).build();
            
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new StrategiaPunteggio(17)).build());

            gioco.Giocata();
            Assert.AreEqual(4, gioco.Giocatori[0].Strategia.Conteggio);
            gioco.Giocata();
            Assert.AreEqual(2, gioco.Giocatori[0].Strategia.Conteggio);
            gioco.Giocata();
            Assert.AreEqual(2, gioco.Giocatori[0].Strategia.Conteggio);
        }

        [Test]
        public void TestSplit()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(0).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).AggiungiPuntataMinima(1).build();
            
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new BasicStrategy()).build());
            gioco.Giocatori.Add(GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new SempliceStrategiaGiocatore()).build());
            
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori));
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
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Cuori));

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Cuori));
            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Cuori));

            gioco.Mazzo.Carte.Add(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Cuori));
            gioco.Giocata();

            Assert.AreEqual(4, gioco.Giocatori.Count);

            gioco.Giocatori.RemoveAll(q => q.GiocatoreSplit != null);

            Assert.AreEqual(1.5,gioco.Giocatori[0].SoldiTotali);
            Assert.AreEqual(-5, gioco.Giocatori[1].SoldiTotali);
            Assert.AreEqual(3.5, gioco.Mazziere.SoldiTotali);

        }

        [Test]
        public void TestDueAssi()
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiNumeroGiocatori(1).AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();
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
            Assert.AreEqual(12, gioco.Giocatori[0].Punteggio);
            Assert.IsTrue(gioco.Mazziere.HasBlackJack());
            Assert.False(gioco.Giocatori[0].HasBlackJack());

        }
    }

    public class TestBasicStrategy
    {
        Gioco gioco;

        [SetUp]
        public void Setup()
        {
            gioco = Gioco.GiocoBuilder.Init().AggiungiSecondaCartaInizialeMazziere().AggiungiMazzi(0).build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategyDeviation())
                .build());

        }

        [Test]
        public void Test17_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test17_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test16_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test16_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test16_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test16_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test16_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test16_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test16_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test16_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_9_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 4; i < 7; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -1; i < -5; i--)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_10_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
            }
        }

        [Test]
        public void Test16_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
            }
        }
        [Test]
        public void Test16_asso_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -1; i < -5; i--)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_jack_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -1; i < -5; i--)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_donna_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -1; i < -5; i--)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test16_re_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void Test15_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test15_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test15_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test15_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test15_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test15_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test15_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test15_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 3; i < -5; i--)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_10_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 4; i < 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_asso_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 5; i < 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 3; i < -5; i--)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_jack_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 4; i < 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 3; i < -5; i--)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_donna_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 4; i < 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 3; i < -5; i--)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test15_re_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 4; i < 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void Test14_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test14_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test14_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test14_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test14_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test14_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test14_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test14_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -4; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test14_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test14_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test14_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void Test14_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test14_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void Test13_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        public void Test13_2_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 0; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test13_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test13_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test13_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test13_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test13_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test13_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test13_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -4; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test13_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test13_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test13_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test13_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test13_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void Test12_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_2_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = 3; i < 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -2; i < 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_3_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = 2; i < 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = 1; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_4_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 0; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test12_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test12_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test12_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, 0));
        }

        [Test]
        public void Test12_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -4; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 4; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test12_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -3; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void Test11_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void Test11_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test11_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void Test10_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_10_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 4; i <= 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_asso_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 3; i <= 8; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_jack_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 4; i <= 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_donna_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 4; i <= 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test10_re_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 4; i <= 10; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void Test9_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_2_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 1; i <= 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test] 
        public void Test9_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_7_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 3; i <= 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void Test9_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test9_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void Test8_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_6_rev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = 2; i <= 8; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void Test8_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 2; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void Test8_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -4; i <= 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void TestA9_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void TestA9_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA9_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void TestA8_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_4_rev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = 3; i < 8; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_5_rev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = 1; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 0; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_6_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = 1; i <= 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA8_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void TestA7_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Stai, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void TestA7_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA7_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void TestA6_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i <= 0; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_2_dev()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = 1; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void TestA6_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA6_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void TestA5_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void TestA5_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA5_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void TestA4_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void TestA4_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA4_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void TestA3_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }


        [Test]
        public void TestA3_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA3_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        /// <summary>
        /// 
        /// </summary>

        [Test]
        public void TestA2_2()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_3()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_4()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 3; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_5()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 1; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_6()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Raddoppia, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_7()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_8()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_9()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_10()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_asso()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_jack()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_donna()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

        [Test]
        public void TestA2_re()
        {
            gioco.Mazziere
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori));

            gioco.Giocatori.First()
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori))
                .AggiungiCarta(new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori));

            for (int i = -5; i < 5; i++)
            {
                Assert.AreEqual(GiocatoreSemplice.Giocata.Chiama, gioco.Giocatori.First().Strategia.Strategy(gioco.Giocatori.First(), gioco.Mazziere, i));
            }
        }

    }

    public class TestCript
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestTripleDes()
        {
            byte[] EncryptedData = null;
            string b64 = "";
            var Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            var IV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            string stringa = "test string";
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                des.Key = Key;
                des.IV = IV;
                byte[] input = Encoding.UTF8.GetBytes(stringa);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                    EncryptedData = ms.ToArray();
                    b64 = Convert.ToBase64String(EncryptedData);
                }
            }

            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                byte[] input = Convert.FromBase64String(b64);
                des.Key = Key;
                des.IV = IV;
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                    byte[] output = ms.ToArray();
                    Assert.AreEqual(stringa, Encoding.UTF8.GetString(output));
                }
            }

        }

        [Test]
        public void TestAes()
        {
            byte[] EncryptedData = null;
            string b64 = "";
            var Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            var IV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            string stringa = "test string";
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = Key;
                aes.IV = IV;
                byte[] input = Encoding.UTF8.GetBytes(stringa);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                    EncryptedData = ms.ToArray();
                    b64 = Convert.ToBase64String(EncryptedData);
                }
            }

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                byte[] input = Convert.FromBase64String(b64);
                aes.Key = Key;
                aes.IV = IV;
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                    cs.FlushFinalBlock();
                    byte[] output = ms.ToArray();
                    Assert.AreEqual(stringa, Encoding.UTF8.GetString(output));
                }
            }

        }

    }
}

