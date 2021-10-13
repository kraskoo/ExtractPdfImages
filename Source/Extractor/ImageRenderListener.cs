namespace Extractor
{
    using iTextSharp.text.pdf;
    using iTextSharp.text.pdf.parser;
    using System.Collections.Generic;
    using System.Drawing;

    internal class ImageRenderListener : IRenderListener
    {
        private readonly Dictionary<Image, string> images = new Dictionary<Image, string>();
        
        public Dictionary<Image, string> Images => images;

        public void BeginTextBlock()
        {
        }

        public void EndTextBlock()
        {
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            var image = renderInfo.GetImage();
            if (image.Get(PdfName.FILTER) is PdfName filter)
            {
                var drawingImage = image.GetDrawingImage();
                string extension = ".";
                if (filter == PdfName.DCTDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.JPG.FileExtension;
                }
                else if (filter == PdfName.JPXDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.JP2.FileExtension;
                }
                else if (filter == PdfName.FLATEDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.PNG.FileExtension;
                }
                else if (filter == PdfName.LZWDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.CCITT.FileExtension;
                }

                this.Images.Add(drawingImage, extension);
            }
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
        }
    }
}