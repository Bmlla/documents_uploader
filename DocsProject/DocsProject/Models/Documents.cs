using System.ComponentModel.DataAnnotations;

namespace DocsProject.Models
{
    public class Documents
    {
        [Key] //Define Codigo como primary key
        public int Codigo { get; set; }
        public string Titulo { get; set; }
        public string Processo { get; set; }
        public string Categoria { get; set; }
        public byte[] Arquivo { get; set; }
        public string Nome_Arquivo { get; set; }
    }
}
