using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocsProject.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;

namespace DocsProject.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly DocumentsContext _context;
        IHostingEnvironment _appEnvironment;

        public DocumentsController(DocumentsContext context, IHostingEnvironment env)
        {
            _context = context;
            _appEnvironment = env;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            return View(await _context.documentos_salvos.ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documents = await _context.documentos_salvos
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (documents == null)
            {
                return NotFound();
            }

            return View(documents);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> file, Documents documents)
        {
            if (ModelState.IsValid)
            {
                foreach (var data in file)
                {
                    if (data.Length > 0)
                    {
                        documents.Nome_Arquivo = data.FileName;
                        using (var memorySt = new MemoryStream())
                        {
                            data.CopyTo(memorySt);
                            var fileBytes = memorySt.ToArray();
                            string s = Convert.ToBase64String(fileBytes);

                            documents.Arquivo = fileBytes;
                        }
                    }
                }
                _context.Add(documents);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(documents);
        }   

        
        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documents = await _context.documentos_salvos.FindAsync(id);
            if (documents == null)
            {
                return NotFound();
            }
            return View(documents);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,Titulo,Processo,Categoria,Arquivo")] Documents documents)
        {
            if (id != documents.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(documents);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentsExists(documents.Codigo))
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
            return View(documents);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documents = await _context.documentos_salvos
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (documents == null)
            {
                return NotFound();
            }

            return View(documents);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documents = await _context.documentos_salvos.FindAsync(id);
            _context.documentos_salvos.Remove(documents);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentsExists(int id)
        {
            return _context.documentos_salvos.Any(e => e.Codigo == id);
        }

        //User document selection
        public IActionResult UploadDocument(IList<IFormFile> arquivos)
        {
            IFormFile imagemEnviada = arquivos.FirstOrDefault();
            if (imagemEnviada != null || imagemEnviada.ContentType.ToLower().StartsWith("image/"))
            {
                MemoryStream ms = new MemoryStream();
                imagemEnviada.OpenReadStream().CopyTo(ms);
                Documents imagemEntity = new Documents()
                {
                    Arquivo = ms.ToArray()
                };
                _context.documentos_salvos.Add(imagemEntity);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");

        }
    }
}
