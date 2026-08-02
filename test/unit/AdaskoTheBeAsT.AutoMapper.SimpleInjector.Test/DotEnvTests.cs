using System;
using System.IO;
using AwesomeAssertions;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

#pragma warning disable SCS0018 // Path traversal - all paths are generated below the temp directory by the test itself
public class DotEnvTests
{
    [Fact]
    public void ShouldProvideAutoMapperLicenseKeyToTestProcess()
    {
        // The module initializer of DotEnv runs before any test, so the key comes either
        // from the local .env file or from the environment injected by CI.
        Environment.GetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY").Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldSetVariableFromFileInParentDirectory()
    {
        // Arrange
        var name = "ADASKO_DOTENV_TEST_" + Guid.NewGuid().ToString("N");
        var root = CreateTempDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, ".env"),
                $"# comment{Environment.NewLine}{name}=\"some-value\"{Environment.NewLine}");
            var nested = Directory.CreateDirectory(Path.Combine(root, "bin", "Debug")).FullName;

            // Act
            DotEnv.Load(nested);

            // Assert
            Environment.GetEnvironmentVariable(name).Should().Be("some-value");
        }
        finally
        {
            Environment.SetEnvironmentVariable(name, value: null);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ShouldNotOverrideAlreadySetVariable()
    {
        // Arrange
        var name = "ADASKO_DOTENV_TEST_" + Guid.NewGuid().ToString("N");
        var root = CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(root, ".env"), $"{name}=from-file{Environment.NewLine}");
            Environment.SetEnvironmentVariable(name, "from-ci");

            // Act
            DotEnv.Load(root);

            // Assert
            Environment.GetEnvironmentVariable(name).Should().Be("from-ci");
        }
        finally
        {
            Environment.SetEnvironmentVariable(name, value: null);
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ShouldDoNothingWhenNoFileFound()
    {
        // Arrange
        var root = CreateTempDirectory();
        try
        {
            // Act
            Action action = () => DotEnv.Load(root);

            // Assert
            action.Should().NotThrow();
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateTempDirectory()
        => Directory.CreateDirectory(
            Path.Combine(Path.GetTempPath(), "dotenv-" + Guid.NewGuid().ToString("N"))).FullName;
}
#pragma warning restore SCS0018
