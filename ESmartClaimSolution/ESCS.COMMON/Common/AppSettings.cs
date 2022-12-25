using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class AppSettings
    {
        public static string Environment { get; set; }
        public static string PathFolderNotDelete { get; set; }
        public static string PathFolderNotDeleteFull { get; set; }
        public static string KeyEryptData { get; set; }
        public static string SignatureName { get; set; }
        public static bool ConnectApiCorePartner { get; set; }
        public static bool ConnectApiCorePartnerHealth { get; set; }
        public static bool InsuranceToCore { get; set; }
        public static bool UseDeveloperExceptionPage { get; set; }
        public static string QRCodeLink { get; set; }
        public static string QRBSHSCodeLink { get; set; }
        public static string QRCodeLinkDanhGiaRuiRo { get; set; }
        public static string QRCodeLinkDanhGiaRuiRoVersion { get; set; }
        public static string BVQRCodeLink { get; set; }
        public static string QRCodeVersion { get; set; }
        public static string BVQRCodeVersion { get; set; }
        public static string KeyExpiryTime { get; set; }
        public static string Internal { get; set; }
        public static string HashMIC { get; set; }
        public static string HashABIC { get; set; }
        public static bool FolderSharedUsed { get; set; }
        //public static string FolderSharedIP { get; set; }
        //public static string FolderSharedUser { get; set; }
        //public static string FolderSharedPas { get; set; }
    }
}
