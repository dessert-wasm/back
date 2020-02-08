using System;

namespace Dessert.Models
{
    public class Module
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long AuthorId { get; set; }
        public bool IsCore { get; set; }
        public DateTime PublishedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }

        public Account Author { get; set; }
        //published ?
    }
}