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
            Gioco gioco = new Gioco(0);

            gioco.Mazziere.SoldiTotali = 100;
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, new StrategiaConteggio(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, new StrategiaConteggio(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));

            SetSessionGioco(gioco);

            return View();
        }

        public IActionResult TestCount()
        {

            return View();
        }

        void SetSessionGioco(Gioco gioco)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            var giocatorijson = JsonConvert.SerializeObject(gioco.Giocatori);
            HttpContext.Session.SetString("Giocatori", giocatorijson);
            var giocatori = gioco.Giocatori;

            gioco.Giocatori = null;
            var gi = JsonConvert.SerializeObject(gioco, settings);
            HttpContext.Session.SetString("Gioco", gi);
            gioco.Giocatori = giocatori;
        }

        Gioco GetSessionGioco()
        {
            string json = HttpContext.Session.GetString("Giocatori");
            List<Giocatore> giocatori = JsonConvert.DeserializeObject<List<Giocatore>>(json);

            json = HttpContext.Session.GetString("Gioco");
            Gioco gioco = JsonConvert.DeserializeObject<Gioco>(json);
            gioco.Giocatori = giocatori;
            foreach(Giocatore g in gioco.Giocatori)
            {
                g.Gioco = gioco;
                if (g.TipoStrategia == 0)
                    g.Strategia = new BasicStrategy();
                else if (g.TipoStrategia == 1)
                    g.Strategia = new StrategiaConteggio();
                else 
                    g.Strategia = new SempliceStrategiaGiocatore();
            }

            gioco.Mazziere.Gioco = gioco;


            return gioco;
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

        [HttpPost]
        public JsonResult GetGioco()
        {
            Gioco gioco = GetSessionGioco();

            gioco.Giocata();

            SetSessionGioco(gioco);

            return Json(new { gioco = JsonConvert.SerializeObject(gioco), trueCount = gioco.Mazzo.GetTrueCount() });
        }

        [HttpPost]
        public JsonResult GetCarteTest()
        {
            Gioco gioco = new Gioco(1);
            for (int i = 0; i < 20; i++)
            {
                gioco.Giocatori[0].Pesca();
            }

            return Json(new { gioco = JsonConvert.SerializeObject(gioco) });
        }


    }
}
