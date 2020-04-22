using DataSets.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Interfaces
{
    public interface IOrderDetails : IRepository<OrderDetails>
    {
        void Update(OrderDetails orderDetails);
    }
}
