// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.EntityFrameworkCore.Metadata.Internal;

public class PropertyParameterBindingFactoryTest
{
    [Theory]
    [InlineData("complex")]
    [InlineData("Complex")]
    [InlineData("_complex")]
    [InlineData("_Complex")]
    [InlineData("m_complex")]
    [InlineData("m_Complex")]
    public void Binds_complex_properties_using_scalar_property_name_conventions(string propertyName)
    {
        var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();
        var entityType = builder.Entity<ConstructorEntity>().Metadata;
        var complexProperty = builder.Entity<ConstructorEntity>().ComplexProperty<ConstructorComplex>(propertyName).Metadata;
        var nestedProperty = builder.Entity<ConstructorEntity>().ComplexProperty<ConstructorComplex>(propertyName)
            .ComplexProperty<ConstructorComplex>(propertyName).Metadata;
        var factory = new PropertyParameterBindingFactory();

        var entityBinding = Assert.IsType<ComplexPropertyParameterBinding>(
            factory.FindParameter((IEntityType)entityType, typeof(ConstructorComplex), "complex"));
        Assert.Same(complexProperty, entityBinding.ConsumedProperties.Single());
        var complexBinding = Assert.IsType<ComplexPropertyParameterBinding>(
            factory.FindParameter((IComplexType)complexProperty.ComplexType, typeof(ConstructorComplex), "complex"));
        Assert.Same(nestedProperty, complexBinding.ConsumedProperties.Single());

        Assert.Null(factory.FindParameter((IEntityType)entityType, typeof(object), "complex"));
        Assert.Null(factory.FindParameter((IEntityType)entityType, typeof(ConstructorComplex), "other"));
    }

    [Fact]
    public void Does_not_bind_complex_collections()
    {
        var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();
        var entityType = builder.Entity<ConstructorEntity>().Metadata;
        builder.Entity<ConstructorEntity>().ComplexCollection(e => e.Collection);

        Assert.Null(new PropertyParameterBindingFactory().FindParameter(
            (IEntityType)entityType, typeof(List<ConstructorComplex>), "collection"));
    }

    private class ConstructorEntity
    {
        public int Id { get; set; }
        public List<ConstructorComplex> Collection { get; set; } = [];
    }

    private class ConstructorComplex
    {
        public int Value { get; set; }
    }
}
