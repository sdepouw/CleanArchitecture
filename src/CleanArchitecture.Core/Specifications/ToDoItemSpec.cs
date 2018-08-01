using CleanArchitecture.Core.Entities;
using System;
using System.Linq.Expressions;

namespace CleanArchitecture.Core.Specifications
{
    public class ToDoItemSpec : BaseSpecification<ToDoItem>
    {
        public ToDoItemSpec(Expression<Func<ToDoItem, bool>> criteria) : base(criteria)
        { }

        public static ToDoItemSpec ByIsDone(bool isDone) => new ToDoItemSpec(todoItem => todoItem.IsDone == isDone);
        public static ToDoItemSpec ByTitle(string title) => new ToDoItemSpec(todoItem => todoItem.Title == title);
    }
}
