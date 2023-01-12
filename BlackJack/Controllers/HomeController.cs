﻿using BlackJack.Models;
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
        static bool Stop = false;
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
            ViewBag.IdGiocatore = HttpContext.Session.GetString("IdGiocatore");

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
            Gioco gioco = new Gioco(0, nome: "Partita " + (Partite.Count + 1));
           // gioco.Mazzo.Carte[2].Numero = gioco.Mazzo.Carte[0].Numero; //riga di test per lo split
            Giocatore giocatore = new Giocatore(gioco, nome: nome);
            gioco.Giocatori.Add(giocatore);

            gioco.Inizializza();
            Partite.Add(gioco);

            HttpContext.Session.SetString("IdGiocatore", giocatore.Id);

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
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.Stai();

            return Json(JsonGioco(gioco));
        }

        public JsonResult Pesca(string id, string idGiocatore, int puntata)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.Pesca();

            return Json(JsonGioco(gioco));
        }

        public JsonResult Raddoppia(string id, string idGiocatore)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.Raddoppia();

            return Json(JsonGioco(gioco));
        }

        public JsonResult Abbandona(string id, string idGiocatore)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            gioco.Giocatori.Remove(giocatore);

            return Json(JsonGioco(gioco));
        }

        public JsonResult Split(string id, string idGiocatore)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.Split();
            Giocatore gsplit = gioco.Giocatori.SkipWhile(q => q.Id != idGiocatore).Skip(1).FirstOrDefault();

            return Json(new { json = JsonGioco(gioco), idGiocatore = gsplit.Id });
        }


        public JsonResult Partecipa(string id, string nome)
        {
            string idGiocatore = HttpContext.Session.GetString("IdGiocatore");
            if (string.IsNullOrEmpty(idGiocatore))
            {
                idGiocatore = DateTime.Now.Ticks.ToString();
                HttpContext.Session.SetString("IdGiocatore", idGiocatore);
            }
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            Giocatore giocatore = new Giocatore(gioco, nome: nome);
            giocatore.Id = idGiocatore;
            gioco.Giocatori.Add(giocatore);

            return Json(new { json = JsonGioco(gioco), idGiocatore = idGiocatore });
        }

        public JsonResult NuovaMano(string id)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Inizializza();

            return Json(JsonGioco(gioco));
        }

        public JsonResult GetStop()
        {
            return Json(Stop);
        }

        public JsonResult SetStop(bool stop)
        {
            Stop = stop;
            return Json(true);
        }

        string JsonGioco(Gioco gioco) => JsonConvert.SerializeObject(gioco, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        string JsonPartite(List<Gioco> partite) => JsonConvert.SerializeObject(partite, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });


    }
}
