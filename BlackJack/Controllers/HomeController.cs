using BlackJack.Models;
using Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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
            string json = JsonConvert.SerializeObject(Partite, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(json);
        }

        public JsonResult GetPartita(string id)
        {
            Gioco gioco = Partite.Where(q => q.Id == id).FirstOrDefault();
            string json = JsonConvert.SerializeObject(gioco, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(json);
        }

        public JsonResult NuovaPartita(string nome)
        {
            Gioco gioco = new Gioco(1, nome: "Partita " + (Partite.Count + 1));
            gioco.Giocatori[0].Nome = nome;

            gioco.Giocatori.ForEach(q => q.Carte = new List<Carta>());
            gioco.Mazziere.Carte = new List<Carta>();
            gioco.Giocatori.ForEach(q => q.PuntataCorrente = q.Strategia.Puntata(gioco.PuntataMinima, 50, gioco.Mazzo.GetTrueCount()));

            foreach (Giocatore giocatore in gioco.Giocatori.Where(q => q.PuntataCorrente > 0))
            {
                giocatore.Pesca();
            }
            gioco.Mazziere.Pesca();
            foreach (Giocatore giocatore in gioco.Giocatori.Where(q => q.PuntataCorrente > 0))
            {
                giocatore.Pesca();
            }
            gioco.Mazziere.Pesca();

            Partite.Add(gioco);


            return Json(gioco.Id);
        }

    }
}
