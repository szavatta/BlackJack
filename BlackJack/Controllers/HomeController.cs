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
            //Gioco gioco = new Gioco(0,1);
            //gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy()));
            //gioco.Giocatori.Add(new Giocatore(gioco));
            //gioco.Giocata();
            //HttpContext.Session.SetObject("Gioco", gioco);

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
            Gioco gioco = new Gioco(0, 0);

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

            gioco.Mazziere.SoldiTotali = 100;
            gioco.Giocatori.Add(new Giocatore(gioco, new BasicStrategy(), soldi: 100));
            gioco.Giocatori.Add(new Giocatore(gioco, soldi: 100));
            gioco.Giocata();

            gioco.Mazziere.Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso11).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso);
            gioco.Giocatori.ForEach(q => q.Carte.Where(q => q.Numero == Carta.NumeroCarta.Asso11).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso));

            string json = JsonConvert.SerializeObject(gioco);

            return Json(json);
        }
    }
}
