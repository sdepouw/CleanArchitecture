using Ardalis.GuardClauses;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CleanArchitecture.Core.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
        where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public string CacheKey { get; private set; }

        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
        protected void AddInclude(string includeString) => IncludeStrings.Add(includeString);
        public bool ShouldCache => !string.IsNullOrWhiteSpace(CacheKey);

        protected void EnableCache(string specMethodName, params object[] args)
        {
            Guard.Against.NullOrWhiteSpace(specMethodName, nameof(specMethodName));

            List<string> keyNames = new List<string> { typeof(T).Name, specMethodName };
            keyNames.AddRange(args.Select(arg => arg?.ToString() ?? string.Empty));
            CacheKey = string.Join("-", keyNames);
        }
    }
}
