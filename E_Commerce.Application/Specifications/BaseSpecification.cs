using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace E_Commerce.Application.Specifications
{
    internal abstract class BaseSpecification<TEntity, Tkey> : ISpecifications<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        public ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];

        public Expression<Func<TEntity, bool>> Criteria { get; private set; }

        public Expression<Func<TEntity, object>>? Orderby { get; private set; }

        public Expression<Func<TEntity, object>>? OrderbyDescending { get; private set; }

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPaginated { get; private set; }

        protected void ApplyPagination(int pageSize , int pageIndex)
        {
            IsPaginated = true;
            Take = pageSize;
            Skip = (pageIndex - 1) * pageSize;
        }


        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByexpression)
        {
            Orderby = orderByexpression;
        }
        protected void AddOrderDescBy(Expression<Func<TEntity, object>> orderByDescexpression)
        {
            OrderbyDescending = orderByDescexpression;
        }

        protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude (Expression<Func<TEntity,object>> include)
        {
            IncludeExpressions.Add(include);
        }
    }
}
