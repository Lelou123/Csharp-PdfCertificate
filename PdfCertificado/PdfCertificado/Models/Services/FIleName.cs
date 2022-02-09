using System.IO;
using System;
using System.Collections.Generic;
using Spire.Pdf.Graphics;
using System.Drawing;
using Spire.Pdf.Security;
using Spire.Pdf;
using Microsoft.AspNetCore.Hosting;

namespace PdfCertificado.Models.Services
{
    public class FIleName
    {
        public string FileName { get; set; }
        public string CertPassword { get; set; }


        public void CreateSenha(string targetPath, FIleName filen)
		{
            User user = new User();
            
            using (StreamWriter sw = File.AppendText(targetPath))
            {
                sw.WriteLine(filen.CertPassword);
            }
        }

        public void AddAssinaturaDigital(string filePath, string passwordPath, string certPath, string userName,  IWebHostEnvironment _environment)
        {

            string password;
            string pathPassword = passwordPath;
            string pathAssinados = Path.Combine(_environment.WebRootPath, "uploads", userName, "docsAssinados");
            string pathUserAssinados = Path.Combine(pathAssinados);
            
            using (StreamReader sr = new StreamReader(pathPassword))
            {
                password = sr.ReadLine();
            }

            PdfDocument document = new PdfDocument();
            document.LoadFromFile(filePath);

            PdfCertificate cert = new PdfCertificate(certPath, password);

            PdfSignature signature = new PdfSignature(document, document.Pages[document.Pages.Count - 1], cert, "MinhaAssinatura");

            RectangleF rectangleF = new RectangleF(document.Pages[0].ActualSize.Width - 220, 50, 200, 80);
            signature.Bounds = rectangleF;

            signature.Certificated = true;

            signature.GraphicsMode = GraphicMode.SignDetail;



            signature.Name = userName;
            signature.Date = DateTime.Now;

            signature.SignDetailsFont = new PdfTrueTypeFont(new Font("Arial Unicode MS", 12, FontStyle.Regular));

            signature.DocumentPermissions = PdfCertificationFlags.ForbidChanges | PdfCertificationFlags.AllowFormFill;

            document.SaveToFile(Path.Combine(pathUserAssinados, Path.GetFileName(filePath)));
            File.Delete(filePath);
            document.Close();
        }

        public void DeletArquivo(string sourcePath, string targetPath)
        {

            File.Copy(sourcePath, targetPath);
            File.Delete(sourcePath);
        }


    }
}
