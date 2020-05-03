namespace Dessert.Domain.Entities
{
    public class ModuleModuleTagRelation
    {
        public long ModuleId { get; set; }
        public long ModuleTagId { get; set; }

        public Module Module { get; set; }
        public ModuleTag ModuleTag { get; set; }
    }
}