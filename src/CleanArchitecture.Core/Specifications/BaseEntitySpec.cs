using System;
using System.Linq.Expressions;
using CleanArchitecture.Core.SharedKernel;

namespace CleanArchitecture.Core.Specifications
{
    public class BaseEntitySpec<T> : BaseSpecification<T> where T : BaseEntity
    {
        public BaseEntitySpec(Expression<Func<T, bool>> criteria) : base(criteria)
        { }

        public static BaseEntitySpec<T> ById(int id) => new BaseEntitySpec<T>(entity => entity.Id == id);
    }
}