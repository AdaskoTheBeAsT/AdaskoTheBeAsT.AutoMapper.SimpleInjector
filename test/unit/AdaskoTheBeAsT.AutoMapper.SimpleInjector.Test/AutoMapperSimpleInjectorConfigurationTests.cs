using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AwesomeAssertions;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

public class AutoMapperSimpleInjectorConfigurationTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void DefaultLicenseKey_ShouldBeNull()
    {
        // Arrange & Act
        var config = new AutoMapperSimpleInjectorConfiguration();

        // Assert
        config.LicenseKey.Should().BeNull();
    }

    [Fact]
    public void WithLicense_ShouldThrow_WhenPassingNull()
    {
        // Arrange
        var config = new AutoMapperSimpleInjectorConfiguration();

        Action action = () => config.WithLicenseKey(null!);

        // Act and Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithLicense_ShouldProperlyConfigure()
    {
        // Arrange
        var licenseKey = _fixture.Create<string>();
        var config = new AutoMapperSimpleInjectorConfiguration();

        // Act
        config.WithLicenseKey(licenseKey);

        // Assert
        config.LicenseKey.Should().Be(licenseKey);
    }

    [Fact]
    public void WithAssembliesToScan_ShouldThrow_WhenPassingNull()
    {
        // Arrange
        var config = new AutoMapperSimpleInjectorConfiguration();

        Action action = () => config.WithAssembliesToScan(null!);

        // Act and Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithAssembliesToScan_ShouldDefensivelyCopy()
    {
        // Arrange
        var config = new AutoMapperSimpleInjectorConfiguration();
        var assemblies = new List<Assembly> { typeof(AutoMapperSimpleInjectorConfigurationTests).Assembly };

        // Act
        config.WithAssembliesToScan(assemblies);
        assemblies.Clear();

        // Assert
        config.AssembliesToScan.Should().ContainSingle();
    }

    [Fact]
    public void WithMapperAssemblyMarkerTypes_Params_ShouldThrow_WhenPassingNull()
    {
        // Arrange
        var config = new AutoMapperSimpleInjectorConfiguration();

        Action action = () => config.WithMapperAssemblyMarkerTypes((Type[])null!);

        // Act and Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithMapperAssemblyMarkerTypes_Enumerable_ShouldThrow_WhenPassingNull()
    {
        // Arrange
        var config = new AutoMapperSimpleInjectorConfiguration();

        Action action = () => config.WithMapperAssemblyMarkerTypes((IEnumerable<Type>)null!);

        // Act and Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithMapperAssemblyMarkerTypes_ShouldEagerlyMaterialize()
    {
        // Arrange
        var config = new AutoMapperSimpleInjectorConfiguration();
        var types = new List<Type> { typeof(AutoMapperSimpleInjectorConfigurationTests) };

        // Act
        config.WithMapperAssemblyMarkerTypes(types);
        types.Clear();

        // Assert
        config.AssembliesToScan.Should().ContainSingle();
    }
}
