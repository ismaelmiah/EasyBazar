using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategory Category { get; }
        IProduct Product { get; }
        IApplicationUser ApplicationUser { get; }
        IShoppingCart ShoppingCart { get; }
        IOrderDetails OrderDetails { get; }
        IOrderHeader OrderHeader { get; }
        void Save();
    }
}
