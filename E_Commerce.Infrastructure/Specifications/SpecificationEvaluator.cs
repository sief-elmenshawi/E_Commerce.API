using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Infrastructure.Specifications
{
    internal static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity,Tkey>(IQueryable<TEntity> inputQuery,ISpecifications<TEntity,Tkey> spec) where TEntity : BaseEntity<Tkey>
        {
            var query = inputQuery;

            if (spec.Criteria != null)  
            {
                query = query.Where(spec.Criteria);
            }


            if (spec.IncludeExpressions.Any())
            {
                // dbContext.Products;
                // dbContext.Products.include(Exp)
                // dbContext.Products.include(Exp).include(exp02)
                query = spec.IncludeExpressions.Aggregate(query, (current, nextExp) => current.Include(nextExp));
            }

            if (spec.Orderby != null)
            {
                query = query.OrderBy(spec.Orderby);
            }else if (spec.OrderbyDescending != null)
            {
                query = query.OrderByDescending(spec.OrderbyDescending);
            }
            return query;
        }
    }
}
