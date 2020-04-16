using DataSets.Data;
using DataSets.Entity;
using DataSets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSets.Repository
{
    public class ProductRepository : Repository<Product>, IProduct
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var data = _db.Products.FirstOrDefault(x => x.ID == product.ID);
            if (data != null)
            {
                if (product.ImageURL != null)
                {
                    data.ImageURL = product.ImageURL;
                }
                data.Price = product.Price;
                data.Description = product.Description;
                data.CategoryID = product.CategoryID;
                data.Name = product.Name;
            }
        }
    }
}
