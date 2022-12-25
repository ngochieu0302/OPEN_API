using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class ExcelHelper
    {
        public static Dictionary<string, List<ExcelDataConfig>> GetConfigTemplate(IFormFile file)
        {
            Dictionary<string, List<ExcelDataConfig>> dicConfig = new Dictionary<string, List<ExcelDataConfig>>();
            using (var workBook = new XLWorkbook(file.OpenReadStream()))
            {
                var workSheets = workBook.Worksheets;
                foreach (var workSheet in workSheets)
                {
                    List<ExcelDataConfig> config = new List<ExcelDataConfig>();
                    var row = workSheet.Rows().FirstOrDefault();
                    foreach (IXLCell cell in row.Cells())
                    {
                        ExcelDataConfig cf = new ExcelDataConfig();
                        cf.row_index = cell.Address.RowNumber;
                        cf.col_index = cell.Address.ColumnNumber;
                        StringReader strReader = new StringReader(cell.Comment.Text);
                        while (true)
                        {
                            var aLine = strReader.ReadLine();
                            if (aLine != null)
                            {
                                aLine = aLine.Trim();
                                var arrProperty = aLine.Split(':');
                                var pr = arrProperty[0].Trim();
                                var count = arrProperty.Length;
                                switch (pr)
                                {
                                    case "cur":
                                        cf.cur = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    case "field":
                                        cf.field = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    case "type":
                                        cf.type = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    case "format":
                                        cf.format = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    case "required":
                                        cf.required = count <= 1 ? (bool?)null : arrProperty[1].Trim() == "true" ? true : false;
                                        break;
                                    case "max":
                                        decimal val_max;
                                        cf.max = count <= 1 ? (decimal?)null : decimal.TryParse(arrProperty[1].Trim(), out val_max) ? val_max : (decimal?)null;
                                        break;
                                    case "min":
                                        decimal val_min;
                                        cf.min = count <= 1 ? (decimal?)null : decimal.TryParse(arrProperty[1].Trim(), out val_min) ? val_min : (decimal?)null;
                                        break;
                                    case "minlength":
                                        decimal val_minlength;
                                        cf.minlength = count <= 1 ? (decimal?)null : decimal.TryParse(arrProperty[1].Trim(), out val_minlength) ? val_minlength : (decimal?)null;
                                        break;
                                    case "maxlength":
                                        decimal val_maxlength;
                                        cf.maxlength = count <= 1 ? (decimal?)null : decimal.TryParse(arrProperty[1].Trim(), out val_maxlength) ? val_maxlength : (decimal?)null;
                                        break;
                                    case "result":
                                        cf.result = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        config.Add(cf);
                    }
                    dicConfig.Add(workSheet.Name, config);
                }
            }
            return dicConfig;
        }
        public static Dictionary<string, List<ExcelDataConfig>> GetConfigExportTemplate(string pathFile)
        {
            Dictionary<string, List<ExcelDataConfig>> dicConfig = new Dictionary<string, List<ExcelDataConfig>>();
            using (var workBook = new XLWorkbook(pathFile))
            {
                var workSheets = workBook.Worksheets;
                foreach (var workSheet in workSheets)
                {
                    List<ExcelDataConfig> config = new List<ExcelDataConfig>();
                    var cells = workSheet.CellsUsed();
                    foreach (IXLCell cell in cells)
                    {
                        if (string.IsNullOrEmpty(cell.Comment.Text))
                        {
                            continue;
                        }
                        ExcelDataConfig cf = new ExcelDataConfig();
                        cf.row_index = cell.Address.RowNumber;
                        cf.col_index = cell.Address.ColumnNumber;
                        StringReader strReader = new StringReader(cell.Comment.Text);
                        while (true)
                        {
                            var aLine = strReader.ReadLine();
                            if (aLine != null)
                            {
                                aLine = aLine.Trim();
                                var arrProperty = aLine.Split(':');
                                var pr = arrProperty[0].Trim();
                                var count = arrProperty.Length;
                                switch (pr)
                                {
                                    case "cur":
                                        cf.cur = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    case "field":
                                        cf.field = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    case "type":
                                        cf.type = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    case "format":
                                        cf.format = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    case "required":
                                        cf.required = count <= 1 ? (bool?)null : arrProperty[1].Trim() == "true" ? true : false;
                                        break;
                                    case "max":
                                        decimal val_max;
                                        cf.max = count <= 1 ? (decimal?)null : decimal.TryParse(arrProperty[1].Trim(), out val_max) ? val_max : (decimal?)null;
                                        break;
                                    case "min":
                                        decimal val_min;
                                        cf.min = count <= 1 ? (decimal?)null : decimal.TryParse(arrProperty[1].Trim(), out val_min) ? val_min : (decimal?)null;
                                        break;
                                    case "minlength":
                                        decimal val_minlength;
                                        cf.minlength = count <= 1 ? (decimal?)null : decimal.TryParse(arrProperty[1].Trim(), out val_minlength) ? val_minlength : (decimal?)null;
                                        break;
                                    case "maxlength":
                                        decimal val_maxlength;
                                        cf.maxlength = count <= 1 ? (decimal?)null : decimal.TryParse(arrProperty[1].Trim(), out val_maxlength) ? val_maxlength : (decimal?)null;
                                        break;
                                    case "result":
                                        cf.result = count <= 1 ? null : arrProperty[1].Trim();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        config.Add(cf);
                    }
                    dicConfig.Add(workSheet.Name, config);
                }
            }
            return dicConfig;
        }
        public static byte[] ExportExcel(string filename, int numRows, int numCols)
        {
            var template = new FileInfo(filename);
            using (var templateStream = new MemoryStream())
            {
                using (SpreadsheetDocument spreadDocument = SpreadsheetDocument.Open(filename, true))
                {
                    WorkbookPart workbookPart = spreadDocument.WorkbookPart;
                    Sheet sheet = spreadDocument.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                    Worksheet worksheet = (spreadDocument.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                    SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                    for (int row = 0; row < numRows; row++)
                    {
                        Row r = new Row();
                        r.RowIndex = (uint)row + 2;
                        for (int col = 0; col < numCols; col++)
                        {
                            Cell c = new Cell();
                            CellValue v = new CellValue();
                            v.Text = "Khi sự nghiệp đang nở rộ, Hà Gia Kính đột ngột rút khỏi ngành giải trí, chuyển hướng sang kinh doanh, khiến người hâm mộ không khỏi bất ngờ";
                            c.AppendChild(v);
                            r.AppendChild(c);
                        }
                        sheetData.AppendChild(r);
                    }
                    spreadDocument.WorkbookPart.Workbook.Save();
                    spreadDocument.Close();
                }
                byte[] templateBytes = File.ReadAllBytes(template.FullName);
                templateStream.Write(templateBytes, 0, templateBytes.Length);
                templateStream.Position = 0;
                var result = templateStream.ToArray();
                templateStream.Flush();
                try
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }
                catch
                {
                }
                return result;
            }

        }
    }
}
