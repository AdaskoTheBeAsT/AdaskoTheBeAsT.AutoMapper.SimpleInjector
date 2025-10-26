using System;
using AutoFixture;
using AwesomeAssertions;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

public class AutoMapperSimpleInjectorConfigurationTests
{
    private readonly Fixture _fixture = new();

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
}
