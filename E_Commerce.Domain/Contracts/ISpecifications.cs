using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace E_Commerce.Domain.Contracts
{
    public interface ISpecifications<TEntity,Tkey> where TEntity : BaseEntity<Tkey>
    {
        ICollection<Expression<Func<TEntity , object>>> IncludeExpressions { get;  } 

        Expression<Func<TEntity,bool>> Criteria { get; } 
        Expression<Func<TEntity,object>>? Orderby { get; }
        Expression<Func<TEntity,object>>? OrderbyDescending { get; }
    }
}
