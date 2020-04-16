using DataSets.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Interfaces
{
    public interface IProduct : IRepository<Product>
    {
        void Update(Product ProductChanges);
    }
}
