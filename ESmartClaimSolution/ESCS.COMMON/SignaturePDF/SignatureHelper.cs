using ESCS.COMMON.Http;
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
//using Org.BouncyCastle.Pkcs;
//using iTextSharp.text.pdf;
//using iTextSharp.text.pdf.security;
//using Org.BouncyCastle.Crypto;
//using Org.BouncyCastle.Crypto.Parameters;
//using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static iText.Signatures.PdfSigner;

namespace ESCS.COMMON.SignaturePDF
{
    public class SignatureHelper
    {
        public string PathPFX { get; set; }
        public string PathOutput { get; set; }
        public string PasswordPFX { get; set; }
        public SignatureHelper(string pathPFX, string passwordPFX,string pathOutput)
        {
            this.PathPFX = pathPFX;
            this.PathOutput = pathOutput;
            this.PasswordPFX = passwordPFX;
        }
        #region itextsharp
        //private byte[] inputPDFByteArr = null;
        //private string inputPDF = "";
        //private byte[] outputPDFByteArr = null;
        //private string outputPDF = null;
        //private Cert myCert;
        //private MetaData metadata;
       // private NetworkCredentialItem network;
        //public SignatureHelper(string input, string output)
        //{
        //    this.inputPDF = input;
        //    this.outputPDF = output;
        //}
        //public SignatureHelper(string input, string output, Cert cert)
        //{
        //    this.inputPDF = input;
        //    this.outputPDF = output;
        //    this.myCert = cert;
        //}
        //public SignatureHelper(string input, string output, MetaData md)
        //{
        //    this.inputPDF = input;
        //    this.outputPDF = output;
        //    this.metadata = md;
        //}
        //public SignatureHelper(string input, string output, Cert cert, MetaData md)
        //{
        //    this.inputPDF = input;
        //    this.outputPDF = output;
        //    this.myCert = cert;
        //    this.metadata = md;
        //}
        //public SignatureHelper(byte[] byte_arr, string output, Cert cert, MetaData md, NetworkCredentialItem network)
        //{
        //    this.inputPDFByteArr = byte_arr;
        //    this.outputPDF = output;
        //    this.myCert = cert;
        //    this.metadata = md;
        //    this.network = network;
        //}

        //public bool Vertify(string pathFile)
        //{
        //    PdfReader reader = new PdfReader(pathFile);
        //    AcroFields af = reader.AcroFields;
        //    var names = af.GetSignatureNames();
        //    if (names.Count == 0)
        //    {
        //        return false;
        //    }
        //    foreach (string name in names)
        //    {
        //        PdfPKCS7 pk = af.VerifySignature(name);
        //        if (!pk.Verify())
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        #endregion

        //public void SignPdf(string SigReason, string SigContact, string SigLocation)
        //{
        //    PdfReader reader = new PdfReader(this.inputPDF);
        //    PdfStamper stamper = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0', null, true);
        //    PdfSignatureAppearance signatureAppearance = stamper.SignatureAppearance;
        //    signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION;
        //    signatureAppearance.Reason = SigReason;
        //    signatureAppearance.Contact = SigContact;
        //    signatureAppearance.Location = SigLocation;
        //    X509Certificate2 cert = new X509Certificate2(myCert.Path, myCert.Password, X509KeyStorageFlags.Exportable);
        //    var keyPair = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(cert.PrivateKey).Private;
        //    Org.BouncyCastle.X509.X509Certificate bcCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert);
        //    var chain = new List<Org.BouncyCastle.X509.X509Certificate> { bcCert };
        //    IExternalSignature signature = new PrivateKeySignature(keyPair, "SHA-256");
        //    MakeSignature.SignDetached(signatureAppearance, signature, chain, null, null, null, 0, CryptoStandard.CMS);
        //    stamper.Close();
        //}
        //public void SignPdfByteArray(string SigReason, string SigContact, string SigLocation)
        //{
        //    PdfReader reader = new PdfReader(this.inputPDFByteArr);
        //    PdfStamper stamper = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0', null, true);
        //    PdfSignatureAppearance signatureAppearance = stamper.SignatureAppearance;
        //    signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION;
        //    signatureAppearance.Reason = SigReason;
        //    signatureAppearance.Contact = SigContact;
        //    signatureAppearance.Location = SigLocation;
        //    X509Certificate2 cert = new X509Certificate2(myCert.Path, myCert.Password, X509KeyStorageFlags.Exportable);
        //    var keyPair = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(cert.PrivateKey).Private;
        //    Org.BouncyCastle.X509.X509Certificate bcCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert);
        //    var chain = new List<Org.BouncyCastle.X509.X509Certificate> { bcCert };
        //    IExternalSignature signature = new PrivateKeySignature(keyPair, "SHA-256");
        //    MakeSignature.SignDetached(signatureAppearance, signature, chain, null, null, null, 0, CryptoStandard.CMS);
        //    stamper.Close();
        //}


        //public void SignPdf(string SigReason, string SigContact, string SigLocation)
        //{
        //    PdfReader reader = new PdfReader(this.inputPDF);

        //    //PdfStamper stamper = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0', null, true);

        //    PdfSignatureAppearance signatureAppearance = stamper.SignatureAppearance;
        //    signatureAppearance.SetRenderingMode(PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
        //    signatureAppearance.SetReason(SigReason);
        //    signatureAppearance.SetContact(SigContact);
        //    signatureAppearance.SetLocation(SigLocation);
        //    X509Certificate2 cert = new X509Certificate2(myCert.Path, myCert.Password, X509KeyStorageFlags.Exportable);
        //    var keyPair = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(cert.PrivateKey).Private;
        //    Org.BouncyCastle.X509.X509Certificate bcCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert);
        //    var chain = new List<Org.BouncyCastle.X509.X509Certificate> { bcCert };
        //    IExternalSignature signature = new PrivateKeySignature(keyPair, "SHA-256");
        //    MakeSignature.SignDetached(signatureAppearance, signature, chain, null, null, null, 0, CryptoStandard.CMS);
        //    stamper.Close();
        //}

        public void SignPdfByteArray(string SigReason, string SigContact, string SigLocation, Stream stream)
        {
            Pkcs12Store pk12 = new Pkcs12Store(new FileStream(PathPFX, System.IO.FileMode.Open, FileAccess.Read), PasswordPFX.ToCharArray());
            string alias = null;
            foreach (object a in pk12.Aliases)
            {
                alias = ((string)a);
                if (pk12.IsKeyEntry(alias))
                {
                    break;
                }
            }
            ICipherParameters pk = pk12.GetKey(alias).Key;
            X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
            {
                chain[k] = ce[k].Certificate;
            }
            PdfReader reader = new PdfReader(stream);
            PdfSigner signer = new PdfSigner(reader, new FileStream(PathOutput, System.IO.FileMode.Create), new StampingProperties());
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetReason(SigReason)
                .SetLocation(SigLocation)
                .SetContact(SigContact);
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
        }

        //public static AsymmetricKeyParameter TransformRSAPrivateKey(AsymmetricAlgorithm privateKey)
        //{
        //    RSACryptoServiceProvider prov = privateKey as RSACryptoServiceProvider;
        //    RSAParameters parameters = prov.ExportParameters(true);

        //    return new RsaPrivateCrtKeyParameters(
        //        new BigInteger(1, parameters.Modulus),
        //        new BigInteger(1, parameters.Exponent),
        //        new BigInteger(1, parameters.D),
        //        new BigInteger(1, parameters.P),
        //        new BigInteger(1, parameters.Q),
        //        new BigInteger(1, parameters.DP),
        //        new BigInteger(1, parameters.DQ),
        //        new BigInteger(1, parameters.InverseQ));
        //}
    }
}
