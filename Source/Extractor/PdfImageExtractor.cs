namespace Extractor
{
    using iTextSharp.text.pdf;
    using iTextSharp.text.pdf.parser;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public static class PdfImageExtractor
    {
        public static Dictionary<string, Image> ExtractImages(string filename)
        {
            var images = new Dictionary<string, Image>();
            using (var reader = new PdfReader(filename))
            {
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    var parser = new PdfReaderContentParser(reader);
                    var listener = new ImageRenderListener();
                    parser.ProcessContent(i, listener);
                    var index = 1;
                    if (listener.Images.Count > 0)
                    {
                        foreach (var pair in listener.Images)
                        {
                            images.Add($"{filename.Split('.').Last()}_Page_{i:D4}_Image_{index:D4}{pair.Value}", pair.Key);
                            index++;
                        }
                    }
                }

                return images;
            }
        }
    }
}