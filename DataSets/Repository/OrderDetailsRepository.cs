using DataSets.Data;
using DataSets.Entity;
using DataSets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetails
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetails orderDetails)
        {
            _db.Update(orderDetails);
        }
    }
}
