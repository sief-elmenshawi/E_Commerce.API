using E_Commerce.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Application.Specifications
{
    internal class OrderSpecification :BaseSpecification<Order,Guid>
    {
        public OrderSpecification(string email):base(x=>x.BuyerEmail == email)
        {
            AddInclude(x => x.DeliveryMethod); 
            AddInclude(x => x.Item);
            AddOrderDescBy(x => x.OrderDate);
        }
        public OrderSpecification(Guid id,string email):base(x=>x.Id == id && x.BuyerEmail == email)
        {
            
        }
    }
}
