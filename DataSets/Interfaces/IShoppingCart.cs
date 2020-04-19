using DataSets.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Interfaces
{
    public interface IShoppingCart : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
    }
}
