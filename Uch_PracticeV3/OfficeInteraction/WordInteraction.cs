using Aspose.Words;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using GridColumn = DocumentFormat.OpenXml.Wordprocessing.GridColumn;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableGrid = DocumentFormat.OpenXml.Wordprocessing.TableGrid;
using TableProperties = DocumentFormat.OpenXml.Wordprocessing.TableProperties;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TableStyle = DocumentFormat.OpenXml.Wordprocessing.TableStyle;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace Uch_PracticeV3.OfficeInteraction
{
    public class WordInteraction
    {
        //	application/vnd.openxmlformats-officedocument.wordprocessingml.document
        public static MemoryStream ConvertToWord(EnterParams enterParams)
        {
            var stream = new MemoryStream();
            using (WordprocessingDocument doc = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                new Document(new Body()).Save(mainPart);

                Body body = mainPart.Document.Body;

                Paragraph p = new Paragraph();
                ParagraphProperties pp = new ParagraphProperties();
                pp.Justification = new Justification() { Val = JustificationValues.Center };
                p.Append(pp);

                Run run = new Run();
                RunProperties rp = new RunProperties();
                rp.FontSize = new FontSize() { Val = new StringValue("32") };
                rp.Bold = new Bold();
                run.Append(rp);
                Text text = new Text(enterParams.sheetName) { Space = SpaceProcessingModeValues.Preserve };
                run.Append(text);
                p.Append(run);
                body.Append(p);

                Table table = new Table();

                TableProperties tableProp = new TableProperties();
                TableBorders tableBorders = new TableBorders() 
                { 
                    InsideHorizontalBorder = 
                    new DocumentFormat.OpenXml.Wordprocessing.InsideHorizontalBorder() 
                    { Val = BorderValues.Single },
                    InsideVerticalBorder = 
                    new DocumentFormat.OpenXml.Wordprocessing.InsideVerticalBorder()
                    { Val = BorderValues.Single },
                    TopBorder= new DocumentFormat.OpenXml.Wordprocessing.TopBorder()
                    { Val = BorderValues.Single },
                    BottomBorder= new DocumentFormat.OpenXml.Wordprocessing.BottomBorder()
                    { Val = BorderValues.Single },
                    StartBorder=new DocumentFormat.OpenXml.Wordprocessing.StartBorder() 
                    { Val = BorderValues.Single },
                    EndBorder= new DocumentFormat.OpenXml.Wordprocessing.EndBorder()
                    { Val = BorderValues.Single }
                };
                TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };
                TableWidth tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };

                tableProp.Append(tableStyle, tableWidth, tableBorders);
                table.AppendChild(tableProp);

                TableGrid tableGrid = new TableGrid();
                for (int col=0; col < enterParams.colnames.Count; col++)
                {
                    tableGrid.Append(new GridColumn() { Width = "2394" });
                }
                table.AppendChild(tableGrid);

                TableRow tableHeaderRow = new TableRow();

                for (int col = 0; col < enterParams.colnames.Count; col++)
                {
                    Paragraph p1 = new Paragraph();
                    ParagraphProperties pp1 = new ParagraphProperties();
                    pp1.Justification = new Justification() { Val = JustificationValues.Center };
                    p1.Append(pp1);
                    Run run1 = new Run();
                    RunProperties rp1 = new RunProperties();
                    rp1.Bold = new Bold();
                    run1.Append(rp1);
                    Text text1 = new Text(enterParams.colnames[col]) { Space = SpaceProcessingModeValues.Preserve };
                    run1.Append(text1);
                    p1.Append(run1);
                    tableHeaderRow.Append(new TableCell(p1));
                }

                table.AppendChild(tableHeaderRow);

                for (int row = 0; row < enterParams.rows.Count; row++)
                {
                    TableRow tableRow = new TableRow();
                    for (int col = 0; col < enterParams.colnames.Count; col++)
                    {
                        Paragraph p1 = new Paragraph();
                        ParagraphProperties pp1 = new ParagraphProperties();
                        pp1.Justification = new Justification() { Val = JustificationValues.Center };
                        p1.Append(pp1);

                        Run run1 = new Run();
                        RunProperties rp1 = new RunProperties();
                        run1.Append(rp1);
                        Text text1 = new Text(enterParams.rows[row][col]) { Space = SpaceProcessingModeValues.Preserve };
                        run1.Append(text1);
                        p1.Append(run1);
                        tableRow.Append(new TableCell(p1));
                    }
                    table.AppendChild(tableRow);
                }
                body.AppendChild(table);
                mainPart.Document.Save();
            }
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}