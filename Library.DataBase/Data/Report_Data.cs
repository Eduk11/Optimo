using Library.DataBase.Connection;
using Library.DataBase.Data.IData;
using Library.Log.ILog;
using Library.Model.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library.DataBase.Data
{
    public class Report_Data: IReport_Data
    {
        private readonly string _conexion;
        private readonly ILoggerAdapter<Report_Data> _logger;
        public Report_Data(IConfiguration _configuration, ILoggerAdapter<Report_Data> logger)
        {
            _conexion = Connection.Connection.ConnectionDB_PeportePagosServicios(_configuration);
            _logger = logger;
        }

        public async Task<List<Venta>> GetVentaReporte()
        {
            string Method = "GetVentaReporte";
            try
            {
                List<Venta>? getVenta = new List<Venta>();
                StoreProcedure sp = new StoreProcedure("dbo.reportVenta");
                DataTable data = await sp.ExecuteQueryAsync(_conexion);
                _logger.LogInformation($"Method: {Method}, Respuesta de la base de datos: {JsonConvert.SerializeObject(data)}");
                if (sp.Error.Length <= 0)
                {
                    getVenta = JArray.FromObject(data).ToObject<List<Venta>>();
                    _logger.LogInformation($"Method: {Method}, Pasa a objeto Sucursal para procesar: {JsonConvert.SerializeObject(getVenta)}");
                }
                else
                    _logger.LogError($"Method: {Method}, Error al realizar la consulta: {sp.Error}");
                return getVenta == null ? new List<Venta>() : getVenta;
            }
            catch (Exception ex)
            {
                _logger.LogCriticalE(ex, $"Method: {Method}, Exception: ");
                throw new ArgumentNullException(ex.Message);
            }
        }
    }
}
