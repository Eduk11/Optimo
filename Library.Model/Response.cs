using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    public class Response
    {
        public int State { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public object? Exceptions { get; set; }

        public static Response Success(object data)
        {
            var res = new Response()
            {
                State = 0,
                Message = Messages.Exception(Messages.Exceptions.COMPLETED),
                Data = data,
                Exceptions = null
            };
            return res;
        }

        public static Response SuccessGenerate(object data, Messages.ReplyMessages message)
        {
            var res = new Response()
            {
                State = 0,
                Message = Messages.ReplyMessage(message),
                Data = data,
                Exceptions = null
            };
            return res;
        }

        public static Response Exception(object data)
        {
            var res = new Response()
            {
                State = 1,
                Message = Messages.Exception(Messages.Exceptions.EXCEPTION),
                Data = null,
                Exceptions = data
            };
            return res;
        }

        public static Response ExceptionGenerate(object exception, Messages.ReplyMessages message)
        {
            var res = new Response()
            {
                State = 1,
                Message = Messages.ReplyMessage(message),
                Data = null,
                Exceptions = exception
            };
            return res;
        }

        public static Response Error(string message)
        {
            var res = new Response()
            {
                State = 2,
                Message = message,
                Data = null,
                Exceptions = null
            };
            return res;
        }
    }
}
