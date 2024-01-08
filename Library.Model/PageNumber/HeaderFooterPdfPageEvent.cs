using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Library.Model.PageNumber
{
    public class HeaderFooterPdfPageEvent : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            // Número de página actual
            int pageNumber = writer.PageNumber;

            // Número total de páginas
            int totalPages = writer.PageNumber;

            // Posiciona el cursor en la parte inferior central de la página
            float x = document.PageSize.GetRight(40);
            float y = document.PageSize.GetBottom(30);

            // Fuente y tamaño del texto
            var helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            var font = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Construye el texto "pag x de y"
            string text = $"pag {pageNumber} de {totalPages}";

            // Agrega el texto al documento
            ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, new Phrase(text, font), x, y, 0);
        }
    }
}
