using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ESCS.API.Attributes;
using ESCS.API.Hubs;
using ESCS.BUS.OpenID;
using ESCS.BUS.Services;
using ESCS.COMMON.Caches.interfaces;
using ESCS.COMMON.CallApp;
using ESCS.COMMON.Common;
using ESCS.COMMON.Hubs;
using ESCS.COMMON.MongoDb;
using ESCS.COMMON.MongoDb.LogEntities;
using ESCS.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ESCS.API.Controllers
{
    [Route("api/esmartclaim")]
    [ApiController]
    [ESCSAuth]
    public class InvoiceController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly ILogMongoService<LogContent> _logContent;
        private readonly IOpenIdCallApp _openIdCallApp;
        public InvoiceController(
            ICacheServer cacheServer,
            IDynamicService dynamicService,
            IOpenIdService openIdService,
            ILogMongoService<LogException> logRequestService,
            IHubContext<PartnerNotifyHub> partnerNotifyHub,
            IErrorCodeService errorCodeService,
            IDataProtectionProvider provider,
            IUserConnectionManager userConnectionManager,
            ILogMongoService<LogContent> logContent,
            IOpenIdCallApp openIdCallApp,
            IWebHostEnvironment webHostEnvironment)
            : base(cacheServer, dynamicService, openIdService,
                 logRequestService, partnerNotifyHub,
                 errorCodeService, provider)
        {
            _userConnectionManager = userConnectionManager;
            _logContent = logContent;
            _openIdCallApp = openIdCallApp;
            _webHostEnvironment = webHostEnvironment;
        }

        private EBill getEBillTCT(Models.TCT.Invoice objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.InvoiceData.TemplateCode != null ? objInvoice.InvoiceData.TemplateCode : "";
            obj.InvoiceSeries = objInvoice.InvoiceData.InvoiceSeries != null ? objInvoice.InvoiceData.InvoiceSeries : "";
            obj.InvoiceNumber = objInvoice.InvoiceData.InvoiceNumber != null ? objInvoice.InvoiceData.InvoiceNumber : "";
            obj.InvoiceName = objInvoice.InvoiceData.InvoiceName != null ? objInvoice.InvoiceData.InvoiceName : "";
            if (objInvoice.InvoiceData.InvoiceIssuedDate != null)
            {
                string temp = objInvoice.InvoiceData.InvoiceIssuedDate.Replace("-", "");

                if (temp.Trim().Length == 10)
                {
                    int vi_tri_gach_cheo = temp.Trim().LastIndexOf("/");
                    if (vi_tri_gach_cheo == 7)
                    {
                        obj.InvoiceIssuedDate = temp.Substring(8, 2) + "/" + temp.Substring(5, 2) + "/" + temp.Substring(0, 4);
                    }
                    else
                        obj.InvoiceIssuedDate = temp;
                }
                else
                {
                    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
                }
            }
            else
                obj.InvoiceIssuedDate = "";
            obj.SignedDate = objInvoice.InvoiceData.SignedDate != null ? objInvoice.InvoiceData.SignedDate : "";
            obj.CurrencyCode = objInvoice.InvoiceData.CurrencyCode != null ? objInvoice.InvoiceData.CurrencyCode : "";

            obj.InvoiceNote = objInvoice.InvoiceData.InvoiceNote != null ? objInvoice.InvoiceData.InvoiceNote : "";

            obj.SellerLegalName = objInvoice.InvoiceData.SellerLegalName != null ? objInvoice.InvoiceData.SellerLegalName : "";
            obj.SellerTaxCode = objInvoice.InvoiceData.SellerTaxCode != null ? objInvoice.InvoiceData.SellerTaxCode : "";
            obj.SellerAddressLine = objInvoice.InvoiceData.SellerAddressLine != null ? objInvoice.InvoiceData.SellerAddressLine : "";
            obj.SellerPhoneNumber = objInvoice.InvoiceData.SellerPhoneNumber != null ? objInvoice.InvoiceData.SellerPhoneNumber : "";
            obj.SellerEmail = objInvoice.InvoiceData.SellerEmail != null ? objInvoice.InvoiceData.SellerEmail : "";
            obj.SellerBankName = objInvoice.InvoiceData.SellerBankName != null ? objInvoice.InvoiceData.SellerBankName : "";
            obj.SellerBankAccount = objInvoice.InvoiceData.SellerBankAccount != null ? objInvoice.InvoiceData.SellerBankAccount : "";
            obj.SellerContactPersonName = objInvoice.InvoiceData.SellerContactPersonName != null ? objInvoice.InvoiceData.SellerContactPersonName : "";

            obj.BuyerDisplayName = objInvoice.InvoiceData.BuyerDisplayName != null ? objInvoice.InvoiceData.BuyerDisplayName :
                        (objInvoice.InvoiceData.BuyerLegalName != null ? objInvoice.InvoiceData.BuyerLegalName : "");
            obj.BuyerLegalName = objInvoice.InvoiceData.BuyerLegalName != null ? objInvoice.InvoiceData.BuyerLegalName :
                    (objInvoice.InvoiceData.BuyerDisplayName != null ? objInvoice.InvoiceData.BuyerDisplayName : "");
            obj.BuyerTaxCode = objInvoice.InvoiceData.BuyerTaxCode != null ? objInvoice.InvoiceData.BuyerTaxCode : "";
            obj.BuyerAddressLine = objInvoice.InvoiceData.BuyerAddressLine != null ? objInvoice.InvoiceData.BuyerAddressLine : "";
            obj.BuyerPhoneNumber = objInvoice.InvoiceData.BuyerPhoneNumber != null ? objInvoice.InvoiceData.BuyerPhoneNumber : "";
            obj.BuyerEmail = objInvoice.InvoiceData.BuyerEmail != null ? objInvoice.InvoiceData.BuyerEmail : "";
            obj.BuyerBankAccount = objInvoice.InvoiceData.BuyerBankAccount != null ? objInvoice.InvoiceData.BuyerBankAccount : "";

            if (objInvoice.InvoiceData.Seller != null)
            {
                obj.SellerLegalName = objInvoice.InvoiceData.Seller.SellerLegalName != null ? objInvoice.InvoiceData.Seller.SellerLegalName : "";
                obj.SellerTaxCode = objInvoice.InvoiceData.Seller.SellerTaxCode != null ? objInvoice.InvoiceData.Seller.SellerTaxCode : "";
                obj.SellerAddressLine = objInvoice.InvoiceData.Seller.SellerAddressLine != null ? objInvoice.InvoiceData.Seller.SellerAddressLine : "";
                obj.SellerPhoneNumber = objInvoice.InvoiceData.Seller.SellerPhoneNumber != null ? objInvoice.InvoiceData.Seller.SellerPhoneNumber : "";
                obj.SellerEmail = objInvoice.InvoiceData.Seller.SellerEmail != null ? objInvoice.InvoiceData.Seller.SellerEmail : "";
                obj.SellerBankName = objInvoice.InvoiceData.Seller.SellerBankName != null ? objInvoice.InvoiceData.Seller.SellerBankName : "";
                obj.SellerBankAccount = objInvoice.InvoiceData.Seller.SellerBankAccount != null ? objInvoice.InvoiceData.Seller.SellerBankAccount : "";
                obj.SellerContactPersonName = objInvoice.InvoiceData.Seller.SellerContactPersonName != null ? objInvoice.InvoiceData.Seller.SellerContactPersonName : "";
            }

            if (objInvoice.InvoiceData.Buyer != null)
            {
                obj.BuyerDisplayName = objInvoice.InvoiceData.Buyer.BuyerDisplayName != null ? objInvoice.InvoiceData.Buyer.BuyerDisplayName :
                        (objInvoice.InvoiceData.Buyer.BuyerLegalName != null ? objInvoice.InvoiceData.Buyer.BuyerLegalName : "");
                obj.BuyerLegalName = objInvoice.InvoiceData.Buyer.BuyerLegalName != null ? objInvoice.InvoiceData.Buyer.BuyerLegalName :
                        (objInvoice.InvoiceData.Buyer.BuyerDisplayName != null ? objInvoice.InvoiceData.Buyer.BuyerDisplayName : "");
                obj.BuyerTaxCode = objInvoice.InvoiceData.Buyer.BuyerTaxCode != null ? objInvoice.InvoiceData.Buyer.BuyerTaxCode : "";
                obj.BuyerAddressLine = objInvoice.InvoiceData.Buyer.BuyerAddressLine != null ? objInvoice.InvoiceData.Buyer.BuyerAddressLine : "";
                obj.BuyerPhoneNumber = objInvoice.InvoiceData.Buyer.BuyerPhoneNumber != null ? objInvoice.InvoiceData.Buyer.BuyerPhoneNumber : "";
                obj.BuyerEmail = objInvoice.InvoiceData.Buyer.BuyerEmail != null ? objInvoice.InvoiceData.Buyer.BuyerEmail : "";
                obj.BuyerBankAccount = objInvoice.InvoiceData.Buyer.BuyerBankAccount != null ? objInvoice.InvoiceData.Buyer.BuyerBankAccount : "";
            }

            obj.TotalAmountWithoutVAT = objInvoice.InvoiceData.TotalAmountWithoutVAT != null ? objInvoice.InvoiceData.TotalAmountWithoutVAT : "0";
            obj.TotalVATAmount = objInvoice.InvoiceData.TotalVATAmount != null ? objInvoice.InvoiceData.TotalVATAmount : "0";
            obj.TotalAmountWithVAT = objInvoice.InvoiceData.TotalAmountWithVAT != null ? objInvoice.InvoiceData.TotalAmountWithVAT : "0";
            obj.DiscountAmount = objInvoice.InvoiceData.DiscountAmount != null ? objInvoice.InvoiceData.DiscountAmount : "0";
            obj.TotalAmountWithVATInWords = objInvoice.InvoiceData.TotalAmountWithVATInWords != null ? objInvoice.InvoiceData.TotalAmountWithVATInWords : "";

            obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;
            if (objInvoice.InvoiceData.Items != null)
            {
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.InvoiceData.Items.Item.Count; i++)
                {
                    Models.TCT.Item item = objInvoice.InvoiceData.Items.Item[i];

                    item.ProdPrice = item.UnitPrice != null ? item.UnitPrice : "0";
                    item.ProdQuantity = item.Quantity != null ? item.Quantity : "0";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;
                }

                obj.ItemName = itemName;
                obj.VatPercentage = vatPercentage;
            }


            return obj;
        }
        private EBill getEBillTCT2(Models.TCT2.Invoice objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.InvoiceData.TemplateCode != null ? objInvoice.InvoiceData.TemplateCode : "";
            obj.InvoiceSeries = objInvoice.InvoiceData.InvoiceSeries != null ? objInvoice.InvoiceData.InvoiceSeries : "";
            obj.InvoiceNumber = objInvoice.InvoiceData.InvoiceNumber != null ? objInvoice.InvoiceData.InvoiceNumber : "";
            obj.InvoiceName = objInvoice.InvoiceData.InvoiceName != null ? objInvoice.InvoiceData.InvoiceName : "";
            //if (objInvoice.InvoiceData.InvoiceIssuedDate != null)
            //{
            //    string temp = objInvoice.InvoiceData.InvoiceIssuedDate.Replace("-", "");
            //    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
            //}
            //else
            //    obj.InvoiceIssuedDate = "";

            if (objInvoice.InvoiceData.InvoiceIssuedDate != null)
            {
                string temp = objInvoice.InvoiceData.InvoiceIssuedDate.Replace("-", "");

                if (temp.Trim().Length == 10)
                {
                    int vi_tri_gach_cheo = temp.Trim().LastIndexOf("/");
                    if (vi_tri_gach_cheo == 7)
                    {
                        obj.InvoiceIssuedDate = temp.Substring(8, 2) + "/" + temp.Substring(5, 2) + "/" + temp.Substring(0, 4);
                    }
                    else
                        obj.InvoiceIssuedDate = temp;
                }
                else
                {
                    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
                }
            }
            else
                obj.InvoiceIssuedDate = "";

            obj.SignedDate = objInvoice.InvoiceData.SignedDate != null ? objInvoice.InvoiceData.SignedDate : "";
            obj.CurrencyCode = objInvoice.InvoiceData.CurrencyCode != null ? objInvoice.InvoiceData.CurrencyCode : "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.InvoiceData.SellerLegalName != null ? objInvoice.InvoiceData.SellerLegalName : "";
            obj.SellerTaxCode = objInvoice.InvoiceData.SellerTaxCode != null ? objInvoice.InvoiceData.SellerTaxCode : "";
            obj.SellerAddressLine = objInvoice.InvoiceData.SellerAddressLine != null ? objInvoice.InvoiceData.SellerAddressLine : "";
            obj.SellerPhoneNumber = objInvoice.InvoiceData.SellerPhoneNumber != null ? objInvoice.InvoiceData.SellerPhoneNumber : "";
            obj.SellerEmail = objInvoice.InvoiceData.SellerEmail != null ? objInvoice.InvoiceData.SellerEmail : "";
            obj.SellerBankName = objInvoice.InvoiceData.SellerBankName != null ? objInvoice.InvoiceData.SellerBankName : "";
            obj.SellerBankAccount = objInvoice.InvoiceData.SellerBankAccount != null ? objInvoice.InvoiceData.SellerBankAccount : "";
            obj.SellerContactPersonName = objInvoice.InvoiceData.SellerContactPersonName != null ? objInvoice.InvoiceData.SellerContactPersonName : "";

            obj.BuyerDisplayName = objInvoice.InvoiceData.BuyerDisplayName != null ? objInvoice.InvoiceData.BuyerDisplayName : "";
            obj.BuyerLegalName = objInvoice.InvoiceData.BuyerLegalName != null ? objInvoice.InvoiceData.BuyerLegalName :
                    (objInvoice.InvoiceData.BuyerDisplayName != null ? objInvoice.InvoiceData.BuyerDisplayName : "");
            obj.BuyerTaxCode = objInvoice.InvoiceData.BuyerTaxCode != null ? objInvoice.InvoiceData.BuyerTaxCode : "";
            obj.BuyerAddressLine = objInvoice.InvoiceData.BuyerAddressLine != null ? objInvoice.InvoiceData.BuyerAddressLine : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = objInvoice.InvoiceData.BuyerEmail != null ? objInvoice.InvoiceData.BuyerEmail : "";
            obj.BuyerBankAccount = objInvoice.InvoiceData.BuyerBankAccount != null ? objInvoice.InvoiceData.BuyerBankAccount : "";

            obj.TotalAmountWithoutVAT = objInvoice.InvoiceData.TotalAmountWithoutVAT != null ? objInvoice.InvoiceData.TotalAmountWithoutVAT : "0";
            obj.TotalVATAmount = objInvoice.InvoiceData.TotalVATAmount != null ? objInvoice.InvoiceData.TotalVATAmount : "0";
            obj.TotalAmountWithVAT = objInvoice.InvoiceData.TotalAmountWithVAT != null ? objInvoice.InvoiceData.TotalAmountWithVAT : "0";
            obj.TotalAmountWithVATInWords = objInvoice.InvoiceData.TotalAmountWithVATInWords != null ? objInvoice.InvoiceData.TotalAmountWithVATInWords : "";
            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.InvoiceData.Items != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.InvoiceData.Items.Item.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    Models.TCT2.Item product = objInvoice.InvoiceData.Items.Item[i];

                    item.LineNumber = "";
                    item.ItemCode = product.ItemCode != null ? product.ItemCode : "";
                    item.ItemName = product.ItemName != null ? product.ItemName : "";
                    item.UnitCode = product.UnitCode != null ? product.UnitCode : "";
                    item.UnitName = product.UnitName != null ? product.UnitName : "";
                    item.ItemTotalAmountWithoutVat = product.ItemTotalAmountWithoutVat != null ? product.ItemTotalAmountWithoutVat : "0";
                    item.VatPercentage = product.VatPercentage != null ? product.VatPercentage : "";
                    item.VatAmount = product.VatAmount != null ? product.VatAmount : "0";

                    item.ProdPrice = product.UnitPrice != null ? product.UnitPrice : "0";
                    item.ProdQuantity = product.Quantity != null ? product.Quantity : "0";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT3(Models.TCT3.HDon objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.DLHDon.TTChung.MauSo != null ? objInvoice.DLHDon.TTChung.MauSo : "";
            obj.InvoiceSeries = objInvoice.DLHDon.TTChung.KHHDon != null ? objInvoice.DLHDon.TTChung.KHHDon : "";
            //obj.InvoiceNumber = objInvoice.DLHDon.TTChung.So != null ? objInvoice.DLHDon.TTChung.So : "";

            obj.InvoiceNumber = objInvoice.DLHDon.TTChung.So != null ? objInvoice.DLHDon.TTChung.So :
                    (objInvoice.DLHDon.TTChung.SHDon != null ? objInvoice.DLHDon.TTChung.SHDon : "");

            obj.InvoiceName = objInvoice.DLHDon.TTChung.THDon != null ? objInvoice.DLHDon.TTChung.THDon : "";

            //if (objInvoice.DLHDon.TTChung.TDLap != null)
            //{
            //    string temp = objInvoice.DLHDon.TTChung.TDLap.Replace("-", "");
            //    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
            //}
            //else
            //    obj.InvoiceIssuedDate = "";

            if (objInvoice.DLHDon.TTChung.TDLap != null)
            {
                string temp = objInvoice.DLHDon.TTChung.TDLap.Replace("-", "");

                if (temp.Trim().Length == 10)
                {
                    int vi_tri_gach_cheo = temp.Trim().LastIndexOf("/");
                    if (vi_tri_gach_cheo == 7)
                    {
                        obj.InvoiceIssuedDate = temp.Substring(8, 2) + "/" + temp.Substring(5, 2) + "/" + temp.Substring(0, 4);
                    }
                    else
                        obj.InvoiceIssuedDate = temp;
                }
                else
                {
                    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
                }
            }
            else
                obj.InvoiceIssuedDate = "";

            obj.SignedDate = "";
            obj.CurrencyCode = objInvoice.DLHDon.TTChung.DVTTe != null ? objInvoice.DLHDon.TTChung.DVTTe : "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.DLHDon.NDHDon.NBan.Ten != null ? objInvoice.DLHDon.NDHDon.NBan.Ten : "";
            obj.SellerTaxCode = objInvoice.DLHDon.NDHDon.NBan.MST != null ? objInvoice.DLHDon.NDHDon.NBan.MST : "";
            obj.SellerAddressLine = objInvoice.DLHDon.NDHDon.NBan.DChi != null ? objInvoice.DLHDon.NDHDon.NBan.DChi : "";
            obj.SellerPhoneNumber = "";
            obj.SellerEmail = "";
            obj.SellerBankName = "";
            obj.SellerBankAccount = "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.DLHDon.NDHDon.NMua.Ten != null ? objInvoice.DLHDon.NDHDon.NMua.Ten : "";
            obj.BuyerLegalName = objInvoice.DLHDon.NDHDon.NMua.Ten != null ? objInvoice.DLHDon.NDHDon.NMua.Ten : "";
            obj.BuyerTaxCode = objInvoice.DLHDon.NDHDon.NMua.MST != null ? objInvoice.DLHDon.NDHDon.NMua.MST : "";
            obj.BuyerAddressLine = objInvoice.DLHDon.NDHDon.NMua.DChi != null ? objInvoice.DLHDon.NDHDon.NMua.DChi : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.DLHDon.NDHDon.TToan.TgTCThue != null ? objInvoice.DLHDon.NDHDon.TToan.TgTCThue : "0";/* TTChung.TTKhac.Tax.Amt*/
            obj.TotalVATAmount = objInvoice.DLHDon.NDHDon.TToan.TgTThue != null ? objInvoice.DLHDon.NDHDon.TToan.TgTThue : "0";
            obj.TotalAmountWithVAT = objInvoice.DLHDon.NDHDon.TToan.TgTTTBSo != null ? objInvoice.DLHDon.NDHDon.TToan.TgTTTBSo : "0";    //objInvoice.DLHDon.TTChung.TTKhac.Total
            obj.TotalAmountWithVATInWords = objInvoice.DLHDon.NDHDon.TToan.TgTTTBChu != null ? objInvoice.DLHDon.NDHDon.TToan.TgTTTBChu : "";

            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.DLHDon.NDHDon.DSHHDVu != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.DLHDon.NDHDon.DSHHDVu.HHDVu.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    //  Models.TCT2.Item product = objInvoice.InvoiceData.Items.Item[i];

                    Models.TCT3.HHDVu product = objInvoice.DLHDon.NDHDon.DSHHDVu.HHDVu[i];

                    item.LineNumber = "";
                    item.ItemCode = "";
                    item.ItemName = product.Ten != null ? product.Ten : "";
                    item.UnitCode = "";
                    item.UnitName = product.DVTinh != null ? product.DVTinh : "";
                    item.ItemTotalAmountWithoutVat = product.ThTien != null ? product.ThTien : "0";
                    item.VatPercentage = product.TSuat != null ? product.TSuat : "";
                    item.VatAmount = "0";

                    item.ProdPrice = product.DGia != null ? product.DGia : "0";
                    item.ProdQuantity = product.SLuong != null ? product.SLuong : "0";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT12(Models.TCT12.HDon objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.DLHDon.TTChung.KHMSHDon != null ? objInvoice.DLHDon.TTChung.KHMSHDon : "";
            obj.InvoiceSeries = objInvoice.DLHDon.TTChung.KHHDon != null ? objInvoice.DLHDon.TTChung.KHHDon : "";
            //obj.InvoiceNumber = objInvoice.DLHDon.TTChung.So != null ? objInvoice.DLHDon.TTChung.So : "";

            obj.InvoiceNumber = objInvoice.DLHDon.TTChung.SHDon != null ? objInvoice.DLHDon.TTChung.SHDon : "";
            obj.InvoiceName = objInvoice.DLHDon.TTChung.THDon != null ? objInvoice.DLHDon.TTChung.THDon : "";

            //if (objInvoice.DLHDon.TTChung.TDLap != null)
            //{
            //    string temp = objInvoice.DLHDon.TTChung.TDLap.Replace("-", "");
            //    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
            //}
            //else
            //    obj.InvoiceIssuedDate = "";

            if (objInvoice.DLHDon.TTChung.TDLap != null)
            {
                string temp = objInvoice.DLHDon.TTChung.TDLap.Replace("-", "");

                if (temp.Trim().Length == 10)
                {
                    int vi_tri_gach_cheo = temp.Trim().LastIndexOf("/");
                    if (vi_tri_gach_cheo == 7)
                    {
                        obj.InvoiceIssuedDate = temp.Substring(8, 2) + "/" + temp.Substring(5, 2) + "/" + temp.Substring(0, 4);
                    }
                    else
                        obj.InvoiceIssuedDate = temp;
                }
                else
                {
                    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
                }
            }
            else
                obj.InvoiceIssuedDate = "";

            obj.SignedDate = "";
            obj.CurrencyCode = objInvoice.DLHDon.TTChung.DVTTe != null ? objInvoice.DLHDon.TTChung.DVTTe : "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.DLHDon.NDHDon.NBan.Ten != null ? objInvoice.DLHDon.NDHDon.NBan.Ten : "";
            obj.SellerTaxCode = objInvoice.DLHDon.NDHDon.NBan.MST != null ? objInvoice.DLHDon.NDHDon.NBan.MST : "";
            obj.SellerAddressLine = objInvoice.DLHDon.NDHDon.NBan.DChi != null ? objInvoice.DLHDon.NDHDon.NBan.DChi : "";
            obj.SellerPhoneNumber = "";
            obj.SellerEmail = "";
            obj.SellerBankName = "";
            obj.SellerBankAccount = "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.DLHDon.NDHDon.NMua.Ten != null ? objInvoice.DLHDon.NDHDon.NMua.Ten : "";
            obj.BuyerLegalName = objInvoice.DLHDon.NDHDon.NMua.Ten != null ? objInvoice.DLHDon.NDHDon.NMua.Ten : "";
            obj.BuyerTaxCode = objInvoice.DLHDon.NDHDon.NMua.MST != null ? objInvoice.DLHDon.NDHDon.NMua.MST : "";
            obj.BuyerAddressLine = objInvoice.DLHDon.NDHDon.NMua.DChi != null ? objInvoice.DLHDon.NDHDon.NMua.DChi : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.DLHDon.NDHDon.TToan.TgTCThue != null ? objInvoice.DLHDon.NDHDon.TToan.TgTCThue : "0";/* TTChung.TTKhac.Tax.Amt*/
            obj.TotalVATAmount = objInvoice.DLHDon.NDHDon.TToan.TgTThue != null ? objInvoice.DLHDon.NDHDon.TToan.TgTThue : "0";
            obj.TotalAmountWithVAT = objInvoice.DLHDon.NDHDon.TToan.TgTTTBSo != null ? objInvoice.DLHDon.NDHDon.TToan.TgTTTBSo : "0";    //objInvoice.DLHDon.TTChung.TTKhac.Total
            obj.TotalAmountWithVATInWords = objInvoice.DLHDon.NDHDon.TToan.TgTTTBChu != null ? objInvoice.DLHDon.NDHDon.TToan.TgTTTBChu : "";

            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.DLHDon.NDHDon.DSHHDVu != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.DLHDon.NDHDon.DSHHDVu.HHDVu.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    //  Models.TCT2.Item product = objInvoice.InvoiceData.Items.Item[i];

                    Models.TCT12.HHDVu product = objInvoice.DLHDon.NDHDon.DSHHDVu.HHDVu[i];

                    item.LineNumber = "";
                    item.ItemCode = "";
                    item.ItemName = product.Ten != null ? product.Ten : "";
                    item.UnitCode = "";
                    item.UnitName = product.DVTinh != null ? product.DVTinh : "";
                    item.ItemTotalAmountWithoutVat = product.ThTien != null ? product.ThTien : "0";
                    item.VatPercentage = product.TSuat != null ? product.TSuat : "";
                    item.VatAmount = "0";

                    item.ProdPrice = product.DGia != null ? product.DGia : "0";
                    item.ProdQuantity = product.SLuong != null ? product.SLuong : "0";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT4(Models.TCT4.HoaDonDienTu objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.HoaDon.ThongTinHoaDon.MauSo != null ? objInvoice.HoaDon.ThongTinHoaDon.MauSo : "";
            obj.InvoiceSeries = objInvoice.HoaDon.ThongTinHoaDon.Kyhieu != null ? objInvoice.HoaDon.ThongTinHoaDon.Kyhieu : "";
            obj.InvoiceNumber = objInvoice.HoaDon.ThongTinHoaDon.SoHoaDon != null ? objInvoice.HoaDon.ThongTinHoaDon.SoHoaDon : "";
            obj.InvoiceName = objInvoice.HoaDon.ThongTinHoaDon.TenHoaDon != null ? objInvoice.HoaDon.ThongTinHoaDon.TenHoaDon : "";

            if (objInvoice.HoaDon.ThongTinHoaDon.NgayHoaDon != null)
            {
                string temp = objInvoice.HoaDon.ThongTinHoaDon.NgayHoaDon.Replace("-", "");

                if (temp.Trim().Length == 10)
                {
                    int vi_tri_gach_cheo = temp.Trim().LastIndexOf("/");
                    if (vi_tri_gach_cheo == 7)
                    {
                        obj.InvoiceIssuedDate = temp.Substring(8, 2) + "/" + temp.Substring(5, 2) + "/" + temp.Substring(0, 4);
                    }
                    else
                        obj.InvoiceIssuedDate = temp;
                }
                else
                {
                    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
                }
            }
            else
                obj.InvoiceIssuedDate = "";

            //if (objInvoice.HoaDon.ThongTinHoaDon.NgayHoaDon != null)
            //{
            //    string temp = objInvoice.HoaDon.ThongTinHoaDon.NgayHoaDon.Replace("-", "");
            //    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
            //}
            //else
            //    obj.InvoiceIssuedDate = "";

            obj.SignedDate = objInvoice.CertifiedData.NgayKy != null ? objInvoice.CertifiedData.NgayKy : "";
            obj.CurrencyCode = objInvoice.HoaDon.ThongTinHoaDon.MaTienTe != null ? objInvoice.HoaDon.ThongTinHoaDon.MaTienTe : "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.HoaDon.ThongTinHoaDon.TenNguoiBan != null ? objInvoice.HoaDon.ThongTinHoaDon.TenNguoiBan : "";
            obj.SellerTaxCode = objInvoice.HoaDon.ThongTinHoaDon.MaSoThueNguoiBan != null ? objInvoice.HoaDon.ThongTinHoaDon.MaSoThueNguoiBan : "";
            obj.SellerAddressLine = objInvoice.HoaDon.ThongTinHoaDon.DiaChiNguoiBan != null ? objInvoice.HoaDon.ThongTinHoaDon.DiaChiNguoiBan : "";
            obj.SellerPhoneNumber = "";
            obj.SellerEmail = "";
            obj.SellerBankName = "";
            obj.SellerBankAccount = "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.HoaDon.ThongTinHoaDon.TenDoanhNghiepMua != null ? objInvoice.HoaDon.ThongTinHoaDon.TenDoanhNghiepMua : "";
            obj.BuyerLegalName = objInvoice.HoaDon.ThongTinHoaDon.TenDoanhNghiepMua != null ? objInvoice.HoaDon.ThongTinHoaDon.TenDoanhNghiepMua : "";
            obj.BuyerTaxCode = objInvoice.HoaDon.ThongTinHoaDon.MaSoThueNguoiMua != null ? objInvoice.HoaDon.ThongTinHoaDon.MaSoThueNguoiMua : "";
            obj.BuyerAddressLine = objInvoice.HoaDon.ThongTinHoaDon.DiaChiNguoiMua != null ? objInvoice.HoaDon.ThongTinHoaDon.DiaChiNguoiMua : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.HoaDon.ThongTinHoaDon.TongTienTruocThue != null ? objInvoice.HoaDon.ThongTinHoaDon.TongTienTruocThue : "0";
            obj.TotalVATAmount = objInvoice.HoaDon.ThongTinHoaDon.TongTienThue != null ? objInvoice.HoaDon.ThongTinHoaDon.TongTienThue : "0";
            obj.TotalAmountWithVAT = objInvoice.HoaDon.ThongTinHoaDon.TongTien != null ? objInvoice.HoaDon.ThongTinHoaDon.TongTien : "0";
            obj.TotalAmountWithVATInWords = objInvoice.HoaDon.ThongTinHoaDon.ThanhTienBangChu != null ? objInvoice.HoaDon.ThongTinHoaDon.ThanhTienBangChu : "";

            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.HoaDon.ChiTietHoaDon != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.HoaDon.ChiTietHoaDon.ChiTiet.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    //  Models.TCT2.Item product = objInvoice.InvoiceData.Items.Item[i];

                    Models.TCT4.ChiTiet product = objInvoice.HoaDon.ChiTietHoaDon.ChiTiet[i];

                    item.LineNumber = "";
                    item.ItemCode = product.MaHang != null ? product.MaHang : "";
                    item.ItemName = product.TenHang != null ? product.TenHang : "";
                    item.UnitCode = "";
                    item.UnitName = "";
                    item.ItemTotalAmountWithoutVat = product.TienTruocThue != null ? product.TienTruocThue : "0";
                    item.VatPercentage = product.PhanTramThue != null ? product.PhanTramThue : "";
                    item.VatAmount = product.TienThue != null ? product.TienThue : "";

                    item.ProdPrice = "0";
                    item.ProdQuantity = "0";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT6(Models.TCT6.Invoices objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.Invoice.Ten_Vat != null ? objInvoice.Invoice.Ten_Vat : "";
            obj.InvoiceSeries = objInvoice.Invoice.Ten_Seri != null ? objInvoice.Invoice.Ten_Seri : "";
            obj.InvoiceNumber = objInvoice.Invoice.So_ct != null ? objInvoice.Invoice.So_ct : "";
            obj.InvoiceName = objInvoice.Invoice.M_title1 != null ? objInvoice.Invoice.M_title1 : "";

            if (objInvoice.Invoice.Ngay_ct != null)
            {
                string temp = objInvoice.Invoice.Ngay_ct.Replace("-", "");

                if (temp.Trim().Length == 10)
                {
                    int vi_tri_gach_cheo = temp.Trim().LastIndexOf("/");
                    if (vi_tri_gach_cheo == 7)
                    {
                        obj.InvoiceIssuedDate = temp.Substring(8, 2) + "/" + temp.Substring(5, 2) + "/" + temp.Substring(0, 4);
                    }
                    else
                        obj.InvoiceIssuedDate = temp;
                }
                else
                {
                    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
                }
            }
            else
                obj.InvoiceIssuedDate = "";

            //if (objInvoice.Invoice.Ngay_ct != null)
            //{
            //    string temp = objInvoice.Invoice.Ngay_ct.Replace("-", "");
            //    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
            //}
            //else
            //    obj.InvoiceIssuedDate = "";

            obj.SignedDate = objInvoice.Invoice.StrNgay_Ky != null ? objInvoice.Invoice.StrNgay_Ky : "";
            obj.CurrencyCode = objInvoice.Invoice.Ma_nt != null ? objInvoice.Invoice.Ma_nt : "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.Invoice.M_ten_Cty != null ? objInvoice.Invoice.M_ten_Cty : "";
            obj.SellerTaxCode = objInvoice.Invoice.M_Ma_So_Thue != null ? objInvoice.Invoice.M_Ma_So_Thue : "";
            obj.SellerAddressLine = objInvoice.Invoice.M_Dia_Chi != null ? objInvoice.Invoice.M_Dia_Chi : "";
            obj.SellerPhoneNumber = objInvoice.Invoice.M_Dien_Thoai != null ? objInvoice.Invoice.M_Dien_Thoai : "";
            obj.SellerEmail = objInvoice.Invoice.M_EMail != null ? objInvoice.Invoice.M_EMail : "";
            obj.SellerBankName = objInvoice.Invoice.M_Ten_NH != null ? objInvoice.Invoice.M_Ten_NH : "";
            obj.SellerBankAccount = objInvoice.Invoice.M_Tk_NH != null ? objInvoice.Invoice.M_Tk_NH : "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.Invoice.Ten_KH != null ? objInvoice.Invoice.Ten_KH : "";
            obj.BuyerLegalName = objInvoice.Invoice.Ten_KH != null ? objInvoice.Invoice.Ten_KH : "";
            obj.BuyerTaxCode = objInvoice.Invoice.Ma_So_Thue != null ? objInvoice.Invoice.Ma_So_Thue : "";
            obj.BuyerAddressLine = objInvoice.Invoice.Dia_Chi != null ? objInvoice.Invoice.Dia_Chi : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.Invoice.T_tien_nt2 != null ? objInvoice.Invoice.T_tien_nt2 : "0";
            obj.TotalVATAmount = objInvoice.Invoice.T_thue_nt != null ? objInvoice.Invoice.T_thue_nt : "0";
            obj.TotalAmountWithVAT = objInvoice.Invoice.T_tt_nt != null ? objInvoice.Invoice.T_tt_nt : "0";
            obj.TotalAmountWithVATInWords = objInvoice.Invoice.Bang_Chu != null ? objInvoice.Invoice.Bang_Chu : "";
            obj.VatPercentage = objInvoice.Invoice.Thue_suat != null ? objInvoice.Invoice.Thue_suat : "";

            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.Products != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.Products.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    //  Models.TCT2.Item product = objInvoice.InvoiceData.Items.Item[i];

                    Models.TCT6.Products product = objInvoice.Products[i];

                    item.LineNumber = product.Stt_Rec0 != null ? product.Stt_Rec0 : "";
                    item.ItemCode = "";
                    item.ItemName = product.Ten_VT != null ? product.Ten_VT : "";
                    item.UnitCode = "";
                    item.UnitName = "";
                    item.ItemTotalAmountWithoutVat = product.Tien_nt2 != null ? product.Tien_nt2 : "0";
                    item.VatPercentage = product.Thue_suat != null ? product.Thue_suat : "";
                    item.VatAmount = product.Thue_nt != null ? product.Thue_nt : "";

                    item.ProdPrice = "0";
                    item.ProdQuantity = product.So_luong != null ? product.So_luong : "";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                //obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT7(Models.TCT7.Invoice1 objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.CusivoiceTempt.TemptCode != null ? objInvoice.CusivoiceTempt.TemptCode : "";
            obj.InvoiceSeries = objInvoice.CusivoiceTempt.Symbol != null ? objInvoice.CusivoiceTempt.Symbol : "";
            obj.InvoiceNumber = objInvoice.Ivoice.InvoiceNumber != null ? objInvoice.Ivoice.InvoiceNumber : "";
            obj.InvoiceName = objInvoice.CusivoiceTempt.IvoiceType != null ? objInvoice.CusivoiceTempt.IvoiceType : "";

            obj.InvoiceIssuedDate = objInvoice.Ivoice.DateofInvoice != null ? objInvoice.Ivoice.DateofInvoice : "";
            obj.SignedDate = objInvoice.Ivoice.DateofSign != null ? objInvoice.Ivoice.DateofSign : "";
            obj.CurrencyCode = "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.CusivoiceTempt.CompanyName != null ? objInvoice.CusivoiceTempt.CompanyName : "";
            obj.SellerTaxCode = objInvoice.CusivoiceTempt.Taxcode != null ? objInvoice.CusivoiceTempt.Taxcode : "";
            obj.SellerAddressLine = objInvoice.CusivoiceTempt.Address != null ? objInvoice.CusivoiceTempt.Address : "";
            obj.SellerPhoneNumber = objInvoice.CusivoiceTempt.PhoneNumber != null ? objInvoice.CusivoiceTempt.PhoneNumber : "";
            obj.SellerEmail = objInvoice.CusivoiceTempt.Email != null ? objInvoice.CusivoiceTempt.Email : "";
            obj.SellerBankName = "";
            obj.SellerBankAccount = objInvoice.CusivoiceTempt.AccBank != null ? objInvoice.CusivoiceTempt.AccBank : "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.Ivoice.CompanyName != null ? objInvoice.Ivoice.CompanyName : "";
            obj.BuyerLegalName = objInvoice.Ivoice.CompanyName != null ? objInvoice.Ivoice.CompanyName : "";
            obj.BuyerTaxCode = objInvoice.Ivoice.CompanyTaxcode != null ? objInvoice.Ivoice.CompanyTaxcode : "";
            obj.BuyerAddressLine = objInvoice.Ivoice.CompanyAdd != null ? objInvoice.Ivoice.CompanyAdd : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.Ivoice.TotalMoneyNoTax != null ? objInvoice.Ivoice.TotalMoneyNoTax : "0";
            obj.TotalVATAmount = objInvoice.Ivoice.MoneyTax != null ? objInvoice.Ivoice.MoneyTax : "0";
            obj.TotalAmountWithVAT = objInvoice.Ivoice.TotalMoney != null ? objInvoice.Ivoice.TotalMoney : "0";
            obj.TotalAmountWithVATInWords = objInvoice.Ivoice.TextTotalMoney != null ? objInvoice.Ivoice.TextTotalMoney : "";
            obj.VatPercentage = objInvoice.Ivoice.Tax != null ? objInvoice.Ivoice.Tax.Replace("%", "") : "";

            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.IvoiceDetail != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.IvoiceDetail.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    //  Models.TCT2.Item product = objInvoice.InvoiceData.Items.Item[i];

                    Models.TCT7.IvoiceDetail product = objInvoice.IvoiceDetail[i];

                    item.LineNumber = product.STT != null ? product.STT : "";
                    item.ItemCode = "";
                    item.ItemName = product.ProductName != null ? product.ProductName : "";
                    item.UnitCode = "";
                    item.UnitName = product.Unit != null ? product.Unit : "";
                    item.ItemTotalAmountWithoutVat = product.TotalMoney.ToString() != null ? product.TotalMoney.ToString() : "0";
                    item.VatPercentage = obj.VatPercentage;
                    item.VatAmount = "";

                    item.ProdPrice = product.Price != null ? product.Price : "";
                    item.ProdQuantity = product.Number != null ? product.Number : "";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                //obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT8(Models.TCT8.Vht objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.Hoadon.Ky_hieu_mau != null ? objInvoice.Hoadon.Ky_hieu_mau : "";
            obj.InvoiceSeries = objInvoice.Hoadon.So_seri != null ? objInvoice.Hoadon.So_seri : "";
            obj.InvoiceNumber = objInvoice.Hoadon.So_hd != null ? objInvoice.Hoadon.So_hd : "";
            obj.InvoiceName = objInvoice.Hoadon.Ten_loaihd != null ? objInvoice.Hoadon.Ten_loaihd : "";
            obj.InvoiceIssuedDate = objInvoice.Hoadon.Ngay_hd != null ? objInvoice.Hoadon.Ngay_hd : "";

            obj.SignedDate = objInvoice.Hoadon.Ngay_hd != null ? objInvoice.Hoadon.Ngay_hd : "";
            obj.CurrencyCode = objInvoice.Hoadon.Ma_tte != null ? objInvoice.Hoadon.Ma_tte : "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.Hoadon.Don_vi_ban != null ? objInvoice.Hoadon.Don_vi_ban : "";
            obj.SellerTaxCode = objInvoice.Hoadon.Mst != null ? objInvoice.Hoadon.Mst : "";
            obj.SellerAddressLine = objInvoice.Hoadon.Dia_chi != null ? objInvoice.Hoadon.Dia_chi : "";
            obj.SellerPhoneNumber = objInvoice.Hoadon.Dien_thoai != null ? objInvoice.Hoadon.Dien_thoai : "";
            obj.SellerEmail = "";
            obj.SellerBankName = objInvoice.Hoadon.Ngan_hang != null ? objInvoice.Hoadon.Ngan_hang : "";
            obj.SellerBankAccount = objInvoice.Hoadon.So_tk != null ? objInvoice.Hoadon.So_tk : "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.Hoadon.Don_vi_mua != null ? objInvoice.Hoadon.Don_vi_mua : "";
            obj.BuyerLegalName = objInvoice.Hoadon.Don_vi_mua != null ? objInvoice.Hoadon.Don_vi_mua : "";
            obj.BuyerTaxCode = objInvoice.Hoadon.Mst_nguoi_mua != null ? objInvoice.Hoadon.Mst_nguoi_mua : "";
            obj.BuyerAddressLine = objInvoice.Hoadon.Dia_chi_nguoi_mua != null ? objInvoice.Hoadon.Dia_chi_nguoi_mua : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.Hoadon.Tien_hang != null ? objInvoice.Hoadon.Tien_hang : "0";
            obj.TotalVATAmount = objInvoice.Hoadon.Tien_thue != null ? objInvoice.Hoadon.Tien_thue : "0";
            obj.TotalAmountWithVAT = objInvoice.Hoadon.Tong_tien != null ? objInvoice.Hoadon.Tong_tien : "0";
            obj.TotalAmountWithVATInWords = "";

            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.Chitiethoadon != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.Chitiethoadon.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    Models.TCT8.Chitiethoadon product = objInvoice.Chitiethoadon[i];

                    item.LineNumber = product.Stt0 != null ? product.Stt0 : "";
                    item.ItemCode = product.Ma_vt != null ? product.Ma_vt : "";
                    item.ItemName = product.Dien_giai != null ? product.Dien_giai : "";
                    item.UnitCode = "";
                    item.UnitName = product.Dvt != null ? product.Dvt : "";
                    item.ItemTotalAmountWithoutVat = product.Tien != null ? product.Tien : "0";
                    item.VatPercentage = product.Thue_gtgt != null ? product.Thue_gtgt : "";
                    item.VatAmount = product.Thue != null ? product.Thue : "";

                    item.ProdPrice = product.Gia != null ? product.Gia : "";
                    item.ProdQuantity = product.So_luong != null ? product.So_luong : "";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT11(Models.TCT11.Invoice objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.InvoiceTemplate.ObjTempInvoice.FormNo != null ? objInvoice.InvoiceTemplate.ObjTempInvoice.FormNo : "";
            obj.InvoiceSeries = objInvoice.InvoiceTemplate.ObjTempInvoice.Sign != null ? objInvoice.InvoiceTemplate.ObjTempInvoice.Sign : "";
            obj.InvoiceNumber = objInvoice.InvoiceTemplate.ObjInvoice.InvoiceNo != null ? objInvoice.InvoiceTemplate.ObjInvoice.InvoiceNo : "";
            obj.InvoiceName = "";

            if (objInvoice.InvoiceTemplate.ObjInvoice.InvoiceDateUTC != null)
            {
                string temp = objInvoice.InvoiceTemplate.ObjInvoice.InvoiceDateUTC.Replace("-", "");
                obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
            }
            else
                obj.InvoiceIssuedDate = "";

            //   obj.InvoiceIssuedDate = objInvoice.InvoiceTemplate.ObjInvoice.InvoiceDateUTC != null ? objInvoice.InvoiceTemplate.ObjInvoice.InvoiceDateUTC : "";

            obj.SignedDate = objInvoice.InvoiceTemplate.ObjInvoice.InvoiceDateUTC != null ? objInvoice.InvoiceTemplate.ObjInvoice.InvoiceDateUTC : "";
            obj.CurrencyCode = "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.InvoiceTemplate.ObjMstNNT.NNTFullName != null ? objInvoice.InvoiceTemplate.ObjMstNNT.NNTFullName : "";
            obj.SellerTaxCode = objInvoice.InvoiceTemplate.ObjMstNNT.MST != null ? objInvoice.InvoiceTemplate.ObjMstNNT.MST : "";
            obj.SellerAddressLine = objInvoice.InvoiceTemplate.ObjMstNNT.NNTAddress != null ? objInvoice.InvoiceTemplate.ObjMstNNT.NNTAddress : "";
            obj.SellerPhoneNumber = objInvoice.InvoiceTemplate.ObjMstNNT.NNTPhone != null ? objInvoice.InvoiceTemplate.ObjMstNNT.NNTPhone : "";
            obj.SellerEmail = objInvoice.InvoiceTemplate.ObjMstNNT.ContactEmail != null ? objInvoice.InvoiceTemplate.ObjMstNNT.ContactEmail : "";
            obj.SellerBankName = "";
            obj.SellerBankAccount = "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.InvoiceTemplate.ObjMstCustomerNNT.CustomerNNTName != null ? objInvoice.InvoiceTemplate.ObjMstCustomerNNT.CustomerNNTName : "";
            obj.BuyerLegalName = objInvoice.InvoiceTemplate.ObjMstCustomerNNT.CustomerNNTName != null ? objInvoice.InvoiceTemplate.ObjMstCustomerNNT.CustomerNNTName : "";
            obj.BuyerTaxCode = objInvoice.InvoiceTemplate.ObjMstCustomerNNT.MST != null ? objInvoice.InvoiceTemplate.ObjMstCustomerNNT.MST : "";
            obj.BuyerAddressLine = objInvoice.InvoiceTemplate.ObjMstCustomerNNT.CustomerNNTAddress != null ? objInvoice.InvoiceTemplate.ObjMstCustomerNNT.CustomerNNTAddress : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.InvoiceTemplate.ObjInvoice.TongTienChuaThue != null ? objInvoice.InvoiceTemplate.ObjInvoice.TongTienChuaThue : "0";
            obj.TotalVATAmount = objInvoice.InvoiceTemplate.ObjInvoice.TongTienThue != null ? objInvoice.InvoiceTemplate.ObjInvoice.TongTienThue : "0";
            obj.TotalAmountWithVAT = objInvoice.InvoiceTemplate.ObjInvoice.TongTienCoThue != null ? objInvoice.InvoiceTemplate.ObjInvoice.TongTienCoThue : "0";
            obj.TotalAmountWithVATInWords = objInvoice.InvoiceTemplate.ObjInvoice.SoTienBangChu != null ? objInvoice.InvoiceTemplate.ObjInvoice.SoTienBangChu : "";

            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.InvoiceTemplate.ListInvoiceDtl != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.InvoiceTemplate.ListInvoiceDtl.ObjInvoiceDtl.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();

                    Models.TCT11.ObjInvoiceDtl product
                        = objInvoice.InvoiceTemplate.ListInvoiceDtl.ObjInvoiceDtl[i];

                    item.LineNumber = product.STT != null ? product.STT : "";
                    item.ItemCode = "";
                    item.ItemName = product.Ms_SpecName != null ? product.Ms_SpecName : "";
                    item.UnitCode = product.UnitCode != null ? product.UnitCode : "";
                    item.UnitName = product.UnitCode != null ? product.UnitCode : "";
                    item.ItemTotalAmountWithoutVat = product.ThanhTien != null ? product.ThanhTien : "0";
                    item.VatPercentage = product.VATRate != null ? product.VATRate : "";
                    item.VatAmount = "";

                    item.ProdPrice = product.UnitPrice != null ? product.UnitPrice : "";
                    item.ProdQuantity = product.Qty != null ? product.Qty : "";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT10(Models.TCT10.CyberInvoiceData objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.PhInvoices.PhInvoice.TEN_VAT != null ? objInvoice.PhInvoices.PhInvoice.TEN_VAT : "";
            obj.InvoiceSeries = objInvoice.PhInvoices.PhInvoice.TEN_SERI != null ? objInvoice.PhInvoices.PhInvoice.TEN_SERI : "";
            obj.InvoiceNumber = objInvoice.PhInvoices.PhInvoice.SO_CT != null ? objInvoice.PhInvoices.PhInvoice.SO_CT : "";
            obj.InvoiceName = "";
            obj.InvoiceIssuedDate = objInvoice.PhInvoices.PhInvoice.NGAY_TAO != null ? objInvoice.PhInvoices.PhInvoice.NGAY_TAO : "";

            obj.SignedDate = objInvoice.PhInvoices.PhInvoice.NGAY_CT != null ? objInvoice.PhInvoices.PhInvoice.NGAY_CT : "";
            obj.CurrencyCode = objInvoice.PhInvoices.PhInvoice.MA_CT != null ? objInvoice.PhInvoices.PhInvoice.MA_CT : "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.PhInvoices.PhInvoice.SALERLEGALNAME != null ? objInvoice.PhInvoices.PhInvoice.SALERLEGALNAME : "";
            obj.SellerTaxCode = objInvoice.PhInvoices.PhInvoice.SALERTAXCODE != null ? objInvoice.PhInvoices.PhInvoice.SALERTAXCODE : "";
            obj.SellerAddressLine = objInvoice.PhInvoices.PhInvoice.SALERADDRESSLINE != null ? objInvoice.PhInvoices.PhInvoice.SALERADDRESSLINE : "";
            obj.SellerPhoneNumber = "";
            obj.SellerEmail = "";
            obj.SellerBankName = "";
            obj.SellerBankAccount = "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.PhInvoices.PhInvoice.TEN_KH != null ? objInvoice.PhInvoices.PhInvoice.TEN_KH : "";
            obj.BuyerLegalName = objInvoice.PhInvoices.PhInvoice.TEN_KH != null ? objInvoice.PhInvoices.PhInvoice.TEN_KH : "";
            obj.BuyerTaxCode = objInvoice.PhInvoices.PhInvoice.MA_SO_THUE != null ? objInvoice.PhInvoices.PhInvoice.MA_SO_THUE : "";
            obj.BuyerAddressLine = objInvoice.PhInvoices.PhInvoice.DIA_CHI != null ? objInvoice.PhInvoices.PhInvoice.DIA_CHI : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.PhInvoices.PhInvoice.T_TIEN_NT2 != null ? objInvoice.PhInvoices.PhInvoice.T_TIEN_NT2 : "0";
            obj.TotalVATAmount = objInvoice.PhInvoices.PhInvoice.T_THUE_NT != null ? objInvoice.PhInvoices.PhInvoice.T_THUE_NT : "0";
            obj.TotalAmountWithVAT = objInvoice.PhInvoices.PhInvoice.T_TT_NT != null ? objInvoice.PhInvoices.PhInvoice.T_TT_NT : "0";
            obj.TotalAmountWithVATInWords = "";

            //obj.Items = objInvoice.InvoiceData.Items != null ? objInvoice.InvoiceData.Items : null;

            if (objInvoice.CtInvoices != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.CtInvoices.CtInvoice.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    Models.TCT10.CtInvoice product = objInvoice.CtInvoices.CtInvoice[i];

                    item.LineNumber = product.STT_REC0 != null ? product.STT_REC0 : "";
                    item.ItemCode = product.MA_VT != null ? product.MA_VT : "";
                    item.ItemName = product.TEN_VT != null ? product.TEN_VT : "";
                    item.UnitCode = "";
                    item.UnitName = product.DVT != null ? product.DVT : "";
                    item.ItemTotalAmountWithoutVat = product.TIEN_NT2 != null ? product.TIEN_NT2 : "0";
                    item.VatPercentage = product.THUE_SUAT != null ? product.THUE_SUAT : "";
                    item.VatAmount = product.THUE_NT != null ? product.THUE_NT : "";

                    item.ProdPrice = product.GIA_NT2 != null ? product.GIA_NT2 : "";
                    item.ProdQuantity = "";

                    itemName += item.ItemName + " ";
                    vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                obj.VatPercentage = vatPercentage;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;

            }

            return obj;
        }
        private EBill getEBillTCT9(Models.TCT9.ACPRO objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.MAU_HOADON != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.MAU_HOADON : "";
            obj.InvoiceSeries = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.KYHIEU_HOADON != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.KYHIEU_HOADON : "";
            obj.InvoiceNumber = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.SO_HOADON != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.SO_HOADON : "";
            obj.InvoiceName = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_HOADON != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_HOADON : "";
            obj.InvoiceIssuedDate = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.NGAY_HOADON != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.NGAY_HOADON : "";

            obj.SignedDate = "";
            obj.CurrencyCode = "";
            obj.InvoiceNote = "";
            obj.SellerLegalName = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_CONGTY != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_CONGTY : "";
            obj.SellerTaxCode = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.MST_CONGTY != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.MST_CONGTY : "";
            obj.SellerAddressLine = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.DIACHI_CONGTY != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.DIACHI_CONGTY : "";
            obj.SellerPhoneNumber = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEL_CONGTY != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEL_CONGTY : "";
            obj.SellerEmail = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.EMAIL_CONGTY != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.EMAIL_CONGTY : "";
            obj.SellerBankName = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_NH_CONGTY != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_NH_CONGTY : "";
            obj.SellerBankAccount = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.SOTK_NH_CONGTY != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.SOTK_NH_CONGTY : "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_DONVI != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_DONVI : "";
            obj.BuyerLegalName = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_DONVI != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.TEN_DONVI : "";
            obj.BuyerTaxCode = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.MST_KHACHHANG != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.MST_KHACHHANG : "";
            obj.BuyerAddressLine = objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.DIACHI_KHACHHANG != null ? objInvoice.ACPRO_THONGTIN.ACPRO_KHACHHANG.DIACHI_KHACHHANG : "";
            obj.BuyerPhoneNumber = "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = "";

            obj.TotalAmountWithoutVAT = objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TONG_TIENHANG != null ? objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TONG_TIENHANG : "0";
            obj.TotalVATAmount = objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TIEN_THUE != null ? objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TIEN_THUE : "0";

            obj.DiscountAmount = objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TIEN_GIAMGIA != null ? objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TIEN_GIAMGIA : "0";
            obj.TotalAmountWithVAT = objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TONG_CONG != null ? objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TONG_CONG : "0";
            obj.TotalAmountWithVATInWords = "";

            if (objInvoice.ACPRO_THONGTIN.ACPRO_DM_HANGHOA.ACPRO_DM_HANGHOA_CT != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.ACPRO_THONGTIN.ACPRO_DM_HANGHOA.ACPRO_DM_HANGHOA_CT.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    Models.TCT9.ACPRO_DM_HANGHOA_CT product = objInvoice.ACPRO_THONGTIN.ACPRO_DM_HANGHOA.ACPRO_DM_HANGHOA_CT[i];

                    item.LineNumber = "";
                    item.ItemCode = product.COL_MA_VT != null ? product.COL_MA_VT : "";
                    item.ItemName = product.COL_TEN_VT != null ? product.COL_TEN_VT : "";
                    item.UnitCode = "";
                    item.UnitName = product.COL_DV_TINH != null ? product.COL_DV_TINH : "";
                    item.ItemTotalAmountWithoutVat = product.COL_DON_GIA != null ? product.COL_DON_GIA : "0";
                    item.VatPercentage = product.COL_THUE_SUAT != null ? product.COL_THUE_SUAT : "";
                    item.VatAmount = product.COL_TIEN_THUE != null ? product.COL_TIEN_THUE : "";

                    item.ProdPrice = product.COL_DON_GIA != null ? product.COL_DON_GIA : "";
                    item.ProdQuantity = product.COL_SO_LUONG != null ? product.COL_SO_LUONG : "";

                    itemName += item.ItemName + " ";
                    //vatPercentage = item.VatPercentage;

                    lst.Add(item);

                }

                obj.ItemName = itemName;
                obj.VatPercentage = objInvoice.ACPRO_THONGTIN.ACPRO_THANHTOAN.TILE_THUE;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;
            }

            return obj;
        }
        private EBill getEBillVNPT(Models.VNPT.Invoice objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.Content.InvoicePattern != null ? objInvoice.Content.InvoicePattern : "";
            obj.InvoiceSeries = objInvoice.Content.SerialNo != null ? objInvoice.Content.SerialNo : "";
            obj.InvoiceNumber = objInvoice.Content.InvoiceNo != null ? objInvoice.Content.InvoiceNo : "";
            obj.InvoiceName = objInvoice.Content.InvoiceName != null ? objInvoice.Content.InvoiceName : "";
            //obj.InvoiceIssuedDate = objInvoice.Content.ArisingDate != null ? objInvoice.Content.ArisingDate : "";

            if (objInvoice.Content.ArisingDate != null)
            {
                string temp = "";

                if (objInvoice.Content.ArisingDate.Substring(2, 1).Contains("-"))
                    temp = objInvoice.Content.ArisingDate.Replace("-", "/");
                else
                    temp = objInvoice.Content.ArisingDate.Replace("-", "");

                if (temp.Contains("/"))
                    obj.InvoiceIssuedDate = temp.Substring(0, 10);
                else
                    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
            }
            else
                obj.InvoiceIssuedDate = "";

            obj.SignedDate = objInvoice.Content.SignDate != null ? objInvoice.Content.SignDate : "";
            obj.CurrencyCode = objInvoice.Content.CurrencyUnit != null ? objInvoice.Content.CurrencyUnit : "";
            obj.InvoiceNote = objInvoice.Content.Extra != null ? objInvoice.Content.Extra : "";
            obj.SellerLegalName = objInvoice.Content.ComName != null ? objInvoice.Content.ComName : "";
            obj.SellerTaxCode = objInvoice.Content.ComTaxCode != null ? objInvoice.Content.ComTaxCode : "";
            obj.SellerAddressLine = objInvoice.Content.ComAddress != null ? objInvoice.Content.ComAddress : "";
            obj.SellerPhoneNumber = objInvoice.Content.ComPhone != null ? objInvoice.Content.ComPhone : "";
            obj.SellerEmail = objInvoice.Content.ComEmail != null ? objInvoice.Content.ComEmail : "";
            obj.SellerBankName = objInvoice.Content.ComBankName != null ? objInvoice.Content.ComBankName : "";
            obj.SellerBankAccount = objInvoice.Content.ComBankNo != null ? objInvoice.Content.ComBankNo : "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.Content.CusName != null ? objInvoice.Content.CusName : "";
            obj.BuyerLegalName = objInvoice.Content.CusName != null ? objInvoice.Content.CusName : "";
            obj.BuyerCode = objInvoice.Content.CusCode != null ? objInvoice.Content.CusCode : "";
            obj.BuyerTaxCode = objInvoice.Content.CusTaxCode != null ? objInvoice.Content.CusTaxCode : "";
            obj.BuyerAddressLine = objInvoice.Content.CusAddress != null ? objInvoice.Content.CusAddress : "";
            obj.BuyerPhoneNumber = objInvoice.Content.CusPhone != null ? objInvoice.Content.CusPhone : "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = objInvoice.Content.CusBankNo != null ? objInvoice.Content.CusBankNo : "";

            obj.TotalAmountWithoutVAT = objInvoice.Content.Total != null ? objInvoice.Content.Total : "0";

            obj.TotalVATAmount = objInvoice.Content.VATAmount != null ? objInvoice.Content.VATAmount :
                   (objInvoice.Content.VAT_Amount != null ? objInvoice.Content.VAT_Amount : "0");

            obj.TotalAmountWithVAT = objInvoice.Content.Amount != null ? objInvoice.Content.Amount : "0";

            obj.VatPercentage = objInvoice.Content.VATRate != null ? objInvoice.Content.VATRate :
                        (objInvoice.Content.VAT_Rate != null ? objInvoice.Content.VAT_Rate : "0");

            obj.TotalAmountWithVATInWords = objInvoice.Content.AmountInWords != null ? objInvoice.Content.AmountInWords :
                (objInvoice.Content.Amount_words != null ? objInvoice.Content.Amount_words : "");

            if (objInvoice.Content.Products != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.Content.Products.Product.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    Models.VNPT.Product product = objInvoice.Content.Products.Product[i];

                    item.LineNumber = "";
                    item.ItemCode = product.Code != null ? product.Code : "";
                    item.ItemName = product.ProdName != null ? product.ProdName : "";
                    item.UnitCode = product.ProdUnit != null ? product.ProdUnit : "";
                    item.UnitName = product.ProdUnit != null ? product.ProdUnit : "";
                    item.ItemTotalAmountWithoutVat = product.Total != null ? product.Total : "0";
                    item.VatPercentage = product.VATRate != null ? product.VATRate : "";
                    item.VatAmount = product.VATAmount != null ? product.VATAmount : "0";

                    item.ProdPrice = product.ProdPrice != null ? product.ProdPrice : "0";
                    item.ProdQuantity = product.ProdQuantity != null ? product.ProdQuantity : "0";

                    itemName += item.ItemName + " ";

                    lst.Add(item);
                }


                if (objInvoice.Content.Buyer != null)
                    itemName += objInvoice.Content.Buyer + " ";

                obj.ItemName = itemName;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;
            }

            if (objInvoice.Content.Groups != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.Content.Groups.Group.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    Models.VNPT.Group product = objInvoice.Content.Groups.Group[i];

                    item.LineNumber = "";
                    item.ItemCode = product.DocNo_RO != null ? product.DocNo_RO : "";
                    item.ItemName = product.Note != null ? product.Note : "";
                    item.UnitCode = "";
                    item.UnitName = "";
                    item.ItemTotalAmountWithoutVat = product.Amount != null ? product.Amount : "0";
                    item.VatPercentage = "";
                    item.VatAmount = "0";

                    item.ProdPrice = product.ProdPrice != null ? product.ProdPrice : "0";
                    item.ProdQuantity = product.ProdQuantity != null ? product.ProdQuantity : "0";

                    itemName += item.ItemName + " ";

                    lst.Add(item);
                }


                if (objInvoice.Content.Buyer != null)
                    itemName += objInvoice.Content.Buyer + " ";

                obj.ItemName = itemName;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;
            }

            return obj;
        }
        private EBill getEBillVNPT2(Models.VNPT2.Invoice objInvoice)
        {
            EBill obj = new EBill();

            obj.TemplateCode = objInvoice.Content.InvoicePattern != null ? objInvoice.Content.InvoicePattern : "";
            obj.InvoiceSeries = objInvoice.Content.InvoiceSerial != null ? objInvoice.Content.InvoiceSerial : "";
            obj.InvoiceNumber = objInvoice.Content.InvoiceNumber != null ? objInvoice.Content.InvoiceNumber : "";
            obj.InvoiceName = objInvoice.Content.InvoiceName != null ? objInvoice.Content.InvoiceName : "";
            //obj.InvoiceIssuedDate = objInvoice.Content.ArisingDate != null ? objInvoice.Content.ArisingDate : "";

            if (objInvoice.Content.InvoiceDate != null)
            {
                string temp = "";

                if (objInvoice.Content.InvoiceDate.Substring(2, 1).Contains("-"))
                    temp = objInvoice.Content.InvoiceDate.Replace("-", "/");
                else
                    temp = objInvoice.Content.InvoiceDate.Replace("-", "");

                if (temp.Contains("/"))
                    obj.InvoiceIssuedDate = temp.Substring(0, 10);
                else
                    obj.InvoiceIssuedDate = temp.Substring(6, 2) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(0, 4);
            }
            else
                obj.InvoiceIssuedDate = "";

            obj.SignedDate = objInvoice.Content.SignedDate != null ? objInvoice.Content.SignedDate : "";
            obj.CurrencyCode = objInvoice.Content.CurrencyUnit != null ? objInvoice.Content.CurrencyUnit : "";
            obj.InvoiceNote = objInvoice.Content.Note != null ? objInvoice.Content.Note : "";
            obj.SellerLegalName = objInvoice.Content.CompanyName != null ? objInvoice.Content.CompanyName : "";
            obj.SellerTaxCode = objInvoice.Content.CompanyTaxCode != null ? objInvoice.Content.CompanyTaxCode : "";
            obj.SellerAddressLine = objInvoice.Content.CompanyAddress != null ? objInvoice.Content.CompanyAddress : "";
            obj.SellerPhoneNumber = objInvoice.Content.CompanyPhone != null ? objInvoice.Content.CompanyPhone : "";
            obj.SellerEmail = objInvoice.Content.CompanyEmail != null ? objInvoice.Content.CompanyEmail : "";
            obj.SellerBankName = objInvoice.Content.CompanyBankName != null ? objInvoice.Content.CompanyBankName : "";
            obj.SellerBankAccount = objInvoice.Content.CompanyBankAccount != null ? objInvoice.Content.CompanyBankAccount : "";
            obj.SellerContactPersonName = "";

            obj.BuyerDisplayName = objInvoice.Content.CustomerName != null ? objInvoice.Content.CustomerName : "";
            obj.BuyerLegalName = objInvoice.Content.CustomerName != null ? objInvoice.Content.CustomerName : "";
            obj.BuyerTaxCode = objInvoice.Content.CustomerTaxCode != null ? objInvoice.Content.CustomerTaxCode : "";
            //  obj.BuyerTaxCode = objInvoice.Content.CusTaxCode != null ? objInvoice.Content.CusTaxCode : "";
            obj.BuyerAddressLine = objInvoice.Content.CustomerAddress != null ? objInvoice.Content.CustomerAddress : "";
            obj.BuyerPhoneNumber = objInvoice.Content.CustomerPhone != null ? objInvoice.Content.CustomerPhone : "";
            obj.BuyerEmail = "";
            obj.BuyerBankAccount = objInvoice.Content.CustomerBankAccount != null ? objInvoice.Content.CustomerBankAccount : "";

            obj.TotalAmountWithoutVAT = objInvoice.Content.Amount != null ? objInvoice.Content.Amount : "0";

            obj.TotalVATAmount = objInvoice.Content.TaxAmount != null ? objInvoice.Content.TaxAmount :
                   (objInvoice.Content.TaxAmount != null ? objInvoice.Content.TaxAmount : "0");

            obj.TotalAmountWithVAT = objInvoice.Content.TotalAmount != null ? objInvoice.Content.TotalAmount : "0";

            obj.VatPercentage = objInvoice.Content.TaxRate != null ? objInvoice.Content.TaxRate :
                        (objInvoice.Content.TaxRate != null ? objInvoice.Content.TaxRate : "0");

            obj.TotalAmountWithVATInWords = objInvoice.Content.AmountInWords != null ? objInvoice.Content.AmountInWords :
                (objInvoice.Content.AmountInWords != null ? objInvoice.Content.AmountInWords : "");

            if (objInvoice.Content.Products != null)
            {
                List<Models.TCT.Item> lst = new List<Models.TCT.Item>();
                string itemName = "";
                string vatPercentage = "";

                for (int i = 0; i < objInvoice.Content.Products.Product.Count; i++)
                {
                    Models.TCT.Item item = new Models.TCT.Item();
                    Models.VNPT2.Product product = objInvoice.Content.Products.Product[i];

                    item.LineNumber = "";
                    item.ItemCode = product.ItemCode != null ? product.ItemCode : "";
                    item.ItemName = product.ItemName != null ? product.ItemName : "";
                    item.UnitCode = product.UnitOfMeasure != null ? product.UnitOfMeasure : "";
                    item.UnitName = product.UnitOfMeasure != null ? product.UnitOfMeasure : "";
                    item.ItemTotalAmountWithoutVat = product.Amount != null ? product.Amount : "0";
                    item.VatPercentage = product.TaxRate != null ? product.TaxRate : "";
                    item.VatAmount = product.TaxAmount != null ? product.TaxAmount : "0";

                    item.ProdPrice = product.ProdPrice != null ? product.ProdPrice : "0";
                    item.ProdQuantity = product.ProdQuantity != null ? product.ProdQuantity : "0";

                    itemName += item.ItemName + " ";

                    lst.Add(item);
                }


                if (objInvoice.Content.Buyer != null)
                    itemName += objInvoice.Content.Buyer + " ";

                obj.ItemName = itemName;

                obj.Items = new Models.TCT.Items();
                obj.Items.Item = lst;
            }

            return obj;
        }
        [Route("invoice")]
        [HttpPost]
        public IActionResult readEBillXML(ReadEBill objInput)
        {
            EBill objInvoiceResult = new EBill();

            if (objInput.EBillXmlContent.Contains("<HDon>"))
            {
                ESCS.Models.TCT3.HDon objInvoice = new Models.TCT3.HDon();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT3.HDon));
                    objInvoice = (ESCS.Models.TCT3.HDon)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT3(objInvoice);

                    // Check hoa don co toan ven 

                    if (objInvoice.Signature != null)
                        objInvoiceResult.IsSigned = true;
                    else
                        objInvoiceResult.IsSigned = false;
                }

                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("<Ma_loaihd>"))
            {
                ESCS.Models.TCT8.Vht objInvoice = new Models.TCT8.Vht();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT8.Vht));
                    objInvoice = (ESCS.Models.TCT8.Vht)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT8(objInvoice);

                    // Check hoa don co toan ven 

                    objInvoiceResult.IsEntirety = false;

                    if (objInvoice.Signature != null)
                        objInvoiceResult.IsSigned = true;
                    else
                        objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("<CyberInvoiceData>"))
            {
                ESCS.Models.TCT10.CyberInvoiceData objInvoice = new Models.TCT10.CyberInvoiceData();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT10.CyberInvoiceData));
                    objInvoice = (ESCS.Models.TCT10.CyberInvoiceData)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT10(objInvoice);

                    // Check hoa don co toan ven 

                    objInvoiceResult.IsEntirety = false;

                    objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("<ObjTempInvoice>"))
            {
                ESCS.Models.TCT11.Invoice objInvoice = new Models.TCT11.Invoice();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT11.Invoice));
                    objInvoice = (ESCS.Models.TCT11.Invoice)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT11(objInvoice);

                    // Check hoa don co toan ven 

                    objInvoiceResult.IsEntirety = false;
                    objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("ACPRO_KHACHHANG"))
            {
                ESCS.Models.TCT9.ACPRO objInvoice = new Models.TCT9.ACPRO();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT9.ACPRO));
                    objInvoice = (ESCS.Models.TCT9.ACPRO)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT9(objInvoice);

                    // Check hoa don co toan ven 

                    objInvoiceResult.IsEntirety = false;
                    objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("<HoaDonDienTu>"))
            {
                ESCS.Models.TCT4.HoaDonDienTu objInvoice = new Models.TCT4.HoaDonDienTu();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT4.HoaDonDienTu));
                    objInvoice = (ESCS.Models.TCT4.HoaDonDienTu)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT4(objInvoice);

                    // Check hoa don co toan ven 

                    objInvoiceResult.IsEntirety = false;
                    objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("<M_Ma_So_Thue>"))
            {
                ESCS.Models.TCT6.Invoices objInvoice = new Models.TCT6.Invoices();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT6.Invoices));
                    objInvoice = (ESCS.Models.TCT6.Invoices)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT6(objInvoice);

                    // Check hoa don co toan ven 

                    objInvoiceResult.IsEntirety = false;
                    objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("<CusivoiceTempt>"))
            {
                ESCS.Models.TCT7.Invoice1 objInvoice = new Models.TCT7.Invoice1();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT7.Invoice1));
                    objInvoice = (ESCS.Models.TCT7.Invoice1)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT7(objInvoice);

                    // Check hoa don co toan ven 

                    objInvoiceResult.IsEntirety = false;
                    objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("ComName"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    try
                    {
                        ESCS.Models.VNPT.Invoice objInvoice = new Models.VNPT.Invoice();

                        var serializer = new XmlSerializer(typeof(ESCS.Models.VNPT.Invoice));
                        objInvoice = (ESCS.Models.VNPT.Invoice)serializer.Deserialize(reader);

                        if (objInvoice != null)
                        {
                            // Lay thong tin hoa don
                            objInvoiceResult = getEBillVNPT(objInvoice);

                            // Check hoa don co toan ven 
                            objInvoiceResult.IsEntirety = false;   // SignHelper.EnvelopedXmlVerify(objInput.EBillXmlContent);

                            if (objInvoice.Signature != null)
                                objInvoiceResult.IsSigned = true;
                            else
                                objInvoiceResult.IsSigned = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ESCS.Models.VNPT1.Invoice objInvoice = new Models.VNPT1.Invoice();

                        var serializer = new XmlSerializer(typeof(ESCS.Models.VNPT1.Invoice));
                        objInvoice = (ESCS.Models.VNPT1.Invoice)serializer.Deserialize(reader);

                        if (objInvoice != null)
                        {
                            // Lay thong tin hoa don
                            //objInvoiceResult = getEBillVNPT(objInvoice);

                            // Check hoa don co toan ven 
                            objInvoiceResult.IsEntirety = false;   // SignHelper.EnvelopedXmlVerify(objInput.EBillXmlContent);

                            if (objInvoice.Signature != null)
                                objInvoiceResult.IsSigned = true;
                            else
                                objInvoiceResult.IsSigned = false;
                        }
                    }

                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("CompanyName"))
            {
                ESCS.Models.VNPT2.Invoice objInvoice = new Models.VNPT2.Invoice();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.VNPT2.Invoice));
                    objInvoice = (ESCS.Models.VNPT2.Invoice)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillVNPT2(objInvoice);

                    // Check hoa don co toan ven 
                    objInvoiceResult.IsEntirety = false;   // SignHelper.EnvelopedXmlVerify(objInput.EBillXmlContent);

                    if (objInvoice.Signature != null)
                        objInvoiceResult.IsSigned = true;
                    else
                        objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else if (objInput.EBillXmlContent.Contains("<transaction>"))
            {
                ESCS.Models.TCT2.Invoice objInvoice = new Models.TCT2.Invoice();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);
                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT2.Invoice));
                    objInvoice = (ESCS.Models.TCT2.Invoice)serializer.Deserialize(reader);
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT2(objInvoice);

                    // Check hoa don co toan ven 
                    try
                    {
                        objInvoiceResult.IsEntirety = true; //SignHelper.EnvelopedXmlVerify(objInput.EBillXmlContent);
                    }
                    catch (Exception)
                    {
                        objInvoiceResult.IsEntirety = false;
                    }

                    if (objInvoice.Signature != null)
                        objInvoiceResult.IsSigned = true;
                    else
                        objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
            else 
            {
                ESCS.Models.TCT.Invoice objInvoice = new Models.TCT.Invoice();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(objInput.EBillXmlContent);

                using (XmlReader reader = XmlReader.Create((new StringReader(xmlDoc.InnerXml))))
                {
                    var serializer = new XmlSerializer(typeof(ESCS.Models.TCT.Invoice));

                    try
                    {
                        objInvoice = (ESCS.Models.TCT.Invoice)serializer.Deserialize(reader);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (objInvoice != null)
                {
                    // Lay thong tin hoa don
                    objInvoiceResult = getEBillTCT(objInvoice);

                    // Check hoa don co toan ven 
                    try
                    {
                        objInvoiceResult.IsEntirety = true; // SignHelper.EnvelopedXmlVerify(objInput.EBillXmlContent);
                    }
                    catch (Exception ex)
                    {
                        objInvoiceResult.IsEntirety = false;
                    }

                    if (objInvoice.Signature != null)
                        objInvoiceResult.IsSigned = true;
                    else
                        objInvoiceResult.IsSigned = false;
                }
                objInvoiceResult.ChuanHoaDuLieu();
                return Ok(objInvoiceResult);
            }
        }
        [Route("get-invoice")]
        public IActionResult ReadInvoice()
        {
            //Đường dẫn file hóa đơn trên server (.Net thường cú pháp dùng Server.MapPath)
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, @"Files/HoaDon/hoa_don_dien_tu.xml");
            //Load file theo đường dẫn
            XDocument xDoc = XDocument.Load(path);
            //Lấy key ký dữ liệu trong file xml trong element X509Certificate
            var key = xDoc.Descendants().Where(n => n.Name.LocalName == "X509Certificate").FirstOrDefault().Value;
            //Tạo đối tượng X509Certificate2 theo key đã lấy ra
            X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(key));
            //Xác thực hóa đơn (true - thành công, false - thất bại)
            var check = VerifyXMLDocument(path, cert);
            //Đoạn dưới là chuyển XML về json (bỏ qua)
            dynamic root = null;
            if (check)
            {
                root = new ExpandoObject();
                XmlToDynamic.Parse(root, xDoc.Elements().First());
            }
            else
            {
                root = new { error = "INVALID", mesage = "Hóa đơn không hợp lệ" };
            }
            return Ok(root);
        }
        /// <summary>
        /// Hàm vertify hóa đơn điện tử
        /// </summary>
        /// <param name="xmlFilePath">Là đường dẫn file xml hóa đơn</param>
        /// <param name="certificate">Là đối tượng của lớp X509Certificate2</param>
        /// <returns></returns>
        private bool VerifyXMLDocument(string xmlFilePath, X509Certificate2 certificate)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            var signedXml = new SignedXml(xmlDocument);
            var nodeList = xmlDocument.GetElementsByTagName("Signature");
            signedXml.LoadXml((XmlElement)nodeList[0]);
            using (var rsaKey = certificate.PublicKey.Key)
            {
                return signedXml.CheckSignature(rsaKey);
            }
        }
        private bool VerifyXMLInvoice(string xmlFilePath)
        {
            //Install-Package System.Xml.XDocument -Version 4.3.0
            XDocument xDoc = XDocument.Load(xmlFilePath);
            //Lấy key ký dữ liệu trong file xml trong element X509Certificate
            var key = xDoc.Descendants().Where(n => n.Name.LocalName == "X509Certificate").FirstOrDefault().Value;
            //Tạo đối tượng X509Certificate2 theo key đã lấy ra
            X509Certificate2 certificate = new X509Certificate2(Convert.FromBase64String(key));
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFilePath);
            var signedXml = new SignedXml(xmlDocument);
            var nodeList = xmlDocument.GetElementsByTagName("Signature");
            signedXml.LoadXml((XmlElement)nodeList[0]);
            using (var rsaKey = certificate.PublicKey.Key)
            {
                return signedXml.CheckSignature(rsaKey);
            }
        }
    }
}
