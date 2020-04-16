using DataSets.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Interfaces
{
    public interface ICategory : IRepository<Category>
    {
        void Update(Category categoryChanges);
    }
}
