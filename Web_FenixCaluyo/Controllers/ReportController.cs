using Library.Business.IBusiness;
using Library.Log.ILog;
using Library.Model;
using Library.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Rotativa.AspNetCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_FenixCaluyo.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly IReport _report;
        private readonly ILoggerAdapter<ReportController> _logger;
        private List<Venta> _ventas;

        public ReportController(IReport report, ILoggerAdapter<ReportController> logger)
        {
            _report = report;
            _logger = logger;
        }

        private async Task<List<Venta>> ObtenerDatosVenta() =>
            _ventas ??= (await _report.GetVenta())?.Data as List<Venta> ?? new List<Venta>();

        [HttpGet("ReporteVenta")]
        public async Task<IActionResult> ReporteVenta() =>
            Ok(await ObtenerDatosVenta());

        private IActionResult GenerarPDF(string viewName)
        {
            var ventas = ObtenerDatosVenta().Result; // Sincronizamos la llamada

            var reporte = new ListVenta { ListReport = ventas };

            var customSwitches = new Dictionary<string, string>
        {
            { "footer-right", "Pag [page] de [topage]" },
            { "footer-font-size", "7" },
            { "margin-left", "30mm" }
        };

            var customSwitchesString = string.Join(" ", customSwitches.Select(kv => $"--{kv.Key} \"{kv.Value}\""));

            return new ViewAsPdf(viewName, reporte)
            {
                FileName = "Venta.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                CustomSwitches = customSwitchesString
            };
        }

        [HttpGet("ExportPDF")]
        public IActionResult ExportPDF() => GenerarPDF("ImprimirVenta");

        [HttpGet("ExportPDFDetallado")]
        public IActionResult ExportPDFDetallado() => GenerarPDF("ImprimirVentaDetallado");
    }
}


//[HttpGet]
//public ActionResult<string> Get()
//{
//    return "                #########          ##########      #########\n	       #### #####        ###########      #### #####\n	      ####   ####      ######            ####   ####\n	     #### #####      ######             #### #####\n	    #########       #####              #########\n	   #### #####      ######             ####\n	  ####   ####      ######            ####\n	 #### #####        ###########      ####\n	#########          ##########      ####\n";
//}