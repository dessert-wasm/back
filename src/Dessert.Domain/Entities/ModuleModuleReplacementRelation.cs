namespace Dessert.Domain.Entities
{
    public class ModuleModuleReplacementRelation
    {
        public long ModuleId { get; set; }
        public long ModuleReplacementId { get; set; }

        public Module Module { get; set; }
        public ModuleReplacement ModuleReplacement { get; set; }
    }
}