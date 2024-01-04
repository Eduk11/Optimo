using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataBase.Connection
{
    public static class Connection
    {
        public static string ConnectionDB_PeportePagosServicios(IConfiguration? configuration)
        {
            try
            {
                string response = string.Empty;
                string? GET_SERV = configuration?.GetSection("ConnectionString:ServBD_REPORT").Value;
                string? SERV_DB_REPORT = GET_SERV != null ? GET_SERV.ToString() : string.Empty;
                string? GET_NAME = configuration?.GetSection("ConnectionString:NameBD_REPORT").Value;
                string? NAME_BD_REPORT = GET_NAME != null ? GET_NAME.ToString() : string.Empty;

                response = $"Data Source = {SERV_DB_REPORT}; Initial Catalog = {NAME_BD_REPORT}; Integrated Security = true; TrustServerCertificate=True;";

                return response;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
