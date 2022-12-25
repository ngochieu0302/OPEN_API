using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.Common
{
    public class PDFWatermark
    {

        public static void WatermarkPDF(string sourceFile, string destinationPath)
        {
            float watermarkTrimmingRectangleWidth = 300;
            float watermarkTrimmingRectangleHeight = 300;
            float formWidth = 300;
            float formHeight = 300;
            float formXOffset = 0;
            float formYOffset = 0;
            float xTranslation = 50;
            float yTranslation = 25;
            double rotationInRads = Math.PI / 3;
            PdfFont font = PdfFontFactory.CreateFont(FontConstants.TIMES_ROMAN);
            float fontSize = 50;

            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFile), new PdfWriter(destinationPath));
            var numberOfPages = pdfDoc.GetNumberOfPages();
            PdfPage page = null;

            for (var i = 1; i <= numberOfPages; i++)
            {
                page = pdfDoc.GetPage(i);
                Rectangle ps = page.GetPageSize();

                //Center the annotation
                float bottomLeftX = ps.GetWidth() / 2 - watermarkTrimmingRectangleWidth / 2;
                float bottomLeftY = ps.GetHeight() / 2 - watermarkTrimmingRectangleHeight / 2;
                Rectangle watermarkTrimmingRectangle = new Rectangle(bottomLeftX, bottomLeftY, watermarkTrimmingRectangleWidth, watermarkTrimmingRectangleHeight);
                PdfWatermarkAnnotation watermark = new PdfWatermarkAnnotation(watermarkTrimmingRectangle);
                //Apply linear algebra rotation math
                //Create identity matrix
                AffineTransform transform = new AffineTransform();//No-args constructor creates the identity transform
                                                                  //Apply translation
                transform.Translate(xTranslation, yTranslation);
                //Apply rotation
                transform.Rotate(rotationInRads);

                PdfFixedPrint fixedPrint = new PdfFixedPrint();
                watermark.SetFixedPrint(fixedPrint);
                //Create appearance
                Rectangle formRectangle = new Rectangle(formXOffset, formYOffset, formWidth, formHeight);
                //Observation: font XObject will be resized to fit inside the watermark rectangle
                PdfFormXObject form = new PdfFormXObject(formRectangle);
                PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.6f);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);

                float[] transformValues = new float[6];
                transform.GetMatrix(transformValues);
                canvas.SaveState()
                    .BeginText().SetColor(ColorConstants.GRAY, true).SetExtGState(gs1)
                    .SetTextMatrix(transformValues[0], transformValues[1], transformValues[2], transformValues[3], transformValues[4], transformValues[5])
                    .SetFontAndSize(font, fontSize)
                    .ShowText("watermark text")
                    .EndText()
                    .RestoreState();

                canvas.Release();

                watermark.SetAppearance(PdfName.N, new PdfAnnotationAppearance(form.GetPdfObject()));
                watermark.SetFlags(PdfAnnotation.PRINT);
                page.AddAnnotation(watermark);
            }
            page?.Flush();
            pdfDoc.Close();
        }
        public static void RemovetWatermarkPDF(string sourceFile, string destinationPath)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFile), new PdfWriter(destinationPath));
            var numberOfPages = pdfDoc.GetNumberOfPages();

            for (var i = 1; i <= numberOfPages; i++)
            {
                PdfDictionary pageDict = pdfDoc.GetPage(i).GetPdfObject();
                PdfArray annots = pageDict.GetAsArray(PdfName.Annots);
                for (int j = 0; j < annots.Size(); j++)
                {
                    PdfDictionary annotation = annots.GetAsDictionary(j);
                    if (PdfName.Watermark.Equals(annotation.GetAsName(PdfName.Subtype)))
                    {
                        annotation.Clear();
                    }
                }
            }
            pdfDoc.Close();
        }
    }
}
