using Aspose.Pdf;
using Aspose.Pdf.Devices;

namespace IngestaoCardapio.Utils
{
    public class PdfToPng
    {
        string pdfPath;

        string pngPath;

        public PdfToPng(string pdfPath, string pngPath)
        {
            string directory = System.IO.Directory.GetParent(pngPath).FullName;
            string pathComplete = System.IO.Path.Combine(directory, System.IO.Path.GetFileNameWithoutExtension(pngPath)) + ".pdf";
            this.pdfPath = pathComplete;
            this.pngPath = pngPath;
        }


        public void Convert()
        {
            Document pdfDocument = new Document(pdfPath);

            // Itera sobre cada página do PDF
            for (int pageNumber = 1; pageNumber <= pdfDocument.Pages.Count; pageNumber++)
            {
                // Cria um dispositivo de renderização para PNG com a resolução desejada (300 DPI)
                Resolution resolution = new Resolution(300);
                PngDevice pngDevice = new PngDevice(resolution);

                // Cria um arquivo de imagem para cada página do PDF
                if(Directory.Exists(@$".\{pngPath}.png"))
                    Directory.Delete(@$".\{pngPath}.png", true);
                pngDevice.Process(pdfDocument.Pages[pageNumber], @$".\{pngPath}.png");
            }   
        }
    }
}
