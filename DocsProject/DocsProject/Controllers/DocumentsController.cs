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
            var docs = from documentos_salvos in _context.documentos_salvos select documentos_salvos;

            docs = docs.OrderBy(documentos_salvos => documentos_salvos.Titulo);
            return View(docs.ToList());
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
        public async Task<IActionResult> Create(List<IFormFile> file, [FromForm] string titulo, [FromForm] string processo, [FromForm] string categoria, Documents documents)
        {
            if (ModelState.IsValid)
            {
                this.convertFileByte(file, titulo, processo, categoria, documents);

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
        public async Task<IActionResult> Edit(int id, List<IFormFile> file, [FromForm] string titulo, [FromForm] string processo, [FromForm] string categoria, Documents documents)
        {
            if (id != documents.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                this.convertFileByte(file, titulo, processo, categoria, documents);

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


        public async Task<IActionResult> DownloadFile(int? id)
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

            string folder = @"C:\DownloadDocs";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            //Baixa o arquivo
            byte[] bytes = documents.Arquivo;
            FileStream fs = new FileStream(@"C:\DownloadDocs\" + documents.Nome_Arquivo, FileMode.OpenOrCreate);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            return RedirectToAction(nameof(Index));
        }

        private Documents convertFileByte(List<IFormFile> file, [FromForm] string titulo, [FromForm] string processo, [FromForm] string categoria, Documents documents)
        {
            documents.Titulo = titulo;
            documents.Processo = processo;
            documents.Categoria = categoria;

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

            return documents;
        }
    }
}
