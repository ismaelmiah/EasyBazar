using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataSets.Entity
{
    public class BaseEntity
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; }
        public string ImageURL { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
