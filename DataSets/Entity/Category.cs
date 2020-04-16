using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataSets.Entity
{
    public class Category : BaseEntity
    {
        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }
        public List<Product> Products { get; set; }

    }
}
