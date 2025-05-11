using Data_Access_Layer.Entities;
using Data_Access_Layer.Repository;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Models.DTOs.Category;
using FBookRating.Services;
using Data_Access_Layer;
using Microsoft.EntityFrameworkCore;


namespace FBookRating.Tests.Services
{

    public class CategoryServiceTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions(string dbName)
            => new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        [Fact]
        public async Task GetAllCategoriesAsync_WithData_ReturnsAllDTOs()
        {
            // Arrange
            var opts = CreateNewContextOptions(nameof(GetAllCategoriesAsync_WithData_ReturnsAllDTOs));
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Categories.AddRange(
                    new Category { Id = Guid.NewGuid(), Name = "A", Description = "Desc A" },
                    new Category { Id = Guid.NewGuid(), Name = "B", Description = "Desc B" }
                );
                seedContext.SaveChanges();
            }

            // Act
            using (var testContext = new ApplicationDbContext(opts))
            {
                var service = new CategoryService(new UnitOfWork(testContext));
                var result = await service.GetAllCategoriesAsync();

                // Assert
                Assert.Equal(2, result.Count());
                Assert.Contains(result, dto => dto.Name == "A" && dto.Description == "Desc A");
                Assert.Contains(result, dto => dto.Name == "B" && dto.Description == "Desc B");
            }
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ExistingId_ReturnsDTO()
        {
            // Arrange
            var id = Guid.NewGuid();
            var opts = CreateNewContextOptions(nameof(GetCategoryByIdAsync_ExistingId_ReturnsDTO));
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Categories.Add(new Category { Id = id, Name = "X", Description = "Y" });
                seedContext.SaveChanges();
            }

            // Act & Assert
            using (var testContext = new ApplicationDbContext(opts))
            {
                var service = new CategoryService(new UnitOfWork(testContext));
                var dto = await service.GetCategoryByIdAsync(id);

                Assert.NotNull(dto);
                Assert.Equal(id, dto.Id);
                Assert.Equal("X", dto.Name);
                Assert.Equal("Y", dto.Description);
            }
        }

        [Fact]
        public async Task GetCategoryByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var opts = CreateNewContextOptions(nameof(GetCategoryByIdAsync_NonExistingId_ReturnsNull));
            // no seeding

            // Act & Assert
            using (var testContext = new ApplicationDbContext(opts))
            {
                var service = new CategoryService(new UnitOfWork(testContext));
                var dto = await service.GetCategoryByIdAsync(Guid.NewGuid());
                Assert.Null(dto);
            }
        }

        [Fact]
        public async Task AddCategoryAsync_ValidDTO_AddsToDatabase()
        {
            // Arrange
            var opts = CreateNewContextOptions(nameof(AddCategoryAsync_ValidDTO_AddsToDatabase));
            // start empty

            // Act
            using (var testContext = new ApplicationDbContext(opts))
            {
                var service = new CategoryService(new UnitOfWork(testContext));
                var createDto = new CategoryCreateDTO { Name = "NewCat", Description = "NewDesc" };

                await service.AddCategoryAsync(createDto);
                // after AddCategoryAsync, SaveChangesAsync was called inside the service
            }

            // Assert: new context to verify persistence
            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var inserted = await verifyContext.Categories.SingleOrDefaultAsync();
                Assert.NotNull(inserted);
                Assert.Equal("NewCat", inserted.Name);
                Assert.Equal("NewDesc", inserted.Description);
            }
        }

        [Fact]
        public async Task UpdateCategoryAsync_ExistingEntity_UpdatesDatabase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var opts = CreateNewContextOptions(nameof(UpdateCategoryAsync_ExistingEntity_UpdatesDatabase));
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Categories.Add(new Category { Id = id, Name = "Old", Description = "OldDesc" });
                seedContext.SaveChanges();
            }

            // Act
            using (var testContext = new ApplicationDbContext(opts))
            {
                var service = new CategoryService(new UnitOfWork(testContext));
                var updateDto = new CategoryUpdateDTO { Name = "Updated", Description = "UpdatedDesc" };

                await service.UpdateCategoryAsync(id, updateDto);
            }

            // Assert
            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var updated = await verifyContext.Categories.FindAsync(id);
                Assert.Equal("Updated", updated.Name);
                Assert.Equal("UpdatedDesc", updated.Description);
            }
        }

        [Fact]
        public async Task UpdateCategoryAsync_NonExistingEntity_ThrowsException()
        {
            // Arrange
            var opts = CreateNewContextOptions(nameof(UpdateCategoryAsync_NonExistingEntity_ThrowsException));
            // no seeding

            // Act & Assert
            using (var testContext = new ApplicationDbContext(opts))
            {
                var service = new CategoryService(new UnitOfWork(testContext));
                var bogusId = Guid.NewGuid();
                var updateDto = new CategoryUpdateDTO();

                await Assert.ThrowsAsync<Exception>(
                    () => service.UpdateCategoryAsync(bogusId, updateDto));
            }
        }

        [Fact]
        public async Task DeleteCategoryAsync_ExistingEntity_DeletesFromDatabase()
        {
            // Arrange
            var id = Guid.NewGuid();
            var opts = CreateNewContextOptions(nameof(DeleteCategoryAsync_ExistingEntity_DeletesFromDatabase));
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Categories.Add(new Category { Id = id, Name = "ToDelete", Description = "" });
                seedContext.SaveChanges();
            }

            // Act
            using (var testContext = new ApplicationDbContext(opts))
            {
                var service = new CategoryService(new UnitOfWork(testContext));
                await service.DeleteCategoryAsync(id);
            }

            // Assert
            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var deleted = await verifyContext.Categories.FindAsync(id);
                Assert.Null(deleted);
            }
        }

        [Fact]
        public async Task DeleteCategoryAsync_NonExistingEntity_DoesNothing()
        {
            // Arrange
            var opts = CreateNewContextOptions(nameof(DeleteCategoryAsync_NonExistingEntity_DoesNothing));
            // no seeding

            // Act
            using (var testContext = new ApplicationDbContext(opts))
            {
                var service = new CategoryService(new UnitOfWork(testContext));
                await service.DeleteCategoryAsync(Guid.NewGuid());
            }

            // Assert: still empty
            using (var verifyContext = new ApplicationDbContext(opts))
            {
                Assert.Empty(await verifyContext.Categories.ToListAsync());
            }
        }
    }


}
