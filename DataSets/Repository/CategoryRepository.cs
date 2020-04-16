using DataSets.Data;
using DataSets.Entity;
using DataSets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSets.Repository
{
    public class CategoryRepository : Repository<Category>, ICategory
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            var data = _db.Categories.FirstOrDefault(x => x.ID == category.ID);
            if (data != null)
            {
                data.Name = category.Name;
                data.Description = category.Description;
                data.IsFeatured = category.IsFeatured;
                data.ImageURL = category.ImageURL;
                data.Products = category.Products;
            }
        }
    }
}
