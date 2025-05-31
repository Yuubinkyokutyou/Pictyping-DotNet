using Pictyping.Core.Entities;
using Xunit;

namespace Pictyping.Core.Tests.Entities;

public class TestEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_HasIdProperty()
    {
        var entity = new TestEntity();

        Assert.True(entity.Id >= 0);
    }

    [Fact]
    public void BaseEntity_HasCreatedAtProperty()
    {
        var entity = new TestEntity();

        Assert.Equal(default(DateTime), entity.CreatedAt);
    }

    [Fact]
    public void BaseEntity_HasUpdatedAtProperty()
    {
        var entity = new TestEntity();

        Assert.Equal(default(DateTime), entity.UpdatedAt);
    }

    [Fact]
    public void BaseEntity_CanSetId()
    {
        var entity = new TestEntity();
        var id = 123;

        entity.Id = id;

        Assert.Equal(id, entity.Id);
    }

    [Fact]
    public void BaseEntity_CanSetCreatedAt()
    {
        var entity = new TestEntity();
        var createdAt = DateTime.UtcNow;

        entity.CreatedAt = createdAt;

        Assert.Equal(createdAt, entity.CreatedAt);
    }

    [Fact]
    public void BaseEntity_CanSetUpdatedAt()
    {
        var entity = new TestEntity();
        var updatedAt = DateTime.UtcNow;

        entity.UpdatedAt = updatedAt;

        Assert.Equal(updatedAt, entity.UpdatedAt);
    }
}