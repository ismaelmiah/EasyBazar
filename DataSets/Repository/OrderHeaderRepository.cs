using DataSets.Data;
using DataSets.Entity;
using DataSets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Repository
{
    class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeader
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.Update(orderHeader);
        }
    }
}
