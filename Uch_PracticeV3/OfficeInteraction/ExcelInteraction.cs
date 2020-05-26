using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Uch_PracticeV3.OfficeInteraction
{
    public class ExcelInteraction
    {
        public static FileContentResult ConvertToXlsx(EnterParams enterParams)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                IXLWorksheet worksheet;
                if (enterParams.sheetName.Length >= 30)
                {
                    worksheet = workbook.Worksheets.Add(enterParams.sheetName.Substring(0, 30));
                }
                else
                {
                    worksheet = workbook.Worksheets.Add(enterParams.sheetName);
                }
                //заголовки
                for (int col = 0; col < enterParams.colnames.Count; col++)
                {
                    var cell = worksheet.Cell(1, col + 1);
                    cell.Value = enterParams.colnames[col];
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Double;
                }
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.Beige;

                //нумерация строк/столбцов начинается с индекса 1 (не 0)
                for (int row = 0; row < enterParams.rows.Count; row++)
                {
                    for(int col =0; col<enterParams.colnames.Count; col++)
                    {
                        var cell = worksheet.Cell(row + 2, col + 1);
                        cell.Value = enterParams.rows[row][col];
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Double;
                    }
                }

                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"{enterParams.sheetName}.xlsx"
                    };
                }
            }
        }
    }
}