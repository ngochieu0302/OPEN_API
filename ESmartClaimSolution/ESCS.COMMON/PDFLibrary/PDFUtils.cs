using DevExpress.XtraRichEdit;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlHelpers.Word;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ESCS.COMMON.PDFLibrary
{
    public static class PDFUtils
    {
        public static byte[] WordTemplateToPDF()
        {
            //string path, dynamic model
            string path = @"D:\source\openapi\ESmartClaimSolution\ESCS.API\App_Data\test_doc.docx";
            object model = new TruongHoc()
            {
                MA_TRUONG = "HUMG",
                TEN_TRUONG = "Đại học mỏ địa chất",
                CK = new List<ChuyenKhoa>()
                {
                    new ChuyenKhoa()
                    {
                        MA = "CNTT", TEN = "Công nghệ thông tin",
                        LH = new List<LopHoc>()
                        {
                            new LopHoc(){ MA = "TKT", TEN="Lớp tin kinh tế"},
                            new LopHoc(){ MA = "PM", TEN="Lớp công nghệ phần mềm"},
                            new LopHoc(){ MA = "TTD", TEN="Lớp tin trắc địa"}
                        }
                    },
                    new ChuyenKhoa()
                    {
                        MA = "TDIA", TEN = "Trắc địa",
                        LH = new List<LopHoc>()
                        {
                            new LopHoc(){ MA = "KKT", TEN="Lớp khoan khai thác"},
                            new LopHoc(){ MA = "KKH", TEN="Lớp luyện kim khí"}
                        }
                    },
                },
                TEST = new List<ChuyenKhoa>()
                {
                    new ChuyenKhoa()
                    {
                        MA = "CNTT", TEN = "Công nghệ thông tin",
                        LH = new List<LopHoc>()
                        {
                            new LopHoc(){ MA = "TKT", TEN="Lớp tin kinh tế"},
                            new LopHoc(){ MA = "PM", TEN="Lớp công nghệ phần mềm"},
                            new LopHoc(){ MA = "TTD", TEN="Lớp tin trắc địa"}
                        }
                    },
                    new ChuyenKhoa()
                    {
                        MA = "TDIA", TEN = "Trắc địa",
                        LH = new List<LopHoc>()
                        {
                            new LopHoc(){ MA = "KKT", TEN="Lớp khoan khai thác"},
                            new LopHoc(){ MA = "KKH", TEN="Lớp luyện kim khí"}
                        }
                    },
                }
            };
            var propertyInfos = model.GetType().GetProperties();
            var dicSingle = propertyInfos.Where(n => !typeof(ICollection).IsAssignableFrom(n.PropertyType)).Select(n => new { Key = n.Name, Value = GetPropValue(model, n.Name) }).ToDictionary(o => o.Key, o => o.Value); ;
            byte[] byteArray = File.ReadAllBytes(path);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, (int)byteArray.Length);
                using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(stream, true))
                {
                    #region Đổ dữ liệu đơn vào template
                    //var fSimpleSingle = wordprocessingDocument.GetSimpleFields().Where(n => !n.InnerText.Contains("[*]"));
                    //if (fSimpleSingle != null)
                    //    foreach (var fld in fSimpleSingle)
                    //        fld.ReplaceWithText(dicSingle.GetStringFromDic(fld.InnerText?.Replace("«", "")?.Replace("»", "")));
                    #endregion
                    #region Lặp table và đổ dữ liệu vào table
                    var tables = wordprocessingDocument.MainDocumentPart.Document.Body.Elements<Table>();
                    foreach (var table in tables)
                    {
                        int beginRowIndex = 0;
                        int endRowIndex = table.Elements<TableRow>().Count() - 1;
                        GennerateTableTemplate(table, model, beginRowIndex, endRowIndex);
                    }

                    #endregion
                    #region Lưu và xuất file PDF
                    wordprocessingDocument.MainDocumentPart.Document.Save();
                    wordprocessingDocument.Close();
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using (RichEditDocumentServer wordProcessor = new RichEditDocumentServer())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        wordProcessor.LoadDocument(stream);
                        MemoryStream convertStream = new MemoryStream();
                        wordProcessor.ExportToPdf(convertStream);
                        convertStream.Seek(0, SeekOrigin.Begin);
                        return convertStream.ToArray();
                    }
                    #endregion
                }
            }
        }
        public static void GennerateTableTemplate(Table table, object model, int beginRowIndex, int endRowIndex)
        {
            //Tìm group row lặp
            IEnumerable<string> groups = TimGroupLap(table, beginRowIndex, endRowIndex);
            if (groups == null || groups.Count() <= 0)
                return;
            foreach (var group in groups)
                GennerateGroupTemplate(table, model, group);

        }
        public static void GennerateGroupTemplate(Table table, object model, string group)
        {
            var parent = group + "[*]";
        }
        private static IEnumerable<string> TimGroupLap(Table table, int beginRowIndex, int endRowIndex, string parent = "")
        {
            var simpleFieldLoop = table.Descendants<SimpleField>().Select(n => n.InnerText.Replace("«", "")?.Replace("»", ""))
                                            .Where(n => n.StartsWith(parent) && n.EndsWith("_LOOP"))
                                            .WhereIf(string.IsNullOrEmpty(parent), n => n.Count(f => f == '*') == 0)
                                            .WhereIf(!string.IsNullOrEmpty(parent), n => n.Replace(parent, "").Count(f => f == '*') == 0)
                                            .Select(n => n.Replace("_LOOP", ""));
            return simpleFieldLoop;
        }
        private static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            return source;
        }

        #region Private method
        public static TemplatePDFConfig GetConfigTemplatePDF()
        {
            string path = @"D:\source\openapi\ESmartClaimSolution\ESCS.API\App_Data\test_doc.docx";
            byte[] byteArray = File.ReadAllBytes(path);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, (int)byteArray.Length);
                using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(stream, true))
                {

                    var res = GetConfigTemplate(wordprocessingDocument);
                    wordprocessingDocument.Close();
                    return res;

                }
            }
        }
        private static TemplatePDFConfig GetConfigTemplate(WordprocessingDocument wordprocessingDocument)
        {
            TemplatePDFConfig config = new TemplatePDFConfig();
            List<TablePDFConfig> tableConfigs = new List<TablePDFConfig>();
            IEnumerable<Table> tables = wordprocessingDocument.MainDocumentPart.Document.Body.Elements<Table>();
            config.FieldsSingle = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<SimpleField>()?.Select(n => n.InnerText.Replace("«", "")?.Replace("»", ""))?.ToList();
            foreach (var table in tables.WithIndex())
            {
                var tableConfig = new TablePDFConfig()
                {
                    TableIndex = table.index,
                    TableName = "TABLE" + table.index,
                    ChildrenTables = GetTableChildrens(table.item, "TABLE" + table.index)
                };
                int rowCount = 0;
                tableConfig.Fields = table.item.GetFieldsInTable(out rowCount);
                tableConfig.FieldToGroupRowLoops();
                if (config.FieldsSingle != null && config.FieldsSingle.Count() > 0)
                {
                    if (tableConfig.Fields != null && tableConfig.Fields.Count() > 0)
                    {
                        var dsField = tableConfig.Fields.Select(n => n.FieldName);
                        if (dsField != null && dsField.Count() > 0)
                            config.FieldsSingle = config.FieldsSingle.Where(n => !dsField.Contains(n)).ToList();
                    }
                    if (tableConfig.GroupRowLoops != null && tableConfig.GroupRowLoops.Count() > 0)
                    {
                        var dsFieldLoop = tableConfig.GroupRowLoops.Select(n => n.FieldLoop);
                        if (dsFieldLoop != null && dsFieldLoop.Count() > 0)
                            config.FieldsSingle = config.FieldsSingle.Where(n => !dsFieldLoop.Contains(n)).ToList();
                        var dsFieldEndLoop = tableConfig.GroupRowLoops.Select(n => n.FieldEndLoop);
                        if (dsFieldEndLoop != null && dsFieldEndLoop.Count() > 0)
                            config.FieldsSingle = config.FieldsSingle.Where(n => !dsFieldEndLoop.Contains(n)).ToList();
                    }
                }
                tableConfig.RowCount = rowCount;
                tableConfigs.Add(tableConfig);
            }
            config.TableConfigs = tableConfigs;
            return config;
        }
        private static void DupplicateRowFromConfig(WordprocessingDocument wordprocessingDocument, object model, PropertyInfo[] propertyInfos, TablePDFConfig tableConfig, string groupParentName = "", int parentDataIndex = 0)
        {
            if (tableConfig.GroupRowLoops != null && tableConfig.GroupRowLoops.Count() > 0)
            {
                var groupParent = tableConfig.GroupRowLoops.Where(n => n.GroupParent == groupParentName);
                if (groupParent == null || groupParent.Count() <= 0)
                    return;
                var table = wordprocessingDocument.MainDocumentPart.Document.Body.Elements<Table>().ElementAt(tableConfig.TableIndex);
                foreach (var grouRowRepeat in groupParent)
                {
                    int rowIndexLoop = grouRowRepeat.RowIndexLoop;
                    int rowIndexEndLoop = grouRowRepeat.RowIndexEndLoop;
                    var prefix = grouRowRepeat.FieldLoop.Replace("_LOOP", "");
                    var dataTableDupicate = propertyInfos.Where(n => n.Name == prefix).Select(n => new { Key = n.Name, Value = GetPropCollection(model, n.Name) }).FirstOrDefault();
                    if (dataTableDupicate != null && dataTableDupicate.Value != null)
                    {
                        var rowIndexEndLoopTmp = rowIndexEndLoop;
                        for (int indexTable = 0; indexTable < dataTableDupicate.Value.Count; indexTable++)
                        {
                            for (int indexRow = rowIndexLoop; indexRow <= rowIndexEndLoop; indexRow++)
                            {
                                var a = table.Elements<TableRow>();
                                var tableRow = table.Elements<TableRow>().ElementAt(indexRow);
                                var tableRowClone = (TableRow)tableRow.Clone();
                                var test = tableRowClone.Descendants<SimpleField>();
                                var fSimpleSingleTable = tableRowClone.Descendants<SimpleField>().Where(n => n.Instruction.Value.StartsWith(" MERGEFIELD  " + prefix + "[*]") || n.Instruction.Value.StartsWith(prefix + "[*]"));
                                foreach (var fldTable in fSimpleSingleTable)
                                {
                                    fldTable.InnerXml = fldTable.OuterXml.Replace(prefix + "[*]", prefix + "[" + indexTable + "]");
                                    fldTable.Instruction.Value = fldTable.Instruction.Value.Replace(prefix + "[*]", prefix + "[" + indexTable + "]");
                                    fldTable.Instruction.InnerText = fldTable.Instruction.InnerText.Replace(prefix + "[*]", prefix + "[" + indexTable + "]");
                                }

                                table.InsertAfter(tableRowClone, table.Elements<TableRow>().ElementAt(rowIndexEndLoopTmp));
                                rowIndexEndLoopTmp = rowIndexEndLoopTmp + 1;
                            }
                            //Lặp xong vòng ngoài
                            var groupChild = tableConfig.GroupRowLoops.Where(n => n.GroupParent == prefix);
                            if (groupChild != null && groupChild.Count() > 0)
                                foreach (var item in groupChild)
                                    DupplicateRowFromConfig(wordprocessingDocument, model, propertyInfos, tableConfig, item.GroupParent, indexTable);
                        }
                    }
                }
            }
        }

        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        private static ICollection GetPropCollection(object src, string propName)
        {
            return (ICollection)src.GetType().GetProperty(propName).GetValue(src, null);
        }
        private static string GetStringFromDic(this IDictionary<string, object> source, string key)
        {
            if (!source.ContainsKey(key))
                return "";
            else if (source[key] == null)
                return "";
            return source[key].ToString();
        }
        private static bool isCollection(object o)
        {
            return typeof(ICollection).IsAssignableFrom(o.GetType())
                || typeof(ICollection<>).IsAssignableFrom(o.GetType());
        }
        private static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
        private static List<TablePDFConfig> GetTableChildrens(Table tableRoot, string tableNameRoot)
        {
            List<TablePDFConfig> childrens = new List<TablePDFConfig>();
            IEnumerable<Table> tables = tableRoot.GetAllTableElements();
            if (tables == null || tables.Count() <= 0)
                return childrens;
            foreach (var table in tables.WithIndex())
            {
                var tableConfig = new TablePDFConfig()
                {
                    TableIndex = table.index,
                    TableName = tableNameRoot + "." + table.index,
                    ChildrenTables = GetTableChildrens(table.item, tableNameRoot + "." + table.index)
                };
                int rowCount = 0;
                tableConfig.Fields = table.item.GetFieldsInTable(out rowCount);
                tableConfig.FieldToGroupRowLoops();
                tableConfig.RowCount = rowCount;
                childrens.Add(tableConfig);
            }
            return childrens;
        }
        private static IEnumerable<Table> GetAllTableElements(this Table tableRoot)
        {
            List<Table> tableReturn = new List<Table>();
            IEnumerable<TableRow> tblRow = tableRoot.Elements<TableRow>();
            if (tblRow != null && tblRow.Count() > 0)
            {
                foreach (var row in tblRow)
                {
                    IEnumerable<TableCell> tblCell = row.Elements<TableCell>();
                    foreach (var cell in tblCell)
                    {
                        IEnumerable<Table> tables = cell.Elements<Table>();
                        if (tables == null || tables.Count() <= 0)
                            continue;
                        tableReturn.AddRange(tables);
                    }
                }
            }
            return tableReturn;
        }
        private static List<FieldPDFConfig> GetFieldsInTable(this Table tableRoot, out int rowCount)
        {
            rowCount = 0;
            List<FieldPDFConfig> fields = new List<FieldPDFConfig>();
            IEnumerable<TableRow> tblRow = tableRoot.Elements<TableRow>();
            if (tblRow != null && tblRow.Count() > 0)
            {
                int rowIndex = 0;
                foreach (var row in tblRow)
                {
                    rowCount++;
                    IEnumerable<TableCell> tblCell = row.Elements<TableCell>();
                    int cellIndex = 0;
                    foreach (var cell in tblCell)
                    {
                        IEnumerable<Paragraph> paragraphs = cell.Elements<Paragraph>();
                        if (paragraphs == null || paragraphs.Count() <= 0)
                            continue;
                        foreach (var p in paragraphs)
                        {
                            IEnumerable<SimpleField> pSimpleFields = p.Elements<SimpleField>();
                            if (pSimpleFields == null || pSimpleFields.Count() <= 0)
                                continue;
                            var field = pSimpleFields.Select(n => new FieldPDFConfig()
                            {
                                FieldName = n.InnerText?.Replace("«", "")?.Replace("»", ""),
                                RowIndex = rowIndex,
                                CellIndex = cellIndex
                            });
                            fields.AddRange(field);
                        }
                        cellIndex++;
                    }
                    rowIndex++;
                }
            }
            return fields;
        }
        #endregion
    }
    public class TemplatePDFConfig
    {
        public List<string> FieldsSingle { get; set; }
        public List<TablePDFConfig> TableConfigs { get; set; }
    }
    public class TablePDFConfig
    {
        /// <summary>
        /// Table index
        /// </summary>
        public int TableIndex { get; set; }
        /// <summary>
        /// Tên bảng
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Số dòng
        /// </summary>
        public int RowCount { get; set; }
        /// <summary>
        /// Bảng con trong bảng
        /// </summary>
        public List<TablePDFConfig> ChildrenTables { get; set; }

        /// <summary>
        /// Danh sách field trong bảng
        /// </summary>
        public List<FieldPDFConfig> Fields { get; set; }

        /// <summary>
        /// Danh sách group row loop
        /// </summary>
        public List<GroupRowLoopConfig> GroupRowLoops { get; set; }

        public void FieldToGroupRowLoops()
        {
            this.GroupRowLoops = new List<GroupRowLoopConfig>();
            if (this.Fields != null && this.Fields.Count() > 0)
            {
                var fieldLoop = this.Fields.Where(n => n.FieldName.Contains("_LOOP"));
                foreach (var item in fieldLoop)
                {
                    GroupRowLoopConfig gr = new GroupRowLoopConfig();
                    gr.FieldLoop = item.FieldName;
                    gr.RowIndexLoop = item.RowIndex;
                    var endLoop = this.Fields.Where(n => n.FieldName == gr.FieldLoop.Replace("_LOOP", "") + "_ENDLOOP").FirstOrDefault();
                    if (endLoop != null)
                    {
                        gr.FieldEndLoop = endLoop.FieldName;
                        gr.RowIndexEndLoop = endLoop.RowIndex;
                    }
                    gr.Level = gr.FieldLoop.Count(f => f == '*');
                    var parents = endLoop.FieldName.Split(".");
                    if (parents.Length <= 1)
                        gr.GroupParent = "";
                    else
                    {
                        var strParent = "";
                        for (int i = 0; i < parents.Length - 1; i++)
                        {
                            if (i == 0)
                                strParent += parents[i];
                            else
                                strParent += "." + parents[i];
                        }
                        if (strParent.EndsWith("[*]"))
                            strParent = strParent.Substring(0, strParent.Length - 3);
                        gr.GroupParent = strParent;
                    }
                    this.GroupRowLoops.Add(gr);
                }
                this.GroupRowLoops = this.GroupRowLoops.OrderBy(n => n.Level).ToList();
                this.Fields.RemoveAll(n => n.FieldName.Contains("_LOOP") || n.FieldName.Contains("_ENDLOOP"));
            }
        }
    }
    public class FieldPDFConfig
    {
        public string FieldName { get; set; }
        public int RowIndex { get; set; }
        public int CellIndex { get; set; }
    }
    public class GroupRowLoopConfig
    {
        public string FieldLoop { get; set; }
        public string FieldEndLoop { get; set; }
        public int RowIndexLoop { get; set; }
        public int RowIndexEndLoop { get; set; }
        public int Level { get; set; }
        public string GroupParent { get; set; }

    }
    #region Data
    public class TruongHoc
    {
        public string MA_TRUONG { get; set; }
        public string TEN_TRUONG { get; set; }
        public List<ChuyenKhoa> CK { get; set; }
        public List<ChuyenKhoa> TEST { get; set; }
    }
    public class ChuyenKhoa
    {
        public string MA { get; set; }
        public string TEN { get; set; }
        public List<LopHoc> LH { get; set; }
    }
    public class LopHoc
    {
        public string MA { get; set; }
        public string TEN { get; set; }
    }
    #endregion
}
