using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

using ESCS.Models.TCT;


namespace ESCS.Models
{
    public class EBill
    {
        public string TemplateCode { get; set; }
        public string InvoiceSeries { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceIssuedDate { get; set; }
        public string SignedDate { get; set; }
        public string CurrencyCode { get; set; }
        public string InvoiceNote { get; set; }
        public string SellerLegalName { get; set; }
        public string SellerTaxCode { get; set; }
        public string SellerAddressLine { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string SellerEmail { get; set; }
        public string SellerBankName { get; set; }
        public string SellerBankAccount { get; set; }
        public string SellerContactPersonName { get; set; }

        public string BuyerDisplayName { get; set; }
        public string BuyerLegalName { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerTaxCode { get; set; }

        public string BuyerAddressLine { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerBankAccount { get; set; }
        public Items Items { get; set; }
        public string TotalAmountWithoutVAT { get; set; }
        public string TotalVATAmount { get; set; }
        public string TotalAmountWithVAT { get; set; }
        public string TotalAmountWithVATInWords { get; set; }
        public string DiscountAmount { get; set; }
        public string ItemName { get; set; }
        public string VatPercentage { get; set; }
        public bool IsEntirety { get; set; }
        public bool IsSigned { get; set; }
        public void ChuanHoaDuLieu()
        {
            if (!string.IsNullOrEmpty(this.TotalAmountWithoutVAT))
            {
                this.TotalAmountWithoutVAT = this.TotalAmountWithoutVAT.Replace(",", "");
                var arr = this.TotalAmountWithoutVAT.Split(".");
                if (arr.Length == 2 && decimal.TryParse(arr[1], out decimal result) && result==0)
                    this.TotalAmountWithoutVAT = arr[0].ToString();
            }
            if (!string.IsNullOrEmpty(this.TotalVATAmount))
            {
                this.TotalVATAmount = this.TotalVATAmount.Replace(",", "");
                var arr = this.TotalVATAmount.Split(".");
                if (arr.Length == 2 && decimal.TryParse(arr[1], out decimal result) && result == 0)
                    this.TotalVATAmount = arr[0].ToString();
            }
            if (!string.IsNullOrEmpty(this.TotalAmountWithVAT))
            {
                this.TotalAmountWithVAT = this.TotalAmountWithVAT.Replace(",", "");
                var arr = this.TotalAmountWithVAT.Split(".");
                if (arr.Length == 2 && decimal.TryParse(arr[1], out decimal result) && result == 0)
                    this.TotalAmountWithVAT = arr[0].ToString();
            }
            if (this.Items!=null && Items.Item !=null && Items.Item.Count()>0)
            {
                foreach (var item in Items.Item)
                {
                    if (!string.IsNullOrEmpty(item.ItemTotalAmountWithoutVat))
                    {
                        item.ItemTotalAmountWithoutVat = item.ItemTotalAmountWithoutVat.Replace(",", "");
                        var arr = item.ItemTotalAmountWithoutVat.Split(".");
                        if (arr.Length == 2 && decimal.TryParse(arr[1], out decimal result) && result == 0)
                            item.ItemTotalAmountWithoutVat = arr[0].ToString();
                    }
                    if (!string.IsNullOrEmpty(item.ItemTotalAmountWithVat))
                    {
                        item.ItemTotalAmountWithVat = item.ItemTotalAmountWithVat.Replace(",", "");
                        var arr = item.ItemTotalAmountWithVat.Split(".");
                        if (arr.Length == 2 && decimal.TryParse(arr[1], out decimal result) && result == 0)
                            item.ItemTotalAmountWithVat = arr[0].ToString();
                    }
                    if (!string.IsNullOrEmpty(item.VatAmount))
                    {
                        item.VatAmount = item.VatAmount.Replace(",", "");
                        var arr = item.VatAmount.Split(".");
                        if (arr.Length == 2 && decimal.TryParse(arr[1], out decimal result) && result == 0)
                            item.VatAmount = arr[0].ToString();
                    }
                    if (!string.IsNullOrEmpty(item.ProdPrice))
                    {
                        item.ProdPrice = item.ProdPrice.Replace(",", "");
                        var arr = item.ProdPrice.Split(".");
                        if (arr.Length == 2 && decimal.TryParse(arr[1], out decimal result) && result == 0)
                            item.ProdPrice = arr[0].ToString();
                    }
                    if (!string.IsNullOrEmpty(item.ProdQuantity))
                    {
                        item.ProdQuantity = item.ProdQuantity.Replace(",", "");
                        var arr = item.ProdQuantity.Split(".");
                        if (arr.Length == 2 && decimal.TryParse(arr[1], out decimal result) && result == 0)
                            item.ProdQuantity = arr[0].ToString();
                    }
                }
            }
        }
    }

    public class ReadEBill
    {
        public string EBillXmlContent { get; set; }
    }
}

namespace ESCS.Models.VNPT
{
    #region "Bill VNPT"

    [XmlRoot(ElementName = "Invoice")]
    public class Invoice
    {
        [XmlElement(ElementName = "Content")]
        public Content Content { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

    [XmlRoot(ElementName = "Content")]
    public class Content
    {
        [XmlElement(ElementName = "key")]
        public string Key { get; set; }
        [XmlElement(ElementName = "Extra")]
        public string Extra { get; set; }
        [XmlElement(ElementName = "InvoiceName")]
        public string InvoiceName { get; set; }
        [XmlElement(ElementName = "InvoicePattern")]
        public string InvoicePattern { get; set; }
        [XmlElement(ElementName = "SerialNo")]
        public string SerialNo { get; set; }
        [XmlElement(ElementName = "InvoiceNo")]
        public string InvoiceNo { get; set; }
        [XmlElement(ElementName = "ArisingDate")]
        public string ArisingDate { get; set; }
        [XmlElement(ElementName = "Kind_of_Payment")]
        public string Kind_of_Payment { get; set; }
        [XmlElement(ElementName = "ComName")]
        public string ComName { get; set; }
        [XmlElement(ElementName = "ComTaxCode")]
        public string ComTaxCode { get; set; }
        [XmlElement(ElementName = "ComAddress")]
        public string ComAddress { get; set; }
        [XmlElement(ElementName = "ComPhone")]
        public string ComPhone { get; set; }
        [XmlElement(ElementName = "ComEmail")]
        public string ComEmail { get; set; }
        [XmlElement(ElementName = "ComBankNo")]
        public string ComBankNo { get; set; }
        [XmlElement(ElementName = "ComBankName")]
        public string ComBankName { get; set; }
        [XmlElement(ElementName = "CusCode")]
        public string CusCode { get; set; }
        [XmlElement(ElementName = "CusName")]
        public string CusName { get; set; }
        [XmlElement(ElementName = "Buyer")]
        public string Buyer { get; set; }
        [XmlElement(ElementName = "CusAddress")]
        public string CusAddress { get; set; }
        [XmlElement(ElementName = "CusPhone")]
        public string CusPhone { get; set; }
        [XmlElement(ElementName = "CusBankNo")]
        public string CusBankNo { get; set; }
        [XmlElement(ElementName = "CusTaxCode")]
        public string CusTaxCode { get; set; }
        [XmlElement(ElementName = "PaymentMethod")]
        public string PaymentMethod { get; set; }
        [XmlElement(ElementName = "RoomNo")]
        public string RoomNo { get; set; }
        [XmlElement(ElementName = "FolioNo")]
        public string FolioNo { get; set; }
        [XmlElement(ElementName = "Arrival")]
        public string Arrival { get; set; }
        [XmlElement(ElementName = "Departure")]
        public string Departure { get; set; }
        [XmlElement(ElementName = "QuestQuantity")]
        public string QuestQuantity { get; set; }
        [XmlElement(ElementName = "Rate")]
        public string Rate { get; set; }
        [XmlElement(ElementName = "Publisher")]
        public string Publisher { get; set; }
        [XmlElement(ElementName = "BalanceTotal")]
        public string BalanceTotal { get; set; }
        [XmlElement(ElementName = "ServicechargeAmount")]
        public string ServicechargeAmount { get; set; }
        [XmlElement(ElementName = "GrossValue")]
        public string GrossValue { get; set; }
        [XmlElement(ElementName = "VatAmount5")]
        public string VatAmount5 { get; set; }
        [XmlElement(ElementName = "GrossValue5")]
        public string GrossValue5 { get; set; }
        [XmlElement(ElementName = "VatAmount10")]
        public string VatAmount10 { get; set; }
        [XmlElement(ElementName = "GrossValue10")]
        public string GrossValue10 { get; set; }
        [XmlElement(ElementName = "VatAmount30")]
        public string VatAmount30 { get; set; }
        [XmlElement(ElementName = "GrossValue30")]
        public string GrossValue30 { get; set; }
        [XmlElement(ElementName = "EquivalentUSD")]
        public string EquivalentUSD { get; set; }
        [XmlElement(ElementName = "Cashier")]
        public string Cashier { get; set; }
        [XmlElement(ElementName = "GuestID")]
        public string GuestID { get; set; }
        [XmlElement(ElementName = "ServicechargeRate")]
        public string ServicechargeRate { get; set; }
        [XmlElement(ElementName = "LuxuryTaxRate")]
        public string LuxuryTaxRate { get; set; }
        [XmlElement(ElementName = "AmountUSD")]
        public string AmountUSD { get; set; }
        [XmlElement(ElementName = "Exchange_Rate")]
        public string Exchange_Rate { get; set; }
        [XmlElement(ElementName = "Extra1")]
        public string Extra1 { get; set; }
        [XmlElement(ElementName = "Extra2")]
        public string Extra2 { get; set; }
        [XmlElement(ElementName = "Extra3")]
        public string Extra3 { get; set; }
        [XmlElement(ElementName = "Extra4")]
        public string Extra4 { get; set; }
        [XmlElement(ElementName = "OpenCheck")]
        public string OpenCheck { get; set; }
        [XmlElement(ElementName = "CloseCheck")]
        public string CloseCheck { get; set; }
        [XmlElement(ElementName = "FoodTotal")]
        public string FoodTotal { get; set; }
        [XmlElement(ElementName = "BevTotal")]
        public string BevTotal { get; set; }
        [XmlElement(ElementName = "NonAlcoholic")]
        public string NonAlcoholic { get; set; }
        [XmlElement(ElementName = "AmountInWordsEng")]
        public string AmountInWordsEng { get; set; }
        [XmlElement(ElementName = "Discount")]
        public string Discount { get; set; }
        [XmlElement(ElementName = "KindOfService")]
        public string KindOfService { get; set; }
        [XmlElement(ElementName = "Products")]
        public Products Products { get; set; }
        [XmlElement(ElementName = "Groups")]
        public Groups Groups { get; set; }
        [XmlElement(ElementName = "Total")]
        public string Total { get; set; }
        [XmlElement(ElementName = "VATRate")]
        public string VATRate { get; set; }
        [XmlElement(ElementName = "VAT_Rate")]
        public string VAT_Rate { get; set; }
        [XmlElement(ElementName = "VATAmount")]
        public string VATAmount { get; set; }
        [XmlElement(ElementName = "VAT_Amount")]
        public string VAT_Amount { get; set; }
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "AmountInWords")]
        public string AmountInWords { get; set; }

        [XmlElement(ElementName = "Amount_words")]
        public string Amount_words { get; set; }


        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "CusCountry")]
        public string CusCountry { get; set; }
        [XmlElement(ElementName = "ARNumber")]
        public string ARNumber { get; set; }
        [XmlElement(ElementName = "CheckNo")]
        public string CheckNo { get; set; }
        [XmlElement(ElementName = "TableNo")]
        public string TableNo { get; set; }
        [XmlElement(ElementName = "ResourceCode")]
        public string ResourceCode { get; set; }
        [XmlElement(ElementName = "SignDate")]
        public string SignDate { get; set; }
        [XmlElement(ElementName = "CurrencyUnit")]
        public string CurrencyUnit { get; set; }

        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Products")]
    public class Products
    {
        [XmlElement(ElementName = "Product")]
        public List<Product> Product { get; set; }
    }

    [XmlRoot(ElementName = "Product")]
    public class Product
    {
        [XmlElement(ElementName = "Code")]
        public string Code { get; set; }
        [XmlElement(ElementName = "ProdName")]
        public string ProdName { get; set; }
        [XmlElement(ElementName = "ProdPrice")]
        public string ProdPrice { get; set; }
        [XmlElement(ElementName = "ProdQuantity")]
        public string ProdQuantity { get; set; }
        [XmlElement(ElementName = "ProdUnit")]
        public string ProdUnit { get; set; }
        [XmlElement(ElementName = "Total")]
        public string Total { get; set; }
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "VATAmount")]
        public string VATAmount { get; set; }
        [XmlElement(ElementName = "VATRate")]
        public string VATRate { get; set; }
        [XmlElement(ElementName = "Discount")]
        public string Discount { get; set; }
        [XmlElement(ElementName = "DiscountAmount")]
        public string DiscountAmount { get; set; }
        [XmlElement(ElementName = "Extra1")]
        public string Extra1 { get; set; }
        [XmlElement(ElementName = "Extra2")]
        public string Extra2 { get; set; }

        //
    }

    [XmlRoot(ElementName = "Groups")]
    public class Groups
    {
        [XmlElement(ElementName = "Group")]
        public List<Group> Group { get; set; }
    }

    [XmlRoot(ElementName = "Group")]
    public class Group
    {
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "Note")]
        public string Note { get; set; }
        [XmlElement(ElementName = "ModelCarName")]
        public string ModelCarName { get; set; }
        [XmlElement(ElementName = "NumberPlate")]
        public string NumberPlate { get; set; }
        [XmlElement(ElementName = "DocNo_RO")]
        public string DocNo_RO { get; set; }
        [XmlElement(ElementName = "Products")]
        public Products Products { get; set; }
        //

        [XmlElement(ElementName = "ProdPrice")]
        public string ProdPrice { get; set; }
        [XmlElement(ElementName = "ProdQuantity")]
        public string ProdQuantity { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    #endregion

}

namespace ESCS.Models.VNPT1
{
    [XmlRoot(ElementName = "Product")]
    public class Product
    {
        [XmlElement(ElementName = "Code")]
        public string Code { get; set; }
        [XmlElement(ElementName = "Extra")]
        public string Extra { get; set; }
        [XmlElement(ElementName = "Remark")]
        public string Remark { get; set; }
        [XmlElement(ElementName = "Total")]
        public string Total { get; set; }
        [XmlElement(ElementName = "ProdName")]
        public string ProdName { get; set; }
        [XmlElement(ElementName = "ProdUnit")]
        public string ProdUnit { get; set; }
        [XmlElement(ElementName = "ProdQuantity")]
        public string ProdQuantity { get; set; }
        [XmlElement(ElementName = "ProdPrice")]
        public string ProdPrice { get; set; }
        [XmlElement(ElementName = "Discount")]
        public string Discount { get; set; }
        [XmlElement(ElementName = "DiscountAmount")]
        public string DiscountAmount { get; set; }
        [XmlElement(ElementName = "VATRate")]
        public string VATRate { get; set; }
        [XmlElement(ElementName = "VATAmount")]
        public string VATAmount { get; set; }
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "BuiltinOrder")]
        public string BuiltinOrder { get; set; }
    }

    [XmlRoot(ElementName = "Products")]
    public class Products
    {
        [XmlElement(ElementName = "Product")]
        public List<Product> Product { get; set; }
    }

    [XmlRoot(ElementName = "Group")]
    public class Group
    {
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "Note")]
        public string Note { get; set; }
        [XmlElement(ElementName = "ModelCarName")]
        public string ModelCarName { get; set; }
        [XmlElement(ElementName = "NumberPlate")]
        public string NumberPlate { get; set; }
        [XmlElement(ElementName = "DocNo_RO")]
        public string DocNo_RO { get; set; }
        [XmlElement(ElementName = "Products")]
        public Products Products { get; set; }
    }

    [XmlRoot(ElementName = "Groups")]
    public class Groups
    {
        [XmlElement(ElementName = "Group")]
        public Group Group { get; set; }
    }

    [XmlRoot(ElementName = "Content")]
    public class Content
    {
        [XmlElement(ElementName = "ArisingDate")]
        public string ArisingDate { get; set; }
        [XmlElement(ElementName = "ComFax")]
        public string ComFax { get; set; }
        [XmlElement(ElementName = "Note")]
        public string Note { get; set; }
        [XmlElement(ElementName = "ParentName")]
        public string ParentName { get; set; }
        [XmlElement(ElementName = "InvoiceName")]
        public string InvoiceName { get; set; }
        [XmlElement(ElementName = "InvoicePattern")]
        public string InvoicePattern { get; set; }
        [XmlElement(ElementName = "SerialNo")]
        public string SerialNo { get; set; }
        [XmlElement(ElementName = "InvoiceNo")]
        public string InvoiceNo { get; set; }
        [XmlElement(ElementName = "Kind_of_Payment")]
        public string Kind_of_Payment { get; set; }
        [XmlElement(ElementName = "ComName")]
        public string ComName { get; set; }
        [XmlElement(ElementName = "ComTaxCode")]
        public string ComTaxCode { get; set; }
        [XmlElement(ElementName = "ComAddress")]
        public string ComAddress { get; set; }
        [XmlElement(ElementName = "ComPhone")]
        public string ComPhone { get; set; }
        [XmlElement(ElementName = "ComBankNo")]
        public string ComBankNo { get; set; }
        [XmlElement(ElementName = "ComBankName")]
        public string ComBankName { get; set; }
        [XmlElement(ElementName = "Buyer")]
        public string Buyer { get; set; }
        [XmlElement(ElementName = "CusCode")]
        public string CusCode { get; set; }
        [XmlElement(ElementName = "CusName")]
        public string CusName { get; set; }
        [XmlElement(ElementName = "CusTaxCode")]
        public string CusTaxCode { get; set; }
        [XmlElement(ElementName = "CusPhone")]
        public string CusPhone { get; set; }
        [XmlElement(ElementName = "CusAddress")]
        public string CusAddress { get; set; }
        [XmlElement(ElementName = "CusBankName")]
        public string CusBankName { get; set; }
        [XmlElement(ElementName = "CusBankNo")]
        public string CusBankNo { get; set; }
        [XmlElement(ElementName = "Total")]
        public string Total { get; set; }
        [XmlElement(ElementName = "VAT_Amount")]
        public string VAT_Amount { get; set; }
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "Amount_words")]
        public string Amount_words { get; set; }
        [XmlElement(ElementName = "GrossValue")]
        public string GrossValue { get; set; }
        [XmlElement(ElementName = "VatAmount5")]
        public List<string> VatAmount5 { get; set; }
        [XmlElement(ElementName = "GrossValue5")]
        public List<string> GrossValue5 { get; set; }
        [XmlElement(ElementName = "ComBranchName")]
        public string ComBranchName { get; set; }
        [XmlElement(ElementName = "ComEngName")]
        public string ComEngName { get; set; }
        [XmlElement(ElementName = "InvoiceType")]
        public string InvoiceType { get; set; }
        [XmlElement(ElementName = "PrintType")]
        public string PrintType { get; set; }
        [XmlElement(ElementName = "VAT_Rate")]
        public string VAT_Rate { get; set; }
        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }
        [XmlElement(ElementName = "Rate")]
        public string Rate { get; set; }
        [XmlElement(ElementName = "SearchKey")]
        public string SearchKey { get; set; }
        [XmlElement(ElementName = "Groups")]
        public Groups Groups { get; set; }
        [XmlElement(ElementName = "SignDate")]
        public string SignDate { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "Invoice")]
    public class Invoice
    {
        [XmlElement(ElementName = "Content")]
        public Content Content { get; set; }
        [XmlElement(ElementName = "qrCodeData")]
        public string QrCodeData { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

}


namespace ESCS.Models.VNPT2
{
    [XmlRoot(ElementName = "Product")]
    public class Product
    {
        [XmlElement(ElementName = "LineNumber")]
        public string LineNumber { get; set; }
        [XmlElement(ElementName = "LinePrint")]
        public string LinePrint { get; set; }
        [XmlElement(ElementName = "ItemCode")]
        public string ItemCode { get; set; }
        [XmlElement(ElementName = "ItemName")]
        public string ItemName { get; set; }
        [XmlElement(ElementName = "UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }
        [XmlElement(ElementName = "IsPromotion")]
        public string IsPromotion { get; set; }
        [XmlElement(ElementName = "Quantity")]
        public string Quantity { get; set; }
        [XmlElement(ElementName = "Price")]
        public string Price { get; set; }
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "DiscountRate")]
        public string DiscountRate { get; set; }
        [XmlElement(ElementName = "DiscountAmount")]
        public string DiscountAmount { get; set; }
        [XmlElement(ElementName = "TaxRate")]
        public string TaxRate { get; set; }
        [XmlElement(ElementName = "TaxAmount")]
        public string TaxAmount { get; set; }
        [XmlElement(ElementName = "Note")]
        public string Note { get; set; }
        [XmlElement(ElementName = "External1")]
        public string External1 { get; set; }
        [XmlElement(ElementName = "External2")]
        public string External2 { get; set; }
        [XmlElement(ElementName = "External3")]
        public string External3 { get; set; }
        [XmlElement(ElementName = "NumberExternal1")]
        public string NumberExternal1 { get; set; }
        [XmlElement(ElementName = "NumberExternal2")]
        public string NumberExternal2 { get; set; }
        [XmlElement(ElementName = "NumberExternal3")]
        public string NumberExternal3 { get; set; }

        [XmlElement(ElementName = "ProdPrice")]
        public string ProdPrice { get; set; }
        [XmlElement(ElementName = "ProdQuantity")]
        public string ProdQuantity { get; set; }
    }

    [XmlRoot(ElementName = "Products")]
    public class Products
    {
        [XmlElement(ElementName = "Product")]
        public List<Product> Product { get; set; }
    }

    [XmlRoot(ElementName = "Content")]
    public class Content
    {
        [XmlElement(ElementName = "InvoiceName")]
        public string InvoiceName { get; set; }
        [XmlElement(ElementName = "InvoiceType")]
        public string InvoiceType { get; set; }
        [XmlElement(ElementName = "InvoicePattern")]
        public string InvoicePattern { get; set; }
        [XmlElement(ElementName = "InvoiceSerial")]
        public string InvoiceSerial { get; set; }
        [XmlElement(ElementName = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }
        [XmlElement(ElementName = "InvoiceDate")]
        public string InvoiceDate { get; set; }
        [XmlElement(ElementName = "SignedDate")]
        public string SignedDate { get; set; }
        [XmlElement(ElementName = "PaymentMethod")]
        public string PaymentMethod { get; set; }
        [XmlElement(ElementName = "CompanyName")]
        public string CompanyName { get; set; }
        [XmlElement(ElementName = "CompanyTaxCode")]
        public string CompanyTaxCode { get; set; }
        [XmlElement(ElementName = "CompanyAddress")]
        public string CompanyAddress { get; set; }
        [XmlElement(ElementName = "CompanyPhone")]
        public string CompanyPhone { get; set; }
        [XmlElement(ElementName = "CompanyFax")]
        public string CompanyFax { get; set; }
        [XmlElement(ElementName = "CompanyEmail")]
        public string CompanyEmail { get; set; }
        [XmlElement(ElementName = "CompanyBankAccount")]
        public string CompanyBankAccount { get; set; }
        [XmlElement(ElementName = "CompanyBankName")]
        public string CompanyBankName { get; set; }
        [XmlElement(ElementName = "CustomerCode")]
        public string CustomerCode { get; set; }
        [XmlElement(ElementName = "CustomerTaxCode")]
        public string CustomerTaxCode { get; set; }
        [XmlElement(ElementName = "Buyer")]
        public string Buyer { get; set; }
        [XmlElement(ElementName = "CustomerName")]
        public string CustomerName { get; set; }
        [XmlElement(ElementName = "CustomerAddress")]
        public string CustomerAddress { get; set; }
        [XmlElement(ElementName = "CustomerPhone")]
        public string CustomerPhone { get; set; }
        [XmlElement(ElementName = "CustomerFax")]
        public string CustomerFax { get; set; }
        [XmlElement(ElementName = "EmailDeliver")]
        public string EmailDeliver { get; set; }
        [XmlElement(ElementName = "CustomerBankAccount")]
        public string CustomerBankAccount { get; set; }
        [XmlElement(ElementName = "CustomerBankName")]
        public string CustomerBankName { get; set; }
        [XmlElement(ElementName = "CustomerType")]
        public string CustomerType { get; set; }
        [XmlElement(ElementName = "CurrencyUnit")]
        public string CurrencyUnit { get; set; }
        [XmlElement(ElementName = "ExchangeRate")]
        public string ExchangeRate { get; set; }
        [XmlElement(ElementName = "Amount")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "TaxRate")]
        public string TaxRate { get; set; }
        [XmlElement(ElementName = "TaxAmount")]
        public string TaxAmount { get; set; }
        [XmlElement(ElementName = "TotalAmount")]
        public string TotalAmount { get; set; }
        [XmlElement(ElementName = "AmountInWords")]
        public string AmountInWords { get; set; }
        [XmlElement(ElementName = "TaxAmountFree")]
        public string TaxAmountFree { get; set; }
        [XmlElement(ElementName = "TaxAmount0")]
        public string TaxAmount0 { get; set; }
        [XmlElement(ElementName = "TaxAmount5")]
        public string TaxAmount5 { get; set; }
        [XmlElement(ElementName = "TaxAmount10")]
        public string TaxAmount10 { get; set; }
        [XmlElement(ElementName = "DiscountAmount")]
        public string DiscountAmount { get; set; }
        [XmlElement(ElementName = "PromotionAmount")]
        public string PromotionAmount { get; set; }
        [XmlElement(ElementName = "Note")]
        public string Note { get; set; }
        [XmlElement(ElementName = "External1")]
        public string External1 { get; set; }
        [XmlElement(ElementName = "External2")]
        public string External2 { get; set; }
        [XmlElement(ElementName = "External3")]
        public string External3 { get; set; }
        [XmlElement(ElementName = "NumberExternal1")]
        public string NumberExternal1 { get; set; }
        [XmlElement(ElementName = "NumberExternal2")]
        public string NumberExternal2 { get; set; }
        [XmlElement(ElementName = "NumberExternal3")]
        public string NumberExternal3 { get; set; }
        [XmlElement(ElementName = "KindOfInvoice")]
        public string KindOfInvoice { get; set; }
        [XmlElement(ElementName = "Products")]
        public Products Products { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "ExternalKeys")]
    public class ExternalKeys
    {
        [XmlElement(ElementName = "UnitCode")]
        public string UnitCode { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "Invoice")]
    public class Invoice
    {
        [XmlElement(ElementName = "Content")]
        public Content Content { get; set; }
        [XmlElement(ElementName = "Key")]
        public string Key { get; set; }
        [XmlElement(ElementName = "ExternalKeys")]
        public ExternalKeys ExternalKeys { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

}

namespace ESCS.Models.TCT
{
    #region "TCT"

    [XmlRoot(ElementName = "payment", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Payment
    {
        [XmlElement(ElementName = "paymentMethodName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string PaymentMethodName { get; set; }
    }

    [XmlRoot(ElementName = "payments", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Payments
    {
        [XmlElement(ElementName = "payment", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Payment Payment { get; set; }
    }

    [XmlRoot(ElementName = "delivery", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Delivery
    {
        [XmlElement(ElementName = "deliveryOrderNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string DeliveryOrderNumber { get; set; }
        [XmlElement(ElementName = "fromWarehouseName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string FromWarehouseName { get; set; }
        [XmlElement(ElementName = "containerNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ContainerNumber { get; set; }
    }

    [XmlRoot(ElementName = "item", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Item
    {
        [XmlElement(ElementName = "lineNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string LineNumber { get; set; }
        [XmlElement(ElementName = "itemCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ItemCode { get; set; }
        [XmlElement(ElementName = "itemName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ItemName { get; set; }
        [XmlElement(ElementName = "unitCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UnitCode { get; set; }
        [XmlElement(ElementName = "unitName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UnitName { get; set; }
        [XmlElement(ElementName = "itemTotalAmountWithoutVat", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ItemTotalAmountWithoutVat { get; set; }

        [XmlElement(ElementName = "itemTotalAmountWithVat", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ItemTotalAmountWithVat { get; set; }

        [XmlElement(ElementName = "unitPrice", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UnitPrice { get; set; }

        [XmlElement(ElementName = "vatPercentage", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatPercentage { get; set; }
        [XmlElement(ElementName = "vatAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatAmount { get; set; }

        [XmlElement(ElementName = "prodPrice", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ProdPrice { get; set; }

        [XmlElement(ElementName = "prodQuantity", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ProdQuantity { get; set; }

        [XmlElement(ElementName = "quantity", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string Quantity { get; set; }
    }

    [XmlRoot(ElementName = "items", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Items
    {
        [XmlElement(ElementName = "item", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public List<Item> Item { get; set; }
    }

    [XmlRoot(ElementName = "invoiceTaxBreakdown", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class InvoiceTaxBreakdown
    {
        [XmlElement(ElementName = "vatPercentage", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatPercentage { get; set; }
        [XmlElement(ElementName = "vatTaxableAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatTaxableAmount { get; set; }
        [XmlElement(ElementName = "vatTaxAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatTaxAmount { get; set; }
    }

    [XmlRoot(ElementName = "invoiceTaxBreakdowns", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class InvoiceTaxBreakdowns
    {
        [XmlElement(ElementName = "invoiceTaxBreakdown", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public InvoiceTaxBreakdown InvoiceTaxBreakdown { get; set; }
    }

    [XmlRoot(ElementName = "seller", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Seller
    {
        [XmlElement(ElementName = "sellerCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerCode { get; set; }
        [XmlElement(ElementName = "sellerLegalName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerLegalName { get; set; }
        [XmlElement(ElementName = "sellerTaxCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerTaxCode { get; set; }
        [XmlElement(ElementName = "sellerAddressLine", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerAddressLine { get; set; }
        [XmlElement(ElementName = "sellerPhoneNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerPhoneNumber { get; set; }
        [XmlElement(ElementName = "sellerFaxNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerFaxNumber { get; set; }
        [XmlElement(ElementName = "sellerEmail", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerEmail { get; set; }
        [XmlElement(ElementName = "sellerBankName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerBankName { get; set; }
        [XmlElement(ElementName = "sellerBankAccount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerBankAccount { get; set; }
        [XmlElement(ElementName = "sellerContactPersonName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerContactPersonName { get; set; }
        [XmlElement(ElementName = "sellerContactPersonPhoneNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerContactPersonPhoneNumber { get; set; }
        [XmlElement(ElementName = "sellerContactPersonEmail", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerContactPersonEmail { get; set; }
        [XmlElement(ElementName = "sellerWebsite", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerWebsite { get; set; }
        [XmlElement(ElementName = "sellerBusinessLicenseNo", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerBusinessLicenseNo { get; set; }
        [XmlElement(ElementName = "sellerRepresentative", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerRepresentative { get; set; }
        [XmlElement(ElementName = "sellerBankAccountOwner", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerBankAccountOwner { get; set; }
        [XmlElement(ElementName = "sellerRepresentativeIdType", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerRepresentativeIdType { get; set; }
        [XmlElement(ElementName = "sellerSearchInvoiceLink", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerSearchInvoiceLink { get; set; }
        [XmlElement(ElementName = "sellerRepresentativeIdNo", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerRepresentativeIdNo { get; set; }
    }

    [XmlRoot(ElementName = "buyer", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Buyer
    {
        [XmlElement(ElementName = "buyerCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerCode { get; set; }
        [XmlElement(ElementName = "buyerDisplayName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerDisplayName { get; set; }
        [XmlElement(ElementName = "buyerLegalName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerLegalName { get; set; }
        [XmlElement(ElementName = "buyerTaxCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerTaxCode { get; set; }
        [XmlElement(ElementName = "buyerAddressLine", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerAddressLine { get; set; }
        [XmlElement(ElementName = "buyerPostalCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerPostalCode { get; set; }
        [XmlElement(ElementName = "buyerDistrictName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerDistrictName { get; set; }
        [XmlElement(ElementName = "buyerCityName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerCityName { get; set; }
        [XmlElement(ElementName = "buyerCountryCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerCountryCode { get; set; }
        [XmlElement(ElementName = "buyerPhoneNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerPhoneNumber { get; set; }
        [XmlElement(ElementName = "buyerFaxNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerFaxNumber { get; set; }
        [XmlElement(ElementName = "buyerEmail", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerEmail { get; set; }
        [XmlElement(ElementName = "buyerBankName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerBankName { get; set; }
        [XmlElement(ElementName = "buyerBankAccount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerBankAccount { get; set; }
        [XmlElement(ElementName = "buyerIdType", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerIdType { get; set; }
        [XmlElement(ElementName = "buyerIdNo", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerIdNo { get; set; }
        [XmlElement(ElementName = "buyerBirthDay", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerBirthDay { get; set; }
    }

    [XmlRoot(ElementName = "invoiceData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class InvoiceData
    {
        [XmlElement(ElementName = "sellerAppRecordId", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerAppRecordId { get; set; }
        [XmlElement(ElementName = "invoiceAppRecordId", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceAppRecordId { get; set; }
        [XmlElement(ElementName = "invoiceType", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceType { get; set; }
        [XmlElement(ElementName = "templateCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TemplateCode { get; set; }
        [XmlElement(ElementName = "invoiceSeries", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceSeries { get; set; }
        [XmlElement(ElementName = "invoiceNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceNumber { get; set; }
        [XmlElement(ElementName = "invoiceName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceName { get; set; }
        [XmlElement(ElementName = "invoiceIssuedDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceIssuedDate { get; set; }
        [XmlElement(ElementName = "signedDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SignedDate { get; set; }
        [XmlElement(ElementName = "submittedDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SubmittedDate { get; set; }
        [XmlElement(ElementName = "contractNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ContractNumber { get; set; }
        [XmlElement(ElementName = "currencyCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string CurrencyCode { get; set; }
        [XmlElement(ElementName = "invoiceNote", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceNote { get; set; }
        [XmlElement(ElementName = "adjustmentType", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string AdjustmentType { get; set; }
        [XmlElement(ElementName = "payments", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Payments Payments { get; set; }
        [XmlElement(ElementName = "delivery", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Delivery Delivery { get; set; }
        [XmlElement(ElementName = "sellerLegalName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerLegalName { get; set; }
        [XmlElement(ElementName = "sellerTaxCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerTaxCode { get; set; }
        [XmlElement(ElementName = "sellerAddressLine", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerAddressLine { get; set; }
        [XmlElement(ElementName = "sellerPhoneNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerPhoneNumber { get; set; }
        [XmlElement(ElementName = "sellerFaxNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerFaxNumber { get; set; }
        [XmlElement(ElementName = "sellerEmail", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerEmail { get; set; }
        [XmlElement(ElementName = "sellerBankName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerBankName { get; set; }
        [XmlElement(ElementName = "sellerBankAccount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerBankAccount { get; set; }
        [XmlElement(ElementName = "sellerContactPersonName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerContactPersonName { get; set; }
        [XmlElement(ElementName = "sellerSignedPersonName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerSignedPersonName { get; set; }
        [XmlElement(ElementName = "sellerSubmittedPersonName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerSubmittedPersonName { get; set; }
        [XmlElement(ElementName = "buyerDisplayName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerDisplayName { get; set; }
        [XmlElement(ElementName = "buyerLegalName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerLegalName { get; set; }
        [XmlElement(ElementName = "buyerTaxCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerTaxCode { get; set; }
        [XmlElement(ElementName = "buyerAddressLine", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerAddressLine { get; set; }
        [XmlElement(ElementName = "buyerPhoneNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerPhoneNumber { get; set; }
        [XmlElement(ElementName = "buyerFaxNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerFaxNumber { get; set; }
        [XmlElement(ElementName = "buyerEmail", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerEmail { get; set; }
        [XmlElement(ElementName = "buyerBankName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerBankName { get; set; }
        [XmlElement(ElementName = "buyerBankAccount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerBankAccount { get; set; }
        [XmlElement(ElementName = "seller", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Seller Seller { get; set; }
        [XmlElement(ElementName = "buyer", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Buyer Buyer { get; set; }
        [XmlElement(ElementName = "items", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Items Items { get; set; }
        [XmlElement(ElementName = "invoiceTaxBreakdowns", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public InvoiceTaxBreakdowns InvoiceTaxBreakdowns { get; set; }
        [XmlElement(ElementName = "sumOfTotalLineAmountWithoutVAT", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SumOfTotalLineAmountWithoutVAT { get; set; }
        [XmlElement(ElementName = "totalAmountWithoutVAT", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TotalAmountWithoutVAT { get; set; }
        [XmlElement(ElementName = "totalVATAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TotalVATAmount { get; set; }
        [XmlElement(ElementName = "totalAmountWithVAT", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TotalAmountWithVAT { get; set; }
        [XmlElement(ElementName = "totalAmountWithVATInWords", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TotalAmountWithVATInWords { get; set; }
        [XmlElement(ElementName = "discountAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string DiscountAmount { get; set; }
        [XmlElement(ElementName = "isDiscountAmtPos", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string IsDiscountAmtPos { get; set; }
        [XmlElement(ElementName = "userDefines", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UserDefines { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "controlData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class ControlData
    {
        [XmlElement(ElementName = "systemCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SystemCode { get; set; }
        [XmlElement(ElementName = "qrCodeData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string QrCodeData { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<Transform> Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "invoice", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Invoice
    {
        [XmlElement(ElementName = "invoiceData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public InvoiceData InvoiceData { get; set; }
        [XmlElement(ElementName = "controlData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public ControlData ControlData { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
        [XmlAttribute(AttributeName = "ds", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ds { get; set; }
        [XmlAttribute(AttributeName = "inv", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Inv { get; set; }
    }

    #endregion

}

namespace ESCS.Models.TCT2
{
    #region "TCT2"

    [XmlRoot(ElementName = "payment", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Payment
    {
        [XmlElement(ElementName = "paymentDueDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string PaymentDueDate { get; set; }
        [XmlElement(ElementName = "paymentMethodName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string PaymentMethodName { get; set; }
        [XmlElement(ElementName = "paymentAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string PaymentAmount { get; set; }
    }

    [XmlRoot(ElementName = "payments", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Payments
    {
        [XmlElement(ElementName = "payment", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Payment Payment { get; set; }
    }

    [XmlRoot(ElementName = "delivery", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Delivery
    {
        [XmlElement(ElementName = "deliveryOrderNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string DeliveryOrderNumber { get; set; }
        [XmlElement(ElementName = "deliveryOrderDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string DeliveryOrderDate { get; set; }
        [XmlElement(ElementName = "containerNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ContainerNumber { get; set; }
    }

    [XmlRoot(ElementName = "item", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Item
    {
        [XmlElement(ElementName = "lineNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string LineNumber { get; set; }
        [XmlElement(ElementName = "itemCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ItemCode { get; set; }
        [XmlElement(ElementName = "itemName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ItemName { get; set; }
        [XmlElement(ElementName = "unitCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UnitCode { get; set; }
        [XmlElement(ElementName = "unitName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UnitName { get; set; }
        [XmlElement(ElementName = "userDefinesItem", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UserDefinesItem { get; set; }
        [XmlElement(ElementName = "quantity", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string Quantity { get; set; }
        [XmlElement(ElementName = "note", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string Note { get; set; }
        [XmlElement(ElementName = "itemTotalAmountWithoutVat", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ItemTotalAmountWithoutVat { get; set; }
        [XmlElement(ElementName = "vatPercentage", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatPercentage { get; set; }
        [XmlElement(ElementName = "vatAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatAmount { get; set; }
        [XmlElement(ElementName = "unitPrice", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UnitPrice { get; set; }
        [XmlElement(ElementName = "promotion", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string Promotion { get; set; }
        [XmlElement(ElementName = "discountPercentage", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string DiscountPercentage { get; set; }
        [XmlElement(ElementName = "discountAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string DiscountAmount { get; set; }
    }

    [XmlRoot(ElementName = "items", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Items
    {
        [XmlElement(ElementName = "item", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public List<Item> Item { get; set; }
    }

    [XmlRoot(ElementName = "invoiceTaxBreakdown", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class InvoiceTaxBreakdown
    {
        [XmlElement(ElementName = "vatPercentage", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatPercentage { get; set; }
        [XmlElement(ElementName = "vatTaxableAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatTaxableAmount { get; set; }
        [XmlElement(ElementName = "vatExemptionReason", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatExemptionReason { get; set; }
        [XmlElement(ElementName = "vatTaxAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string VatTaxAmount { get; set; }
    }

    [XmlRoot(ElementName = "invoiceTaxBreakdowns", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class InvoiceTaxBreakdowns
    {
        [XmlElement(ElementName = "invoiceTaxBreakdown", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public InvoiceTaxBreakdown InvoiceTaxBreakdown { get; set; }
    }

    [XmlRoot(ElementName = "invoiceData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class InvoiceData
    {
        [XmlElement(ElementName = "invoiceAppRecordId", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceAppRecordId { get; set; }
        [XmlElement(ElementName = "invoiceType", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceType { get; set; }
        [XmlElement(ElementName = "templateCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TemplateCode { get; set; }
        [XmlElement(ElementName = "invoiceSeries", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceSeries { get; set; }
        [XmlElement(ElementName = "invoiceNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceNumber { get; set; }
        [XmlElement(ElementName = "invoiceName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceName { get; set; }
        [XmlElement(ElementName = "invoiceIssuedDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string InvoiceIssuedDate { get; set; }
        [XmlElement(ElementName = "signedDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SignedDate { get; set; }
        [XmlElement(ElementName = "submittedDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SubmittedDate { get; set; }
        [XmlElement(ElementName = "contractDate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ContractDate { get; set; }
        [XmlElement(ElementName = "currencyCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string CurrencyCode { get; set; }
        [XmlElement(ElementName = "exchangeRate", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string ExchangeRate { get; set; }
        [XmlElement(ElementName = "adjustmentType", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string AdjustmentType { get; set; }
        [XmlElement(ElementName = "payments", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Payments Payments { get; set; }
        [XmlElement(ElementName = "delivery", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Delivery Delivery { get; set; }
        [XmlElement(ElementName = "sellerLegalName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerLegalName { get; set; }
        [XmlElement(ElementName = "sellerTaxCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerTaxCode { get; set; }
        [XmlElement(ElementName = "sellerAddressLine", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerAddressLine { get; set; }
        [XmlElement(ElementName = "sellerDistrictName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerDistrictName { get; set; }
        [XmlElement(ElementName = "sellerCityName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerCityName { get; set; }
        [XmlElement(ElementName = "sellerCountryCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerCountryCode { get; set; }
        [XmlElement(ElementName = "sellerPhoneNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerPhoneNumber { get; set; }
        [XmlElement(ElementName = "sellerFaxNumber", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerFaxNumber { get; set; }
        [XmlElement(ElementName = "sellerEmail", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerEmail { get; set; }
        [XmlElement(ElementName = "sellerBankName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerBankName { get; set; }
        [XmlElement(ElementName = "sellerBankAccount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerBankAccount { get; set; }
        [XmlElement(ElementName = "sellerContactPersonName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerContactPersonName { get; set; }
        [XmlElement(ElementName = "sellerSubmittedPersonName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SellerSubmittedPersonName { get; set; }
        [XmlElement(ElementName = "buyerDisplayName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerDisplayName { get; set; }
        [XmlElement(ElementName = "buyerLegalName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerLegalName { get; set; }
        [XmlElement(ElementName = "buyerTaxCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerTaxCode { get; set; }
        [XmlElement(ElementName = "buyerAddressLine", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerAddressLine { get; set; }
        [XmlElement(ElementName = "buyerEmail", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerEmail { get; set; }
        [XmlElement(ElementName = "buyerBankName", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerBankName { get; set; }
        [XmlElement(ElementName = "buyerBankAccount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerBankAccount { get; set; }
        [XmlElement(ElementName = "buyerSoDonHang", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string BuyerSoDonHang { get; set; }
        [XmlElement(ElementName = "items", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Items Items { get; set; }
        [XmlElement(ElementName = "invoiceTaxBreakdowns", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public InvoiceTaxBreakdowns InvoiceTaxBreakdowns { get; set; }
        [XmlElement(ElementName = "sumOfTotalLineAmountWithoutVAT", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SumOfTotalLineAmountWithoutVAT { get; set; }
        [XmlElement(ElementName = "totalAmountWithoutVAT", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TotalAmountWithoutVAT { get; set; }
        [XmlElement(ElementName = "totalVATAmount", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TotalVATAmount { get; set; }
        [XmlElement(ElementName = "totalAmountWithVAT", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TotalAmountWithVAT { get; set; }
        [XmlElement(ElementName = "totalAmountWithVATInWords", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TotalAmountWithVATInWords { get; set; }
        [XmlElement(ElementName = "isDiscountAmtPos", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string IsDiscountAmtPos { get; set; }
        [XmlElement(ElementName = "userDefines", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string UserDefines { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "controlData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class ControlData
    {
        [XmlElement(ElementName = "systemCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string SystemCode { get; set; }
        [XmlElement(ElementName = "taxOfficeCode", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string TaxOfficeCode { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509SubjectName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SubjectName { get; set; }
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "invoice", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Invoice
    {
        [XmlElement(ElementName = "invoiceData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public InvoiceData InvoiceData { get; set; }
        [XmlElement(ElementName = "controlData", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public ControlData ControlData { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

    [XmlRoot(ElementName = "invoices", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Invoices
    {
        [XmlElement(ElementName = "invoice", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Invoice Invoice { get; set; }
    }

    [XmlRoot(ElementName = "transaction", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public class Transaction
    {
        [XmlElement(ElementName = "resend", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public string Resend { get; set; }
        [XmlElement(ElementName = "invoices", Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
        public Invoices Invoices { get; set; }
        [XmlAttribute(AttributeName = "inv", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Inv { get; set; }
        [XmlAttribute(AttributeName = "ds", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ds { get; set; }
    }

    #endregion

}

namespace ESCS.Models.TCT3
{
    [XmlRoot(ElementName = "tax")]
    public class Tax
    {
        [XmlElement(ElementName = "amt")]
        public string Amt { get; set; }
        [XmlElement(ElementName = "vat")]
        public string Vat { get; set; }
        [XmlElement(ElementName = "vrn")]
        public string Vrn { get; set; }
        [XmlElement(ElementName = "vrt")]
        public string Vrt { get; set; }
        [XmlElement(ElementName = "amtv")]
        public string Amtv { get; set; }
        [XmlElement(ElementName = "vatv")]
        public string Vatv { get; set; }
    }

    [XmlRoot(ElementName = "TTKhac")]
    public class TTKhac
    {
        [XmlElement(ElementName = "taxo")]
        public string Taxo { get; set; }
        [XmlElement(ElementName = "tax")]
        public Tax Tax { get; set; }
        [XmlElement(ElementName = "inc")]
        public string Inc { get; set; }
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "sec")]
        public string Sec { get; set; }
        [XmlElement(ElementName = "exrt")]
        public string Exrt { get; set; }
        [XmlElement(ElementName = "note")]
        public string Note { get; set; }
        [XmlElement(ElementName = "paym")]
        public string Paym { get; set; }
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "form")]
        public string Form { get; set; }
        [XmlElement(ElementName = "serial")]
        public string Serial { get; set; }
        [XmlElement(ElementName = "ou")]
        public string Ou { get; set; }
        [XmlElement(ElementName = "uc")]
        public string Uc { get; set; }
        [XmlElement(ElementName = "Tel")]
        public string Tel { get; set; }
        [XmlElement(ElementName = "Mail")]
        public string Mail { get; set; }
        [XmlElement(ElementName = "Acc")]
        public string Acc { get; set; }
        [XmlElement(ElementName = "Bank")]
        public string Bank { get; set; }
        [XmlElement(ElementName = "Buyer")]
        public string Buyer { get; set; }
        [XmlElement(ElementName = "discount")]
        public string Discount { get; set; }
        [XmlElement(ElementName = "sum")]
        public string Sum { get; set; }
        [XmlElement(ElementName = "vat")]
        public string Vat { get; set; }
        [XmlElement(ElementName = "total")]
        public string Total { get; set; }
    }

    [XmlRoot(ElementName = "TTChung")]
    public class TTChung
    {
        [XmlElement(ElementName = "TTKhac")]
        public TTKhac TTKhac { get; set; }
        [XmlElement(ElementName = "THDon")]
        public string THDon { get; set; }
        [XmlElement(ElementName = "So")]
        public string So { get; set; }
        [XmlElement(ElementName = "SHDon")]
        public string SHDon { get; set; }

        [XmlElement(ElementName = "TDLap")]
        public string TDLap { get; set; }
        [XmlElement(ElementName = "DVTTe")]
        public string DVTTe { get; set; }
        [XmlElement(ElementName = "PBan")]
        public string PBan { get; set; }
        [XmlElement(ElementName = "KHHDon")]
        public string KHHDon { get; set; }
        [XmlElement(ElementName = "KHMSHDon")]
        public string KHMSHDon { get; set; }
        [XmlElement(ElementName = "TChat")]
        public string TChat { get; set; }
        [XmlElement(ElementName = "MauSo")]
        public string MauSo { get; set; }
    }

    [XmlRoot(ElementName = "NBan")]
    public class NBan
    {
        [XmlElement(ElementName = "Ten")]
        public string Ten { get; set; }
        [XmlElement(ElementName = "MST")]
        public string MST { get; set; }
        [XmlElement(ElementName = "DChi")]
        public string DChi { get; set; }
        [XmlElement(ElementName = "TTKhac")]
        public TTKhac TTKhac { get; set; }
    }

    [XmlRoot(ElementName = "NMua")]
    public class NMua
    {
        [XmlElement(ElementName = "Ten")]
        public string Ten { get; set; }
        [XmlElement(ElementName = "MST")]
        public string MST { get; set; }
        [XmlElement(ElementName = "DChi")]
        public string DChi { get; set; }
        [XmlElement(ElementName = "TTKhac")]
        public TTKhac TTKhac { get; set; }
    }

    [XmlRoot(ElementName = "TToan")]
    public class TToan
    {
        [XmlElement(ElementName = "TTKhac")]
        public TTKhac TTKhac { get; set; }
        [XmlElement(ElementName = "TgTCThue")]
        public string TgTCThue { get; set; }
        [XmlElement(ElementName = "TgTThue")]
        public string TgTThue { get; set; }
        [XmlElement(ElementName = "TgTTTBSo")]
        public string TgTTTBSo { get; set; }
        [XmlElement(ElementName = "TgTTTBChu")]
        public string TgTTTBChu { get; set; }
        [XmlElement(ElementName = "TgTCThue10")]
        public string TgTCThue10 { get; set; }
        [XmlElement(ElementName = "TgTThue10")]
        public string TgTThue10 { get; set; }
        [XmlElement(ElementName = "TgTTToan10")]
        public string TgTTToan10 { get; set; }
    }

    [XmlRoot(ElementName = "HHDVu")]
    public class HHDVu
    {
        [XmlElement(ElementName = "STT")]
        public string STT { get; set; }
        [XmlElement(ElementName = "Ten")]
        public string Ten { get; set; }
        [XmlElement(ElementName = "DVTinh")]
        public string DVTinh { get; set; }
        [XmlElement(ElementName = "DGia")]
        public string DGia { get; set; }
        [XmlElement(ElementName = "SLuong")]
        public string SLuong { get; set; }
        [XmlElement(ElementName = "TSuat")]
        public string TSuat { get; set; }
        [XmlElement(ElementName = "ThTien")]
        public string ThTien { get; set; }
    }

    [XmlRoot(ElementName = "DSHHDVu")]
    public class DSHHDVu
    {
        [XmlElement(ElementName = "HHDVu")]
        public List<HHDVu> HHDVu { get; set; }
    }

    [XmlRoot(ElementName = "NDHDon")]
    public class NDHDon
    {
        [XmlElement(ElementName = "NBan")]
        public NBan NBan { get; set; }
        [XmlElement(ElementName = "NMua")]
        public NMua NMua { get; set; }
        [XmlElement(ElementName = "TToan")]
        public TToan TToan { get; set; }
        [XmlElement(ElementName = "DSHHDVu")]
        public DSHHDVu DSHHDVu { get; set; }
    }

    [XmlRoot(ElementName = "DLHDon")]
    public class DLHDon
    {
        [XmlElement(ElementName = "TTChung")]
        public TTChung TTChung { get; set; }
        [XmlElement(ElementName = "NDHDon")]
        public NDHDon NDHDon { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "HDon")]
    public class HDon
    {
        [XmlElement(ElementName = "DLHDon")]
        public DLHDon DLHDon { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }



}

namespace ESCS.Models.TCT5
{
    // loại mới
    public class Relationship
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }
    }

    [XmlRoot(ElementName = "Relationships", Namespace = "http://schemas.openxmlformats.org/package/2006/relationships")]
    public class Relationships
    {
        [XmlElement(ElementName = "Relationship", Namespace = "http://schemas.openxmlformats.org/package/2006/relationships")]
        public List<Relationship> Relationship { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "xmlData", Namespace = "http://schemas.microsoft.com/office/2006/xmlPackage")]
    public class XmlData
    {
        [XmlElement(ElementName = "Relationships", Namespace = "http://schemas.openxmlformats.org/package/2006/relationships")]
        public Relationships Relationships { get; set; }
        [XmlElement(ElementName = "document", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Document Document { get; set; }
        [XmlElement(ElementName = "theme", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Theme Theme { get; set; }
        [XmlElement(ElementName = "settings", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Settings Settings { get; set; }
        [XmlElement(ElementName = "fonts", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Fonts Fonts { get; set; }
        [XmlElement(ElementName = "webSettings", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public WebSettings WebSettings { get; set; }
        [XmlElement(ElementName = "Properties", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public Properties Properties { get; set; }
        [XmlElement(ElementName = "coreProperties", Namespace = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties")]
        public CoreProperties CoreProperties { get; set; }
        [XmlElement(ElementName = "styles", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Styles Styles { get; set; }
    }

    [XmlRoot(ElementName = "part", Namespace = "http://schemas.microsoft.com/office/2006/xmlPackage")]
    public class Part
    {
        [XmlElement(ElementName = "xmlData", Namespace = "http://schemas.microsoft.com/office/2006/xmlPackage")]
        public XmlData XmlData { get; set; }
        [XmlAttribute(AttributeName = "name", Namespace = "http://schemas.microsoft.com/office/2006/xmlPackage")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "contentType", Namespace = "http://schemas.microsoft.com/office/2006/xmlPackage")]
        public string ContentType { get; set; }
        [XmlAttribute(AttributeName = "padding", Namespace = "http://schemas.microsoft.com/office/2006/xmlPackage")]
        public string Padding { get; set; }
    }

    [XmlRoot(ElementName = "pStyle", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class PStyle
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "rFonts", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class RFonts
    {
        [XmlAttribute(AttributeName = "ascii", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Ascii { get; set; }
        [XmlAttribute(AttributeName = "hAnsi", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string HAnsi { get; set; }
        [XmlAttribute(AttributeName = "cs", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Cs { get; set; }
        [XmlAttribute(AttributeName = "asciiTheme", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string AsciiTheme { get; set; }
        [XmlAttribute(AttributeName = "eastAsiaTheme", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string EastAsiaTheme { get; set; }
        [XmlAttribute(AttributeName = "hAnsiTheme", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string HAnsiTheme { get; set; }
        [XmlAttribute(AttributeName = "cstheme", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Cstheme { get; set; }
    }

    [XmlRoot(ElementName = "rPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class RPr
    {
        [XmlElement(ElementName = "rFonts", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public RFonts RFonts { get; set; }
        [XmlElement(ElementName = "rtl", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Rtl { get; set; }
        [XmlElement(ElementName = "sz", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Sz Sz { get; set; }
        [XmlElement(ElementName = "szCs", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public SzCs SzCs { get; set; }
        [XmlElement(ElementName = "lang", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Lang Lang { get; set; }
    }

    [XmlRoot(ElementName = "pPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class PPr
    {
        [XmlElement(ElementName = "pStyle", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public PStyle PStyle { get; set; }
        [XmlElement(ElementName = "rPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public RPr RPr { get; set; }
        [XmlElement(ElementName = "spacing", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Spacing Spacing { get; set; }
    }

    [XmlRoot(ElementName = "bookmarkStart", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class BookmarkStart
    {
        [XmlAttribute(AttributeName = "id", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "bookmarkEnd", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class BookmarkEnd
    {
        [XmlAttribute(AttributeName = "id", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "r", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class R
    {
        [XmlElement(ElementName = "rPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public RPr RPr { get; set; }
        [XmlAttribute(AttributeName = "rsidRPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string RsidRPr { get; set; }
        [XmlElement(ElementName = "t", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public T T { get; set; }
        [XmlElement(ElementName = "lastRenderedPageBreak", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string LastRenderedPageBreak { get; set; }
    }

    [XmlRoot(ElementName = "t", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class T
    {
        [XmlAttribute(AttributeName = "space", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Space { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "p", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class P
    {
        [XmlElement(ElementName = "pPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public PPr PPr { get; set; }
        [XmlElement(ElementName = "bookmarkStart", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public BookmarkStart BookmarkStart { get; set; }
        [XmlElement(ElementName = "bookmarkEnd", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public BookmarkEnd BookmarkEnd { get; set; }
        [XmlElement(ElementName = "r", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public List<R> R { get; set; }
        [XmlAttribute(AttributeName = "rsidR", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string RsidR { get; set; }
        [XmlAttribute(AttributeName = "rsidRPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string RsidRPr { get; set; }
        [XmlAttribute(AttributeName = "rsidRDefault", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string RsidRDefault { get; set; }
        [XmlAttribute(AttributeName = "rsidP", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string RsidP { get; set; }
    }

    [XmlRoot(ElementName = "pgSz", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class PgSz
    {
        [XmlAttribute(AttributeName = "w", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "h", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string H { get; set; }
    }

    [XmlRoot(ElementName = "pgMar", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class PgMar
    {
        [XmlAttribute(AttributeName = "top", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Top { get; set; }
        [XmlAttribute(AttributeName = "right", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Right { get; set; }
        [XmlAttribute(AttributeName = "bottom", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Bottom { get; set; }
        [XmlAttribute(AttributeName = "left", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Left { get; set; }
        [XmlAttribute(AttributeName = "header", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Header { get; set; }
        [XmlAttribute(AttributeName = "footer", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Footer { get; set; }
        [XmlAttribute(AttributeName = "gutter", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Gutter { get; set; }
    }

    [XmlRoot(ElementName = "cols", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Cols
    {
        [XmlAttribute(AttributeName = "space", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Space { get; set; }
    }

    [XmlRoot(ElementName = "docGrid", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class DocGrid
    {
        [XmlAttribute(AttributeName = "linePitch", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string LinePitch { get; set; }
    }

    [XmlRoot(ElementName = "sectPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class SectPr
    {
        [XmlElement(ElementName = "pgSz", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public PgSz PgSz { get; set; }
        [XmlElement(ElementName = "pgMar", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public PgMar PgMar { get; set; }
        [XmlElement(ElementName = "cols", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Cols Cols { get; set; }
        [XmlElement(ElementName = "docGrid", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public DocGrid DocGrid { get; set; }
        [XmlAttribute(AttributeName = "rsidR", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string RsidR { get; set; }
        [XmlAttribute(AttributeName = "rsidRPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string RsidRPr { get; set; }
        [XmlAttribute(AttributeName = "rsidSect", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string RsidSect { get; set; }
    }

    [XmlRoot(ElementName = "body", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Body
    {
        [XmlElement(ElementName = "p", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public P P { get; set; }
        [XmlElement(ElementName = "sectPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public SectPr SectPr { get; set; }
    }

    [XmlRoot(ElementName = "document", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Document
    {
        [XmlElement(ElementName = "body", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Body Body { get; set; }
        [XmlAttribute(AttributeName = "Ignorable", Namespace = "http://schemas.openxmlformats.org/markup-compatibility/2006")]
        public string Ignorable { get; set; }
        [XmlAttribute(AttributeName = "wpc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wpc { get; set; }
        [XmlAttribute(AttributeName = "mc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mc { get; set; }
        [XmlAttribute(AttributeName = "o", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string O { get; set; }
        [XmlAttribute(AttributeName = "r", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string R { get; set; }
        [XmlAttribute(AttributeName = "m", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string M { get; set; }
        [XmlAttribute(AttributeName = "v", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string V { get; set; }
        [XmlAttribute(AttributeName = "wp14", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wp14 { get; set; }
        [XmlAttribute(AttributeName = "wp", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wp { get; set; }
        [XmlAttribute(AttributeName = "w10", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W10 { get; set; }
        [XmlAttribute(AttributeName = "w", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "w14", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W14 { get; set; }
        [XmlAttribute(AttributeName = "w15", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W15 { get; set; }
        [XmlAttribute(AttributeName = "wpg", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wpg { get; set; }
        [XmlAttribute(AttributeName = "wpi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wpi { get; set; }
        [XmlAttribute(AttributeName = "wne", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wne { get; set; }
        [XmlAttribute(AttributeName = "wps", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wps { get; set; }
    }

    [XmlRoot(ElementName = "sysClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class SysClr
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
        [XmlAttribute(AttributeName = "lastClr")]
        public string LastClr { get; set; }
    }

    [XmlRoot(ElementName = "dk1", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Dk1
    {
        [XmlElement(ElementName = "sysClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SysClr SysClr { get; set; }
    }

    [XmlRoot(ElementName = "lt1", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Lt1
    {
        [XmlElement(ElementName = "sysClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SysClr SysClr { get; set; }
    }

    [XmlRoot(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class SrgbClr
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
        [XmlElement(ElementName = "alpha", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Alpha Alpha { get; set; }
    }

    [XmlRoot(ElementName = "dk2", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Dk2
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "lt2", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Lt2
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "accent1", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Accent1
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "accent2", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Accent2
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "accent3", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Accent3
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "accent4", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Accent4
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "accent5", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Accent5
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "accent6", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Accent6
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "hlink", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Hlink
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "folHlink", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class FolHlink
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
    }

    [XmlRoot(ElementName = "clrScheme", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class ClrScheme
    {
        [XmlElement(ElementName = "dk1", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Dk1 Dk1 { get; set; }
        [XmlElement(ElementName = "lt1", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Lt1 Lt1 { get; set; }
        [XmlElement(ElementName = "dk2", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Dk2 Dk2 { get; set; }
        [XmlElement(ElementName = "lt2", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Lt2 Lt2 { get; set; }
        [XmlElement(ElementName = "accent1", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Accent1 Accent1 { get; set; }
        [XmlElement(ElementName = "accent2", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Accent2 Accent2 { get; set; }
        [XmlElement(ElementName = "accent3", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Accent3 Accent3 { get; set; }
        [XmlElement(ElementName = "accent4", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Accent4 Accent4 { get; set; }
        [XmlElement(ElementName = "accent5", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Accent5 Accent5 { get; set; }
        [XmlElement(ElementName = "accent6", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Accent6 Accent6 { get; set; }
        [XmlElement(ElementName = "hlink", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Hlink Hlink { get; set; }
        [XmlElement(ElementName = "folHlink", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public FolHlink FolHlink { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "latin", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Latin
    {
        [XmlAttribute(AttributeName = "typeface")]
        public string Typeface { get; set; }
        [XmlAttribute(AttributeName = "panose")]
        public string Panose { get; set; }
    }

    [XmlRoot(ElementName = "ea", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Ea
    {
        [XmlAttribute(AttributeName = "typeface")]
        public string Typeface { get; set; }
    }

    [XmlRoot(ElementName = "cs", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Cs
    {
        [XmlAttribute(AttributeName = "typeface")]
        public string Typeface { get; set; }
    }

    [XmlRoot(ElementName = "font", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Font
    {
        [XmlAttribute(AttributeName = "script")]
        public string Script { get; set; }
        [XmlAttribute(AttributeName = "typeface")]
        public string Typeface { get; set; }
    }

    [XmlRoot(ElementName = "majorFont", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class MajorFont
    {
        [XmlElement(ElementName = "latin", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Latin Latin { get; set; }
        [XmlElement(ElementName = "ea", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Ea Ea { get; set; }
        [XmlElement(ElementName = "cs", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Cs Cs { get; set; }
        [XmlElement(ElementName = "font", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public List<Font> Font { get; set; }
    }

    [XmlRoot(ElementName = "minorFont", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class MinorFont
    {
        [XmlElement(ElementName = "latin", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Latin Latin { get; set; }
        [XmlElement(ElementName = "ea", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Ea Ea { get; set; }
        [XmlElement(ElementName = "cs", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Cs Cs { get; set; }
        [XmlElement(ElementName = "font", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public List<Font> Font { get; set; }
    }

    [XmlRoot(ElementName = "fontScheme", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class FontScheme
    {
        [XmlElement(ElementName = "majorFont", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public MajorFont MajorFont { get; set; }
        [XmlElement(ElementName = "minorFont", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public MinorFont MinorFont { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "schemeClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class SchemeClr
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
        [XmlElement(ElementName = "lumMod", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public LumMod LumMod { get; set; }
        [XmlElement(ElementName = "satMod", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SatMod SatMod { get; set; }
        [XmlElement(ElementName = "tint", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Tint Tint { get; set; }
        [XmlElement(ElementName = "shade", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Shade Shade { get; set; }
    }

    [XmlRoot(ElementName = "solidFill", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class SolidFill
    {
        [XmlElement(ElementName = "schemeClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SchemeClr SchemeClr { get; set; }
    }

    [XmlRoot(ElementName = "lumMod", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class LumMod
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "satMod", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class SatMod
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "tint", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Tint
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "gs", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Gs
    {
        [XmlElement(ElementName = "schemeClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SchemeClr SchemeClr { get; set; }
        [XmlAttribute(AttributeName = "pos")]
        public string Pos { get; set; }
    }

    [XmlRoot(ElementName = "gsLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class GsLst
    {
        [XmlElement(ElementName = "gs", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public List<Gs> Gs { get; set; }
    }

    [XmlRoot(ElementName = "lin", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Lin
    {
        [XmlAttribute(AttributeName = "ang")]
        public string Ang { get; set; }
        [XmlAttribute(AttributeName = "scaled")]
        public string Scaled { get; set; }
    }

    [XmlRoot(ElementName = "gradFill", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class GradFill
    {
        [XmlElement(ElementName = "gsLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public GsLst GsLst { get; set; }
        [XmlElement(ElementName = "lin", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Lin Lin { get; set; }
        [XmlAttribute(AttributeName = "rotWithShape")]
        public string RotWithShape { get; set; }
    }

    [XmlRoot(ElementName = "shade", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Shade
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "fillStyleLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class FillStyleLst
    {
        [XmlElement(ElementName = "solidFill", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SolidFill SolidFill { get; set; }
        [XmlElement(ElementName = "gradFill", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public List<GradFill> GradFill { get; set; }
    }

    [XmlRoot(ElementName = "prstDash", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class PrstDash
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "miter", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Miter
    {
        [XmlAttribute(AttributeName = "lim")]
        public string Lim { get; set; }
    }

    [XmlRoot(ElementName = "ln", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Ln
    {
        [XmlElement(ElementName = "solidFill", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SolidFill SolidFill { get; set; }
        [XmlElement(ElementName = "prstDash", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public PrstDash PrstDash { get; set; }
        [XmlElement(ElementName = "miter", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Miter Miter { get; set; }
        [XmlAttribute(AttributeName = "w")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "cap")]
        public string Cap { get; set; }
        [XmlAttribute(AttributeName = "cmpd")]
        public string Cmpd { get; set; }
        [XmlAttribute(AttributeName = "algn")]
        public string Algn { get; set; }
    }

    [XmlRoot(ElementName = "lnStyleLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class LnStyleLst
    {
        [XmlElement(ElementName = "ln", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public List<Ln> Ln { get; set; }
    }

    [XmlRoot(ElementName = "effectStyle", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class EffectStyle
    {
        [XmlElement(ElementName = "effectLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public EffectLst EffectLst { get; set; }
    }

    [XmlRoot(ElementName = "alpha", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Alpha
    {
        [XmlAttribute(AttributeName = "val")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "outerShdw", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class OuterShdw
    {
        [XmlElement(ElementName = "srgbClr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public SrgbClr SrgbClr { get; set; }
        [XmlAttribute(AttributeName = "blurRad")]
        public string BlurRad { get; set; }
        [XmlAttribute(AttributeName = "dist")]
        public string Dist { get; set; }
        [XmlAttribute(AttributeName = "dir")]
        public string Dir { get; set; }
        [XmlAttribute(AttributeName = "algn")]
        public string Algn { get; set; }
        [XmlAttribute(AttributeName = "rotWithShape")]
        public string RotWithShape { get; set; }
    }

    [XmlRoot(ElementName = "effectLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class EffectLst
    {
        [XmlElement(ElementName = "outerShdw", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public OuterShdw OuterShdw { get; set; }
    }

    [XmlRoot(ElementName = "effectStyleLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class EffectStyleLst
    {
        [XmlElement(ElementName = "effectStyle", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public List<EffectStyle> EffectStyle { get; set; }
    }

    [XmlRoot(ElementName = "bgFillStyleLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class BgFillStyleLst
    {
        [XmlElement(ElementName = "solidFill", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public List<SolidFill> SolidFill { get; set; }
        [XmlElement(ElementName = "gradFill", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public GradFill GradFill { get; set; }
    }

    [XmlRoot(ElementName = "fmtScheme", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class FmtScheme
    {
        [XmlElement(ElementName = "fillStyleLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public FillStyleLst FillStyleLst { get; set; }
        [XmlElement(ElementName = "lnStyleLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public LnStyleLst LnStyleLst { get; set; }
        [XmlElement(ElementName = "effectStyleLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public EffectStyleLst EffectStyleLst { get; set; }
        [XmlElement(ElementName = "bgFillStyleLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public BgFillStyleLst BgFillStyleLst { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "themeElements", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class ThemeElements
    {
        [XmlElement(ElementName = "clrScheme", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public ClrScheme ClrScheme { get; set; }
        [XmlElement(ElementName = "fontScheme", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public FontScheme FontScheme { get; set; }
        [XmlElement(ElementName = "fmtScheme", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public FmtScheme FmtScheme { get; set; }
    }

    [XmlRoot(ElementName = "themeFamily", Namespace = "http://schemas.microsoft.com/office/thememl/2012/main")]
    public class ThemeFamily
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "vid")]
        public string Vid { get; set; }
        [XmlAttribute(AttributeName = "thm15", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Thm15 { get; set; }
    }

    [XmlRoot(ElementName = "ext", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Ext
    {
        [XmlElement(ElementName = "themeFamily", Namespace = "http://schemas.microsoft.com/office/thememl/2012/main")]
        public ThemeFamily ThemeFamily { get; set; }
        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }
    }

    [XmlRoot(ElementName = "extLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class ExtLst
    {
        [XmlElement(ElementName = "ext", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public Ext Ext { get; set; }
    }

    [XmlRoot(ElementName = "theme", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
    public class Theme
    {
        [XmlElement(ElementName = "themeElements", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public ThemeElements ThemeElements { get; set; }
        [XmlElement(ElementName = "objectDefaults", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public string ObjectDefaults { get; set; }
        [XmlElement(ElementName = "extraClrSchemeLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public string ExtraClrSchemeLst { get; set; }
        [XmlElement(ElementName = "extLst", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
        public ExtLst ExtLst { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "a", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string A { get; set; }
    }

    [XmlRoot(ElementName = "zoom", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Zoom
    {
        [XmlAttribute(AttributeName = "percent", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Percent { get; set; }
    }

    [XmlRoot(ElementName = "defaultTabStop", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class DefaultTabStop
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "characterSpacingControl", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class CharacterSpacingControl
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "compatSetting", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class CompatSetting
    {
        [XmlAttribute(AttributeName = "name", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "uri", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Uri { get; set; }
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "compat", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Compat
    {
        [XmlElement(ElementName = "compatSetting", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public List<CompatSetting> CompatSetting { get; set; }
    }

    [XmlRoot(ElementName = "rsidRoot", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class RsidRoot
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "rsid", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Rsid
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "rsids", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Rsids
    {
        [XmlElement(ElementName = "rsidRoot", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public RsidRoot RsidRoot { get; set; }
        [XmlElement(ElementName = "rsid", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public List<Rsid> Rsid { get; set; }
    }

    [XmlRoot(ElementName = "mathFont", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class MathFont
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "brkBin", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class BrkBin
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "brkBinSub", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class BrkBinSub
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "smallFrac", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class SmallFrac
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "lMargin", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class LMargin
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "rMargin", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class RMargin
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "defJc", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class DefJc
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "wrapIndent", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class WrapIndent
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "intLim", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class IntLim
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "naryLim", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class NaryLim
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "mathPr", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
    public class MathPr
    {
        [XmlElement(ElementName = "mathFont", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public MathFont MathFont { get; set; }
        [XmlElement(ElementName = "brkBin", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public BrkBin BrkBin { get; set; }
        [XmlElement(ElementName = "brkBinSub", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public BrkBinSub BrkBinSub { get; set; }
        [XmlElement(ElementName = "smallFrac", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public SmallFrac SmallFrac { get; set; }
        [XmlElement(ElementName = "dispDef", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public string DispDef { get; set; }
        [XmlElement(ElementName = "lMargin", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public LMargin LMargin { get; set; }
        [XmlElement(ElementName = "rMargin", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public RMargin RMargin { get; set; }
        [XmlElement(ElementName = "defJc", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public DefJc DefJc { get; set; }
        [XmlElement(ElementName = "wrapIndent", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public WrapIndent WrapIndent { get; set; }
        [XmlElement(ElementName = "intLim", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public IntLim IntLim { get; set; }
        [XmlElement(ElementName = "naryLim", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public NaryLim NaryLim { get; set; }
    }

    [XmlRoot(ElementName = "themeFontLang", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class ThemeFontLang
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "clrSchemeMapping", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class ClrSchemeMapping
    {
        [XmlAttribute(AttributeName = "bg1", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Bg1 { get; set; }
        [XmlAttribute(AttributeName = "t1", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string T1 { get; set; }
        [XmlAttribute(AttributeName = "bg2", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Bg2 { get; set; }
        [XmlAttribute(AttributeName = "t2", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string T2 { get; set; }
        [XmlAttribute(AttributeName = "accent1", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Accent1 { get; set; }
        [XmlAttribute(AttributeName = "accent2", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Accent2 { get; set; }
        [XmlAttribute(AttributeName = "accent3", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Accent3 { get; set; }
        [XmlAttribute(AttributeName = "accent4", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Accent4 { get; set; }
        [XmlAttribute(AttributeName = "accent5", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Accent5 { get; set; }
        [XmlAttribute(AttributeName = "accent6", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Accent6 { get; set; }
        [XmlAttribute(AttributeName = "hyperlink", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Hyperlink { get; set; }
        [XmlAttribute(AttributeName = "followedHyperlink", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string FollowedHyperlink { get; set; }
    }

    [XmlRoot(ElementName = "shapedefaults", Namespace = "urn:schemas-microsoft-com:office:office")]
    public class Shapedefaults
    {
        [XmlAttribute(AttributeName = "ext", Namespace = "urn:schemas-microsoft-com:vml")]
        public string Ext { get; set; }
        [XmlAttribute(AttributeName = "spidmax")]
        public string Spidmax { get; set; }
    }

    [XmlRoot(ElementName = "idmap", Namespace = "urn:schemas-microsoft-com:office:office")]
    public class Idmap
    {
        [XmlAttribute(AttributeName = "ext", Namespace = "urn:schemas-microsoft-com:vml")]
        public string Ext { get; set; }
        [XmlAttribute(AttributeName = "data")]
        public string Data { get; set; }
    }

    [XmlRoot(ElementName = "shapelayout", Namespace = "urn:schemas-microsoft-com:office:office")]
    public class Shapelayout
    {
        [XmlElement(ElementName = "idmap", Namespace = "urn:schemas-microsoft-com:office:office")]
        public Idmap Idmap { get; set; }
        [XmlAttribute(AttributeName = "ext", Namespace = "urn:schemas-microsoft-com:vml")]
        public string Ext { get; set; }
    }

    [XmlRoot(ElementName = "shapeDefaults", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class ShapeDefaults
    {
        [XmlElement(ElementName = "shapedefaults", Namespace = "urn:schemas-microsoft-com:office:office")]
        public Shapedefaults Shapedefaults { get; set; }
        [XmlElement(ElementName = "shapelayout", Namespace = "urn:schemas-microsoft-com:office:office")]
        public Shapelayout Shapelayout { get; set; }
    }

    [XmlRoot(ElementName = "decimalSymbol", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class DecimalSymbol
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "listSeparator", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class ListSeparator
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "docId", Namespace = "http://schemas.microsoft.com/office/word/2012/wordml")]
    public class DocId
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.microsoft.com/office/word/2012/wordml")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "settings", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Settings
    {
        [XmlElement(ElementName = "zoom", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Zoom Zoom { get; set; }
        [XmlElement(ElementName = "defaultTabStop", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public DefaultTabStop DefaultTabStop { get; set; }
        [XmlElement(ElementName = "characterSpacingControl", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public CharacterSpacingControl CharacterSpacingControl { get; set; }
        [XmlElement(ElementName = "compat", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Compat Compat { get; set; }
        [XmlElement(ElementName = "rsids", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Rsids Rsids { get; set; }
        [XmlElement(ElementName = "mathPr", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
        public MathPr MathPr { get; set; }
        [XmlElement(ElementName = "themeFontLang", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public ThemeFontLang ThemeFontLang { get; set; }
        [XmlElement(ElementName = "clrSchemeMapping", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public ClrSchemeMapping ClrSchemeMapping { get; set; }
        [XmlElement(ElementName = "shapeDefaults", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public ShapeDefaults ShapeDefaults { get; set; }
        [XmlElement(ElementName = "decimalSymbol", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public DecimalSymbol DecimalSymbol { get; set; }
        [XmlElement(ElementName = "listSeparator", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public ListSeparator ListSeparator { get; set; }
        [XmlElement(ElementName = "chartTrackingRefBased", Namespace = "http://schemas.microsoft.com/office/word/2012/wordml")]
        public string ChartTrackingRefBased { get; set; }
        [XmlElement(ElementName = "docId", Namespace = "http://schemas.microsoft.com/office/word/2012/wordml")]
        public DocId DocId { get; set; }
        [XmlAttribute(AttributeName = "Ignorable", Namespace = "http://schemas.openxmlformats.org/markup-compatibility/2006")]
        public string Ignorable { get; set; }
        [XmlAttribute(AttributeName = "mc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mc { get; set; }
        [XmlAttribute(AttributeName = "o", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string O { get; set; }
        [XmlAttribute(AttributeName = "r", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string R { get; set; }
        [XmlAttribute(AttributeName = "m", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string M { get; set; }
        [XmlAttribute(AttributeName = "v", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string V { get; set; }
        [XmlAttribute(AttributeName = "w10", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W10 { get; set; }
        [XmlAttribute(AttributeName = "w", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "w14", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W14 { get; set; }
        [XmlAttribute(AttributeName = "w15", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W15 { get; set; }
        [XmlAttribute(AttributeName = "sl", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Sl { get; set; }
    }

    [XmlRoot(ElementName = "panose1", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Panose1
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "charset", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Charset
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "family", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Family
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "pitch", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Pitch
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "sig", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Sig
    {
        [XmlAttribute(AttributeName = "usb0", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Usb0 { get; set; }
        [XmlAttribute(AttributeName = "usb1", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Usb1 { get; set; }
        [XmlAttribute(AttributeName = "usb2", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Usb2 { get; set; }
        [XmlAttribute(AttributeName = "usb3", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Usb3 { get; set; }
        [XmlAttribute(AttributeName = "csb0", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Csb0 { get; set; }
        [XmlAttribute(AttributeName = "csb1", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Csb1 { get; set; }
    }

    [XmlRoot(ElementName = "font", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Font2
    {
        [XmlElement(ElementName = "panose1", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Panose1 Panose1 { get; set; }
        [XmlElement(ElementName = "charset", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Charset Charset { get; set; }
        [XmlElement(ElementName = "family", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Family Family { get; set; }
        [XmlElement(ElementName = "pitch", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Pitch Pitch { get; set; }
        [XmlElement(ElementName = "sig", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Sig Sig { get; set; }
        [XmlAttribute(AttributeName = "name", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "fonts", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Fonts
    {
        [XmlElement(ElementName = "font", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public List<Font2> Font2 { get; set; }
        [XmlAttribute(AttributeName = "Ignorable", Namespace = "http://schemas.openxmlformats.org/markup-compatibility/2006")]
        public string Ignorable { get; set; }
        [XmlAttribute(AttributeName = "mc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mc { get; set; }
        [XmlAttribute(AttributeName = "r", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string R { get; set; }
        [XmlAttribute(AttributeName = "w", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "w14", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W14 { get; set; }
        [XmlAttribute(AttributeName = "w15", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W15 { get; set; }
    }

    [XmlRoot(ElementName = "webSettings", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class WebSettings
    {
        [XmlElement(ElementName = "optimizeForBrowser", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string OptimizeForBrowser { get; set; }
        [XmlElement(ElementName = "allowPNG", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string AllowPNG { get; set; }
        [XmlAttribute(AttributeName = "Ignorable", Namespace = "http://schemas.openxmlformats.org/markup-compatibility/2006")]
        public string Ignorable { get; set; }
        [XmlAttribute(AttributeName = "mc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mc { get; set; }
        [XmlAttribute(AttributeName = "r", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string R { get; set; }
        [XmlAttribute(AttributeName = "w", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "w14", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W14 { get; set; }
        [XmlAttribute(AttributeName = "w15", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W15 { get; set; }
    }

    [XmlRoot(ElementName = "variant", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
    public class Variant
    {
        [XmlElement(ElementName = "lpstr", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
        public string Lpstr { get; set; }
        [XmlElement(ElementName = "i4", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
        public string I4 { get; set; }
    }

    [XmlRoot(ElementName = "vector", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
    public class Vector
    {
        [XmlElement(ElementName = "variant", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
        public List<Variant> Variant { get; set; }
        [XmlAttribute(AttributeName = "size")]
        public string Size { get; set; }
        [XmlAttribute(AttributeName = "baseType")]
        public string BaseType { get; set; }
        [XmlElement(ElementName = "lpstr", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
        public string Lpstr { get; set; }
    }

    [XmlRoot(ElementName = "HeadingPairs", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
    public class HeadingPairs
    {
        [XmlElement(ElementName = "vector", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
        public Vector Vector { get; set; }
    }

    [XmlRoot(ElementName = "TitlesOfParts", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
    public class TitlesOfParts
    {
        [XmlElement(ElementName = "vector", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
        public Vector Vector { get; set; }
    }

    [XmlRoot(ElementName = "Properties", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
    public class Properties
    {
        [XmlElement(ElementName = "Template", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string Template { get; set; }
        [XmlElement(ElementName = "TotalTime", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string TotalTime { get; set; }
        [XmlElement(ElementName = "Pages", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string Pages { get; set; }
        [XmlElement(ElementName = "Words", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string Words { get; set; }
        [XmlElement(ElementName = "Characters", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string Characters { get; set; }
        [XmlElement(ElementName = "Application", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string Application { get; set; }
        [XmlElement(ElementName = "DocSecurity", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string DocSecurity { get; set; }
        [XmlElement(ElementName = "Lines", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string Lines { get; set; }
        [XmlElement(ElementName = "Paragraphs", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string Paragraphs { get; set; }
        [XmlElement(ElementName = "ScaleCrop", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string ScaleCrop { get; set; }
        [XmlElement(ElementName = "HeadingPairs", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public HeadingPairs HeadingPairs { get; set; }
        [XmlElement(ElementName = "TitlesOfParts", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public TitlesOfParts TitlesOfParts { get; set; }
        [XmlElement(ElementName = "Company", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string Company { get; set; }
        [XmlElement(ElementName = "LinksUpToDate", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string LinksUpToDate { get; set; }
        [XmlElement(ElementName = "CharactersWithSpaces", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string CharactersWithSpaces { get; set; }
        [XmlElement(ElementName = "SharedDoc", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string SharedDoc { get; set; }
        [XmlElement(ElementName = "HyperlinksChanged", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string HyperlinksChanged { get; set; }
        [XmlElement(ElementName = "AppVersion", Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties")]
        public string AppVersion { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "vt", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Vt { get; set; }
    }

    [XmlRoot(ElementName = "created", Namespace = "http://purl.org/dc/terms/")]
    public class Created
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "modified", Namespace = "http://purl.org/dc/terms/")]
    public class Modified
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "coreProperties", Namespace = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties")]
    public class CoreProperties
    {
        [XmlElement(ElementName = "title", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Title { get; set; }
        [XmlElement(ElementName = "subject", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Subject { get; set; }
        [XmlElement(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Creator { get; set; }
        [XmlElement(ElementName = "keywords", Namespace = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties")]
        public string Keywords { get; set; }
        [XmlElement(ElementName = "description", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Description { get; set; }
        [XmlElement(ElementName = "lastModifiedBy", Namespace = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties")]
        public string LastModifiedBy { get; set; }
        [XmlElement(ElementName = "revision", Namespace = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties")]
        public string Revision { get; set; }
        [XmlElement(ElementName = "created", Namespace = "http://purl.org/dc/terms/")]
        public Created Created { get; set; }
        [XmlElement(ElementName = "modified", Namespace = "http://purl.org/dc/terms/")]
        public Modified Modified { get; set; }
        [XmlAttribute(AttributeName = "cp", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Cp { get; set; }
        [XmlAttribute(AttributeName = "dc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dc { get; set; }
        [XmlAttribute(AttributeName = "dcterms", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dcterms { get; set; }
        [XmlAttribute(AttributeName = "dcmitype", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dcmitype { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }

    [XmlRoot(ElementName = "sz", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Sz
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "szCs", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class SzCs
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "lang", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Lang
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
        [XmlAttribute(AttributeName = "eastAsia", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string EastAsia { get; set; }
        [XmlAttribute(AttributeName = "bidi", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Bidi { get; set; }
    }

    [XmlRoot(ElementName = "rPrDefault", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class RPrDefault
    {
        [XmlElement(ElementName = "rPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public RPr RPr { get; set; }
    }

    [XmlRoot(ElementName = "spacing", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Spacing
    {
        [XmlAttribute(AttributeName = "after", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string After { get; set; }
        [XmlAttribute(AttributeName = "line", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Line { get; set; }
        [XmlAttribute(AttributeName = "lineRule", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string LineRule { get; set; }
    }

    [XmlRoot(ElementName = "pPrDefault", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class PPrDefault
    {
        [XmlElement(ElementName = "pPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public PPr PPr { get; set; }
    }

    [XmlRoot(ElementName = "docDefaults", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class DocDefaults
    {
        [XmlElement(ElementName = "rPrDefault", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public RPrDefault RPrDefault { get; set; }
        [XmlElement(ElementName = "pPrDefault", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public PPrDefault PPrDefault { get; set; }
    }

    [XmlRoot(ElementName = "lsdException", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class LsdException
    {
        [XmlAttribute(AttributeName = "name", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "uiPriority", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string UiPriority { get; set; }
        [XmlAttribute(AttributeName = "qFormat", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string QFormat { get; set; }
        [XmlAttribute(AttributeName = "semiHidden", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string SemiHidden { get; set; }
        [XmlAttribute(AttributeName = "unhideWhenUsed", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string UnhideWhenUsed { get; set; }
    }

    [XmlRoot(ElementName = "latentStyles", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class LatentStyles
    {
        [XmlElement(ElementName = "lsdException", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public List<LsdException> LsdException { get; set; }
        [XmlAttribute(AttributeName = "defLockedState", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string DefLockedState { get; set; }
        [XmlAttribute(AttributeName = "defUIPriority", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string DefUIPriority { get; set; }
        [XmlAttribute(AttributeName = "defSemiHidden", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string DefSemiHidden { get; set; }
        [XmlAttribute(AttributeName = "defUnhideWhenUsed", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string DefUnhideWhenUsed { get; set; }
        [XmlAttribute(AttributeName = "defQFormat", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string DefQFormat { get; set; }
        [XmlAttribute(AttributeName = "count", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Count { get; set; }
    }

    [XmlRoot(ElementName = "name", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Name
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "style", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Style
    {
        [XmlElement(ElementName = "name", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Name Name { get; set; }
        [XmlElement(ElementName = "qFormat", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string QFormat { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "default", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Default { get; set; }
        [XmlAttribute(AttributeName = "styleId", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string StyleId { get; set; }
        [XmlElement(ElementName = "uiPriority", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public UiPriority UiPriority { get; set; }
        [XmlElement(ElementName = "semiHidden", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string SemiHidden { get; set; }
        [XmlElement(ElementName = "unhideWhenUsed", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string UnhideWhenUsed { get; set; }
        [XmlElement(ElementName = "tblPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public TblPr TblPr { get; set; }
        [XmlElement(ElementName = "basedOn", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public BasedOn BasedOn { get; set; }
        [XmlElement(ElementName = "link", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Link Link { get; set; }
        [XmlElement(ElementName = "rsid", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Rsid Rsid { get; set; }
        [XmlElement(ElementName = "pPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public PPr PPr { get; set; }
        [XmlElement(ElementName = "rPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public RPr RPr { get; set; }
        [XmlAttribute(AttributeName = "customStyle", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string CustomStyle { get; set; }
    }

    [XmlRoot(ElementName = "uiPriority", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class UiPriority
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "tblInd", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class TblInd
    {
        [XmlAttribute(AttributeName = "w", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "top", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Top
    {
        [XmlAttribute(AttributeName = "w", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "left", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Left
    {
        [XmlAttribute(AttributeName = "w", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "bottom", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Bottom
    {
        [XmlAttribute(AttributeName = "w", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "right", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Right
    {
        [XmlAttribute(AttributeName = "w", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "tblCellMar", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class TblCellMar
    {
        [XmlElement(ElementName = "top", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Top Top { get; set; }
        [XmlElement(ElementName = "left", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Left Left { get; set; }
        [XmlElement(ElementName = "bottom", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Bottom Bottom { get; set; }
        [XmlElement(ElementName = "right", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public Right Right { get; set; }
    }

    [XmlRoot(ElementName = "tblPr", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class TblPr
    {
        [XmlElement(ElementName = "tblInd", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public TblInd TblInd { get; set; }
        [XmlElement(ElementName = "tblCellMar", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public TblCellMar TblCellMar { get; set; }
    }

    [XmlRoot(ElementName = "basedOn", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class BasedOn
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "link", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Link
    {
        [XmlAttribute(AttributeName = "val", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public string Val { get; set; }
    }

    [XmlRoot(ElementName = "styles", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
    public class Styles
    {
        [XmlElement(ElementName = "docDefaults", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public DocDefaults DocDefaults { get; set; }
        [XmlElement(ElementName = "latentStyles", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public LatentStyles LatentStyles { get; set; }
        [XmlElement(ElementName = "style", Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
        public List<Style> Style { get; set; }
        [XmlAttribute(AttributeName = "Ignorable", Namespace = "http://schemas.openxmlformats.org/markup-compatibility/2006")]
        public string Ignorable { get; set; }
        [XmlAttribute(AttributeName = "mc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mc { get; set; }
        [XmlAttribute(AttributeName = "r", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string R { get; set; }
        [XmlAttribute(AttributeName = "w", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W { get; set; }
        [XmlAttribute(AttributeName = "w14", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W14 { get; set; }
        [XmlAttribute(AttributeName = "w15", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string W15 { get; set; }
    }

    [XmlRoot(ElementName = "package", Namespace = "http://schemas.microsoft.com/office/2006/xmlPackage")]
    public class Package
    {
        [XmlElement(ElementName = "part", Namespace = "http://schemas.microsoft.com/office/2006/xmlPackage")]
        public List<Part> Part { get; set; }
        [XmlAttribute(AttributeName = "pkg", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Pkg { get; set; }
    }
}

namespace ESCS.Models.TCT6
{
    // loại mới
    public class Invoice
    {
        [XmlElement(ElementName = "M_ten_Cty")]
        public string M_ten_Cty { get; set; }
        [XmlElement(ElementName = "M_Ten_TCty")]
        public string M_Ten_TCty { get; set; }
        [XmlElement(ElementName = "M_Dia_Chi")]
        public string M_Dia_Chi { get; set; }
        [XmlElement(ElementName = "M_Ma_So_Thue")]
        public string M_Ma_So_Thue { get; set; }
        [XmlElement(ElementName = "StrM_Ma_So_Thue")]
        public string StrM_Ma_So_Thue { get; set; }
        [XmlElement(ElementName = "M_Dien_Thoai")]
        public string M_Dien_Thoai { get; set; }
        [XmlElement(ElementName = "M_Fax")]
        public string M_Fax { get; set; }
        [XmlElement(ElementName = "M_EMail")]
        public string M_EMail { get; set; }
        [XmlElement(ElementName = "M_Ten_GD")]
        public string M_Ten_GD { get; set; }
        [XmlElement(ElementName = "M_Ten_KTT")]
        public string M_Ten_KTT { get; set; }
        [XmlElement(ElementName = "M_WebSite")]
        public string M_WebSite { get; set; }
        [XmlElement(ElementName = "M_Tk_NH")]
        public string M_Tk_NH { get; set; }
        [XmlElement(ElementName = "M_Ten_NH")]
        public string M_Ten_NH { get; set; }
        [XmlElement(ElementName = "M_Tk_NH2")]
        public string M_Tk_NH2 { get; set; }
        [XmlElement(ElementName = "M_Ten_NH2")]
        public string M_Ten_NH2 { get; set; }
        [XmlElement(ElementName = "M_RePortName")]
        public string M_RePortName { get; set; }
        [XmlElement(ElementName = "M_title1")]
        public string M_title1 { get; set; }
        [XmlElement(ElementName = "M_title2")]
        public string M_title2 { get; set; }
        [XmlElement(ElementName = "Stt_Rec")]
        public string Stt_Rec { get; set; }
        [XmlElement(ElementName = "Stt_Rec_TT_DC")]
        public string Stt_Rec_TT_DC { get; set; }
        [XmlElement(ElementName = "ma_Ct")]
        public string Ma_Ct { get; set; }
        [XmlElement(ElementName = "Ma_dv")]
        public string Ma_dv { get; set; }
        [XmlElement(ElementName = "Id_kh")]
        public string Id_kh { get; set; }
        [XmlElement(ElementName = "Ma_Tra_Cuu")]
        public string Ma_Tra_Cuu { get; set; }
        [XmlElement(ElementName = "loai_hd")]
        public string Loai_hd { get; set; }
        [XmlElement(ElementName = "Ma_Post")]
        public string Ma_Post { get; set; }
        [XmlElement(ElementName = "ngay_ct")]
        public string Ngay_ct { get; set; }
        [XmlElement(ElementName = "ngay_lct")]
        public string Ngay_lct { get; set; }
        [XmlElement(ElementName = "Ma_GD")]
        public string Ma_GD { get; set; }
        [XmlElement(ElementName = "Mau_VAT")]
        public string Mau_VAT { get; set; }
        [XmlElement(ElementName = "Ma_Seri")]
        public string Ma_Seri { get; set; }
        [XmlElement(ElementName = "so_ct")]
        public string So_ct { get; set; }
        [XmlElement(ElementName = "Ma_HT")]
        public string Ma_HT { get; set; }
        [XmlElement(ElementName = "Ma_Nhan")]
        public string Ma_Nhan { get; set; }
        [XmlElement(ElementName = "Ten_KH")]
        public string Ten_KH { get; set; }
        [XmlElement(ElementName = "Ma_So_Thue")]
        public string Ma_So_Thue { get; set; }
        [XmlElement(ElementName = "ong_ba")]
        public string Ong_ba { get; set; }
        [XmlElement(ElementName = "Dia_Chi")]
        public string Dia_Chi { get; set; }
        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "Dien_thoai")]
        public string Dien_thoai { get; set; }
        [XmlElement(ElementName = "dien_giai")]
        public string Dien_giai { get; set; }
        [XmlElement(ElementName = "ma_nt")]
        public string Ma_nt { get; set; }
        [XmlElement(ElementName = "ty_gia")]
        public string Ty_gia { get; set; }
        [XmlElement(ElementName = "ma_thue")]
        public string Ma_thue { get; set; }
        [XmlElement(ElementName = "thue_suat")]
        public string Thue_suat { get; set; }
        [XmlElement(ElementName = "t_thue_nt")]
        public string T_thue_nt { get; set; }
        [XmlElement(ElementName = "t_thue")]
        public string T_thue { get; set; }
        [XmlElement(ElementName = "t_tien_nt2")]
        public string T_tien_nt2 { get; set; }
        [XmlElement(ElementName = "t_tien2")]
        public string T_tien2 { get; set; }
        [XmlElement(ElementName = "t_ck_nt")]
        public string T_ck_nt { get; set; }
        [XmlElement(ElementName = "t_ck")]
        public string T_ck { get; set; }
        [XmlElement(ElementName = "t_tt_nt")]
        public string T_tt_nt { get; set; }
        [XmlElement(ElementName = "t_tt")]
        public string T_tt { get; set; }
        [XmlElement(ElementName = "Ngay_Tao")]
        public string Ngay_Tao { get; set; }
        [XmlElement(ElementName = "user_id")]
        public string User_id { get; set; }
        [XmlElement(ElementName = "F1")]
        public string F1 { get; set; }
        [XmlElement(ElementName = "F2")]
        public string F2 { get; set; }
        [XmlElement(ElementName = "F3")]
        public string F3 { get; set; }
        [XmlElement(ElementName = "F4")]
        public string F4 { get; set; }
        [XmlElement(ElementName = "F5")]
        public string F5 { get; set; }
        [XmlElement(ElementName = "F6")]
        public string F6 { get; set; }
        [XmlElement(ElementName = "F7")]
        public string F7 { get; set; }
        [XmlElement(ElementName = "F8")]
        public string F8 { get; set; }
        [XmlElement(ElementName = "F9")]
        public string F9 { get; set; }
        [XmlElement(ElementName = "F10")]
        public string F10 { get; set; }
        [XmlElement(ElementName = "CD")]
        public string CD { get; set; }
        [XmlElement(ElementName = "Ngay_Cd")]
        public string Ngay_Cd { get; set; }
        [XmlElement(ElementName = "Is_SuaThue")]
        public string Is_SuaThue { get; set; }
        [XmlElement(ElementName = "Tk_NH")]
        public string Tk_NH { get; set; }
        [XmlElement(ElementName = "Ten_NH")]
        public string Ten_NH { get; set; }
        [XmlElement(ElementName = "Xmlsigned")]
        public string Xmlsigned { get; set; }
        [XmlElement(ElementName = "Ma_Dc")]
        public string Ma_Dc { get; set; }
        [XmlElement(ElementName = "ly_do_dc")]
        public string Ly_do_dc { get; set; }
        [XmlElement(ElementName = "strThue_Suat")]
        public string StrThue_Suat { get; set; }
        [XmlElement(ElementName = "Bang_Chu")]
        public string Bang_Chu { get; set; }
        [XmlElement(ElementName = "StrMa_So_Thue_KH")]
        public string StrMa_So_Thue_KH { get; set; }
        [XmlElement(ElementName = "strNgay_CT")]
        public string StrNgay_CT { get; set; }
        [XmlElement(ElementName = "strNgay_HT")]
        public string StrNgay_HT { get; set; }
        [XmlElement(ElementName = "Ten_Vat")]
        public string Ten_Vat { get; set; }
        [XmlElement(ElementName = "Ten_Seri")]
        public string Ten_Seri { get; set; }
        [XmlElement(ElementName = "Ten_HT")]
        public string Ten_HT { get; set; }
        [XmlElement(ElementName = "M_Loai_HD")]
        public string M_Loai_HD { get; set; }
        [XmlElement(ElementName = "Bold")]
        public string Bold { get; set; }
        [XmlElement(ElementName = "strNgay_CD")]
        public string StrNgay_CD { get; set; }
        [XmlElement(ElementName = "Comment_CD")]
        public string Comment_CD { get; set; }
        [XmlElement(ElementName = "strCD")]
        public string StrCD { get; set; }
        [XmlElement(ElementName = "Xmlsigned1")]
        public string Xmlsigned1 { get; set; }
        [XmlElement(ElementName = "strNgay_Ky")]
        public string StrNgay_Ky { get; set; }
        [XmlElement(ElementName = "Tien_thueKK")]
        public string Tien_thueKK { get; set; }
        [XmlElement(ElementName = "Tien_thueKT")]
        public string Tien_thueKT { get; set; }
        [XmlElement(ElementName = "Tien_thue0")]
        public string Tien_thue0 { get; set; }
        [XmlElement(ElementName = "Tien_thue5")]
        public string Tien_thue5 { get; set; }
        [XmlElement(ElementName = "Tien_thue10")]
        public string Tien_thue10 { get; set; }
        [XmlElement(ElementName = "Tien_hangKK")]
        public string Tien_hangKK { get; set; }
        [XmlElement(ElementName = "Tien_hangKT")]
        public string Tien_hangKT { get; set; }
        [XmlElement(ElementName = "Tien_hang0")]
        public string Tien_hang0 { get; set; }
        [XmlElement(ElementName = "Tien_hang5")]
        public string Tien_hang5 { get; set; }
        [XmlElement(ElementName = "Tien_hang10")]
        public string Tien_hang10 { get; set; }
        [XmlElement(ElementName = "T_TienKK")]
        public string T_TienKK { get; set; }
        [XmlElement(ElementName = "T_TienKT")]
        public string T_TienKT { get; set; }
        [XmlElement(ElementName = "T_Tien0")]
        public string T_Tien0 { get; set; }
        [XmlElement(ElementName = "T_Tien5")]
        public string T_Tien5 { get; set; }
        [XmlElement(ElementName = "T_Tien10")]
        public string T_Tien10 { get; set; }
        [XmlElement(ElementName = "strDay")]
        public string StrDay { get; set; }
        [XmlElement(ElementName = "strMonth")]
        public string StrMonth { get; set; }
        [XmlElement(ElementName = "strYear")]
        public string StrYear { get; set; }
        [XmlElement(ElementName = "IsKhKy")]
        public string IsKhKy { get; set; }
    }

    [XmlRoot(ElementName = "Products")]
    public class Products
    {
        [XmlElement(ElementName = "Stt_Rec")]
        public string Stt_Rec { get; set; }
        [XmlElement(ElementName = "Stt_Rec0")]
        public string Stt_Rec0 { get; set; }
        [XmlElement(ElementName = "ma_Ct")]
        public string Ma_Ct { get; set; }
        [XmlElement(ElementName = "Id_kh")]
        public string Id_kh { get; set; }
        [XmlElement(ElementName = "Id_Kh0")]
        public string Id_Kh0 { get; set; }
        [XmlElement(ElementName = "Ten_VT")]
        public string Ten_VT { get; set; }
        [XmlElement(ElementName = "ma_vt")]
        public string Ma_vt { get; set; }
        [XmlElement(ElementName = "so_luong")]
        public string So_luong { get; set; }
        [XmlElement(ElementName = "dvt")]
        public string Dvt { get; set; }
        [XmlElement(ElementName = "gia_nt2")]
        public string Gia_nt2 { get; set; }
        [XmlElement(ElementName = "gia2")]
        public string Gia2 { get; set; }
        [XmlElement(ElementName = "tien_nt2")]
        public string Tien_nt2 { get; set; }
        [XmlElement(ElementName = "tien2")]
        public string Tien2 { get; set; }
        [XmlElement(ElementName = "ma_thue")]
        public string Ma_thue { get; set; }
        [XmlElement(ElementName = "thue_suat")]
        public string Thue_suat { get; set; }
        [XmlElement(ElementName = "thue")]
        public string Thue { get; set; }
        [XmlElement(ElementName = "thue_nt")]
        public string Thue_nt { get; set; }
        [XmlElement(ElementName = "tien0")]
        public string Tien0 { get; set; }
        [XmlElement(ElementName = "tien_nt0")]
        public string Tien_nt0 { get; set; }
        [XmlElement(ElementName = "Pt_Ck")]
        public string Pt_Ck { get; set; }
        [XmlElement(ElementName = "ck")]
        public string Ck { get; set; }
        [XmlElement(ElementName = "ck_nt")]
        public string Ck_nt { get; set; }
        [XmlElement(ElementName = "Ma_Lo")]
        public string Ma_Lo { get; set; }
        [XmlElement(ElementName = "Han_SD")]
        public string Han_SD { get; set; }
        [XmlElement(ElementName = "Ghi_Chu")]
        public string Ghi_Chu { get; set; }
        [XmlElement(ElementName = "Td1")]
        public string Td1 { get; set; }
        [XmlElement(ElementName = "Td2")]
        public string Td2 { get; set; }
        [XmlElement(ElementName = "Td3")]
        public string Td3 { get; set; }
        [XmlElement(ElementName = "Td4")]
        public string Td4 { get; set; }
        [XmlElement(ElementName = "Td5")]
        public string Td5 { get; set; }
        [XmlElement(ElementName = "Td6")]
        public string Td6 { get; set; }
        [XmlElement(ElementName = "Td7")]
        public string Td7 { get; set; }
        [XmlElement(ElementName = "Td8")]
        public string Td8 { get; set; }
        [XmlElement(ElementName = "Td9")]
        public string Td9 { get; set; }
        [XmlElement(ElementName = "Td10")]
        public string Td10 { get; set; }
        [XmlElement(ElementName = "Ma_loai")]
        public string Ma_loai { get; set; }
        [XmlElement(ElementName = "STT")]
        public string STT { get; set; }
    }

    [XmlRoot(ElementName = "Invoices")]
    public class Invoices
    {
        [XmlElement(ElementName = "Invoice")]
        public Invoice Invoice { get; set; }
        [XmlElement(ElementName = "Products")]
        public List<Products> Products { get; set; }
    }
}

namespace ESCS.Models.TCT4
{
    [XmlRoot(ElementName = "ThongTinHoaDon")]
    public class ThongTinHoaDon
    {
        [XmlElement(ElementName = "SellerAppRecordId")]
        public string SellerAppRecordId { get; set; }
        [XmlElement(ElementName = "TenHoaDon")]
        public string TenHoaDon { get; set; }
        [XmlElement(ElementName = "LoaiHoaDon")]
        public string LoaiHoaDon { get; set; }
        [XmlElement(ElementName = "MauSo")]
        public string MauSo { get; set; }
        [XmlElement(ElementName = "Kyhieu")]
        public string Kyhieu { get; set; }
        [XmlElement(ElementName = "SoHoaDon")]
        public string SoHoaDon { get; set; }
        [XmlElement(ElementName = "NgayHoaDon")]
        public string NgayHoaDon { get; set; }
        [XmlElement(ElementName = "HinhThucThanhToan")]
        public string HinhThucThanhToan { get; set; }
        [XmlElement(ElementName = "MaTienTe")]
        public string MaTienTe { get; set; }
        [XmlElement(ElementName = "TyGia")]
        public string TyGia { get; set; }
        [XmlElement(ElementName = "TenNguoiBan")]
        public string TenNguoiBan { get; set; }
        [XmlElement(ElementName = "MaSoThueNguoiBan")]
        public string MaSoThueNguoiBan { get; set; }
        [XmlElement(ElementName = "DiaChiNguoiBan")]
        public string DiaChiNguoiBan { get; set; }
        [XmlElement(ElementName = "DienThoaiNguoiBan")]
        public string DienThoaiNguoiBan { get; set; }
        [XmlElement(ElementName = "TenNguoiMua")]
        public string TenNguoiMua { get; set; }
        [XmlElement(ElementName = "TenDoanhNghiepMua")]
        public string TenDoanhNghiepMua { get; set; }
        [XmlElement(ElementName = "MaSoThueNguoiMua")]
        public string MaSoThueNguoiMua { get; set; }
        [XmlElement(ElementName = "DiaChiNguoiMua")]
        public string DiaChiNguoiMua { get; set; }
        [XmlElement(ElementName = "TaiKhoanBenMua")]
        public string TaiKhoanBenMua { get; set; }
        [XmlElement(ElementName = "NganHangBenMua")]
        public string NganHangBenMua { get; set; }
        [XmlElement(ElementName = "SoDonHang")]
        public string SoDonHang { get; set; }
        [XmlElement(ElementName = "TongTienTruocThue")]
        public string TongTienTruocThue { get; set; }
        [XmlElement(ElementName = "TongTienChietKhau")]
        public string TongTienChietKhau { get; set; }
        [XmlElement(ElementName = "TongTienThue")]
        public string TongTienThue { get; set; }
        [XmlElement(ElementName = "TongTien")]
        public string TongTien { get; set; }
        [XmlElement(ElementName = "ThanhTienBangChu")]
        public string ThanhTienBangChu { get; set; }
        [XmlElement(ElementName = "NguoiDaiDienBan")]
        public string NguoiDaiDienBan { get; set; }
        [XmlElement(ElementName = "ChucVuBan")]
        public string ChucVuBan { get; set; }
        [XmlElement(ElementName = "PT_Thue")]
        public string PT_Thue { get; set; }
        [XmlElement(ElementName = "ma_dt")]
        public string Ma_dt { get; set; }
        [XmlElement(ElementName = "TongTienThue10")]
        public string TongTienThue10 { get; set; }
        [XmlElement(ElementName = "TongTienTruocThue10")]
        public string TongTienTruocThue10 { get; set; }
        [XmlElement(ElementName = "sobaomat")]
        public string Sobaomat { get; set; }
    }

    [XmlRoot(ElementName = "ChiTiet")]
    public class ChiTiet
    {
        [XmlElement(ElementName = "stt")]
        public string Stt { get; set; }
        [XmlElement(ElementName = "MaHang")]
        public string MaHang { get; set; }
        [XmlElement(ElementName = "TenHang")]
        public string TenHang { get; set; }
        [XmlElement(ElementName = "TienTruocThue")]
        public string TienTruocThue { get; set; }
        [XmlElement(ElementName = "TienChietKhau")]
        public string TienChietKhau { get; set; }
        [XmlElement(ElementName = "PhanTramThue")]
        public string PhanTramThue { get; set; }
        [XmlElement(ElementName = "TienThue")]
        public string TienThue { get; set; }
        [XmlElement(ElementName = "ThanhTien")]
        public string ThanhTien { get; set; }
    }

    [XmlRoot(ElementName = "ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [XmlElement(ElementName = "ChiTiet")]
        public List<ChiTiet> ChiTiet { get; set; }
    }

    [XmlRoot(ElementName = "HoaDon")]
    public class HoaDon
    {
        [XmlElement(ElementName = "ThongTinHoaDon")]
        public ThongTinHoaDon ThongTinHoaDon { get; set; }
        [XmlElement(ElementName = "ChiTietHoaDon")]
        public ChiTietHoaDon ChiTietHoaDon { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "CertifiedData")]
    public class CertifiedData
    {
        [XmlElement(ElementName = "qrCodeData")]
        public string QrCodeData { get; set; }
        [XmlElement(ElementName = "ChuKySo")]
        public string ChuKySo { get; set; }
        [XmlElement(ElementName = "SoBaoMat")]
        public string SoBaoMat { get; set; }
        [XmlElement(ElementName = "NguoiKy")]
        public string NguoiKy { get; set; }
        [XmlElement(ElementName = "NgayKy")]
        public string NgayKy { get; set; }
        [XmlElement(ElementName = "DonViKy")]
        public string DonViKy { get; set; }
        [XmlElement(ElementName = "DonViPhanMem")]
        public string DonViPhanMem { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<Transform> Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509SubjectName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SubjectName { get; set; }
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
        [XmlElement(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyValue KeyValue { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class RSAKeyValue
    {
        [XmlElement(ElementName = "Modulus", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Modulus { get; set; }
        [XmlElement(ElementName = "Exponent", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Exponent { get; set; }
    }

    [XmlRoot(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyValue
    {
        [XmlElement(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public RSAKeyValue RSAKeyValue { get; set; }
    }

    [XmlRoot(ElementName = "Signatures")]
    public class Signatures
    {
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<Signature> Signature { get; set; }
    }

    [XmlRoot(ElementName = "HoaDonDienTu")]
    public class HoaDonDienTu
    {
        [XmlElement(ElementName = "HoaDon")]
        public HoaDon HoaDon { get; set; }
        [XmlElement(ElementName = "CertifiedData")]
        public CertifiedData CertifiedData { get; set; }
        [XmlElement(ElementName = "Signatures")]
        public Signatures Signatures { get; set; }
    }
}

namespace ESCS.Models.TCT7
{
    [XmlRoot(ElementName = "CusivoiceTempt", Namespace = "http://tempuri.org/Invoice1.xsd")]
    public class CusivoiceTempt
    {
        [XmlElement(ElementName = "IvoiceType", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string IvoiceType { get; set; }
        [XmlElement(ElementName = "CompanyName", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string CompanyName { get; set; }
        [XmlElement(ElementName = "Taxcode", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Taxcode { get; set; }
        [XmlElement(ElementName = "Address", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Address { get; set; }
        [XmlElement(ElementName = "PhoneNumber", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string PhoneNumber { get; set; }
        [XmlElement(ElementName = "AccBank", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string AccBank { get; set; }
        [XmlElement(ElementName = "TemptCode", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string TemptCode { get; set; }
        [XmlElement(ElementName = "Symbol", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Symbol { get; set; }
        [XmlElement(ElementName = "CompanySign", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string CompanySign { get; set; }
        [XmlElement(ElementName = "Email", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Email { get; set; }
        [XmlElement(ElementName = "Website", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Website { get; set; }
    }

    [XmlRoot(ElementName = "CusivoiceTemptFieldName", Namespace = "http://tempuri.org/Invoice1.xsd")]
    public class CusivoiceTemptFieldName
    {
        [XmlElement(ElementName = "PhoneNumber", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string PhoneNumber { get; set; }
        [XmlElement(ElementName = "AccBank", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string AccBank { get; set; }
        [XmlElement(ElementName = "PhoneNumber1", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string PhoneNumber1 { get; set; }
        [XmlElement(ElementName = "AccBank1", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string AccBank1 { get; set; }
    }

    [XmlRoot(ElementName = "Ivoice", Namespace = "http://tempuri.org/Invoice1.xsd")]
    public class Ivoice
    {
        [XmlElement(ElementName = "InvoiceNumber", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string InvoiceNumber { get; set; }
        [XmlElement(ElementName = "DateofInvoice", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string DateofInvoice { get; set; }
        [XmlElement(ElementName = "ExchangeRate", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string ExchangeRate { get; set; }
        [XmlElement(ElementName = "CompanyName", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string CompanyName { get; set; }
        [XmlElement(ElementName = "CompanyTaxcode", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string CompanyTaxcode { get; set; }
        [XmlElement(ElementName = "CompanyAdd", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string CompanyAdd { get; set; }
        [XmlElement(ElementName = "AccBank", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string AccBank { get; set; }
        [XmlElement(ElementName = "TotalMoneyNoTax", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string TotalMoneyNoTax { get; set; }
        [XmlElement(ElementName = "Tax", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Tax { get; set; }
        [XmlElement(ElementName = "MoneyTax", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string MoneyTax { get; set; }
        [XmlElement(ElementName = "TotalMoney", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string TotalMoney { get; set; }
        [XmlElement(ElementName = "PaymentTerm", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string PaymentTerm { get; set; }
        [XmlElement(ElementName = "DateofSign", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string DateofSign { get; set; }
        [XmlElement(ElementName = "Status", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Status { get; set; }
        [XmlElement(ElementName = "TextTotalMoney", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string TextTotalMoney { get; set; }
        [XmlElement(ElementName = "ContractName", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string ContractName { get; set; }
        [XmlElement(ElementName = "IvoiceCode", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string IvoiceCode { get; set; }
    }

    [XmlRoot(ElementName = "IvoiceDetail", Namespace = "http://tempuri.org/Invoice1.xsd")]
    public class IvoiceDetail
    {
        [XmlElement(ElementName = "STT", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string STT { get; set; }
        [XmlElement(ElementName = "ProductName", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string ProductName { get; set; }
        [XmlElement(ElementName = "Unit", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Unit { get; set; }
        [XmlElement(ElementName = "Number", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Number { get; set; }
        [XmlElement(ElementName = "Price", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public string Price { get; set; }
        [XmlElement(ElementName = "TotalMoney", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public TotalMoney TotalMoney { get; set; }
    }

    [XmlRoot(ElementName = "TotalMoney", Namespace = "http://tempuri.org/Invoice1.xsd")]
    public class TotalMoney
    {
        [XmlAttribute(AttributeName = "space", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Space { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "Invoice1", Namespace = "http://tempuri.org/Invoice1.xsd")]
    public class Invoice1
    {
        [XmlElement(ElementName = "CusivoiceTempt", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public CusivoiceTempt CusivoiceTempt { get; set; }
        [XmlElement(ElementName = "CusivoiceTemptFieldName", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public CusivoiceTemptFieldName CusivoiceTemptFieldName { get; set; }
        [XmlElement(ElementName = "Ivoice", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public Ivoice Ivoice { get; set; }
        [XmlElement(ElementName = "IvoiceDetail", Namespace = "http://tempuri.org/Invoice1.xsd")]
        public List<IvoiceDetail> IvoiceDetail { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}

namespace ESCS.Models.TCT8
{
    [XmlRoot(ElementName = "Hoadon")]
    public class Hoadon
    {
        [XmlElement(ElementName = "Stt")]
        public string Stt { get; set; }
        [XmlElement(ElementName = "ma_ct")]
        public string Ma_ct { get; set; }
        [XmlElement(ElementName = "Ma_loaihd")]
        public string Ma_loaihd { get; set; }
        [XmlElement(ElementName = "Ten_loaihd")]
        public string Ten_loaihd { get; set; }
        [XmlElement(ElementName = "Ky_hieu_mau")]
        public string Ky_hieu_mau { get; set; }
        [XmlElement(ElementName = "So_seri")]
        public string So_seri { get; set; }
        [XmlElement(ElementName = "So_hd")]
        public string So_hd { get; set; }
        [XmlElement(ElementName = "Ngay_hd")]
        public string Ngay_hd { get; set; }
        [XmlElement(ElementName = "Ma_tte")]
        public string Ma_tte { get; set; }
        [XmlElement(ElementName = "Ty_gia")]
        public string Ty_gia { get; set; }
        [XmlElement(ElementName = "User_log")]
        public string User_log { get; set; }
        [XmlElement(ElementName = "Tien_hang")]
        public string Tien_hang { get; set; }
        [XmlElement(ElementName = "Tien_thue")]
        public string Tien_thue { get; set; }
        [XmlElement(ElementName = "Chiet_khau")]
        public string Chiet_khau { get; set; }
        [XmlElement(ElementName = "Tong_tien")]
        public string Tong_tien { get; set; }
        [XmlElement(ElementName = "Tinhtrang")]
        public string Tinhtrang { get; set; }
        [XmlElement(ElementName = "Ten_tinhtrang")]
        public string Ten_tinhtrang { get; set; }
        [XmlElement(ElementName = "Don_vi_ban")]
        public string Don_vi_ban { get; set; }
        [XmlElement(ElementName = "Mst")]
        public string Mst { get; set; }
        [XmlElement(ElementName = "Dia_chi")]
        public string Dia_chi { get; set; }
        [XmlElement(ElementName = "Dien_thoai")]
        public string Dien_thoai { get; set; }
        [XmlElement(ElementName = "So_tk")]
        public string So_tk { get; set; }
        [XmlElement(ElementName = "Ngan_hang")]
        public string Ngan_hang { get; set; }
        [XmlElement(ElementName = "Nguoi_mua")]
        public string Nguoi_mua { get; set; }
        [XmlElement(ElementName = "Don_vi_mua")]
        public string Don_vi_mua { get; set; }
        [XmlElement(ElementName = "Mst_nguoi_mua")]
        public string Mst_nguoi_mua { get; set; }
        [XmlElement(ElementName = "Dia_chi_nguoi_mua")]
        public string Dia_chi_nguoi_mua { get; set; }
        [XmlElement(ElementName = "Dien_thoai_nguoi_mua")]
        public string Dien_thoai_nguoi_mua { get; set; }
        [XmlElement(ElementName = "Email_nguoi_mua")]
        public string Email_nguoi_mua { get; set; }
        [XmlElement(ElementName = "Httt")]
        public string Httt { get; set; }
        [XmlElement(ElementName = "So_tk_nguoi_mua")]
        public string So_tk_nguoi_mua { get; set; }
        [XmlElement(ElementName = "Stt_hopdong")]
        public string Stt_hopdong { get; set; }
        [XmlElement(ElementName = "So_hop_dong")]
        public string So_hop_dong { get; set; }
        [XmlElement(ElementName = "Ref")]
        public string Ref { get; set; }
        [XmlElement(ElementName = "Reftype")]
        public string Reftype { get; set; }
        [XmlElement(ElementName = "Ngay_chuyen_doi")]
        public string Ngay_chuyen_doi { get; set; }
        [XmlElement(ElementName = "Ky_hieu_mau_dctt")]
        public string Ky_hieu_mau_dctt { get; set; }
        [XmlElement(ElementName = "So_seri_dctt")]
        public string So_seri_dctt { get; set; }
        [XmlElement(ElementName = "So_ct_dctt")]
        public string So_ct_dctt { get; set; }
        [XmlElement(ElementName = "Ngay_ct_dctt")]
        public string Ngay_ct_dctt { get; set; }
        [XmlElement(ElementName = "Lydohuy")]
        public string Lydohuy { get; set; }
        [XmlElement(ElementName = "Ngay_nguoi_mua_ky")]
        public string Ngay_nguoi_mua_ky { get; set; }
    }

    [XmlRoot(ElementName = "Chitiethoadon")]
    public class Chitiethoadon
    {
        [XmlElement(ElementName = "Stt")]
        public string Stt { get; set; }
        [XmlElement(ElementName = "Stt0")]
        public string Stt0 { get; set; }
        [XmlElement(ElementName = "Ma_vt")]
        public string Ma_vt { get; set; }
        [XmlElement(ElementName = "Dien_giai")]
        public string Dien_giai { get; set; }
        [XmlElement(ElementName = "Dvt")]
        public string Dvt { get; set; }
        [XmlElement(ElementName = "So_luong")]
        public string So_luong { get; set; }
        [XmlElement(ElementName = "Gia")]
        public string Gia { get; set; }
        [XmlElement(ElementName = "Tien")]
        public string Tien { get; set; }
        [XmlElement(ElementName = "Ma_thue")]
        public string Ma_thue { get; set; }
        [XmlElement(ElementName = "Thue_gtgt")]
        public string Thue_gtgt { get; set; }
        [XmlElement(ElementName = "Thue")]
        public string Thue { get; set; }
        [XmlElement(ElementName = "Chiet_khau")]
        public string Chiet_khau { get; set; }
        [XmlElement(ElementName = "Tien_chiet_khau")]
        public string Tien_chiet_khau { get; set; }
        [XmlElement(ElementName = "Khuyen_mai")]
        public string Khuyen_mai { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class RSAKeyValue
    {
        [XmlElement(ElementName = "Modulus", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Modulus { get; set; }
        [XmlElement(ElementName = "Exponent", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Exponent { get; set; }
    }

    [XmlRoot(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyValue
    {
        [XmlElement(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public RSAKeyValue RSAKeyValue { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
        [XmlElement(ElementName = "KeyName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string KeyName { get; set; }
        [XmlElement(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyValue KeyValue { get; set; }
    }

    [XmlRoot(ElementName = "Subject")]
    public class Subject
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Issuer")]
    public class Issuer
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "From")]
    public class From
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "To")]
    public class To
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Serial")]
    public class Serial
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Public")]
    public class Public
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Hash")]
    public class Hash
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Algorithm")]
    public class Algorithm
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlElement(ElementName = "Subject")]
        public Subject Subject { get; set; }
        [XmlElement(ElementName = "Issuer")]
        public Issuer Issuer { get; set; }
        [XmlElement(ElementName = "From")]
        public From From { get; set; }
        [XmlElement(ElementName = "To")]
        public To To { get; set; }
        [XmlElement(ElementName = "Serial")]
        public Serial Serial { get; set; }
        [XmlElement(ElementName = "Public")]
        public Public Public { get; set; }
        [XmlElement(ElementName = "Hash")]
        public Hash Hash { get; set; }
        [XmlElement(ElementName = "Algorithm")]
        public Algorithm Algorithm { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "Vht")]
    public class Vht
    {
        [XmlElement(ElementName = "Hoadon")]
        public Hoadon Hoadon { get; set; }
        [XmlElement(ElementName = "Chitiethoadon")]
        public List<Chitiethoadon> Chitiethoadon { get; set; }
        //   public Chitiethoadon Chitiethoadon { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

}

namespace ESCS.Models.TCT10
{
    [XmlRoot(ElementName = "PhInvoice")]
    public class PhInvoice
    {
        [XmlElement(ElementName = "STT_REC")]
        public string STT_REC { get; set; }
        [XmlElement(ElementName = "MA_DV")]
        public string MA_DV { get; set; }
        [XmlElement(ElementName = "MA_CT")]
        public string MA_CT { get; set; }
        [XmlElement(ElementName = "TEN_CT")]
        public string TEN_CT { get; set; }
        [XmlElement(ElementName = "NGAY_CT")]
        public string NGAY_CT { get; set; }
        [XmlElement(ElementName = "NGAY_LCT")]
        public string NGAY_LCT { get; set; }
        [XmlElement(ElementName = "TEN_VAT")]
        public string TEN_VAT { get; set; }
        [XmlElement(ElementName = "TEN_SERI")]
        public string TEN_SERI { get; set; }
        [XmlElement(ElementName = "SO_CT")]
        public string SO_CT { get; set; }
        [XmlElement(ElementName = "PAYMENTMETHODNAME")]
        public string PAYMENTMETHODNAME { get; set; }
        [XmlElement(ElementName = "SALERLEGALNAME")]
        public string SALERLEGALNAME { get; set; }
        [XmlElement(ElementName = "SALERTAXCODE")]
        public string SALERTAXCODE { get; set; }
        [XmlElement(ElementName = "SALERADDRESSLINE")]
        public string SALERADDRESSLINE { get; set; }
        [XmlElement(ElementName = "TEN_KH")]
        public string TEN_KH { get; set; }
        [XmlElement(ElementName = "MA_SO_THUE")]
        public string MA_SO_THUE { get; set; }
        [XmlElement(ElementName = "ONG_BA")]
        public string ONG_BA { get; set; }
        [XmlElement(ElementName = "DIA_CHI")]
        public string DIA_CHI { get; set; }
        [XmlElement(ElementName = "EMAIL")]
        public string EMAIL { get; set; }
        [XmlElement(ElementName = "DIEN_THOAI")]
        public string DIEN_THOAI { get; set; }
        [XmlElement(ElementName = "TK_NH")]
        public string TK_NH { get; set; }
        [XmlElement(ElementName = "TEN_NH")]
        public string TEN_NH { get; set; }
        [XmlElement(ElementName = "DIEN_GIAI")]
        public string DIEN_GIAI { get; set; }
        [XmlElement(ElementName = "MA_NT")]
        public string MA_NT { get; set; }
        [XmlElement(ElementName = "TY_GIA")]
        public string TY_GIA { get; set; }
        [XmlElement(ElementName = "T_THUE_NT")]
        public string T_THUE_NT { get; set; }
        [XmlElement(ElementName = "T_TIEN_NT2")]
        public string T_TIEN_NT2 { get; set; }
        [XmlElement(ElementName = "T_CK_NT")]
        public string T_CK_NT { get; set; }
        [XmlElement(ElementName = "T_TT_NT")]
        public string T_TT_NT { get; set; }
        [XmlElement(ElementName = "NGAY_TAO")]
        public string NGAY_TAO { get; set; }
        [XmlElement(ElementName = "F1")]
        public string F1 { get; set; }
        [XmlElement(ElementName = "F2")]
        public string F2 { get; set; }
        [XmlElement(ElementName = "F3")]
        public string F3 { get; set; }
        [XmlElement(ElementName = "F4")]
        public string F4 { get; set; }
        [XmlElement(ElementName = "F5")]
        public string F5 { get; set; }
        [XmlElement(ElementName = "F6")]
        public string F6 { get; set; }
        [XmlElement(ElementName = "F7")]
        public string F7 { get; set; }
        [XmlElement(ElementName = "F8")]
        public string F8 { get; set; }
        [XmlElement(ElementName = "F9")]
        public string F9 { get; set; }
        [XmlElement(ElementName = "F10")]
        public string F10 { get; set; }
    }

    [XmlRoot(ElementName = "PhInvoices")]
    public class PhInvoices
    {
        [XmlElement(ElementName = "PhInvoice")]
        public PhInvoice PhInvoice { get; set; }
    }

    [XmlRoot(ElementName = "CtInvoice")]
    public class CtInvoice
    {
        [XmlElement(ElementName = "STT_REC")]
        public string STT_REC { get; set; }
        [XmlElement(ElementName = "STT_REC0")]
        public string STT_REC0 { get; set; }
        [XmlElement(ElementName = "MA_VT")]
        public string MA_VT { get; set; }
        [XmlElement(ElementName = "TEN_VT")]
        public string TEN_VT { get; set; }
        [XmlElement(ElementName = "DVT")]
        public string DVT { get; set; }
        [XmlElement(ElementName = "GIA_NT2")]
        public string GIA_NT2 { get; set; }
        [XmlElement(ElementName = "TIEN_NT2")]
        public string TIEN_NT2 { get; set; }
        [XmlElement(ElementName = "THUE_SUAT")]
        public string THUE_SUAT { get; set; }
        [XmlElement(ElementName = "THUE_NT")]
        public string THUE_NT { get; set; }
        [XmlElement(ElementName = "PT_CK")]
        public string PT_CK { get; set; }
        [XmlElement(ElementName = "CK_NT")]
        public string CK_NT { get; set; }
        [XmlElement(ElementName = "MA_LO")]
        public string MA_LO { get; set; }
        [XmlElement(ElementName = "HAN_SD")]
        public string HAN_SD { get; set; }
        [XmlElement(ElementName = "TD1")]
        public string TD1 { get; set; }
        [XmlElement(ElementName = "TD2")]
        public string TD2 { get; set; }
        [XmlElement(ElementName = "TD3")]
        public string TD3 { get; set; }
        [XmlElement(ElementName = "TD4")]
        public string TD4 { get; set; }
        [XmlElement(ElementName = "TD5")]
        public string TD5 { get; set; }
        [XmlElement(ElementName = "TD6")]
        public string TD6 { get; set; }
        [XmlElement(ElementName = "TD7")]
        public string TD7 { get; set; }
        [XmlElement(ElementName = "TD8")]
        public string TD8 { get; set; }
        [XmlElement(ElementName = "TD9")]
        public string TD9 { get; set; }
        [XmlElement(ElementName = "TD10")]
        public string TD10 { get; set; }
    }

    [XmlRoot(ElementName = "CtInvoices")]
    public class CtInvoices
    {
        [XmlElement(ElementName = "CtInvoice")]
        public List<CtInvoice> CtInvoice { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509IssuerSerial", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509IssuerSerial
    {
        [XmlElement(ElementName = "X509IssuerName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509IssuerName { get; set; }
        [XmlElement(ElementName = "X509SerialNumber", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SerialNumber { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509IssuerSerial", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509IssuerSerial X509IssuerSerial { get; set; }
        [XmlElement(ElementName = "X509SubjectName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SubjectName { get; set; }
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "CkyDtDv")]
    public class CkyDtDv
    {
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

    [XmlRoot(ElementName = "CKyDTu")]
    public class CKyDTu
    {
        [XmlElement(ElementName = "CkyDtDv")]
        public CkyDtDv CkyDtDv { get; set; }
        [XmlElement(ElementName = "CkyDtkh")]
        public string CkyDtkh { get; set; }
    }

    [XmlRoot(ElementName = "CyberInvoiceData")]
    public class CyberInvoiceData
    {
        [XmlElement(ElementName = "PhInvoices")]
        public PhInvoices PhInvoices { get; set; }
        [XmlElement(ElementName = "CtInvoices")]
        public CtInvoices CtInvoices { get; set; }
        [XmlElement(ElementName = "CKyDTu")]
        public CKyDTu CKyDTu { get; set; }
    }
}

namespace ESCS.Models.TCT11
{
    [XmlRoot(ElementName = "ObjTempInvoice", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class ObjTempInvoice
    {
        [XmlElement(ElementName = "FormNo", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string FormNo { get; set; }
        [XmlElement(ElementName = "Sign", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string Sign { get; set; }
        [XmlElement(ElementName = "LogoFilePath", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string LogoFilePath { get; set; }
        [XmlElement(ElementName = "WatermarkFilePath", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string WatermarkFilePath { get; set; }
        [XmlElement(ElementName = "TInvoiceCode", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string TInvoiceCode { get; set; }
        [XmlElement(ElementName = "TInvoiceName", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string TInvoiceName { get; set; }
        [XmlElement(ElementName = "InvoiceTGroupCode", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string InvoiceTGroupCode { get; set; }
        [XmlElement(ElementName = "VATType", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string VATType { get; set; }
    }

    [XmlRoot(ElementName = "ObjInvoice", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class ObjInvoice
    {
        [XmlElement(ElementName = "InvoiceCode", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string InvoiceCode { get; set; }
        [XmlElement(ElementName = "InvoiceNo", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string InvoiceNo { get; set; }
        [XmlElement(ElementName = "InvoiceDateUTC", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string InvoiceDateUTC { get; set; }
        [XmlElement(ElementName = "mpm_PaymentMethodName", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string Mpm_PaymentMethodName { get; set; }
        [XmlElement(ElementName = "FlagQRCode", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string FlagQRCode { get; set; }
        [XmlElement(ElementName = "itg_InvoiceTGroupCode", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string Itg_InvoiceTGroupCode { get; set; }
        [XmlElement(ElementName = "itg_Spec_Prd_Type", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string Itg_Spec_Prd_Type { get; set; }
        [XmlElement(ElementName = "SourceInvoiceCode", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string SourceInvoiceCode { get; set; }
        [XmlElement(ElementName = "InvoiceAdjType", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string InvoiceAdjType { get; set; }
        [XmlElement(ElementName = "TongTienChuaThue", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string TongTienChuaThue { get; set; }
        [XmlElement(ElementName = "TongTienThue", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string TongTienThue { get; set; }
        [XmlElement(ElementName = "TongTienCoThue", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string TongTienCoThue { get; set; }
        [XmlElement(ElementName = "SoTienBangChu", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string SoTienBangChu { get; set; }
    }

    [XmlRoot(ElementName = "ObjMstNNT", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class ObjMstNNT
    {
        [XmlElement(ElementName = "NNTFullName", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string NNTFullName { get; set; }
        [XmlElement(ElementName = "MST", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string MST { get; set; }
        [XmlElement(ElementName = "mp_ProvinceName", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string Mp_ProvinceName { get; set; }
        [XmlElement(ElementName = "NNTAddress", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string NNTAddress { get; set; }
        [XmlElement(ElementName = "NNTPhone", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string NNTPhone { get; set; }
        [XmlElement(ElementName = "NNTFax", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string NNTFax { get; set; }
        [XmlElement(ElementName = "Website", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string Website { get; set; }
        [XmlElement(ElementName = "ContactPhone", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string ContactPhone { get; set; }
        [XmlElement(ElementName = "ContactName", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string ContactName { get; set; }
        [XmlElement(ElementName = "ContactEmail", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string ContactEmail { get; set; }
        [XmlElement(ElementName = "AccNo", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string AccNo { get; set; }
    }

    [XmlRoot(ElementName = "ObjMstCustomerNNT", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class ObjMstCustomerNNT
    {
        [XmlElement(ElementName = "ContactName", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string ContactName { get; set; }
        [XmlElement(ElementName = "CustomerNNTName", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string CustomerNNTName { get; set; }
        [XmlElement(ElementName = "MST", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string MST { get; set; }
        [XmlElement(ElementName = "CustomerNNTAddress", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string CustomerNNTAddress { get; set; }
        [XmlElement(ElementName = "AccNo", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string AccNo { get; set; }
    }

    [XmlRoot(ElementName = "ObjInvoiceDtl", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class ObjInvoiceDtl
    {
        [XmlElement(ElementName = "STT", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string STT { get; set; }
        [XmlElement(ElementName = "ms_SpecName", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string Ms_SpecName { get; set; }
        [XmlElement(ElementName = "UnitCode", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string UnitCode { get; set; }
        [XmlElement(ElementName = "Qty", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string Qty { get; set; }
        [XmlElement(ElementName = "UnitPrice", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string UnitPrice { get; set; }
        [XmlElement(ElementName = "VATRate", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string VATRate { get; set; }
        [XmlElement(ElementName = "ThanhTien", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string ThanhTien { get; set; }
    }

    [XmlRoot(ElementName = "ListInvoiceDtl", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class ListInvoiceDtl
    {
        [XmlElement(ElementName = "ObjInvoiceDtl", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public List<ObjInvoiceDtl> ObjInvoiceDtl { get; set; }
    }

    [XmlRoot(ElementName = "InvoiceTemplate", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class InvoiceTemplate
    {
        [XmlElement(ElementName = "ObjTempInvoice", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public ObjTempInvoice ObjTempInvoice { get; set; }
        [XmlElement(ElementName = "ObjInvoiceRefNo", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public string ObjInvoiceRefNo { get; set; }
        [XmlElement(ElementName = "ObjInvoice", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public ObjInvoice ObjInvoice { get; set; }
        [XmlElement(ElementName = "ObjMstNNT", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public ObjMstNNT ObjMstNNT { get; set; }
        [XmlElement(ElementName = "ObjMstCustomerNNT", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public ObjMstCustomerNNT ObjMstCustomerNNT { get; set; }
        [XmlElement(ElementName = "ListInvoiceDtl", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public ListInvoiceDtl ListInvoiceDtl { get; set; }
        //public List<ListInvoiceDtl> ListInvoiceDtl { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<Transform> Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class RSAKeyValue
    {
        [XmlElement(ElementName = "Modulus", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Modulus { get; set; }
        [XmlElement(ElementName = "Exponent", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Exponent { get; set; }
    }

    [XmlRoot(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyValue
    {
        [XmlElement(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public RSAKeyValue RSAKeyValue { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509SubjectName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SubjectName { get; set; }
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyValue KeyValue { get; set; }
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "CKyDTu", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class CKyDTu
    {
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

    [XmlRoot(ElementName = "Invoice", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
    public class Invoice
    {
        [XmlElement(ElementName = "InvoiceTemplate", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public InvoiceTemplate InvoiceTemplate { get; set; }
        [XmlElement(ElementName = "CKyDTu", Namespace = "http://kekhaithue.gdt.gov.vn/HSoDKy")]
        public CKyDTu CKyDTu { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}

namespace ESCS.Models.TCT12
{
    [XmlRoot(ElementName = "TTChung")]
    public class TTChung
    {
        [XmlElement(ElementName = "PBan")]
        public string PBan { get; set; }
        [XmlElement(ElementName = "THDon")]
        public string THDon { get; set; }
        [XmlElement(ElementName = "KHMSHDon")]
        public string KHMSHDon { get; set; }
        [XmlElement(ElementName = "KHHDon")]
        public string KHHDon { get; set; }
        [XmlElement(ElementName = "SHDon")]
        public string SHDon { get; set; }
        [XmlElement(ElementName = "TDLap")]
        public string TDLap { get; set; }
        [XmlElement(ElementName = "TChat")]
        public string TChat { get; set; }
        [XmlElement(ElementName = "DVTTe")]
        public string DVTTe { get; set; }
        [XmlElement(ElementName = "TGia")]
        public string TGia { get; set; }
    }

    [XmlRoot(ElementName = "TTin")]
    public class TTin
    {
        [XmlElement(ElementName = "TTruong")]
        public string TTruong { get; set; }
        [XmlElement(ElementName = "KDLieu")]
        public string KDLieu { get; set; }
        [XmlElement(ElementName = "DLieu")]
        public string DLieu { get; set; }
    }

    [XmlRoot(ElementName = "TTKhac")]
    public class TTKhac
    {
        [XmlElement(ElementName = "TTin")]
        public TTin TTin { get; set; }
    }

    [XmlRoot(ElementName = "NBan")]
    public class NBan
    {
        [XmlElement(ElementName = "Ten")]
        public string Ten { get; set; }
        [XmlElement(ElementName = "MST")]
        public string MST { get; set; }
        [XmlElement(ElementName = "DChi")]
        public string DChi { get; set; }
        [XmlElement(ElementName = "TTKhac")]
        public TTKhac TTKhac { get; set; }
    }

    [XmlRoot(ElementName = "NMua")]
    public class NMua
    {
        [XmlElement(ElementName = "Ten")]
        public string Ten { get; set; }
        [XmlElement(ElementName = "MST")]
        public string MST { get; set; }
        [XmlElement(ElementName = "DChi")]
        public string DChi { get; set; }
        [XmlElement(ElementName = "TTKhac")]
        public TTKhac TTKhac { get; set; }
    }

    [XmlRoot(ElementName = "HHDVu")]
    public class HHDVu
    {
        [XmlElement(ElementName = "TChat")]
        public string TChat { get; set; }
        [XmlElement(ElementName = "STT")]
        public string STT { get; set; }
        [XmlElement(ElementName = "Ten")]
        public string Ten { get; set; }
        [XmlElement(ElementName = "DVTinh")]
        public string DVTinh { get; set; }
        [XmlElement(ElementName = "SLuong")]
        public string SLuong { get; set; }
        [XmlElement(ElementName = "DGia")]
        public string DGia { get; set; }
        [XmlElement(ElementName = "TLCKhau")]
        public string TLCKhau { get; set; }
        [XmlElement(ElementName = "STCKhau")]
        public string STCKhau { get; set; }
        [XmlElement(ElementName = "ThTien")]
        public string ThTien { get; set; }
        [XmlElement(ElementName = "TSuat")]
        public string TSuat { get; set; }
    }

    [XmlRoot(ElementName = "DSHHDVu")]
    public class DSHHDVu
    {
        [XmlElement(ElementName = "HHDVu")]
        //public HHDVu HHDVu { get; set; }
        public List<HHDVu> HHDVu { get; set; }
    }

    [XmlRoot(ElementName = "THTTLTSuat")]
    public class THTTLTSuat
    {
        [XmlElement(ElementName = "TgTThue")]
        public string TgTThue { get; set; }
    }

    [XmlRoot(ElementName = "TToan")]
    public class TToan
    {
        [XmlElement(ElementName = "TgTCThue")]
        public string TgTCThue { get; set; }
        [XmlElement(ElementName = "TgTThue")]
        public string TgTThue { get; set; }
        [XmlElement(ElementName = "TLPhi")]
        public string TLPhi { get; set; }
        [XmlElement(ElementName = "TPhi")]
        public string TPhi { get; set; }
        [XmlElement(ElementName = "TTCKTMai")]
        public string TTCKTMai { get; set; }
        [XmlElement(ElementName = "TgTTTBSo")]
        public string TgTTTBSo { get; set; }
        [XmlElement(ElementName = "TgTTTBChu")]
        public string TgTTTBChu { get; set; }
        [XmlElement(ElementName = "THTTLTSuat")]
        public THTTLTSuat THTTLTSuat { get; set; }
    }

    [XmlRoot(ElementName = "NDHDon")]
    public class NDHDon
    {
        [XmlElement(ElementName = "NBan")]
        public NBan NBan { get; set; }
        [XmlElement(ElementName = "NMua")]
        public NMua NMua { get; set; }
        [XmlElement(ElementName = "DSHHDVu")]
        public DSHHDVu DSHHDVu { get; set; }
        [XmlElement(ElementName = "TToan")]
        public TToan TToan { get; set; }
    }

    [XmlRoot(ElementName = "DLHDon")]
    public class DLHDon
    {
        [XmlElement(ElementName = "TTChung")]
        public TTChung TTChung { get; set; }
        [XmlElement(ElementName = "NDHDon")]
        public NDHDon NDHDon { get; set; }
        [XmlElement(ElementName = "TTKhac")]
        public TTKhac TTKhac { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "DSCKS")]
    public class DSCKS
    {
        [XmlElement(ElementName = "NBan")]
        public string NBan { get; set; }
        [XmlElement(ElementName = "NMua")]
        public string NMua { get; set; }
        [XmlElement(ElementName = "CCKSKhac")]
        public string CCKSKhac { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<Transform> Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
        [XmlElement(ElementName = "X509IssuerSerial", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509IssuerSerial X509IssuerSerial { get; set; }
        [XmlElement(ElementName = "X509SubjectName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SubjectName { get; set; }
    }

    [XmlRoot(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class RSAKeyValue
    {
        [XmlElement(ElementName = "Modulus", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Modulus { get; set; }
        [XmlElement(ElementName = "Exponent", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string Exponent { get; set; }
    }

    [XmlRoot(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyValue
    {
        [XmlElement(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public RSAKeyValue RSAKeyValue { get; set; }
    }

    [XmlRoot(ElementName = "X509IssuerSerial", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509IssuerSerial
    {
        [XmlElement(ElementName = "X509IssuerName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509IssuerName { get; set; }
        [XmlElement(ElementName = "X509SerialNumber", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509SerialNumber { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<X509Data> X509Data { get; set; }
        [XmlElement(ElementName = "KeyName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string KeyName { get; set; }
        [XmlElement(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyValue KeyValue { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "HDon")]
    public class HDon
    {
        [XmlElement(ElementName = "DLHDon")]
        public DLHDon DLHDon { get; set; }
        [XmlElement(ElementName = "DSCKS")]
        public DSCKS DSCKS { get; set; }
        [XmlElement(ElementName = "SigningTime")]
        public string SigningTime { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }
}

namespace ESCS.Models.TCT9
{
    [XmlRoot(ElementName = "ACPRO_KHACHHANG")]
    public class ACPRO_KHACHHANG
    {
        [XmlElement(ElementName = "TEN_CONGTY")]
        public string TEN_CONGTY { get; set; }
        [XmlElement(ElementName = "MST_CONGTY")]
        public string MST_CONGTY { get; set; }
        [XmlElement(ElementName = "DIACHI_CONGTY")]
        public string DIACHI_CONGTY { get; set; }
        [XmlElement(ElementName = "TEL_CONGTY")]
        public string TEL_CONGTY { get; set; }
        [XmlElement(ElementName = "SOTK_NH_CONGTY")]
        public string SOTK_NH_CONGTY { get; set; }
        [XmlElement(ElementName = "TEN_NH_CONGTY")]
        public string TEN_NH_CONGTY { get; set; }
        [XmlElement(ElementName = "EMAIL_CONGTY")]
        public string EMAIL_CONGTY { get; set; }
        [XmlElement(ElementName = "WEBSITE_CONGTY")]
        public string WEBSITE_CONGTY { get; set; }
        [XmlElement(ElementName = "TEN_HOADON")]
        public string TEN_HOADON { get; set; }
        [XmlElement(ElementName = "MAU_HOADON")]
        public string MAU_HOADON { get; set; }
        [XmlElement(ElementName = "KYHIEU_HOADON")]
        public string KYHIEU_HOADON { get; set; }
        [XmlElement(ElementName = "SO_HOADON")]
        public string SO_HOADON { get; set; }
        [XmlElement(ElementName = "MA_KHACHHANG")]
        public string MA_KHACHHANG { get; set; }
        [XmlElement(ElementName = "TEN_KHACHHANG")]
        public string TEN_KHACHHANG { get; set; }
        [XmlElement(ElementName = "MST_KHACHHANG")]
        public string MST_KHACHHANG { get; set; }
        [XmlElement(ElementName = "TEN_DONVI")]
        public string TEN_DONVI { get; set; }
        [XmlElement(ElementName = "DIACHI_KHACHHANG")]
        public string DIACHI_KHACHHANG { get; set; }
        [XmlElement(ElementName = "TEL_KHACHHANG")]
        public string TEL_KHACHHANG { get; set; }
        [XmlElement(ElementName = "SOTK_NGANHANG")]
        public string SOTK_NGANHANG { get; set; }
        [XmlElement(ElementName = "TEN_NGANHANG")]
        public string TEN_NGANHANG { get; set; }
        [XmlElement(ElementName = "HINHTHUC_THANHTOAN")]
        public string HINHTHUC_THANHTOAN { get; set; }
        [XmlElement(ElementName = "NHANVIEN_BANHANG")]
        public string NHANVIEN_BANHANG { get; set; }
        [XmlElement(ElementName = "KHO_HANGHOA")]
        public string KHO_HANGHOA { get; set; }
        [XmlElement(ElementName = "NGAY_HOADON")]
        public string NGAY_HOADON { get; set; }
        [XmlElement(ElementName = "LOGO")]
        public string LOGO { get; set; }
        [XmlElement(ElementName = "LOGO_DN")]
        public string LOGO_DN { get; set; }
        [XmlElement(ElementName = "LOGO_PA")]
        public string LOGO_PA { get; set; }
        [XmlElement(ElementName = "PATH")]
        public string PATH { get; set; }
        [XmlElement(ElementName = "WATER_MARK")]
        public string WATER_MARK { get; set; }
        [XmlElement(ElementName = "TEN_CONGTY_ENGLISH")]
        public string TEN_CONGTY_ENGLISH { get; set; }
    }

    [XmlRoot(ElementName = "ACPRO_XUATKHAU")]
    public class ACPRO_XUATKHAU
    {
        [XmlElement(ElementName = "SO_HDXK")]
        public string SO_HDXK { get; set; }
        [XmlElement(ElementName = "NGAY_HDXK")]
        public string NGAY_HDXK { get; set; }
        [XmlElement(ElementName = "DIEM_GIAOHANG")]
        public string DIEM_GIAOHANG { get; set; }
        [XmlElement(ElementName = "DIEM_NHANHANG")]
        public string DIEM_NHANHANG { get; set; }
        [XmlElement(ElementName = "SO_VANDON")]
        public string SO_VANDON { get; set; }
        [XmlElement(ElementName = "SO_CONTAINER")]
        public string SO_CONTAINER { get; set; }
        [XmlElement(ElementName = "DONVI_VANCHUYEN")]
        public string DONVI_VANCHUYEN { get; set; }
    }

    [XmlRoot(ElementName = "ACPRO_XUATKHO")]
    public class ACPRO_XUATKHO
    {
        [XmlElement(ElementName = "SOHD_KINHTE")]
        public string SOHD_KINHTE { get; set; }
        [XmlElement(ElementName = "NGAY_HDKINHTE")]
        public string NGAY_HDKINHTE { get; set; }
        [XmlElement(ElementName = "XUATKHO_CUA")]
        public string XUATKHO_CUA { get; set; }
        [XmlElement(ElementName = "XUATKHO_TOCHUC")]
        public string XUATKHO_TOCHUC { get; set; }
        [XmlElement(ElementName = "MST_XUATKHO")]
        public string MST_XUATKHO { get; set; }
        [XmlElement(ElementName = "TEN_VC_XUATKHO")]
        public string TEN_VC_XUATKHO { get; set; }
        [XmlElement(ElementName = "SOHD_XUATKHO")]
        public string SOHD_XUATKHO { get; set; }
        [XmlElement(ElementName = "PHUONGTIENVC_XUATKHO")]
        public string PHUONGTIENVC_XUATKHO { get; set; }
        [XmlElement(ElementName = "XUATKHO_TAI")]
        public string XUATKHO_TAI { get; set; }
        [XmlElement(ElementName = "NHAPKHO_TAI")]
        public string NHAPKHO_TAI { get; set; }
        [XmlElement(ElementName = "VE_VIEC")]
        public string VE_VIEC { get; set; }
    }

    [XmlRoot(ElementName = "ACPRO_TRANGTHAI_HD")]
    public class ACPRO_TRANGTHAI_HD
    {
        [XmlElement(ElementName = "XOA_HOADON")]
        public string XOA_HOADON { get; set; }
        [XmlElement(ElementName = "LY_DO_XOA")]
        public string LY_DO_XOA { get; set; }
    }

    [XmlRoot(ElementName = "ACPRO_THANHTOAN")]
    public class ACPRO_THANHTOAN
    {
        [XmlElement(ElementName = "TONG_TIENHANG")]
        public string TONG_TIENHANG { get; set; }
        [XmlElement(ElementName = "TIEN_GIAMGIA")]
        public string TIEN_GIAMGIA { get; set; }
        [XmlElement(ElementName = "TILE_CHIETKHAU")]
        public string TILE_CHIETKHAU { get; set; }
        [XmlElement(ElementName = "TIEN_CHIETKHAU")]
        public string TIEN_CHIETKHAU { get; set; }
        [XmlElement(ElementName = "TILE_THUE")]
        public string TILE_THUE { get; set; }
        [XmlElement(ElementName = "TIEN_THUE")]
        public string TIEN_THUE { get; set; }
        [XmlElement(ElementName = "THUE_KHAC")]
        public string THUE_KHAC { get; set; }
        [XmlElement(ElementName = "PHI_KHAC")]
        public string PHI_KHAC { get; set; }
        [XmlElement(ElementName = "TONG_CONG")]
        public string TONG_CONG { get; set; }
        [XmlElement(ElementName = "THANHTIEN_TEXT")]
        public string THANHTIEN_TEXT { get; set; }
        [XmlElement(ElementName = "TONGTIEN_KCT")]
        public string TONGTIEN_KCT { get; set; }
        [XmlElement(ElementName = "TONGTIEN_TRUOCTHUE0")]
        public string TONGTIEN_TRUOCTHUE0 { get; set; }
        [XmlElement(ElementName = "TONGTIEN_TRUOCTHUE5")]
        public string TONGTIEN_TRUOCTHUE5 { get; set; }
        [XmlElement(ElementName = "TONGTIEN_TRUOCTHUE10")]
        public string TONGTIEN_TRUOCTHUE10 { get; set; }
        [XmlElement(ElementName = "TONGTIEN_VAT5")]
        public string TONGTIEN_VAT5 { get; set; }
        [XmlElement(ElementName = "TONGTIEN_VAT10")]
        public string TONGTIEN_VAT10 { get; set; }
        [XmlElement(ElementName = "TILE_PHUCVU")]
        public string TILE_PHUCVU { get; set; }
        [XmlElement(ElementName = "TIEN_PHUCVU")]
        public string TIEN_PHUCVU { get; set; }
        [XmlElement(ElementName = "TIEN_THUETTDB")]
        public string TIEN_THUETTDB { get; set; }
        [XmlElement(ElementName = "TITLE_BQ_FOOTER")]
        public string TITLE_BQ_FOOTER { get; set; }
        [XmlElement(ElementName = "NO_GIAMGIA")]
        public string NO_GIAMGIA { get; set; }
        [XmlElement(ElementName = "NO_CHIETKHAU")]
        public string NO_CHIETKHAU { get; set; }
        [XmlElement(ElementName = "NO_TEL")]
        public string NO_TEL { get; set; }
        [XmlElement(ElementName = "LOAI_HOADON")]
        public string LOAI_HOADON { get; set; }
        [XmlElement(ElementName = "SD_VATCHEO")]
        public string SD_VATCHEO { get; set; }
        [XmlElement(ElementName = "BOSTT")]
        public string BOSTT { get; set; }
        [XmlElement(ElementName = "GIOKY")]
        public string GIOKY { get; set; }
        [XmlElement(ElementName = "NGAYKY")]
        public string NGAYKY { get; set; }
        [XmlElement(ElementName = "NO_PHIKHAC")]
        public string NO_PHIKHAC { get; set; }
        [XmlElement(ElementName = "NO_PHIDICHVU")]
        public string NO_PHIDICHVU { get; set; }
        [XmlElement(ElementName = "NO_MST_KY")]
        public string NO_MST_KY { get; set; }
        [XmlElement(ElementName = "MA_TRA_CUU")]
        public string MA_TRA_CUU { get; set; }
    }

    [XmlRoot(ElementName = "ACPRO_DM_HANGHOA_CT")]
    public class ACPRO_DM_HANGHOA_CT
    {
        [XmlElement(ElementName = "COL_KHOHANG")]
        public string COL_KHOHANG { get; set; }
        [XmlElement(ElementName = "COL_MA_VT")]
        public string COL_MA_VT { get; set; }
        [XmlElement(ElementName = "COL_TEN_VT")]
        public string COL_TEN_VT { get; set; }
        [XmlElement(ElementName = "COL_DV_TINH")]
        public string COL_DV_TINH { get; set; }
        [XmlElement(ElementName = "COL_DON_GIA")]
        public string COL_DON_GIA { get; set; }
        [XmlElement(ElementName = "COL_SO_LUONG")]
        public string COL_SO_LUONG { get; set; }
        [XmlElement(ElementName = "COL_THANH_TIEN")]
        public string COL_THANH_TIEN { get; set; }
        [XmlElement(ElementName = "COL_TIEN_GG")]
        public string COL_TIEN_GG { get; set; }
        [XmlElement(ElementName = "COL_THUE_SUAT")]
        public string COL_THUE_SUAT { get; set; }
        [XmlElement(ElementName = "COL_TIEN_THUE")]
        public string COL_TIEN_THUE { get; set; }
        [XmlElement(ElementName = "COL_THANHTIEN_VAT")]
        public string COL_THANHTIEN_VAT { get; set; }
    }

    [XmlRoot(ElementName = "ACPRO_DM_HANGHOA")]
    public class ACPRO_DM_HANGHOA
    {
        [XmlElement(ElementName = "ACPRO_DM_HANGHOA_CT")]
        public List<ACPRO_DM_HANGHOA_CT> ACPRO_DM_HANGHOA_CT { get; set; }
    }

    [XmlRoot(ElementName = "ACPRO_THONGTIN")]
    public class ACPRO_THONGTIN
    {
        [XmlElement(ElementName = "ACPRO_KHACHHANG")]
        public ACPRO_KHACHHANG ACPRO_KHACHHANG { get; set; }
        [XmlElement(ElementName = "ACPRO_XUATKHAU")]
        public ACPRO_XUATKHAU ACPRO_XUATKHAU { get; set; }
        [XmlElement(ElementName = "ACPRO_XUATKHO")]
        public ACPRO_XUATKHO ACPRO_XUATKHO { get; set; }
        [XmlElement(ElementName = "ACPRO_TRANGTHAI_HD")]
        public ACPRO_TRANGTHAI_HD ACPRO_TRANGTHAI_HD { get; set; }
        [XmlElement(ElementName = "ACPRO_THANHTOAN")]
        public ACPRO_THANHTOAN ACPRO_THANHTOAN { get; set; }
        [XmlElement(ElementName = "ACPRO_DM_HANGHOA")]
        public ACPRO_DM_HANGHOA ACPRO_DM_HANGHOA { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class CanonicalizationMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignatureMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<Transform> Transform { get; set; }
    }

    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Reference
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string DigestValue { get; set; }
        [XmlAttribute(AttributeName = "URI")]
        public string URI { get; set; }
    }

    [XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class SignedInfo
    {
        [XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public CanonicalizationMethod CanonicalizationMethod { get; set; }
        [XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignatureMethod SignatureMethod { get; set; }
        [XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Reference Reference { get; set; }
    }

    [XmlRoot(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class X509Data
    {
        [XmlElement(ElementName = "X509Certificate", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string X509Certificate { get; set; }
    }

    [XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class KeyInfo
    {
        [XmlElement(ElementName = "X509Data", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public X509Data X509Data { get; set; }
        [XmlElement(ElementName = "KeyName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public List<string> KeyName { get; set; }
    }

    [XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Signature
    {
        [XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public SignedInfo SignedInfo { get; set; }
        [XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public string SignatureValue { get; set; }
        [XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfo KeyInfo { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "ACPRO", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public class ACPRO
    {
        [XmlElement(ElementName = "ACPRO_THONGTIN")]
        public ACPRO_THONGTIN ACPRO_THONGTIN { get; set; }
        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
        [XmlAttribute(AttributeName = "ACPRO_HOADONDIENTU", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string ACPRO_HOADONDIENTU { get; set; }
    }
}
