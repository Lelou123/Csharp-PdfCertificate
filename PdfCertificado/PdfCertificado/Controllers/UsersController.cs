﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfCertificado.Data;
using PdfCertificado.Models;


namespace PdfCertificado.Controllers
{
    public class UsersController : Controller
    {
        private readonly PdfCertificadoContext _context;
        private IWebHostEnvironment _environment;
        public UsersController(PdfCertificadoContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        
        // GET: Users
        /*
        public async Task<IActionResult> Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                TempData["mensagemErro"] = "Voce já está logado em uma conta";
                return View(await _context.User.ToListAsync());

            }
            return View(await _context.User.ToListAsync());
        }

        */

        public async Task<IActionResult> Pdfview()
        {
            if (User.Identity.IsAuthenticated)
            {
                TempData["mensagemErro"] = "Voce já está logado em uma conta";
                return View(await _context.User.ToListAsync());

            }
            return View(await _context.User.ToListAsync());
        }


        


        [HttpPost]
        public async Task<IActionResult> Logar(string username, string senha, bool manterlogado)
        {
            User usuario = _context.User.AsNoTracking().FirstOrDefault(x => x.Username == username && x.Password == senha);

            

            if (usuario != null)
            {
                int usuarioId = usuario.Id;
                string nome = usuario.Username;

                List<Claim> direitosAcesso = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,usuarioId.ToString()),
                    new Claim(ClaimTypes.Name,nome)
                };

                var identity = new ClaimsIdentity(direitosAcesso, "Identity.Login");
                var userPrincipal = new ClaimsPrincipal(new[] { identity });

                await HttpContext.SignInAsync(userPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = manterlogado,
                        ExpiresUtc = DateTime.Now.AddHours(1)
                    });
                
                return RedirectToAction(nameof(Pdfview));
            }

            return Json(new { Msg = "Usuário não encontrado! Verifique suas credenciais!" }); // pop up message e redireciona para o index
        }


        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
            }
            return RedirectToAction("Index", "Users");
        }


        

        

        [HttpPost]
        public async Task<IActionResult> Pdfupload(ICollection<IFormFile> files)
        {
            User user = new User();
            var uploads = Path.Combine(_environment.WebRootPath, "uploads/ " + User.Identity.Name);
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            return RedirectToAction(nameof(Pdfview));
        }




        // GET: Users/Create
        public IActionResult Index()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Id,Username,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                Directory.CreateDirectory(@"Z:\Mundiware\Projeto Treino\Csharp-PdfCertificate\PdfCertificado\PdfCertificado\wwwroot\uploads\" + user.Username);
                return RedirectToAction(nameof(Index)); // Create
            }
            return View(user);
        }




        



        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
