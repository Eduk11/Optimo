using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataBase.Connection
{
    public class StoreProcedure
    {
        private string _SpName;
        private readonly List<Parameter_Sp> _list = new List<Parameter_Sp>();
        private string _errorMessage = String.Empty;
        private readonly int commandTimeOut = 30;

        public StoreProcedure(string storedProcedureName)
        {
            this._SpName = storedProcedureName;
        }

        public void AddParameter(string parameterName, object? parameterValue)
        {
            _list.Add(new Parameter_Sp(parameterName, parameterValue));
        }

        ////public List<StoredPreoceduresParameter> Items
        ////{
        ////    get { return _list; }
        ////    set { _list = value; }
        ////}

        public string SpName
        {
            get { return _SpName ?? String.Empty; }
            set { _SpName = value; }
        }

        public string Error
        {
            get { return _errorMessage; }
        }

        /// <summary>
        /// Stored procedure async executor
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>true for saved data, false in otherwise.</returns>
        public async Task<bool> isExecuteQueryAsync(string connectionString)
        {
            var result = false;
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(_SpName, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = commandTimeOut;
            for (int cont = 0; cont < _list.Count; cont++)
            {
                if (_list[cont].ParameterValue == null)
                    command.Parameters.AddWithValue(_list[cont].ParameterName, DBNull.Value);
                else
                    command.Parameters.AddWithValue(_list[cont].ParameterName, _list[cont].ParameterValue);
            }
            try
            {
                await connection.OpenAsync();
                command.ExecuteNonQuery();
                _errorMessage = string.Empty;
                result = true;
            }
            catch (SqlException error)
            {
                _errorMessage = error.Message;
                result = false;
            }
            finally
            {
                connection.Close();
            }
            return result;
        }


        /// <summary>
        /// Stored procedure async executor
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>DataTable with returned data from the sql</returns>
        public DataTable ExecuteQuery(string connectionString)
        {
            DataTable query = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter command = new SqlDataAdapter(_SpName, connection);
            command.SelectCommand.CommandType = CommandType.StoredProcedure;
            command.SelectCommand.CommandTimeout = commandTimeOut;
            for (int cont = 0; cont < _list.Count; cont++)
            {
                if (_list[cont].ParameterValue == null)
                    command.SelectCommand.Parameters.AddWithValue(_list[cont].ParameterName, DBNull.Value);
                else
                    command.SelectCommand.Parameters.AddWithValue(_list[cont].ParameterName, _list[cont].ParameterValue);
            }
            try
            {
                connection.Open();
                command.Fill(query);
                _errorMessage = string.Empty;
            }
            catch (SqlException error)
            {
                _errorMessage = error.Message;
            }
            finally
            {
                connection.Close();
            }
            return query;
        }

        public async Task<DataTable> ExecuteQueryAsync(string connectionString)
        {
            DataTable query = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(_SpName, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = commandTimeOut;
            for (int cont = 0; cont < _list.Count; cont++)
            {
                if (_list[cont].ParameterValue == null)
                    command.Parameters.AddWithValue(_list[cont].ParameterName, DBNull.Value);
                else
                    command.Parameters.AddWithValue(_list[cont].ParameterName, _list[cont].ParameterValue);
            }
            try
            {
                await connection.OpenAsync();
                SqlDataReader? reader = null;
                try
                {
                    reader = await command.ExecuteReaderAsync();
                    query.Load(reader);
                }
                finally
                {
                    reader?.Close();
                }
                _errorMessage = string.Empty;
            }
            catch (SqlException error)
            {
                _errorMessage = error.Message;
            }
            finally
            {
                connection.Close();
            }
            return query;
        }
    }
}
