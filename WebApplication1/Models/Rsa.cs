using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Rsa
    {
        public int Id { get; set; }
        [Required] //Data Annotation
        [StringLength(2048)]
        public string Value { get; set; }
        [StringLength(2048)]
        public string? Signature { get; set; }
    }
}
