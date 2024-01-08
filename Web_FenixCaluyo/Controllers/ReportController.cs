using iTextSharp.text.pdf;
using iTextSharp.text;
using Library.Business;
using Library.Business.IBusiness;
using Library.Log.ILog;
using Library.Model;
using Library.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Rotativa.AspNetCore;
using System.Drawing;
using Library.Model.PageNumber;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;

namespace Web_FenixCaluyo.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly IReport _report;
        private readonly ILoggerAdapter<ReportController> _logger;
        private ListVenta _datosVenta;

        public ReportController(IReport report, ILoggerAdapter<ReportController> logger, ListVenta datosVenta)
        {
            _report = report;
            _logger = logger;
            _datosVenta = datosVenta ?? new ListVenta(); // Asegúrate de inicializar la lista si datosVenta es nulo
        }

        private async Task<List<Venta>> ObtenerDatosVentaAsync()
        {
            if (_datosVenta.ListReport == null)
            {
                var ventas = await _report.GetVenta();
                _datosVenta.ListReport = JArray.FromObject(ventas.Data).ToObject<List<Venta>>();
            }
            return _datosVenta.ListReport;
        }

        [HttpGet("ImprimirVenta")]
        public async Task<IActionResult> ImprimirVenta()
        {
            var reporte = new ListVenta { ListReport = await ObtenerDatosVentaAsync() };

            var customSwitches = new Dictionary<string, string>
                {
                    { "footer-right", "Pag [page] de [topage]" },
                    { "footer-font-size", "7" },
                };

            var customSwitchesString = string.Join(" ", customSwitches.Select(kv => $"--{kv.Key} \"{kv.Value}\""));

            return View("ImprimirVenta", reporte);
        }



        public async Task<IActionResult> GeneratePdf()
        {
            var reporte = new ListVenta { ListReport = await ObtenerDatosVentaAsync() };

            var dataTable = new DataTable();
            var nameHead = new List<string> { "ID", "NOMBRE", "PRECIO", "STOCK" };

            //listya para encabezado
            string method = "ReporteVenta";
            string head = $"Datos de ejemplo en tabla\nId: {reporte.ListReport.First().IdVenta}";

            foreach (var item in nameHead)
            {
                dataTable.Columns.Add(item);
            }

            var totalsByGroupId = new Dictionary<string, decimal>();  // Cambiado a string como clave

            foreach (var item in reporte.ListReport)
            {
                dataTable.Rows.Add(item.IdVenta, item.nombre, item.precio, item.stock);
            }

            
            using (var ms = new MemoryStream())
            {
                using (var doc = new Document(PageSize.LETTER.Rotate()))
                {
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        writer.PageEvent = new HeaderFooterPdfPageEvent();
                        doc.Open();
                        var colorWhite = ColorTranslator.FromHtml("#fff");
                        var colorBlue = ColorTranslator.FromHtml("#000066");
                        var helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                        var negrita_white = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, new BaseColor(colorWhite));
                        var negrita = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, new BaseColor(Color.Black));
                        var center = Element.ALIGN_CENTER;
                        var right = Element.ALIGN_RIGHT;
                        doc.AddTitle(method);
                        doc.Add(new Phrase(head, negrita));
                        var table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f };
                        foreach (var item in nameHead)
                        {
                            table.AddCell(new PdfPCell(new Phrase(item, negrita_white)) { BorderColor = new BaseColor(colorWhite), BorderWidth = 1f, BackgroundColor = new BaseColor(colorBlue), Padding = 4f, HorizontalAlignment = center });
                        }
                        decimal totalPrecio = 0;
                        decimal totalStock = 0; 
                        string currentGroupId = null;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (currentGroupId == null || currentGroupId != row.Field<string>("ID"))
                            {
                                if (currentGroupId != null)
                                {
                                    PdfPCell totalGroupCell = new PdfPCell(new Phrase("Totales:", negrita_white))
                                    {
                                        BorderColor = new BaseColor(colorWhite),
                                        BorderWidth = 1f,
                                        Colspan = 2,
                                        BackgroundColor = new BaseColor(colorBlue),
                                        Padding = 4f,
                                        HorizontalAlignment = Element.ALIGN_CENTER
                                    };
                                    table.AddCell(totalGroupCell);

                                    PdfPCell totalGroupValueCell = new PdfPCell(new Phrase(totalStock.ToString("0.00"), negrita_white))
                                    {
                                        BorderColor = new BaseColor(colorWhite),
                                        BorderWidth = 1f,
                                        BackgroundColor = new BaseColor(colorBlue),
                                        Padding = 4f,
                                        HorizontalAlignment = Element.ALIGN_RIGHT
                                    };
                                    table.AddCell(totalGroupValueCell);

                                    PdfPCell totalGroupStockValueCell = new PdfPCell(new Phrase(totalPrecio.ToString("0.00"), negrita_white))
                                    {
                                        BorderColor = new BaseColor(colorWhite),
                                        BorderWidth = 1f,
                                        BackgroundColor = new BaseColor(colorBlue),
                                        Padding = 4f,
                                        HorizontalAlignment = Element.ALIGN_RIGHT
                                    };
                                    table.AddCell(totalGroupStockValueCell);
                                    totalPrecio = 0;
                                    totalStock = 0;
                                }
                                currentGroupId = row.Field<string>("ID");
                            }
                            if (dataTable.Columns.Count >= 3)
                            {
                                decimal valorPrecio;
                                if (decimal.TryParse(row[dataTable.Columns.Count - 2].ToString(), out valorPrecio))
                                {
                                    totalPrecio += valorPrecio;
                                }
                            }
                            if (dataTable.Columns.Count >= 4)
                            {
                                int valorStock;
                                if (int.TryParse(row[dataTable.Columns.Count - 1].ToString(), out valorStock))
                                {
                                    totalStock += valorStock;
                                }
                            }
                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                PdfPCell cell;
                                if (i == dataTable.Columns.Count - 2 || i == dataTable.Columns.Count - 1)
                                {
                                    cell = new PdfPCell(new Phrase(row[i].ToString()))
                                    {
                                        BorderColor = new BaseColor(colorWhite),
                                        BorderWidth = 1f,
                                        HorizontalAlignment = Element.ALIGN_RIGHT
                                    }; 
                                }
                                else
                                {
                                    cell = new PdfPCell(new Phrase(row[i].ToString()))
                                    {
                                        BorderColor = new BaseColor(colorWhite),
                                        BorderWidth = 1f
                                    };
                                }
                                table.AddCell(cell);
                            }
                        }
                        if (currentGroupId != null)
                        {
                            PdfPCell totalGroupCell = new PdfPCell(new Phrase("Totales:", negrita_white))
                            {
                                BorderColor = new BaseColor(colorWhite),
                                BorderWidth = 1f,
                                Colspan = 2,
                                BackgroundColor = new BaseColor(colorBlue),
                                Padding = 4f,
                                HorizontalAlignment = Element.ALIGN_CENTER
                            };
                            table.AddCell(totalGroupCell);
                            PdfPCell totalGroupValueCellStock = new PdfPCell(new Phrase(totalStock.ToString("0.00"), negrita_white))
                            {
                                BorderColor = new BaseColor(colorWhite),
                                BorderWidth = 1f,
                                BackgroundColor = new BaseColor(colorBlue),
                                Padding = 4f,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(totalGroupValueCellStock);
                            PdfPCell totalGroupValueCellPrecio = new PdfPCell(new Phrase(totalPrecio.ToString("0.00"), negrita_white))
                            {
                                BorderColor = new BaseColor(colorWhite),
                                BorderWidth = 1f,
                                BackgroundColor = new BaseColor(colorBlue),
                                Padding = 4f,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            table.AddCell(totalGroupValueCellPrecio);
                        }
                        doc.Add(table);
                        doc.Close();
                    }
                    var clonedMs = new MemoryStream(ms.ToArray());
                    clonedMs.Seek(0, SeekOrigin.Begin);
                    return File(clonedMs, "application/pdf", $"{method}.pdf");
                }
            }
        }









        //public async Task<IActionResult> GeneratePdf()
        //{
        //    var reporte = new ListVenta { ListReport = await ObtenerDatosVentaAsync() };

        //    var dataTable = new DataTable();
        //    var nameHead = new List<string> { "ID", "NOMBRE", "PRECIO", "STOCK" };

        //    foreach (var item in nameHead)
        //    {
        //        dataTable.Columns.Add(item);
        //    }
        //    foreach (var item in reporte.ListReport)
        //    {
        //        dataTable.Rows.Add(item.IdVenta, item.nombre, item.precio, item.stock);
        //    }
        //    //listya para encabezado
        //    string method = "ReporteVenta";
        //    string head = $"Datos de ejemplo en tabla\nId: {reporte.ListReport.First().IdVenta}";

        //    double total = 0;

        //    using (var ms = new MemoryStream())
        //    {
        //        using (var doc = new Document(PageSize.LETTER.Rotate()))
        //        {
        //            using (var writer = PdfWriter.GetInstance(doc, ms))
        //            {
        //                writer.PageEvent = new HeaderFooterPdfPageEvent();

        //                doc.Open();
        //                var colorWhite = ColorTranslator.FromHtml("#fff");
        //                var colorBlue = ColorTranslator.FromHtml("#000066");
        //                var helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
        //                var negrita_white = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, new BaseColor(colorWhite));
        //                var negrita = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, new BaseColor(Color.Black));
        //                var center = Element.ALIGN_CENTER;
        //                var right = Element.ALIGN_RIGHT;
        //                doc.AddTitle(method);
        //                doc.Add(new Phrase(head, negrita));
        //                var table = new PdfPTable(new float[] { 25f, 25f, 25f, 25f }) { WidthPercentage = 100f }; // 3 columnas para Id, Nombre y Valor
        //                foreach (var item in nameHead)
        //                {
        //                    table.AddCell(new PdfPCell(new Phrase(item, negrita_white)) { BorderColor = new BaseColor(colorWhite), BorderWidth = 1f, BackgroundColor = new BaseColor(colorBlue), Padding = 4f, HorizontalAlignment = center });
        //                }
        //                foreach (DataRow row in dataTable.Rows)
        //                {
        //                    for (int i = 0; i < dataTable.Columns.Count; i++)
        //                    {
        //                        PdfPCell cell;
        //                        if (i == dataTable.Columns.Count - 2 || i == dataTable.Columns.Count - 1)
        //                        {
        //                            // Si es una de las últimas dos columnas, realiza la suma
        //                            double valor = double.Parse(row[i].ToString());
        //                            total += valor; // variable total debe ser declarada anteriormente

        //                            cell = new PdfPCell(new Phrase(valor.ToString()))
        //                            {
        //                                BorderColor = new BaseColor(colorWhite),
        //                                BorderWidth = 1f,
        //                                HorizontalAlignment = Element.ALIGN_RIGHT
        //                            };
        //                        }
        //                        else
        //                        {
        //                            cell = new PdfPCell(new Phrase(row[i].ToString()))
        //                            {
        //                                BorderColor = new BaseColor(colorWhite),
        //                                BorderWidth = 1f
        //                            };
        //                        }
        //                        table.AddCell(cell);
        //                    }
        //                }
        //                PdfPCell totalCell = new PdfPCell(new Phrase("Total:", negrita_white)){ BorderColor = new BaseColor(colorWhite), BorderWidth = 1f, Colspan = 2, BackgroundColor = new BaseColor(colorBlue), Padding = 4f, HorizontalAlignment = Element.ALIGN_CENTER };
        //                table.AddCell(totalCell);
        //                PdfPCell totalValueCell = new PdfPCell(new Phrase(total.ToString(), negrita_white)){ BorderColor = new BaseColor(colorWhite), BorderWidth = 1f, BackgroundColor = new BaseColor(colorBlue), Padding = 4f, HorizontalAlignment = Element.ALIGN_RIGHT };
        //                table.AddCell(totalValueCell);
        //                foreach (var item in nameHead)
        //                {
        //                    table.AddCell(new PdfPCell(new Phrase(item, negrita_white)){ BorderColor = new BaseColor(colorWhite), BorderWidth = 1f, BackgroundColor = new BaseColor(colorBlue), Padding = 4f, HorizontalAlignment = center });
        //                }
        //                doc.Add(table);
        //                doc.Close();
        //            }
        //            var clonedMs = new MemoryStream(ms.ToArray());
        //            clonedMs.Seek(0, SeekOrigin.Begin);
        //            return File(clonedMs, "application/pdf", $"{method}.pdf");
        //        }
        //    }
        //}
    }
}


//public async Task<IActionResult> Descargar()
//{
//    var reporte = new ListVenta { ListReport = await ObtenerDatosVentaAsync() };

//    var customSwitches = new Dictionary<string, string>
//        {
//            { "footer-right", "Pag [page] de [topage]" },
//            { "footer-font-size", "7" },
//        };

//    var customSwitchesString = string.Join(" ", customSwitches.Select(kv => $"--{kv.Key} \"{kv.Value}\""));

//    return new ViewAsPdf("ImprimirVenta", reporte)
//    {
//        FileName = "Venta.pdf",
//        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
//        PageSize = Rotativa.AspNetCore.Options.Size.Letter,
//        CustomSwitches = customSwitchesString
//    };
//}





//private async Task<List<Venta>> ObtenerDatosVenta() =>
//            _ventas ??= (await _report.GetVenta())?.Data as List<Venta> ?? new List<Venta>();

//[HttpGet("ReporteVenta")]
//public async Task<IActionResult> ReporteVenta() =>
//    Ok(await ObtenerDatosVenta());

//private IActionResult GenerarPDF(string viewName)
//{
//    var ventas = ObtenerDatosVenta().Result; // Sincronizamos la llamada

//    var reporte = new ListVenta { ListReport = ventas };

//    var customSwitches = new Dictionary<string, string>
//        {
//            { "footer-right", "Pag [page] de [topage]" },
//            { "footer-font-size", "7" },
//            { "margin-left", "30mm" }
//        };

//    var customSwitchesString = string.Join(" ", customSwitches.Select(kv => $"--{kv.Key} \"{kv.Value}\""));

//    return new ViewAsPdf(viewName, reporte)
//    {
//        FileName = "Venta.pdf",
//        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
//        PageSize = Rotativa.AspNetCore.Options.Size.Letter,
//        CustomSwitches = customSwitchesString
//    };
//}

//[HttpGet("ExportPDF")]
//public IActionResult ExportPDF() => GenerarPDF("ImprimirVenta");

//[HttpGet("ExportPDFDetallado")]
//public IActionResult ExportPDFDetallado() => GenerarPDF("ImprimirVentaDetallado");


//[HttpGet]
//public ActionResult<string> Get()
//{
//    return "                #########          ##########      #########\n	       #### #####        ###########      #### #####\n	      ####   ####      ######            ####   ####\n	     #### #####      ######             #### #####\n	    #########       #####              #########\n	   #### #####      ######             ####\n	  ####   ####      ######            ####\n	 #### #####        ###########      ####\n	#########          ##########      ####\n";
//}