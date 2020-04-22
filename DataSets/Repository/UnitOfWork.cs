using DataSets.Data;
using DataSets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
        }

        public ICategory Category { get; private set; }
        public IProduct Product { get; private set; }
        public IApplicationUser ApplicationUser { get; private set; }
        public IShoppingCart ShoppingCart { get; private set; }
        public IOrderDetails OrderDetails { get; private set; }
        public IOrderHeader OrderHeader { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
