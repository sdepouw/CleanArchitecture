using CleanArchitecture.Core.Entities;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Specifications;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CleanArchitecture.Tests.Integration.Data
{
    public class EfRepositoryShould
    {
        private AppDbContext _dbContext;

        private static DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase("cleanarchitecture")
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        [Fact]
        public void AddItemAndSetId()
        {
            var repository = GetRepository();
            var item = new ToDoItemBuilder().Build();

            repository.Add(item);

            var newItem = repository.List<ToDoItem>().FirstOrDefault();

            Assert.Equal(item, newItem);
            Assert.True(newItem?.Id > 0);
        }

        [Fact]
        public void UpdateItemAfterAddingIt()
        {
            // add an item
            var repository = GetRepository();
            var initialTitle = Guid.NewGuid().ToString();
            var item = new ToDoItemBuilder().Title(initialTitle).Build();

            repository.Add(item);

            // detach the item so we get a different instance
            _dbContext.Entry(item).State = EntityState.Detached;

            // fetch the item and update its title
            var newItem = repository.List<ToDoItem>()
                .FirstOrDefault(i => i.Title == initialTitle);
            Assert.NotNull(newItem);
            Assert.NotSame(item, newItem);
            var newTitle = Guid.NewGuid().ToString();
            newItem.Title = newTitle;

            // Update the item
            repository.Update(newItem);
            var updatedItem = repository.List<ToDoItem>()
                .FirstOrDefault(i => i.Title == newTitle);

            Assert.NotNull(updatedItem);
            Assert.NotEqual(item.Title, updatedItem.Title);
            Assert.Equal(newItem.Id, updatedItem.Id);
        }

        [Fact]
        public void DeleteItemAfterAddingIt()
        {
            // add an item
            var repository = GetRepository();
            var initialTitle = Guid.NewGuid().ToString();
            var item = new ToDoItemBuilder().Title(initialTitle).Build();
            repository.Add(item);

            // delete the item
            repository.Delete(item);

            // verify it's no longer there
            Assert.DoesNotContain(repository.List<ToDoItem>(), i => i.Title == initialTitle);
        }

        [Fact]
        public void FilterBySpec()
        {
            var repository = GetRepository();
            var todoItems = new List<ToDoItem>
            {
                new ToDoItemBuilder().Id(45).Title("Foo").Build(),
                new ToDoItemBuilder().Title("Bar").Build(),
                new ToDoItemBuilder().Title("Fizz").Build(),
                new ToDoItemBuilder().Title("Buzz").MarkAsDone().Build(),
            };
            todoItems.ForEach(item => repository.Add(item));

            List<ToDoItem> itemsByTitle = repository.List(ToDoItemSpec.ByTitle("Bar"));

            Assert.Single(itemsByTitle);
            Assert.Equal("Bar", itemsByTitle.Single().Title);

            List<ToDoItem> itemsById = repository.List(BaseEntitySpec<ToDoItem>.ById(45));

            Assert.Single(itemsById);
            Assert.Equal("Foo", itemsById.Single().Title);

            List<ToDoItem> itemsByIsDone = repository.List(ToDoItemSpec.ByIsDone(true));

            Assert.Single(itemsByIsDone);
            Assert.Equal("Buzz", itemsByIsDone.Single().Title);
        }

        private IRepository GetRepository()
        {
            var options = CreateNewContextOptions();
            var mockDispatcher = new Mock<IDomainEventDispatcher>();

            _dbContext = new AppDbContext(options, mockDispatcher.Object);
            return new EfRepository(_dbContext);
        }
    }
}
