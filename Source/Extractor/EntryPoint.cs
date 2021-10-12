namespace Extractor
{
    using System;
    using System.IO;
    using System.Linq;

    public class EntryPoint
    {
        public static void Main()
        {
            Console.Write("Enter pdf path: ");
            var pdfPath = Console.ReadLine();
            var pdfImages = Path.Combine(pdfPath, "Images");
            if (!Directory.Exists(pdfImages))
            {
                Directory.CreateDirectory(pdfImages);
            }

            foreach (var filename in Directory.GetFiles(pdfPath, "*.pdf", SearchOption.TopDirectoryOnly))
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                var pdfFileNewDirectory = Path.Combine(pdfImages, fileNameWithoutExtension);
                if (!Directory.Exists(pdfFileNewDirectory))
                {
                    Directory.CreateDirectory(pdfFileNewDirectory);
                }

                var images = PdfImageExtractor.ExtractImages(filename);
                foreach (var name in images.Keys)
                {
                    images[name].Save(Path.Combine(pdfFileNewDirectory, name));
                }
            }
        }
    }
}