using System.ComponentModel.DataAnnotations;

namespace Dessert.Domain.Entities
{
    public class ModuleReplacement
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public string Link { get; set; }
    }
}