using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ESCS.COMMON.Common
{
    public class CExWordMLFiller
    {
        private DataSet dsData;
        private string templateContent;
        private XmlDocument xmlTemplateDoc;
        private XmlDocument xmlDataset;
        private XmlNamespaceManager nsmgr = new XmlNamespaceManager((XmlNameTable)new NameTable());
        private ArrayList errorList = new ArrayList();
        private bool bOperationFailed = false;
        private static string wordmlPrefix = "http://schemas.microsoft.com/office/word/2003/wordml";
        private static string amlPrefix = "http://schemas.microsoft.com/aml/2001/core";
        private static string xsPrefix = "http://www.w3.org/2001/XMLSchema";
        private const string repeatAttribute = "WMLRepeat";
        private string languageName;
        private CultureInfo ci;
        private string dateTimeFormat;
        private NumberFormatInfo numberFormat;
        private string numberDecimalSeparator;
        private string numberGroupSeparator;
        private int numberDecimalDigits;
        public List<ImageSource> imageSources;
        public CExWordMLFiller(DataSet dsData, string templateContent)
        {
            this.dsData = dsData;
            if (this.dsData.Tables.Contains("curs_source"))
            {
                DataTable table = this.dsData.Tables["curs_source"];
                this.imageSources = ConvertDataTable<ImageSource>(table);
            }
            this.xmlDataset = new XmlDocument();
            MemoryStream memoryStream = new MemoryStream();
            dsData.WriteXml((Stream)memoryStream, XmlWriteMode.WriteSchema);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            this.xmlDataset.Load((XmlReader)new XmlTextReader((Stream)memoryStream));
            this.templateContent = templateContent;
            this.LoadTemplate();
        }
        private void LoadTemplate()
        {
            this.errorList = new ArrayList();
            this.bOperationFailed = false;
            try
            {
                this.xmlTemplateDoc = new XmlDocument();
                this.xmlTemplateDoc.LoadXml(this.templateContent);
                this.nsmgr.AddNamespace("w", CExWordMLFiller.wordmlPrefix);
                this.nsmgr.AddNamespace("aml", CExWordMLFiller.amlPrefix);
                XmlNode xmlNode1 = this.xmlTemplateDoc.SelectSingleNode("//w:docVar[@w:name='LanguageName']/@w:val", this.nsmgr);
                if (xmlNode1 != null)
                {
                    this.languageName = xmlNode1.Value;
                    try
                    {
                        this.ci = new CultureInfo(this.languageName, false);
                        this.numberFormat = this.ci.NumberFormat;
                    }
                    catch
                    {
                        this.languageName = (string)null;
                        this.numberFormat = (NumberFormatInfo)null;
                    }
                }
                else
                    this.languageName = (string)null;
                XmlNode xmlNode2 = this.xmlTemplateDoc.SelectSingleNode("//w:docVar[@w:name='DateTimeFormat']/@w:val", this.nsmgr);
                this.dateTimeFormat = xmlNode2 == null ? "dd/MM/yyyy" : xmlNode2.Value;
                XmlNode xmlNode3 = this.xmlTemplateDoc.SelectSingleNode("//w:docVar[@w:name='NumberDecimalSeparator']/@w:val", this.nsmgr);
                if (xmlNode3 != null)
                {
                    this.numberDecimalSeparator = xmlNode3.Value;
                    if (this.numberFormat == null)
                        this.numberFormat = new NumberFormatInfo();
                    this.numberFormat.NumberDecimalSeparator = this.numberDecimalSeparator.Trim() == string.Empty ? "." : this.numberDecimalSeparator;
                }
                else
                    this.numberDecimalSeparator = ".";
                XmlNode xmlNode4 = this.xmlTemplateDoc.SelectSingleNode("//w:docVar[@w:name='NumberGroupSeparator']/@w:val", this.nsmgr);
                if (xmlNode4 != null)
                {
                    this.numberGroupSeparator = xmlNode4.Value;
                    if (this.numberFormat == null)
                        this.numberFormat = new NumberFormatInfo();
                    this.numberFormat.NumberGroupSeparator = this.numberGroupSeparator.Trim() == string.Empty ? "," : this.numberGroupSeparator;
                }
                else
                    this.numberGroupSeparator = ",";
                XmlNode xmlNode5 = this.xmlTemplateDoc.SelectSingleNode("//w:docVar[@w:name='NumberDecimalDigits']/@w:val", this.nsmgr);
                if (xmlNode5 != null)
                {
                    try
                    {
                        this.numberDecimalDigits = int.Parse(xmlNode5.Value);
                    }
                    catch
                    {
                        this.numberDecimalDigits = -1;
                    }
                    if (this.numberFormat == null)
                        this.numberFormat = new NumberFormatInfo();
                    this.numberFormat.NumberDecimalDigits = this.numberDecimalDigits == -1 ? 2 : this.numberDecimalDigits;
                }
                else
                    this.numberDecimalDigits = 0;
            }
            catch (Exception ex)
            {
                for (Exception exception = ex; exception != null; exception = exception.InnerException)
                    this.errorList.Add((object)exception.Message);
                this.bOperationFailed = true;
            }
        }
        public bool OperationFailed => this.bOperationFailed;
        public XmlDocument WordMLDocument => this.xmlTemplateDoc;
        public ArrayList ErrorList => this.errorList;
        private void RemoveMailMergeNode()
        {
            this.errorList = new ArrayList();
            this.bOperationFailed = false;
            XmlNode oldChild = this.xmlTemplateDoc.SelectSingleNode("//w:mailMerge", this.nsmgr);
            if (oldChild == null)
            {
                this.errorList.Add((object)"Invalid mail merge node!");
                this.bOperationFailed = true;
            }
            else
            {
                XmlNode parentNode = oldChild.ParentNode;
                if (parentNode == null)
                {
                    this.errorList.Add((object)"Invalid mail merge parent node!");
                    this.bOperationFailed = true;
                }
                else
                    parentNode.RemoveChild(oldChild);
            }
        }
        private void RemoveUnfilledFields()
        {
            this.errorList = new ArrayList();
            this.bOperationFailed = false;
            this.nsmgr.AddNamespace("xs", CExWordMLFiller.xsPrefix);
            XmlNodeList xmlNodeList = this.xmlDataset.SelectNodes("//xs:element", this.nsmgr);
            if (xmlNodeList == null || xmlNodeList.Count == 0)
                return;
            foreach (XmlElement xmlElement in xmlNodeList)
                this.ReplaceFieldData((XmlNode)this.xmlTemplateDoc.DocumentElement, xmlElement.GetAttribute("name"), string.Empty, Type.Missing.GetType());
        }
        private void ReplaceFieldData(XmlNode baseNode, string fieldName, string data, Type colType)
        {
            this.errorList = new ArrayList();
            this.bOperationFailed = false;
            XmlNodeList xmlNodeList1 = baseNode.SelectNodes("//w:fldSimple[@w:instr=' MERGEFIELD  " + fieldName + " ']", this.nsmgr);
            if (xmlNodeList1 == null || xmlNodeList1.Count == 0)
                xmlNodeList1 = baseNode.SelectNodes("//w:p[w:r/w:instrText=' MERGEFIELD  " + fieldName + " ']", this.nsmgr);
            IEnumerator enumerator = xmlNodeList1.GetEnumerator();
            try
            {
            label_23:
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode)enumerator.Current;
                    XmlNode xmlNode = current.SelectSingleNode("//w:t[.='«" + fieldName + "»']", this.nsmgr);
                    if (!(fieldName == "BPIC")) ;
                    if (xmlNode == null)
                    {
                        this.errorList.Add((object)"The field data is selected from the fields definition data source or merge document is corrupted!");
                        this.bOperationFailed = true;
                        break;
                    }
                    if (colType == typeof(DateTime))
                    {
                        if (this.dateTimeFormat != null)
                        {
                            DateTime dateTime = DateTime.Parse(data);
                            xmlNode.InnerText = dateTime.ToString(this.dateTimeFormat);
                        }
                        else
                            xmlNode.InnerText = data;
                    }
                    else
                        xmlNode.InnerText = data;
                    XmlNodeList xmlNodeList2 = current.SelectNodes("w:r[w:fldChar]", this.nsmgr);
                    if (xmlNodeList2 != null && xmlNodeList2.Count > 0)
                    {
                        for (int i = xmlNodeList2.Count - 1; i >= 0; --i)
                            xmlNodeList2[i].ParentNode.RemoveChild(xmlNodeList2[i]);
                    }
                    while (true)
                    {
                        XmlNode oldChild = current.SelectSingleNode("w:r[w:instrText=' MERGEFIELD  " + fieldName + " ']", this.nsmgr);
                        if (oldChild != null)
                            oldChild.ParentNode.RemoveChild(oldChild);
                        else
                            goto label_23;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                    disposable.Dispose();
            }
        }
        public void Transform()
        {
            foreach (DataTable table in (InternalDataCollectionBase)this.dsData.Tables)
            {
                if (table.TableName.ToLower() == "curs_an_hien" || table.TableName.ToLower() == "curs_source")
                    continue;
                this.TransformDataTable(table);
            }
            //if (this.dsData.Tables.Contains("curs_an_hien"))
            //{
            //    DataTable table = this.dsData.Tables["curs_an_hien"];
            //    if (table.Rows != null && table.Rows.Count > 0)
            //    {
            //        foreach (DataRow row in table.Rows)
            //        {
            //            if (row["hien_thi"].ToString() == "0")
            //            {
            //                this.TransformWordMLNhomShowHide(row["nhom"].ToString());
            //            }
            //        }
            //    }
            //}
            this.RemoveMailMergeNode();
            this.RemoveUnfilledFields();
        }
        private void TransformDataTable(DataTable dt)
        {
            this.TransformWordMLTableRepeat(dt);
            this.TransformWordMLElementRepeat(dt);
            this.TransformNoRepeat(dt);
        }
        private void TransformWordMLTableRepeat(DataTable dt)
        {
            string tableName = dt.TableName;
            XmlNodeList xmlNodeList = this.xmlTemplateDoc.SelectNodes("//w:tbl[contains(descendant::aml:annotation/@w:name, '" + tableName + "WMLRepeat')]", this.nsmgr);
            if (xmlNodeList == null || xmlNodeList.Count <= 0)
                return;
            foreach (XmlNode tableNode in xmlNodeList)
            {
                XmlNode xmlNode = tableNode.SelectSingleNode("w:tr[contains(descendant::w:instrText, ' MERGEFIELD  \"" + tableName + "') or contains(descendant::w:instrText, ' MERGEFIELD  " + tableName + "')]", this.nsmgr);
                if (xmlNode != null && xmlNode.InnerXml.IndexOf("w:tbl") <= -1)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dt.Rows)
                        this.TransformDataRow(row, tableNode, xmlNode);
                    tableNode.RemoveChild(xmlNode);
                }
            }
        }
        private void TransformWordMLNhomShowHide(string nhomAn)
        {
            XmlNodeList xmlNodeList = this.xmlTemplateDoc.SelectNodes("//w:tbl[contains(descendant::aml:annotation/@w:name, '" + nhomAn + "WMLShowHide')]", this.nsmgr);
            if (xmlNodeList == null || xmlNodeList.Count <= 0)
                return;
            foreach (XmlNode tableNode in xmlNodeList)
            {
                tableNode.RemoveAll();
            }
        }
        private void TransformWordMLElementRepeat(DataTable dt)
        {
            string tableName = dt.TableName;
            XmlNodeList xmlNodeList = this.xmlTemplateDoc.SelectNodes("//w:p[contains(descendant::aml:annotation/@w:name, '" + tableName + "') and (contains(descendant::w:instrText, ' MERGEFIELD \"" + tableName + "') or contains(descendant::w:instrText, ' MERGEFIELD " + tableName + "'))]", this.nsmgr);
            if (xmlNodeList == null || xmlNodeList.Count <= 0)
                return;
            foreach (XmlNode baseNode in xmlNodeList)
            {
                foreach (DataRow row in (InternalDataCollectionBase)dt.Rows)
                    this.TransformDataRow(row, baseNode);
            }
        }
        private void TransformNoRepeat(DataTable dt)
        {
            if (dt.Rows.Count <= 0)
                return;
            DataRow row = dt.Rows[0];
            for (int index = 0; index < row.ItemArray.Length; ++index)
                this.ReplaceFieldData((XmlNode)this.xmlTemplateDoc.DocumentElement, dt.TableName + dt.Columns[index].ColumnName.ToUpper(), row[index].ToString(), dt.Columns[index].DataType);
        }
        private void TransformDataRow(DataRow dr, XmlNode tableNode, XmlNode templateRowNode)
        {
            DataTable table = dr.Table;
            string tableName = table.TableName;
            XmlNode xmlNode = templateRowNode.CloneNode(true);
            for (int index = 0; index < dr.ItemArray.Length; ++index)
            {
                string fieldName = table.TableName + table.Columns[index].ColumnName.ToUpper();
                this.ReplaceFieldData(xmlNode, fieldName, dr[index].ToString(), table.Columns[index].DataType);
            }
            tableNode.InsertBefore(xmlNode, templateRowNode);
        }
        private void TransformDataRow(DataRow dr, XmlNode baseNode)
        {
            DataTable table = dr.Table;
            string tableName = table.TableName;
            XmlNode parentNode = baseNode.ParentNode;
            if (parentNode == null)
                return;
            XmlNode xmlNode = baseNode.CloneNode(true);
            for (int index = 0; index < dr.ItemArray.Length; ++index)
            {
                string fieldName = table.TableName + table.Columns[index].ColumnName.ToUpper();
                this.ReplaceFieldData(xmlNode, fieldName, dr[index].ToString(), table.Columns[index].DataType);
            }
            parentNode.InsertBefore(xmlNode, baseNode);
            parentNode.RemoveChild(baseNode);
        }
        public static string GetDataStructure(DataSet ds)
        {
            string str1 = string.Empty;
            string str2 = string.Empty;
            foreach (DataTable table in (InternalDataCollectionBase)ds.Tables)
            {
                for (int index = 0; index < table.Columns.Count; ++index)
                {
                    if (str1 == string.Empty)
                    {
                        str1 = str1 + table.TableName + table.Columns[index].ColumnName.ToUpper();
                        str2 += " ";
                    }
                    else
                    {
                        str1 = str1 + "\t" + table.TableName + table.Columns[index].ColumnName;
                        str2 = "\t ";
                    }
                }
            }
            return str1 + Environment.NewLine + str2;
        }
        public static XElement ImageToXElement(string strBase64Image)
        {
            return new XElement("Image", new XAttribute("PixelData", strBase64Image));
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            if (dt==null)
                return null;
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
                        pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value?null: dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
    public class ImageSource
    {
        public string psource { get; set; }
        public string source { get; set; }
        public double? width_px { get; set; }
        public double? height_px { get; set; }
    }
}
