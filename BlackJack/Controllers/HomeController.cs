using BlackJack.Models;
using Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlackJack.Controllers
{
    public class HomeController : MasterController
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Partita(string id)
        {
            Gioco gioco = Partite.Where(q => q.Id == id).FirstOrDefault();

            return View(gioco);
        }

        public JsonResult GetPartite()
        {
            return Json(JsonPartite(Partite));
        }

        public JsonResult GetPartita(string id)
        {
            Gioco gioco = Partite.Where(q => q.Id == id).FirstOrDefault();

            return Json(JsonGioco(gioco));
        }

        public JsonResult NuovaPartita(string nome)
        {
            Gioco gioco = new Gioco(1, nome: "Partita " + (Partite.Count + 1));
            gioco.Giocatori[0].Nome = nome;
            gioco.Inizializza();
            Partite.Add(gioco);

            return Json(gioco.Id);
        }

        public JsonResult Punta(string id, string idGiocatore, int puntata)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.PuntataCorrente = puntata;

            if (gioco.Giocatori.Where(q => q.PuntataCorrente > 0).Count() == gioco.Giocatori.Count())
                gioco.DistribuisciCarteIniziali();

            return Json(JsonGioco(gioco));
        }

        public JsonResult Stai(string id, string idGiocatore)
        {
            Gioco gioco = Stai(id, idGiocatore, true);

            return Json(JsonGioco(gioco));
        }

        private Gioco Stai(string id, string idGiocatore, bool ret)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            Giocatore next = gioco.Giocatori.SkipWhile(q => q.Id != idGiocatore).Skip(1).FirstOrDefault();
            if (next != null)
            {
                gioco.IdGiocatoreMano = next.Id;
            }
            else
            {
                gioco.IdGiocatoreMano = null;
                gioco.Mazziere.CartaCoperta = false;
                while (gioco.Mazziere.Strategia.Strategy(gioco.Mazziere) == Mazziere.Puntata.Chiama)
                {
                    gioco.Mazziere.Pesca();
                }
                gioco.TerminaMano();
            }

            return gioco;
        }

        public JsonResult Pesca(string id, string idGiocatore, int puntata)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.Pesca();
            if (giocatore.Punteggio > 21)
            {
                gioco = Stai(id, idGiocatore, true);
            }

            return Json(JsonGioco(gioco));
        }

        string JsonGioco(Gioco gioco) => JsonConvert.SerializeObject(gioco, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        string JsonPartite(List<Gioco> partite) => JsonConvert.SerializeObject(partite, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });


    }
}
