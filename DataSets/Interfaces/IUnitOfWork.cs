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
        void Save();
    }
}
