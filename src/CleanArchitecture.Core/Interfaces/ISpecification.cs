using CleanArchitecture.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CleanArchitecture.Core.Interfaces
{
    public interface ISpecification<T> where T : BaseEntity
    {
        Expression<Func<T, bool>> Criteria { get; }
        string CacheKey { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
    }
}
