using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Classes
{
    internal class Utils
    {
        public static string GetDescriptionFromEnumValue(Enum value)
        {
            try
            {
                DescriptionAttribute attribute = value.GetType()
                    .GetField(value.ToString())
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .SingleOrDefault() as DescriptionAttribute;
                return attribute == null ? value.ToString() : attribute.Description;
            }
            catch
            {
                return string.Empty;
            }

        }
    }
}
