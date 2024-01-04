using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataBase.Connection
{
    public class Parameter_Sp
    {
        private string? _name;
        private object? _value;
        public Parameter_Sp(string? parameterName, object? parameterValue)
        {
            this._name = parameterName;
            this._value = parameterValue;
        }

        public string ParameterName
        {
            get { return _name ?? String.Empty; }
            set { _name = value; }
        }

        public object ParameterValue
        {
            get { return _value ?? String.Empty; }
            set { this._value = value; }
        }
    }
}
