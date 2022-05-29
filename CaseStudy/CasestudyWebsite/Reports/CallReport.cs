using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Geom;

using HelpdeskViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasestudyWebsite.Reports
{
    public class CallReport
    {
        PdfFont helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);

        public async Task generateReport(string rootpath) {
            PageSize pg = PageSize.A4.Rotate();
            Image img = new Image(ImageDataFactory.Create(rootpath + "/img/helpdeskbg.png"))
                .ScaleAbsolute(200, 100)
                .SetFixedPosition(((pg.GetWidth() - 200) / 2), 470);

            PdfWriter writer = new PdfWriter(rootpath + "/pdfs/calllist.pdf",
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));

            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, pg); //PageSize(595, 842)

            document.Add(img);
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));

            document.Add(new Paragraph("Current Calls")
               .SetFont(helvetica)
               .SetFontSize(24)
               .SetBold()
               .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));

            Table table = new Table(6); //6 column table

        //title cells
            table.AddCell(addCell("Opened", "h", 24));
            table.AddCell(addCell("Lastname", "h", 12));
            table.AddCell(addCell("Tech", "h", 12));
            table.AddCell(addCell("Problem", "h", 20));
            table.AddCell(addCell("Status", "h", 0));
            table.AddCell(addCell("Closed", "h", 20));
        //spacer cells
            table.AddCell(addCell(" ", "d"));
            table.AddCell(addCell(" ", "d"));
            table.AddCell(addCell(" ", "d")); 
            table.AddCell(addCell(" ", "d"));
            table.AddCell(addCell(" ", "d"));
            table.AddCell(addCell(" ", "d"));

            CallViewModel call = new CallViewModel();
            List<CallViewModel> calls = await call.GetAll();

            foreach (CallViewModel c in calls) {
                //format the dates
                table.AddCell(addCell(c.DateOpened.ToShortDateString(), "d"));
                table.AddCell(addCell(c.EmployeeName, "d"));
                table.AddCell(addCell(c.TechName, "d"));
                table.AddCell(addCell(c.ProblemDescription, "d"));

                if (c.OpenStatus) {
                    table.AddCell(addCell("Open", "d")); //status cell
                    table.AddCell(addCell("-", "d")); //closed date cell
                }
                else {
                    table.AddCell(addCell("Closed", "d")); //status cell
                    table.AddCell(addCell(c.DateClosed.ToString() ?? null, "d"));
                }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));

            document.Add(new Paragraph("Call report written on - " + System.DateTime.Now)
                .SetFontSize(6)
                .SetTextAlignment(TextAlignment.CENTER));
            document.Close();
        }

        private Cell addCell(string data, string celltype, int padleft = 16)
        {
            Cell cell;

            if (celltype == "h")
            {
                cell = new Cell().Add(
                    new Paragraph(data)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetPaddingLeft(padleft)
                    .SetBold())
                    .SetBorder(Border.NO_BORDER);
            } else {
                cell = new Cell().Add(
                    new Paragraph(data)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetPaddingLeft(padleft))
                    .SetBorder(Border.NO_BORDER);
            }
            return cell;
        }
    }
}
