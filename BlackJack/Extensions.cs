//using Microsoft.AspNetCore.Http;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace BlackJack
//{
//    public static class SessionExtensions
//    {
//        public static void SetObject(this ISession session, string key, object value)
//        {
//            session.SetString(key, JsonConvert.SerializeObject(value));
//        }

//        public static T GetObject<T>(this ISession session, string key)
//        {
//            var value = session.GetString(key);

//            if (value != null)
//            {
//                var dyn = JsonConvert.DeserializeObject<dynamic>(value);
//                Classes.Gioco g = ((Newtonsoft.Json.Linq.JObject)dyn).ToObject<Classes.Gioco>();
//            }

//            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
//        }
//    }

//}
