using DataSets.Data;
using DataSets.Entity;
using DataSets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSets.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCart
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            var data = _db.ShoppingCarts.FirstOrDefault(x => x.Id == shoppingCart.Id);
            if (data != null)
            {
                data.Count = shoppingCart.Count;
                data.Price = shoppingCart.Price;
            }
        }

    }
}
