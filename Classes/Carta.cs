using System;
using System.ComponentModel;
using System.IO;

namespace Classes
{
    [Serializable]
    public class Carta
    {
        private byte[] _Immagine { get; set; }
        private NumeroCarta _numero { get; set; }
        public int Valore { get; set; }
        public int Conteggio { get; set; }
        public string NumeroString { get; set; }
        public Carta(NumeroCarta numero, SemeCarta seme)
        {
            Numero = numero;
            Seme = seme;
            PathImage = $"carte/{((int)numero)}.png";
            if (numero >= NumeroCarta.Due && numero <= NumeroCarta.Sei)
                Conteggio = 1;
            else if (numero >= NumeroCarta.Dieci || numero == NumeroCarta.Asso)
                Conteggio = -1;
        }

        public NumeroCarta Numero
        {
            get
            {
                return _numero;
            }
            set
            {
                _numero = value;
                NumeroString = _numero.ToString();
                Valore = GetValore();
            }
        }

        public SemeCarta Seme
        {
            get
            {
                return _seme;
            }
            set
            {
                _seme = value;
                SemeString = _seme.ToString();
            }
        }
        private SemeCarta _seme { get; set; }
        public string SemeString { get; set; }
        public string PathImage { get; set; }
        public byte[] Immagine { get; set; }
        public string ImmagineBase64 { get; set; }

        public byte[] GetImmagine()
        {
            byte[] ret = null;
            try
            {
                string path = $"wwwroot\\carte\\{(int)Seme}-{(int)Numero}.png";
                ret = File.ReadAllBytes(path);
            }
            catch { }
            return ret;
        }
        public string GetBase64Immagine()
        {
            string ret = "";
            try
            {
                ret = Convert.ToBase64String(GetImmagine());
            }
            catch { }
            return ret;
        }


        public enum SemeCarta
        {
            Cuori = 1,
            Quadri = 2,
            Fiori = 3,
            Picche = 4
        }

        private int GetValore()
        {
            if ((int)Numero == 1)
                return 11;
            if ((int)Numero <= 10)
                return (int)Numero;
            else if ((int)Numero < 14)
                return 10;
            else
                return 11;
        }

        public enum NumeroCarta
        {
            Asso = 1,
            Due = 2,
            Tre = 3,
            Quattro = 4,
            Cinque = 5,
            Sei = 6,
            Sette = 7,
            Otto = 8,
            Nove = 9,
            Dieci = 10,
            Jack = 11,
            Donna = 12,
            Re = 13,
        }

        public override bool Equals(object obj)
        {
            bool ret = false;

            if (obj is Carta)
            {
                ret = ((Carta)obj).Seme == Seme && ((Carta)obj).Numero == Numero;
            }

            return ret;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Numero} di {Seme}";
        }

    }

}
