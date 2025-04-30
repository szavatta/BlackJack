using BlackJack.Models;
using Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        public IActionResult TestStrategy()
        {
            return View();
        }

        public IActionResult Partita(string id)
        {
            Gioco gioco = Partite.Where(q => q.Id == id).FirstOrDefault();
            //ViewBag.IdGiocatore = HttpContext.Session.GetString("IdGiocatore");
            ViewBag.IdGiocatore = HttpContext.Request.Cookies["idGiocatore"];
            ViewBag.Strategie = new SelectList(GetStrategie());

            return View(gioco);
        }

        public JsonResult GetPartite()
        {
            return Json(JsonPartite(Partite));
        }

        public JsonResult GetPartita(string id, bool mazzo = true)
        {
            Gioco g = Partite.Where(q => q.Id == id).FirstOrDefault();
            Gioco gioco = null;
            if (g != null)
                gioco = (Gioco)g.Clone();
            if (!mazzo) 
            {
                gioco.Mazzo.Carte = null;
                gioco.Mazzo.Scarti = null;
            }

            return Json(JsonGioco(gioco));
        }

        public JsonResult NuovaPartita(string nome, bool secondaCartaMazziere = true, int puntataMinima = 10, int puntataMassima = 1000, int numMazzi = 6, int percMischiata = 20, bool arresaDisponibile = false, bool visualizzaSceltaStrategia = true)
        {
            if (string.IsNullOrEmpty(nome))
            {
                nome = "Partita " + (Partite.Count + 1);
                int i = 1;
                while (Partite.Count(q => q.Nome == nome) > 0)
                {
                    i++;
                    nome = "Partita " + (Partite.Count + i);
                }
            }

            Gioco gioco = Gioco.GiocoBuilder.Init()
                .AggiungiNumeroGiocatori(0)
                .AggiungiNome(nome)
                .AggiungiMazzi(numMazzi)
                .AggiungiPuntataMinima(puntataMinima)
                .AggiungiPuntataMassima(puntataMassima)
                .AggiungiSecondaCartaInizialeMazziere(secondaCartaMazziere)
                .AggiungiMischiata()
                .AggiungiPercentualeMischiata(percMischiata)
                .AggiungiArresaDisponibile(arresaDisponibile)
                .AggiungiVisualizzaSceltaStrategia(visualizzaSceltaStrategia)
                .build();

            //gioco.Mazzo.Carte.Clear();
            //gioco.Mazzo.Carte[1].Numero = Carta.NumeroCarta.Asso; //righe di test per l'assicurazione
            //gioco.Mazzo.Carte[1].PathImage = $"Carte/{((int)Carta.SemeCarta.Quadri)}-{((int)Carta.NumeroCarta.Asso)}.png";
            //gioco.Mazzo.Carte[3].Numero = Carta.NumeroCarta.Quattro;
            //gioco.Mazzo.Carte[3].PathImage = $"Carte/{((int)Carta.SemeCarta.Quadri)}-{((int)Carta.NumeroCarta.Quattro)}.png";

            //gioco.Mazzo.Carte[2].Numero = gioco.Mazzo.Carte[0].Numero; //righe di test per lo split
            //gioco.Mazzo.Carte[2].PathImage = gioco.Mazzo.Carte[0].PathImage;

            //gioco.Mazzo.Carte[1].Numero = Carta.NumeroCarta.Asso; //righe di test per il black jack del mazziere
            //gioco.Mazzo.Carte[1].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[1].Seme)}-{((int)gioco.Mazzo.Carte[1].Numero)}.png";
            //gioco.Mazzo.Carte[3].Numero = Carta.NumeroCarta.Jack;
            //gioco.Mazzo.Carte[3].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[3].Seme)}-{((int)gioco.Mazzo.Carte[3].Numero)}.png";

            //gioco.Mazzo.Carte[0].Numero = Carta.NumeroCarta.Asso; //righe di test per il black jack del giocatore split
            //gioco.Mazzo.Carte[0].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[0].Seme)}-{((int)gioco.Mazzo.Carte[0].Numero)}.png";
            //gioco.Mazzo.Carte[1].Numero = Carta.NumeroCarta.Otto;
            //gioco.Mazzo.Carte[1].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[1].Seme)}-{((int)gioco.Mazzo.Carte[1].Numero)}.png";
            //gioco.Mazzo.Carte[2].Numero = Carta.NumeroCarta.Asso;
            //gioco.Mazzo.Carte[2].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[2].Seme)}-{((int)gioco.Mazzo.Carte[2].Numero)}.png";
            //gioco.Mazzo.Carte[3].Numero = Carta.NumeroCarta.Otto;
            //gioco.Mazzo.Carte[3].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[3].Seme)}-{((int)gioco.Mazzo.Carte[3].Numero)}.png";
            //gioco.Mazzo.Carte[4].Numero = Carta.NumeroCarta.Due;
            //gioco.Mazzo.Carte[4].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[4].Seme)}-{((int)gioco.Mazzo.Carte[4].Numero)}.png";
            //gioco.Mazzo.Carte[5].Numero = Carta.NumeroCarta.Tre;
            //gioco.Mazzo.Carte[5].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[5].Seme)}-{((int)gioco.Mazzo.Carte[5].Numero)}.png";
            //gioco.Mazzo.Carte[6].Numero = Carta.NumeroCarta.Cinque;
            //gioco.Mazzo.Carte[6].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[6].Seme)}-{((int)gioco.Mazzo.Carte[6].Numero)}.png";

            //gioco.Mazzo.Carte[0].Numero = Carta.NumeroCarta.Tre; //righe di test per il black jack del mazziere + assicurazione
            //gioco.Mazzo.Carte[0].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[0].Seme)}-{((int)gioco.Mazzo.Carte[0].Numero)}.png";
            //gioco.Mazzo.Carte[1].Numero = Carta.NumeroCarta.Asso;
            //gioco.Mazzo.Carte[1].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[1].Seme)}-{((int)gioco.Mazzo.Carte[1].Numero)}.png";
            //gioco.Mazzo.Carte[2].Numero = Carta.NumeroCarta.Tre;
            //gioco.Mazzo.Carte[2].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[2].Seme)}-{((int)gioco.Mazzo.Carte[2].Numero)}.png";
            //gioco.Mazzo.Carte[3].Numero = Carta.NumeroCarta.Dieci;
            //gioco.Mazzo.Carte[3].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[3].Seme)}-{((int)gioco.Mazzo.Carte[3].Numero)}.png";

            //gioco.Mazzo.Carte[0].Numero = Carta.NumeroCarta.Dieci; //righe di test per l'arresa
            //gioco.Mazzo.Carte[0].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[0].Seme)}-{((int)gioco.Mazzo.Carte[0].Numero)}.png";
            //gioco.Mazzo.Carte[1].Numero = Carta.NumeroCarta.Dieci;
            //gioco.Mazzo.Carte[1].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[1].Seme)}-{((int)gioco.Mazzo.Carte[1].Numero)}.png";
            //gioco.Mazzo.Carte[2].Numero = Carta.NumeroCarta.Sei;
            //gioco.Mazzo.Carte[2].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[2].Seme)}-{((int)gioco.Mazzo.Carte[2].Numero)}.png";
            //gioco.Mazzo.Carte[3].Numero = Carta.NumeroCarta.Dieci;
            //gioco.Mazzo.Carte[3].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[3].Seme)}-{((int)gioco.Mazzo.Carte[3].Numero)}.png";

            //gioco.Mazzo.Carte[0].Numero = Carta.NumeroCarta.Tre; //righe di test per il black jack del mazziere senza assicurazione
            //gioco.Mazzo.Carte[0].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[0].Seme)}-{((int)gioco.Mazzo.Carte[0].Numero)}.png";
            //gioco.Mazzo.Carte[1].Numero = Carta.NumeroCarta.Dieci;
            //gioco.Mazzo.Carte[1].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[1].Seme)}-{((int)gioco.Mazzo.Carte[1].Numero)}.png";
            //gioco.Mazzo.Carte[2].Numero = Carta.NumeroCarta.Tre;
            //gioco.Mazzo.Carte[2].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[2].Seme)}-{((int)gioco.Mazzo.Carte[2].Numero)}.png";
            //gioco.Mazzo.Carte[3].Numero = Carta.NumeroCarta.Asso;
            //gioco.Mazzo.Carte[3].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[3].Seme)}-{((int)gioco.Mazzo.Carte[3].Numero)}.png";

            //gioco.Mazzo.Carte[0].Numero = Carta.NumeroCarta.Asso; //righe di test per il black jack del giocatore
            //gioco.Mazzo.Carte[0].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[0].Seme)}-{((int)gioco.Mazzo.Carte[0].Numero)}.png";
            //gioco.Mazzo.Carte[2].Numero = Carta.NumeroCarta.Jack;
            //gioco.Mazzo.Carte[2].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[2].Seme)}-{((int)gioco.Mazzo.Carte[2].Numero)}.png";

            //gioco.Mazzo.Carte[1].Numero = Carta.NumeroCarta.Asso; //righe di test per il black jack del giocatore 2
            //gioco.Mazzo.Carte[1].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[1].Seme)}-{((int)gioco.Mazzo.Carte[1].Numero)}.png";
            //gioco.Mazzo.Carte[4].Numero = Carta.NumeroCarta.Jack;
            //gioco.Mazzo.Carte[4].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[4].Seme)}-{((int)gioco.Mazzo.Carte[4].Numero)}.png";

            //gioco.Mazzo.Carte[0].Numero = Carta.NumeroCarta.Dieci; //righe di test per il 21 del giocatore
            //gioco.Mazzo.Carte[0].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[0].Seme)}-{((int)gioco.Mazzo.Carte[0].Numero)}.png";
            //gioco.Mazzo.Carte[2].Numero = Carta.NumeroCarta.Due;
            //gioco.Mazzo.Carte[2].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[2].Seme)}-{((int)gioco.Mazzo.Carte[2].Numero)}.png";
            //gioco.Mazzo.Carte[4].Numero = Carta.NumeroCarta.Nove;
            //gioco.Mazzo.Carte[4].PathImage = $"Carte/{((int)gioco.Mazzo.Carte[4].Seme)}-{((int)gioco.Mazzo.Carte[4].Numero)}.png";

            gioco.Inizializza();
            Partite.Add(gioco);

            return Json(gioco.Id);
        }

        public JsonResult Punta(string id, string idGiocatore, decimal puntata)
        {
            try
            {
                Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
                Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
                decimal pok = giocatore.Strategia.Puntata(giocatore, gioco.PuntataMinima, gioco.PuntataMinima, giocatore.Strategia.TrueCount);
                decimal? scelta = pok != puntata ? pok : null;
                giocatore.Punta(puntata);

                if (gioco.Giocatori.Where(q => q.PuntataCorrente > 0).Count() == gioco.Giocatori.Count())
                {
                    gioco.DistribuisciCarteIniziali();
                    if (gioco.Mazziere.HasBlackJack() && gioco.Mazziere.Carte[0].Numero != Carta.NumeroCarta.Asso)
                        gioco.Giocatori.Where(q => q.PuntataCorrente > 0).ToList().ForEach(q => q.Stai());
                    else if (gioco.Giocatori[0].Punteggio == 21 && gioco.Mazziere.Carte[0].Numero != Carta.NumeroCarta.Asso)
                        gioco.Giocatori[0].Stai();
                }

                return Json(new { gioco = JsonGioco(gioco), puntata = pok });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message });
            }

        }

        public JsonResult Assicurazione(string id, string idGiocatore, int scelta)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.SceltaAssicurazione = true;
            if (scelta == 1)
                giocatore.Assicurazione();

            if (!gioco.Mazziere.HasBlackJack() && scelta == 1)
            {
                giocatore.SoldiTotali -= giocatore.PuntataAssicurazione;
                gioco.Mazziere.SoldiTotali += giocatore.PuntataAssicurazione;
                giocatore.PuntataAssicurazione = 0;
            }
            if (gioco.Giocatori.Count(q => q.SceltaAssicurazione == false) == 0 
                &&  gioco.Mazziere.HasBlackJack())
                gioco.Giocatori.Where(q => q.PuntataCorrente > 0).ToList().ForEach(q => q.Stai());
            else if (giocatore.Punteggio >= 21)
                giocatore.Stai();

            giocatore.ProssimaScelta = giocatore.ProxScelta();

            return Json(new { gioco = JsonGioco(gioco) });
        }

        public JsonResult Stai(string id, string idGiocatore)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            string scelta = giocatore.Scelta().ToString();
            giocatore.Stai();

            return Json(new { gioco = JsonGioco(gioco), scelta = scelta });
        }

        public JsonResult Chiama(string id, string idGiocatore, int puntata, bool forza = false)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            string scelta = giocatore.Scelta().ToString();
            if (giocatore.CalcolaPunteggio(true) >= 17 && !forza)
            {
                scelta = "?";
            }
            else
            {
                if (giocatore.Punteggio < 21)
                    giocatore.Chiama();

                if (giocatore.Punteggio >= 21)
                    giocatore.Stai();
            }

            return Json(new { gioco = JsonGioco(gioco), scelta = scelta });
        }

        public JsonResult Raddoppia(string id, string idGiocatore, bool forza = false)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            string scelta = giocatore.Scelta().ToString();
            if (giocatore.Punteggio >= 17 && !forza)
            {
                scelta = "?";
            }
            else
            {
                giocatore.Raddoppia();
            }

            return Json(new { gioco = JsonGioco(gioco), scelta = scelta });
        }

        public JsonResult Arresa(string id, string idGiocatore)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.Arresa();

            return Json(new { gioco = JsonGioco(gioco) });
        }
        public JsonResult Esci(string id, string idGiocatore)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            giocatore.Esci();

            if (gioco.Giocatori.Count == 0)
                Partite.Remove(gioco);
            else if (gioco.Giocatori.Where(q => q.PuntataCorrente > 0).Count() == gioco.Giocatori.Count())
                gioco.DistribuisciCarteIniziali();

            return Json(true);
        }

        public JsonResult Split(string id, string idGiocatore)
        {
            Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
            gioco.Iniziato = true;
            Giocatore giocatore = gioco.Giocatori.FirstOrDefault(q => q.Id == idGiocatore);
            string scelta = giocatore.Scelta().ToString();
            giocatore.Split();
            Giocatore gsplit = gioco.Giocatori.SkipWhile(q => q.Id != idGiocatore).Skip(1).FirstOrDefault();

            return Json(new { gioco = JsonGioco(gioco), idGiocatore = gsplit.Id, scelta = scelta });
        }


        public JsonResult Partecipa(string id, string nome, string strategia)
        {
            try
            {
                if (strategia == null)
                    throw new Exception("Seleziona una strategia");

                string idGiocatore = HttpContext.Request.Cookies["idGiocatore"];
                //idGiocatore = HttpContext.Session.GetString("IdGiocatore");
                if (string.IsNullOrEmpty(idGiocatore))
                {
                    idGiocatore = DateTime.Now.Ticks.ToString();
                    //HttpContext.Session.SetString("IdGiocatore", idGiocatore);
                    HttpContext.Response.Cookies.Append("IdGiocatore", idGiocatore, new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(5)
                    });
                }

                Assembly assembly = Assembly.Load("Classes");
                var tipoClasse = assembly.GetType("Classes." + strategia);
                if (tipoClasse == null)
                    throw new Exception($"strategia {strategia} non trovata.");

                StrategiaGiocatore istanzaStrategia = (StrategiaGiocatore)Activator.CreateInstance(tipoClasse);

                Gioco gioco = Partite.FirstOrDefault(q => q.Id == id);
                Giocatore giocatore = GiocatoreBuilder.Init()
                    .AggiungiStrategia(istanzaStrategia)
                    .AggiungiGioco(gioco)
                    .AggiungiNome(nome)
                    .AggiungiPuntataBase(gioco.PuntataMinima)
                    .build();
                giocatore.Id = idGiocatore;
                giocatore.ProssimaPuntata = giocatore.Strategia.Puntata(giocatore, gioco.PuntataMinima, giocatore.PuntataBase, giocatore.Strategia.TrueCount);
                gioco.Giocatori.Add(giocatore);

                return Json(new { json = JsonGioco(gioco), idGiocatore = idGiocatore });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message });
            }
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

        public JsonResult GetScelta(List<int> mazziere, List<int> giocatore, decimal trueCount)
        {
            Gioco gioco = Gioco.GiocoBuilder.Init().AggiungiMazzi(0).build();
            gioco.Giocatori.Add(GiocatoreBuilder.Init()
                .AggiungiGioco(gioco)
                .AggiungiStrategia(new BasicStrategyDeviation())
                .build());

            gioco.Giocatori.First().Strategia.TrueCount = trueCount;

            foreach (var carta in mazziere)
            {
                gioco.Mazziere
                    .AggiungiCarta(new Carta((Carta.NumeroCarta)carta, Carta.SemeCarta.Fiori));
            }

            foreach (var carta in giocatore)
            {
                gioco.Giocatori.First()
                    .AggiungiCarta(new Carta((Carta.NumeroCarta)carta, Carta.SemeCarta.Fiori));
            }

            string scelta = "";
            if (gioco.Giocatori.First().HaSballato())
                scelta = "Sballato";
            else if (giocatore.Count > 1 && mazziere.Count > 0)
                scelta = gioco.Giocatori.First().ProxScelta();

            return Json(scelta);
        }

        private List<string> GetStrategie()
        {
            // Ottieni l'assembly corrente (oppure specificane uno)
            Assembly assembly = typeof(StrategiaGiocatore).Assembly;

            // Tipo base
            Type tipoBase = typeof(StrategiaGiocatore);

            // Filtra tutte le classi che derivano da StrategiaGiocatore
            var strategie = assembly.GetTypes()
                .Where(t => 
                    t.IsClass && 
                    !t.IsAbstract && 
                    tipoBase.IsAssignableFrom(t) &&
                    !t.Name.StartsWith("Test"))
                .Select(q => q.Name)
                .ToList();

            return strategie;
        }

    }
}
