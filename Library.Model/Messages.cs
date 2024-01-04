using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    public class Messages
    {
        public enum Exceptions
        {
            COMPLETED,
            EXCEPTION,
            SESSION_EXPIRED
        }

        public enum ReplyMessages
        {
            Welcome,
            ChangePassword,
            SqlFailed,
            SuccessfulCompanyRegistration,
            SuccessfulCompanyUpdate,
            SuccessfulCompanyDelete,
            SuccessfulParameterRegistration,
            SuccessfulParameterUpdate,
            SuccessfulParameterDelete,
            Success
        }

        public static string Exception(Exceptions exception)
        {
            switch (exception)
            {
                case Exceptions.COMPLETED:
                    return "";
                case Exceptions.EXCEPTION:
                    return "Ups, algo ha salido mal. Por favor, intenta de nuevo más tarde.";
                //case Exceptions.SESSION_EXPIRED:
                //    return "Sesión expirada";
                default:
                    return "Que hay de nuevo.";
            }
        }



        public static string ReplyMessage(Messages.ReplyMessages message)
        {
            switch (message)
            {
                case Messages.ReplyMessages.Welcome:
                    return "Bienvenido a la plataforma.";
                //case Messages.ReplyMessages.ChangePassword:
                //    return "Cambio de contraseña satisfactorio.";
                case Messages.ReplyMessages.SqlFailed:
                    return "Ocurrido un error al solicitar informacion: ";
                case Messages.ReplyMessages.SuccessfulCompanyRegistration:
                    return "Empresa registrada exitosamente.";
                case Messages.ReplyMessages.SuccessfulCompanyUpdate:
                    return "Datos de empresa se actualizaron exitosamente.";
                case Messages.ReplyMessages.SuccessfulCompanyDelete:
                    return "Empresa eliminada exitosamente.";
                case Messages.ReplyMessages.SuccessfulParameterRegistration:
                    return "Parametro registrado exitosamente.";
                case Messages.ReplyMessages.SuccessfulParameterUpdate:
                    return "Datos de parametro se actualizaron exitosamente.";
                case Messages.ReplyMessages.SuccessfulParameterDelete:
                    return "Parametro eliminada exitosamente.";
                case Messages.ReplyMessages.Success:
                    return "Ok.";
                default:
                    return "Que hay de nuevo.";
            }
        }
    }
}
