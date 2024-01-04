using Library.Business.IBusiness;
using Library.DataBase.Data.IData;
using Library.Log.ILog;
using Library.Model;

namespace Library.Business
{
    public class Report: IReport
    {
        private readonly IReport_Data _reportVenta;
        private readonly ILoggerAdapter<Report> _logger;
        public Report(IReport_Data reportVenta, ILoggerAdapter<Report> logger)
        {
            _reportVenta = reportVenta;
            _logger = logger;
        }

        public async Task<Response> GetVenta()
        {
            string Method = "GetSucursal";
            try
            {
                var data = await _reportVenta.GetVentaReporte();
                if (data.Count <= 0)
                {
                    _logger.LogError($"Method: {Method}, Numero de registros: {data.Count}");
                    return Response.Success(Status.Success.Code02);
                }
                else
                {
                    _logger.LogInformation($"Method: {Method}, Respuesta Sucursal De Agencias: {data}");
                    return Response.Success(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCriticalE(ex, $"Method: {Method}, Exception: ");
                throw new ArgumentNullException(ex.Message);
            }
        }
    }
}
