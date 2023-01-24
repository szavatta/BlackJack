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
    public class MasterController : Controller
    {
        public static List<Gioco> Partite = new List<Gioco>();
        public string JsonGioco(Gioco gioco) => JsonConvert.SerializeObject(gioco, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public string JsonPartite(List<Gioco> partite) => JsonConvert.SerializeObject(partite, new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }

}
