using DevExpress.XtraRichEdit;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ESCS.COMMON.ExtensionMethods;
using ESCS.MODEL.MAU_IN;
using OpenXmlHelpers.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace ESCS.COMMON.Common
{
    public class ESCSExportPDF
    {
        public static byte[] ToTrinhPASC(string path, DataSet ds, string pathChuKySoFull = null)
        {
            byte[] byteArray = File.ReadAllBytes(Path.Combine(AppSettings.PathFolderNotDeleteFull, path));
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(byteArray, 0, (int)byteArray.Length);
                using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open(stream, true))
                {
                    #region Fill dữ liệu vào template
                    FillTableRepeat(wordprocessingDocument, ds);
                    MergeFieldSigle(wordprocessingDocument, ds);
                    FillTable(wordprocessingDocument, ds);
                    FillImage(wordprocessingDocument, ds);
                    AnHienNghiepVu(wordprocessingDocument, ds);
                    FillChuKySo(wordprocessingDocument, pathChuKySoFull);
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
        private static void MergeFieldSigle(WordprocessingDocument wordprocessingDocument, DataSet ds)
        {
            var fields = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<FieldCode>();
            var bookmarks = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<BookmarkStart>()?
                            .Where(n => n.Name.Value.EndsWith("WMLRepeat"))
                            .Select(n => n.Name.Value.Replace("WMLRepeat", ""));
            var tableSingle = ds.Tables.Cast<DataTable>().Where(x => !bookmarks.Contains(x.TableName) && x.TableName.Length == 1);
            if (tableSingle != null && tableSingle.Count() > 0)
            {
                foreach (var table in tableSingle)
                {
                    var bFields = fields.Where(n => n.InnerText.Contains(" MERGEFIELD  " + table.TableName.ToUpper())).ToList();
                    if (bFields != null && bFields.Count() > 0)
                        for (int i = bFields.Count() - 1; i >= 0; i--)
                        {
                            var column = bFields[i].InnerText.Replace(" MERGEFIELD  " + table.TableName.ToUpper(), "").Trim().ToLower();
                            if (ds.Tables[table.TableName.ToUpper()].Columns.Contains(column))
                                bFields[i].ReplaceWithText(ds.Tables[table.TableName.ToUpper()].Rows[0][column]?.ToString());
                        }
                }
            }
        }
        private static void FillTable(WordprocessingDocument wordprocessingDocument, DataSet ds)
        {
            var fields = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<FieldCode>();
            var bookmarks = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<BookmarkStart>()?
                            .Where(n => n.Name.Value.EndsWith("WMLRepeat"));

            var bookmarkNames = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<BookmarkStart>()?
                            .Where(n => n.Name.Value.EndsWith("WMLRepeat"))
                            .Select(n => n.Name.Value.Replace("WMLRepeat", ""));
            var tables = ds.Tables.Cast<DataTable>().Where(x => bookmarkNames.Contains(x.TableName) && x.TableName.Length == 1).ToList();
            DataTable dt = ds.Tables["curs_ts"].Copy();
            dt.TableName = "TS";
            tables.Add(dt);
            if (tables != null && tables.Count() > 0)
            {
                foreach (var bookmark in bookmarks)
                {
                    var tr = bookmark.Parent?.Parent?.Parent;
                    if (tr != null)
                    {
                        var tableOpenXml = tr.Parent;
                        var table = tables.Where(n => n.TableName == bookmark.Name.Value.Replace("WMLRepeat", "").ToUpper()).FirstOrDefault();
                        if (table != null)
                        {
                            for (int i = table.Rows.Count - 1; i >= 0; i--)
                            {
                                DataRow row = table.Rows[i];
                                var trClone = (TableRow)tr.Clone();
                                var fieldRows = trClone.Descendants<FieldCode>();
                                foreach (var fieldRow in fieldRows)
                                {
                                    var column = fieldRow.InnerText.Replace(" MERGEFIELD  " + table.TableName.ToUpper(), "").Trim().ToLower();
                                    fieldRow.ReplaceWithText(row[column]?.ToString());
                                }
                                tableOpenXml.InsertAfter(trClone, tr);
                            }
                        }
                        tr.Remove();
                    }
                }
            }
        }
        private static void FillImage(WordprocessingDocument wordprocessingDocument, DataSet ds)
        {
            var fields = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<Text>().Where(n => n.InnerText.StartsWith("PSOURCE_"));
            var tableSingle = ds.Tables.Cast<DataTable>().Where(x => x.TableName.ToLower() == "curs_source").FirstOrDefault();
            if (fields != null && fields.Count() > 0 && tableSingle != null && tableSingle.Rows.Count > 0)
            {
                List<ESCSSource> source = ConvertDataTable<ESCSSource>(tableSingle);
                foreach (var field in fields)
                {
                    var row = source.Where(n => n.psource.ToUpper().ToString() == field.InnerText.ToUpper()).FirstOrDefault();
                    if (row != null)
                    {
                        if (!string.IsNullOrEmpty(row.source))
                        {
                            ImagePart imagePart = wordprocessingDocument.MainDocumentPart.AddImagePart(ImagePartType.Png);
                            using (FileStream stream = new FileStream(Path.Combine(AppSettings.PathFolderNotDeleteFull, row.source), FileMode.Open))
                                imagePart.FeedData(stream);
                            var parent = field.Parent;
                            ImageDetails image = new ImageDetails(Path.Combine(AppSettings.PathFolderNotDeleteFull, row.source));
                            var element = GetImageDrawing(wordprocessingDocument.MainDocumentPart.GetIdOfPart(imagePart), image);
                            if (parent is Run)
                            {
                                field.Parent.InsertAfter<Drawing>(element, field);
                                field.Remove();
                            }
                        }
                    }
                }
                foreach (var field in fields)
                {
                    field.Parent.Parent.Remove();
                }
            }
        }
        private static void FillChuKySo(WordprocessingDocument wordprocessingDocument, string pathFile = null)
        {
            var field = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<Text>().Where(n => n.InnerText.StartsWith("PSIGNATURE")).FirstOrDefault();
            if (!string.IsNullOrEmpty(pathFile) && field != null)
            {
                ImagePart imagePart = wordprocessingDocument.MainDocumentPart.AddImagePart(ImagePartType.Png);
                using (FileStream stream = new FileStream(pathFile, FileMode.Open))
                    imagePart.FeedData(stream);
                var parent = field.Parent;
                ImageDetails image = new ImageDetails(pathFile);
                var element = GetImageDrawing(wordprocessingDocument.MainDocumentPart.GetIdOfPart(imagePart), image);
                if (parent is Run)
                {
                    field.Parent.InsertAfter<Drawing>(element, field);
                    field.Remove();
                }
            }
            else
            {
                field?.Remove();
            }
            
        }
        private static void FillTableRepeat(WordprocessingDocument wordprocessingDocument, DataSet ds)
        {
            var bookmarks = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<BookmarkStart>()?
                            .Where(n => n.Name.Value.EndsWith("TABLERepeat"));

            var bookmarkNames = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<BookmarkStart>()?
                            .Where(n => n.Name.Value.EndsWith("TABLERepeat"))
                            .Select(n => n.Name.Value.Replace("TABLERepeat", ""));
            var tables = ds.Tables.Cast<DataTable>().Where(x => bookmarkNames.Contains(x.TableName) && x.TableName.Length == 1);

            if (tables != null && tables.Count() > 0)
            {
                foreach (var bookmark in bookmarks)
                {
                    var tableTemplate = bookmark.Parent?.Parent?.Parent?.Parent;
                    var table = tables.Where(n => n.TableName == bookmark.Name.Value.Replace("TABLERepeat", "").ToUpper()).FirstOrDefault();
                    var tableCauKeo = ds.Tables["Q"];
                    if (table != null)
                    {
                        List<DoiTuongXeDB> hangMuc = GetListTNDSTAI_SAN(table);
                        List<ChiPhiKhacDB> chiPhiKhac = GetListChiPhiKhac(tableCauKeo);
                        List<DoiTuongXe> doiTuongs = ConvertDataTNDSTAI_SAN(hangMuc, chiPhiKhac);
                        if (doiTuongs != null || doiTuongs.Count() > 0)
                        {
                            foreach (var doiTuong in doiTuongs)
                            {
                                var tableClone = (Table)tableTemplate.Clone();
                                var fields = tableClone.Descendants<FieldCode>();
                                var fieldSingle = fields.Where(n => !n.InnerText.StartsWith(" MERGEFIELD  OTT_") && !n.InnerText.StartsWith(" MERGEFIELD  OSC_") && !n.InnerText.StartsWith(" MERGEFIELD  OSON_"));
                                var fieldRepeat = fields.Where(n => n.InnerText.StartsWith(" MERGEFIELD  OTT_") || n.InnerText.StartsWith(" MERGEFIELD  OSC_") || n.InnerText.StartsWith(" MERGEFIELD  OSON_"));
                                if (fieldSingle != null && fieldSingle.Count() > 0)
                                    foreach (var field in fieldSingle)
                                    {
                                        var fieldSingleName = field.InnerText.Replace(" MERGEFIELD  O", "").ToLower().Trim();
                                        var val = GetPropValue(doiTuong, fieldSingleName);
                                        string valStr = val == null ? "" : val.ToString();
                                        field.ReplaceWithText(valStr);
                                    }
                                var bookmarkTblClone = tableClone.Descendants<BookmarkStart>();
                                var bookmarkSon = bookmarkTblClone?.Where(n => n.Name.Value.StartsWith("OSONWMLRepeat")).FirstOrDefault();
                                var bookmarkSC = bookmarkTblClone?.Where(n => n.Name.Value.StartsWith("OSCWMLRepeat")).FirstOrDefault();
                                var bookmarkTT = bookmarkTblClone?.Where(n => n.Name.Value.StartsWith("OTTWMLRepeat")).FirstOrDefault();
                                if (bookmarkSon != null)
                                {
                                    var tr = bookmarkSon.Parent.Parent.Parent;
                                    if (doiTuong.Son != null && doiTuong.Son.Count() > 0)
                                        foreach (var hang_muc in doiTuong.Son)
                                        {
                                            var trClone = (TableRow)tr.Clone();
                                            var fieldRows = trClone.Descendants<FieldCode>();
                                            if (fieldRows != null && fieldRows.Count() > 0)
                                            {
                                                foreach (var fieldRow in fieldRows)
                                                {
                                                    var fieldName = fieldRow.InnerText.Replace(" MERGEFIELD  OSON_", "").ToLower().Trim();
                                                    var val = GetPropValue(hang_muc, fieldName);
                                                    string valStr = val == null ? "" : val.ToString();
                                                    fieldRow.ReplaceWithText(valStr);
                                                }
                                                tr.InsertAfterSelf(trClone);
                                            }
                                        }
                                    tr.Remove();
                                }
                                if (bookmarkSC != null)
                                {
                                    var tr = bookmarkSC.Parent.Parent.Parent;
                                    if (doiTuong.SuaChua != null && doiTuong.SuaChua.Count() > 0)
                                        foreach (var hang_muc in doiTuong.SuaChua)
                                        {
                                            var trClone = (TableRow)tr.Clone();
                                            var fieldRows = trClone.Descendants<FieldCode>();
                                            if (fieldRows != null && fieldRows.Count() > 0)
                                            {
                                                foreach (var fieldRow in fieldRows)
                                                {
                                                    var fieldName = fieldRow.InnerText.Replace(" MERGEFIELD  OSC_", "").ToLower().Trim();
                                                    var val = GetPropValue(hang_muc, fieldName);
                                                    string valStr = val == null ? "" : val.ToString();
                                                    fieldRow.ReplaceWithText(valStr);
                                                }
                                                tr.InsertAfterSelf(trClone);
                                            }
                                        }
                                    tr.Remove();
                                }
                                if (bookmarkTT != null)
                                {
                                    var tr = bookmarkTT.Parent.Parent.Parent;
                                    if (doiTuong.ThayThe != null && doiTuong.ThayThe.Count() > 0)
                                        foreach (var hang_muc in doiTuong.ThayThe)
                                        {
                                            var trClone = (TableRow)tr.Clone();
                                            var fieldRows = trClone.Descendants<FieldCode>();
                                            if (fieldRows != null && fieldRows.Count() > 0)
                                            {
                                                foreach (var fieldRow in fieldRows)
                                                {
                                                    var fieldName = fieldRow.InnerText.Replace(" MERGEFIELD  OTT_", "").ToLower().Trim();
                                                    var val = GetPropValue(hang_muc, fieldName);
                                                    string valStr = val == null ? "" : val.ToString();
                                                    fieldRow.ReplaceWithText(valStr);
                                                }
                                                tr.InsertAfterSelf(trClone);
                                            }
                                        }
                                    tr.Remove();
                                }
                                foreach (var item in bookmarkTblClone)
                                    item.Remove();
                                tableTemplate.Parent.InsertAfter(tableClone, tableTemplate);
                            }
                        }
                    }
                    tableTemplate.Remove();
                }
            }
        }
        public static void AnHienNghiepVu(WordprocessingDocument wordprocessingDocument, DataSet ds)
        {
            var outerXml = wordprocessingDocument.MainDocumentPart.Document.Body.OuterXml;
            var bookmarkNames = wordprocessingDocument.MainDocumentPart.Document.Body.Descendants<BookmarkStart>()?
                            .Where(n => n.Name.Value.StartsWith("BEGIN_") || n.Name.Value.StartsWith("END_"));

            var table = ds.Tables.Cast<DataTable>().Where(x => x.TableName == "curs_an_hien").FirstOrDefault();
            if (table != null && table.Rows.Count > 0)
            {
                List<AnHienNghiepVu> dsNghiepVu = ConvertDataTable<AnHienNghiepVu>(table);
                dsNghiepVu = dsNghiepVu.Where(n => n.hien_thi == "0").ToList();
                if (dsNghiepVu != null && dsNghiepVu.Count() > 0)
                    foreach (var item in dsNghiepVu)
                    {
                        var bookmarkStartName = "BEGIN_" + item.nhom.ToUpper();
                        var bookmarkEndName = "END_" + item.nhom.ToUpper();
                        var bookmarkStart = bookmarkNames.Where(n => n.Name == bookmarkStartName).FirstOrDefault();
                        var bookmarkEnd = bookmarkNames.Where(n => n.Name == bookmarkEndName).FirstOrDefault();
                        if (bookmarkStart!=null && bookmarkEnd!=null)
                        {
                            var pStart = bookmarkStart.Parent;
                            var pEnd = bookmarkEnd.Parent;
                            bool check = true;
                            do
                            {
                                var next = pStart.NextSibling();
                                if (next is Paragraph)
                                {
                                    var count = next.Descendants<BookmarkStart>().Where(n => n.Name == bookmarkEndName).Count();
                                    if (count>0)
                                        check = false;
                                }
                                if (check)
                                    next?.Remove();
                            } while (check);
                            pStart?.Remove();
                            pEnd?.Remove();
                        }
                    }
            }
        }
        private static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static List<DoiTuongXeDB> GetListTNDSTAI_SAN(DataTable dt)
        {
            var convertedList = (from rw in dt.AsEnumerable()
                                 select new DoiTuongXeDB()
                                 {
                                     so_id_doi_tuong = Convert.ToDecimal(rw["so_id_doi_tuong"]),
                                     ten_doi_tuong = Convert.ToString(rw["ten_doi_tuong"]),
                                     nhom = Convert.ToString(rw["nhom"]),
                                     loai = Convert.ToString(rw["loai"]),
                                     ten_hm = Convert.ToString(rw["ten_hm"]),
                                     tien_bgia = Convert.ToDecimal(rw["tien_bgia"]),
                                     tien_vtu_dx = Convert.ToDecimal(rw["tien_vtu_dx"]),
                                     tien_nhan_cong_dx = Convert.ToDecimal(rw["tien_nhan_cong_dx"]),
                                     tien_khac_dx = Convert.ToDecimal(rw["tien_khac_dx"]),
                                     tien_dx = Convert.ToDecimal(rw["tien_dx"]),
                                     pt_khau_hao = Convert.ToDecimal(rw["pt_khau_hao"]),
                                     tien_khau_hao = Convert.ToDecimal(rw["tien_khau_hao"]),
                                     pt_bao_hiem = Convert.ToDecimal(rw["pt_bao_hiem"]),
                                     tien_bao_hiem = Convert.ToDecimal(rw["tien_bao_hiem"]),
                                     pt_giam_tru = Convert.ToDecimal(rw["pt_giam_tru"]),
                                     tien_giam_tru = Convert.ToDecimal(rw["tien_giam_tru"]),
                                     tl_giam_gia_vtu = Convert.ToDecimal(rw["tl_giam_gia_vtu"]),
                                     tien_giam_gia_vtu = Convert.ToDecimal(rw["tien_giam_gia_vtu"]),
                                     tl_giam_gia_nhan_cong = Convert.ToDecimal(rw["tl_giam_gia_nhan_cong"]),
                                     tien_giam_gia_nhan_cong = Convert.ToDecimal(rw["tien_giam_gia_nhan_cong"]),
                                     tl_giam_gia_khac = Convert.ToDecimal(rw["tl_giam_gia_khac"]),
                                     tien_giam_gia_khac = Convert.ToDecimal(rw["tien_giam_gia_khac"]),
                                     tien_mien_thuong = Convert.ToDecimal(rw["tien_mien_thuong"]),
                                     tien_thue = Convert.ToDecimal(rw["tien_thue"])
                                 }).ToList();

            return convertedList;
        }
        public static List<ChiPhiKhacDB> GetListChiPhiKhac(DataTable dt)
        {
            var convertedList = (from rw in dt.AsEnumerable()
                                 select new ChiPhiKhacDB()
                                 {
                                     ten = Convert.ToString(rw["ten"]),
                                     ma_chi_phi = Convert.ToString(rw["ma_chi_phi"]),
                                     tien_dx = Convert.ToDecimal(rw["tien_dx"]),
                                     tien_bao_hiem = Convert.ToDecimal(rw["tien_bao_hiem"]),
                                     tien_giam_tru = Convert.ToDecimal(rw["tien_giam_tru"]),
                                     tien_thue = Convert.ToDecimal(rw["tien_thue"]),
                                     so_id_doi_tuong = Convert.ToDecimal(rw["so_id_doi_tuong"])
                                 }).ToList();

            return convertedList;
        }
        public static List<DoiTuongXe> ConvertDataTNDSTAI_SAN(List<DoiTuongXeDB> hangMuc, List<ChiPhiKhacDB> chiPhiKhac)
        {
            List<DoiTuongXe> data = new List<DoiTuongXe>();
            if (hangMuc == null || hangMuc.Count() <= 0)
                return data;
            data = hangMuc.Where(n => n.loai == "XE").Select(n => new DoiTuongXe()
            {
                so_id_doi_tuong = n.so_id_doi_tuong.ToString(),
                ten_dt = n.ten_doi_tuong,
                pt_khao = "0",
                tien_khao = "0",
                pt_bhiem = "0",
                tien_bhiem = "0",
                pt_gtru = "0",
                tien_gtru = "0",
                tl_ggia_vtu = "0",
                tien_ggia_vtu = "0",
                tl_ggia_ncong = "0",
                tien_ggia_ncong = "0",
                tl_ggia_khac = "0",
                tien_ggia_khac = "0",
                tl_tien_ktru_bh = "0",
                tien_ktru_bh = "0",
                tien_mien_thuong = "0",
                tong_tt = "0",
                tong_sc = "0",
                tong_son = "0"
            }).ToList();
            List<DoiTuongXe> dataTmp = new List<DoiTuongXe>();
            if (data != null && data.Count() > 0)
                foreach (var item in data)
                    if (dataTmp.Where(n => n.so_id_doi_tuong == item.so_id_doi_tuong).Count() <= 0)
                        dataTmp.Add(item);
            foreach (var item in dataTmp)
            {

                item.ThayThe = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong
                                                                    && n.tien_vtu_dx != null && n.tien_vtu_dx > 0).SelectWithIndex()
                                                .Select(n => new HangMuc()
                                                {
                                                    index = n.index,
                                                    stt = (n.index + 1).ToString(),
                                                    ten_hm = n.item.ten_hm,
                                                    tien_bgia = n.item.tien_bgia == null ? "0" : n.item.tien_bgia.FormatMoney(),
                                                    tien_dx = n.item.tien_vtu_dx == null ? "0" : n.item.tien_vtu_dx.FormatMoney(),
                                                    pt_khao = n.item.pt_khau_hao == null ? "0" : n.item.pt_khau_hao.ToString(),
                                                    ghi_chu = "",
                                                })
                                                .OrderByDescending(n => n.index)
                                                .ToList();
                var tong_tt = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong
                                                                     && n.tien_vtu_dx != null && n.tien_vtu_dx > 0)
                                       .Sum(n => n.tien_vtu_dx);
                item.tong_tt = tong_tt.FormatMoney();

                item.SuaChua = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong
                                                                    && n.tien_nhan_cong_dx != null && n.tien_nhan_cong_dx > 0).SelectWithIndex()
                                                .Select(n => new HangMuc()
                                                {
                                                    index = n.index,
                                                    stt = (n.index + 1).ToString(),
                                                    ten_hm = n.item.ten_hm,
                                                    tien_bgia = n.item.tien_bgia == null ? "0" : n.item.tien_bgia.FormatMoney(),
                                                    tien_dx = n.item.tien_nhan_cong_dx == null ? "0" : n.item.tien_nhan_cong_dx.FormatMoney(),
                                                    pt_khao = n.item.pt_khau_hao == null ? "0" : n.item.pt_khau_hao.ToString(),
                                                    ghi_chu = "",
                                                })
                                                .OrderByDescending(n => n.index)
                                                .ToList();
                var tong_sc = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong
                                                                    && n.tien_nhan_cong_dx != null && n.tien_nhan_cong_dx > 0)
                                       .Sum(n => n.tien_nhan_cong_dx);
                item.tong_sc = tong_sc.FormatMoney();

                item.Son = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong
                                                                    && n.tien_khac_dx != null && n.tien_khac_dx > 0).SelectWithIndex()
                                                .Select(n => new HangMuc()
                                                {
                                                    index = n.index,
                                                    stt = (n.index + 1).ToString(),
                                                    ten_hm = n.item.ten_hm,
                                                    tien_bgia = n.item.tien_bgia == null ? "0" : n.item.tien_bgia.FormatMoney(),
                                                    tien_dx = n.item.tien_khac_dx == null ? "0" : n.item.tien_khac_dx.FormatMoney(),
                                                    pt_khao = n.item.pt_khau_hao == null ? "0" : n.item.pt_khau_hao.ToString(),
                                                    ghi_chu = "",
                                                })
                                                .OrderByDescending(n => n.index)
                                                .ToList();
                var tong_son = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong
                                                                    && n.tien_khac_dx != null && n.tien_khac_dx > 0)
                                       .Sum(n => n.tien_khac_dx);
                item.tong_son = tong_son.FormatMoney();

                var cp_cau = chiPhiKhac.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong && n.ma_chi_phi == "CP_CAU")
                            .Sum(n => n.tien_dx - n.tien_bao_hiem - n.tien_bao_hiem - n.tien_giam_tru);
                item.cp_cau = cp_cau.FormatMoney();
                var cp_keo = chiPhiKhac.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong && n.ma_chi_phi == "CP_KEO")
                            .Sum(n => n.tien_dx - n.tien_bao_hiem - n.tien_bao_hiem - n.tien_giam_tru);
                item.cp_keo = cp_keo.FormatMoney();
                var cp_khac = chiPhiKhac.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong && n.ma_chi_phi == "KHAC")
                            .Sum(n => n.tien_dx - n.tien_bao_hiem - n.tien_bao_hiem - n.tien_giam_tru);
                item.cp_khac = cp_khac.FormatMoney();
                var tien_thue_cp_khac = chiPhiKhac.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_thue);

                item.pt_khao = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Max(n => n.pt_khau_hao)?.ToString();
                var tien_khao = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_khau_hao);
                item.tien_khao = tien_khao.FormatMoney();
                item.pt_bhiem = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Max(n => n.pt_bao_hiem)?.ToString();
                var tien_bhiem = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_bao_hiem);
                item.tien_bhiem = tien_bhiem.FormatMoney();
                item.pt_gtru = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Max(n => n.pt_giam_tru)?.ToString();
                var tien_gtru = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_giam_tru);
                item.tien_gtru = tien_gtru.FormatMoney();
                item.tl_ggia_vtu = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Max(n => n.tl_giam_gia_vtu)?.ToString();
                var tien_ggia_vtu = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_giam_gia_vtu);
                item.tien_ggia_vtu = tien_ggia_vtu.FormatMoney();
                item.tl_ggia_ncong = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Max(n => n.tl_giam_gia_nhan_cong)?.ToString();
                var tien_ggia_ncong = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_giam_gia_nhan_cong);
                item.tien_ggia_ncong = tien_ggia_ncong.FormatMoney();
                item.tl_ggia_khac = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Max(n => n.tl_giam_gia_khac)?.ToString();
                var tien_ggia_khac = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_giam_gia_khac);
                item.tien_ggia_khac = tien_ggia_khac.FormatMoney();
                item.tl_tien_ktru_bh = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Max(n => n.tl_tien_ktru_tien_bh)?.ToString();
                var tien_ktru_bh = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_ktru_tien_bh);
                item.tien_ktru_bh = tien_ktru_bh.FormatMoney();
                var tien_mien_thuong = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_mien_thuong);
                item.tien_mien_thuong = tien_mien_thuong.FormatMoney();
                var tien_thue = hangMuc.Where(n => n.so_id_doi_tuong.ToString() == item.so_id_doi_tuong).Sum(n => n.tien_thue);

                item.pt_khao = string.IsNullOrEmpty(item.pt_khao) ? "0" : item.pt_khao;
                item.tien_khao = string.IsNullOrEmpty(item.tien_khao) ? "0" : item.tien_khao;
                item.pt_bhiem = string.IsNullOrEmpty(item.pt_bhiem) ? "0" : item.pt_bhiem;
                item.tien_bhiem = string.IsNullOrEmpty(item.tien_bhiem) ? "0" : item.tien_bhiem;
                item.pt_gtru = string.IsNullOrEmpty(item.pt_gtru) ? "0" : item.pt_gtru;
                item.tien_gtru = string.IsNullOrEmpty(item.tien_gtru) ? "0" : item.tien_gtru;
                item.tl_ggia_vtu = string.IsNullOrEmpty(item.tl_ggia_vtu) ? "0" : item.tl_ggia_vtu;
                item.tien_ggia_vtu = string.IsNullOrEmpty(item.tien_ggia_vtu) ? "0" : item.tien_ggia_vtu;
                item.tl_ggia_ncong = string.IsNullOrEmpty(item.tl_ggia_ncong) ? "0" : item.tl_ggia_ncong;
                item.tien_ggia_ncong = string.IsNullOrEmpty(item.tien_ggia_ncong) ? "0" : item.tien_ggia_ncong;
                item.tl_ggia_khac = string.IsNullOrEmpty(item.tl_ggia_khac) ? "0" : item.tl_ggia_khac;
                item.tien_ggia_khac = string.IsNullOrEmpty(item.tien_ggia_khac) ? "0" : item.tien_ggia_khac;
                item.tl_tien_ktru_bh = string.IsNullOrEmpty(item.tl_tien_ktru_bh) ? "0" : item.tl_tien_ktru_bh;
                item.tien_ktru_bh = string.IsNullOrEmpty(item.tien_ktru_bh) ? "0" : item.tien_ktru_bh;
                item.tien_mien_thuong = string.IsNullOrEmpty(item.tien_mien_thuong) ? "0" : item.tien_mien_thuong;

                var tong_dx = (tong_tt + tong_sc + tong_son - tien_khao - tien_bhiem - tien_gtru
                                - tien_ggia_vtu - tien_ggia_ncong - tien_ggia_khac
                                - tien_ktru_bh - tien_mien_thuong);
                item.tong_dx = tong_dx.FormatMoney();
                var tong_thue = tien_thue + tien_thue_cp_khac;
                item.tong_thue = tong_thue.FormatMoney();
                item.tong_cong = (tong_dx + tong_thue).FormatMoney();
            }
            return dataTmp;
        }
        private static Drawing GetImageDrawing(string relationshipId, ImageDetails image)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = image.cx, Cy = image.cy },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = image.cx, Cy = image.cy }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            return element;
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName]?.ToString(), null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
    public class AnHienNghiepVu
    {
        public string nhom { get; set; }
        public string hien_thi { get; set; }
    }
}
