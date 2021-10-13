namespace Extractor
{
    using iTextSharp.text.pdf;
    using iTextSharp.text.pdf.parser;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

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
                    foreach (var pair in listener.Images)
                    {
                        images.Add($"{i:D4}_Image_{index:D4}{pair.Value}", pair.Key);
                        index++;
                    }
                }

                return images;
            }
        }
    }
}