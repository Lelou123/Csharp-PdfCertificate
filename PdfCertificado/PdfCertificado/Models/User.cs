﻿using System.ComponentModel.DataAnnotations;

namespace PdfCertificado.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
