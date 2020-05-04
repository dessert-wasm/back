using System;
using Dessert.Domain.Entities.Identity;

namespace Dessert.Domain.Entities
{
    public class Module
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GithubLink { get; set; }
        public long AuthorId { get; set; }
        public bool IsCore { get; set; }
        public DateTime PublishedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }

        public Account Author { get; set; }
        //published ?
    }
}