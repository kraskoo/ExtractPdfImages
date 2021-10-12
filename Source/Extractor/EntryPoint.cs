namespace Extractor
{
    using System;
    using System.IO;

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
                foreach (var kvp in images)
                {
                    images[kvp.Key].Save(Path.Combine(pdfFileNewDirectory, kvp.Key));
                }
            }
        }
    }
}