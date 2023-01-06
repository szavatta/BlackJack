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
using System.Threading.Tasks;

namespace BlackJack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Gioco gioco = new Gioco(0, 0);

            gioco.Mazziere.SoldiTotali = 100;
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));

            string json = JsonConvert.SerializeObject(gioco);

            HttpContext.Session.SetString("Gioco", json);

            return View();
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

        public JsonResult GetGioco()
        {
            //Gioco gioco = HttpContext.Session.GetObject<Gioco>("Gioco");
            //string json = HttpContext.Session.GetString("Gioco");
            //Gioco gioco = (Gioco)JsonConvert.DeserializeObject(json);
            Gioco gioco = new Gioco(0, 0);

            gioco.Mazziere.SoldiTotali = 100;
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));
            gioco.Giocata();

            string json = JsonConvert.SerializeObject(gioco);

            //HttpContext.Session.Set("Gioco", json);

            return Json(json);
        }
    }
}
