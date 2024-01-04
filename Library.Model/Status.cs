using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    public class Status
    {
        public enum Error
        {

            [Description("No existe informacion en el request, Respuesta nula")]
            Code10 = 10,

            [Description("Respuesta nula.")]
            Code20 = 20,

            [Description("Request Fallido: request")]
            Code30 = 30,

            [Description("Limite de espera excedido.")]
            Code40 = 40,

            [Description("Request canceled")]
            Code50 = 50,

            [Description("Error de comunicacion.")]
            Code60 = 60,

            [Description("Error en la peticion a la api.")]
            Code70 = 70,

            [Description("Error general")]
            Code80 = 80,
        }

        public enum Success
        {
            [Description("Ok.")]
            Code00 = 0,
            [Description("Reporte Exitoso")]
            Code01 = 1,
            [Description("No existen registros.")]
            Code02 = 2
        }

        public static string GetDescription(Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[]? attributes = fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attributes?.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}
