﻿using DataSets.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.ViewModels
{
    public class HomeVM
    {
        public List<Category> FeaturedCategories { get; set; }
        public List<Category> Categories { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public List<Product> Products { get; set; }
        public bool IsLatestProduct { get; set; }
        public int? sortBy { get; set; }
    }
}
