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

        public IActionResult TestStrategy()
        {

            return View();
        }
        public IActionResult Index()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(0).build();

            gioco.Mazziere.SoldiTotali = 100;

            Giocatore BasicStrategy = GiocatoreBuilder.Init().AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategy()).AggiungiSoldi(100).build();

            Giocatore StrategiaConteggio = GiocatoreBuilder.Init().AggiungiGioco(gioco)
                .AggiungiStrategia(new StrategiaConteggio()).AggiungiSoldi(100).build();

            Giocatore SempliceStrategiaGiocatore = GiocatoreBuilder.Init().AggiungiGioco(gioco)
                .AggiungiStrategia(new SempliceStrategiaGiocatore()).AggiungiSoldi(100).build();


            gioco.Giocatori.Add(BasicStrategy);
            gioco.Giocatori.Add(BasicStrategy);

            gioco.Giocatori.Add(StrategiaConteggio);
            gioco.Giocatori.Add(StrategiaConteggio);

            gioco.Giocatori.Add(SempliceStrategiaGiocatore);
            gioco.Giocatori.Add(SempliceStrategiaGiocatore);

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
            gioco.Giocatori.AddRange(gioco.GiocatoriSplit);
        }

        Gioco GetSessionGioco()
        {
            string json = HttpContext.Session.GetString("Giocatori");
            List<Giocatore> giocatori = JsonConvert.DeserializeObject<List<Giocatore>>(json);

            json = HttpContext.Session.GetString("Gioco");
            Gioco gioco = JsonConvert.DeserializeObject<Gioco>(json);
            gioco.Giocatori = giocatori;
            foreach (Giocatore g in gioco.Giocatori)
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


        [HttpPost]
        public JsonResult GetCarteTest()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(0).build();
            for (int i = 0; i < 20; i++)
            {
                gioco.Mazziere.Pesca();
            }
            return Json(new { gioco = JsonConvert.SerializeObject(gioco) });
        }

        [HttpPost]
        public JsonResult GetCarteTestStrategy()
        {
            Gioco gioco = GiocoBuilder.Init().AggiungiNumeroGiocatori(1).build();
            gioco.Mazziere.Pesca();
            gioco.Giocatori[0].Pesca();
            gioco.Giocatori[0].Pesca();

            gioco.Mazzo.Conteggio = (int) (new Random().Next() % 6);
            return Json(new { gioco = JsonConvert.SerializeObject(gioco) });
        }


    }
}
