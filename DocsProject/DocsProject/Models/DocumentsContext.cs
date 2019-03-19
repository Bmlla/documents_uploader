using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DocsProject.Models
{
    public class DocumentsContext : DbContext
    {
        public DocumentsContext(DbContextOptions<DocumentsContext> options)
        : base(options)
        {
        }

        public DbSet<Documents> documentos_salvos { get; set; }
    }
}
