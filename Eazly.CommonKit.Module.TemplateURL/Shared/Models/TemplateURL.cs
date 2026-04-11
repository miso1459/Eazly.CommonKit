using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace Eazly.CommonKit.Module.TemplateURL.Models
{
    [Table("Eazly.CommonKitTemplateURL")]
    public class TemplateURL : ModelBase
    {
        [Key]
        public int TemplateURLId { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
    }
}
