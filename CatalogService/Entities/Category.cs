﻿using System.ComponentModel.DataAnnotations;

namespace CatalogService.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public List<Service> Services { get; set; }
    }
}
