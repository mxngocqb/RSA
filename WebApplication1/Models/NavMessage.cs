using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class NavMessage
    {
        public int Id { get; set; }
        [Required] //Data Annotation
        public int SvId { get; set; }
        public int Week { get; set; }
        public int Tow { get; set; }
        [StringLength(2048)]
        public string NavigationMessage { get; set; }
        [StringLength(2048)]
        public string? Signature { get; set; }
    }
}
