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
    public class TestController : MasterController
    {

        public IActionResult TestCount()
        {

            return View();
        }

        public IActionResult TestStrategia()
        {
            Gioco gioco = GiocoBuilder.Init()
                .AggiungiNome("Partita Test")
                .AggiungiNumeroGiocatori(0)
                .AggiungiMazzi(6)
                .build();

            //gioco.Mazzo.Carte[2].Numero = gioco.Mazzo.Carte[0].Numero; //riga di test per lo split
            //gioco.Mazzo.Carte[2].PathImage = gioco.Mazzo.Carte[0].PathImage;

            gioco.Id = "test";
            gioco.Inizializza();
            Partite.Add(gioco);

            Giocatore giocatore = GiocatoreBuilder.Init().AggiungiGioco(gioco).AggiungiStrategia(new BasicStrategy()).build();
            giocatore.Id = "test";
            HttpContext.Session.SetString("IdGiocatore", giocatore.Id);
            gioco.Giocatori.Add(giocatore);

            return RedirectToAction("Partita", "Home", new { id = gioco.Id });

            //return View();
        }
        public IActionResult TestStrategy()
        {

            return View();
        }
        public IActionResult Index()
        {
            Gioco gioco = GiocoBuilder.Init()
                .AggiungiNumeroGiocatori(0)
                .AggiungiMazzi(6)
                .AggiungiMischiata(true)
                .AggiungiMischiataRandom(9)
                .AggiungiPuntataMinima(5)
                .AggiungiPercentualeMischiata(50)
                .build();

            gioco.Mazziere.SoldiTotali = 100;

            Giocatore BasicStrategy = GiocatoreBuilder.Init().AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy()).AggiungiSoldi(100).build();

            Giocatore StrategiaConteggio = GiocatoreBuilder.Init().AggiungiGioco(gioco)
                .AggiungiStrategia(new StrategiaConteggio()).AggiungiSoldi(100).build();

            Giocatore SempliceStrategiaGiocatore = GiocatoreBuilder.Init().AggiungiGioco(gioco)
                .AggiungiStrategia(new SempliceStrategiaGiocatore()).AggiungiSoldi(100).build();

            gioco.Giocatori.Add(BasicStrategy);
            //gioco.Giocatori.Add(BasicStrategy);

            //gioco.Giocatori.Add(StrategiaConteggio);
            //gioco.Giocatori.Add(StrategiaConteggio);

            //gioco.Giocatori.Add(SempliceStrategiaGiocatore);
            //gioco.Giocatori.Add(SempliceStrategiaGiocatore);

            SetSessionGioco(gioco);

            return View();
        }

        [HttpPost]
        public JsonResult GetGioco(int? numGiri)
        {
            Gioco gioco = GetSessionGioco();
            int giri = numGiri ?? 1;
            for (int i = 0; i < giri; i++)
            {
                gioco.Giocata();
            }

            SetSessionGioco(gioco);

            return Json(new { gioco = JsonConvert.SerializeObject(gioco) });
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
            gioco.Mazziere.Gioco = gioco;
            gioco.Giocatori.ForEach(q => q.Gioco = gioco);

            return gioco;
        }


        [HttpPost]
        public JsonResult GetCarteTest()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(0).build();
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazziere.Chiama();
            }
            return Json(new { gioco = JsonConvert.SerializeObject(gioco) });
        }

        [HttpPost]
        public JsonResult GetCarteTestStrategy()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(1).build();
            gioco.Mazziere.Chiama();
            gioco.Giocatori[0].Chiama();
            gioco.Giocatori[0].Chiama();

            return Json(new { gioco = JsonConvert.SerializeObject(gioco) });
        }


    }
}
