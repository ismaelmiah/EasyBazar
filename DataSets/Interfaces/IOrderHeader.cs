using DataSets.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Interfaces
{
    public interface IOrderHeader : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
    }
}
