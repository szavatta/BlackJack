using Classes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJack.Controllers
{
    public class OnLineController : MasterController
    {
        public IActionResult Index()
        {
            Gioco gioco = GiocoBuilder.Init()
                .AggiungiNome("On line")
                .AggiungiMazzi(6)
                .build();

            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy())
                .build());

            Partite.Add(gioco);

            return View();
        }

        public JsonResult PescaCarta(int valore, int soggetto)
        {
            Gioco gioco = Partite.Where(q => q.Nome == "On line").FirstOrDefault();
            Giocatore giocatore = gioco.Giocatori.First();
            gioco.Mazzo.Carte[0] = new Carta((Carta.NumeroCarta)valore, Carta.SemeCarta.Cuori);
            if (soggetto == 0)
                gioco.Mazziere.Chiama();
            else if (soggetto == 1)
                giocatore.Chiama();
            
            return Json(new { numcarte = gioco.Mazzo.Carte.Count, truecount = giocatore.Strategia.GetTrueCount(gioco.Mazzo.Carte.Count) });
        }

        public JsonResult NuovaMano()
        {
            Gioco gioco = Partite.Where(q => q.Nome == "On line").FirstOrDefault();

            return Json(JsonGioco(gioco));
        }

        public JsonResult GetOperazione()
        {
            Gioco gioco = Partite.Where(q => q.Nome == "On line").FirstOrDefault();
            Giocatore giocatore = gioco.Giocatori.First();

            Giocatore.Puntata? puntata = GiocatoreSemplice.Puntata.Chiama;
            if (giocatore.Carte.Count > 0 && gioco.Mazziere.Carte.Count > 0)
                puntata = giocatore.Strategia.Strategy(giocatore, gioco.Mazziere, gioco.Mazzo.Carte.Count);

            return Json(new { 
                operazione = gioco.Mazziere.Carte.Count + giocatore.Carte.Count >= 3 ? Enum.GetName(typeof(Giocatore.Puntata), puntata) : "Seleziona carta",
                puntimiei = giocatore.Punteggio,
                puntimazziere = gioco.Mazziere.Punteggio,
                numcarte = gioco.Mazzo.Carte.Count
            });
        }

        //public JsonResult GetOperazione(List<int> cartaMazziere, List<int> carteMie, int conteggio)
        //{
        //    Giocatore giocatore = GiocatoreBuilder.Init().AggiungiGioco(null).AggiungiStrategia(new BasicStrategy()).build();
        //    foreach(int numcarta in carteMie)
        //    {
        //        giocatore.Carte.Add(new Carta((Carta.NumeroCarta)numcarta, Carta.SemeCarta.Cuori));
        //    }
        //    Mazziere mazziere = new Mazziere(null);
        //    mazziere.Carte.Add(new Carta((Carta.NumeroCarta)cartaMazziere.FirstOrDefault(), Carta.SemeCarta.Picche));

        //    Giocatore.Puntata operazione = giocatore.Strategia.Strategy(giocatore, mazziere, conteggio);

        //    return Json(operazione.ToString());
        //}

        public JsonResult GetPuntata(List<int> cartaMazziere, List<int> carteMie, int conteggio)
        {
            Giocatore giocatore = GiocatoreBuilder.Init().AggiungiGioco(null).AggiungiStrategia(new BasicStrategy()).build();
            foreach (int numcarta in carteMie)
            {
                giocatore.Carte.Add(new Carta((Carta.NumeroCarta)numcarta, Carta.SemeCarta.Cuori));
            }
            Mazziere mazziere = new Mazziere(null);
            mazziere.Carte.Add(new Carta((Carta.NumeroCarta)cartaMazziere.FirstOrDefault(), Carta.SemeCarta.Picche));

            double puntata = giocatore.Strategia.Puntata(giocatore, 1, 1, conteggio);

            return Json(puntata);
        }


    }
}
