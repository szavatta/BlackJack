using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Classes
{
    class StrategiaConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StrategiaGiocatore);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            string strategia = jsonObject["Strategia"].ToString();
            if (strategia == "BasicStrategy") 
            {
                return (BasicStrategy)jsonObject.ToObject<BasicStrategy>(serializer);
            }
            else if (strategia == "StrategiaConteggio")
            {
                return (StrategiaPunteggio)jsonObject.ToObject<StrategiaPunteggio>(serializer);
            }
            else if (strategia == "SempliceStrategiaGiocatore")
            {
                return (SempliceStrategiaGiocatore)jsonObject.ToObject<SempliceStrategiaGiocatore>(serializer);
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            StrategiaGiocatore strategia = (StrategiaGiocatore)value;
            JObject jsonObject = new JObject();
            if (strategia is BasicStrategy)
            {
                jsonObject["Strategia"] = "BasicStrategy";
            }
            else if (strategia is StrategiaPunteggio)
            {
                jsonObject["Strategia"] = "StrategiaConteggio";
                jsonObject["Punteggio"] = ((StrategiaPunteggio)strategia).Punteggio;
            }
            else if (strategia is SempliceStrategiaGiocatore)
            {
                jsonObject["Strategia"] = "SempliceStrategiaGiocatore";
            }
            jsonObject["Conteggio"] = strategia.Conteggio;
            jsonObject["TrueCount"] = strategia.TrueCount;
            jsonObject.WriteTo(writer);
        }
    }
}
