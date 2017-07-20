namespace Extractor
{
    using System;
    using System.Globalization;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using iTextSharp.text.pdf;

    public class EntryPoint
    {
        public static void Main()
        {
            Console.Write("Enter pdf path: ");
            var pdfPath = Console.ReadLine();
            var pdfImages = $"{pdfPath}\\Images";
            if (!Directory.Exists(pdfImages))
            {
                Directory.CreateDirectory(pdfImages);
            }

            var files = Directory.GetFiles(pdfPath);
            foreach (var file in files)
            {
                Console.WriteLine($"Extracting images from {file} ...");
                var indexOfLastSlash = file.LastIndexOf('\\') + 1;
                var fileName = file.Substring(indexOfLastSlash);

                if (fileName.ToLower().EndsWith(".pdf"))
                {
                    var fileNameWithoutExtension = fileName.Substring(0, fileName.LastIndexOf('.'));
                    var pdfFileNewDirectory = $"{pdfImages}\\{fileNameWithoutExtension}";
                    if (!Directory.Exists(pdfFileNewDirectory))
                    {
                        Directory.CreateDirectory(pdfFileNewDirectory);
                    }

                    ExtractImagesFromPDF(file, pdfFileNewDirectory);
                }
            }
        }

        public static void ExtractImagesFromPDF(string sourcePdf, string outputPath)
        {
            PdfReader pdf = new PdfReader(sourcePdf);
            RandomAccessFileOrArray randomAccessFileOrArray = new RandomAccessFileOrArray(sourcePdf);
            try
            {
                for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNumber);
                    PdfObject obj = FindImageInPDFDictionary(pg);
                    if (obj != null)
                    {
                        int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(CultureInfo.InvariantCulture));
                        PdfObject pdfObject = pdf.GetPdfObject(XrefIndex);
                        PdfStream pdfStrem = (PdfStream)pdfObject;
                        byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                        if ((bytes != null))
                        {
                            using (MemoryStream memoryStream = new MemoryStream(bytes))
                            {
                                memoryStream.Position = 0;
                                var image = System.Drawing.Image.FromStream(memoryStream);
                                if (!Directory.Exists(outputPath))
                                {
                                    Directory.CreateDirectory(outputPath);
                                }

                                string path = Path.Combine(outputPath, string.Format(@"{0}.jpg", pageNumber));
                                EncoderParameters parms = new EncoderParameters(1);
                                parms.Param[0] = new EncoderParameter(Encoder.Compression, 0);
                                var encoders = ImageCodecInfo.GetImageEncoders();
                                ImageCodecInfo jpegEncoder = encoders.First(enc => enc.FormatDescription.Equals("JPEG"));
                                image.Save(path, jpegEncoder, parms);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                pdf.Close();
                randomAccessFileOrArray.Close();
            }
        }

        private static PdfObject FindImageInPDFDictionary(PdfDictionary pg)
        {
            PdfDictionary resources = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            PdfDictionary xObject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
            if (xObject != null)
            {
                foreach (PdfName name in xObject.Keys)
                {
                    PdfObject pdfObject = xObject.Get(name);
                    if (pdfObject.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(pdfObject);
                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                        if (PdfName.IMAGE.Equals(type))
                        {
                            return pdfObject;
                        }
                        else if (PdfName.FORM.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        }
                        else if (PdfName.GROUP.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        }
                    }
                }
            }

            return null;
        }
    }
}