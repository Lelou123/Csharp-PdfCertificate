using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PdfCertificado.Models;

namespace PdfCertificado.Data
{
    public class PdfCertificadoContext : DbContext
    {
        public PdfCertificadoContext (DbContextOptions<PdfCertificadoContext> options)
            : base(options)
        {
        }

        public DbSet<PdfCertificado.Models.User> User { get; set; }
    }
}
