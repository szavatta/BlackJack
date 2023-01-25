using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Classes
{
    public class Utils
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

        public static DataTable GetDataTable(IEnumerable ien)
        {
            DataTable dt = new DataTable();
            foreach (object obj in ien)
            {
                Type t = obj.GetType();
                PropertyInfo[] pis = t.GetProperties();
                if (dt.Columns.Count == 0)
                {
                    if (t.IsClass)
                    {
                        foreach (PropertyInfo pi in pis)
                        {
                            Type pt = pi.PropertyType;
                            if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                                pt = Nullable.GetUnderlyingType(pt);
                            //if (pt.IsValueType)
                            //{
                            dt.Columns.Add(pi.Name, pt);
                            //}
                        }
                    }
                    else
                    {
                        dt.Columns.Add(t.Name, t);
                    }
                }
                DataRow dr = dt.NewRow();
                if (t.IsClass)
                {
                    foreach (PropertyInfo pi in pis)
                    {
                        Type pt = pi.PropertyType;
                        if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                            pt = Nullable.GetUnderlyingType(pt);
                        //if (pt.IsValueType)
                        //{
                        object value = pi.GetValue(obj, null);
                        dr[pi.Name] = value == null ? DBNull.Value : value;
                        //}
                    }
                }
                else
                {
                    dr[t.Name] = obj == null ? DBNull.Value : obj;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

    }
}
