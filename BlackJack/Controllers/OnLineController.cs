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
            return View();
        }

        public JsonResult GetPunteggioCarta(int valore)
        {
            Giocatore giocatore = GiocatoreBuilder.Init().AggiungiGioco(null).AggiungiStrategia(new BasicStrategy()).build();
            int punteggio = giocatore.Strategia.Conta(new Carta((Carta.NumeroCarta)valore, Carta.SemeCarta.Cuori));

            return Json(punteggio);
        }

        public JsonResult TipoOperazione(List<int> carteMazziere, List<int> carteMie, int conteggio)
        {
            Giocatore giocatore = GiocatoreBuilder.Init().AggiungiGioco(null).AggiungiStrategia(new BasicStrategy()).build();
            carteMie.ForEach(q => giocatore.Carte.Add(new Carta((Carta.NumeroCarta)q, Carta.SemeCarta.Quadri)));
            Mazziere mazziere = new Mazziere(null);
            carteMazziere.ForEach(q => mazziere.Carte.Add(new Carta((Carta.NumeroCarta)q, Carta.SemeCarta.Picche)));

            var puntata = giocatore.Strategia.Strategy(giocatore, mazziere, conteggio);

            return Json(Enum.GetName(typeof(Giocatore.Puntata), puntata));
        }

        public JsonResult GetPuntata(List<int> cartaMazziere, List<int> carteMie, int conteggio)
        {
            Giocatore giocatore = GiocatoreBuilder.Init().AggiungiGioco(null).AggiungiStrategia(new BasicStrategy()).build();
            foreach(int numcarta in carteMie)
            {
                giocatore.Carte.Add(new Carta((Carta.NumeroCarta)numcarta, Carta.SemeCarta.Cuori));
            }
            Mazziere mazziere = new Mazziere(null);
            mazziere.Carte.Add(new Carta((Carta.NumeroCarta)cartaMazziere.FirstOrDefault(), Carta.SemeCarta.Picche));

            Giocatore.Puntata puntata = giocatore.Strategia.Strategy(giocatore, mazziere, conteggio);

            return Json(puntata.ToString());
        }



    }
}
