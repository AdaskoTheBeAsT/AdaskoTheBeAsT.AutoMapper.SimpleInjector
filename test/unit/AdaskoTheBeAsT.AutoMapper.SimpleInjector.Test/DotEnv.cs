using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

/// <summary>
/// Loads a git ignored <c>.env</c> file from the repository root into the test process,
/// so that locally executed tests see the same environment variables that CI injects
/// from GitHub secrets. AutoMapper itself picks up <c>AUTOMAPPER_LICENSE_KEY</c> from
/// the environment, see <c>AutoMapper.Licensing.LicenseAccessor.ResolveLicenseKey</c>.
/// </summary>
internal static class DotEnv
{
    /// <summary>
    /// Loads the nearest <c>.env</c> file found when walking up from
    /// <paramref name="startDirectory"/>. Variables that are already present in the
    /// environment are left untouched, so CI values always win over local files.
    /// </summary>
    /// <param name="startDirectory">Directory the lookup starts from.</param>
    internal static void Load(string startDirectory)
    {
        var file = FindFile(startDirectory);
        if (file is null)
        {
            return;
        }

#pragma warning disable SCS0018 // Path traversal - the path is discovered on the local developer machine, not user input
#pragma warning disable SEC0116 // Path tampering - the path is discovered on the local developer machine, not user input
        foreach (var line in File.ReadLines(file))
#pragma warning restore SEC0116
#pragma warning restore SCS0018
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0
                || trimmed[0] == '#')
            {
                continue;
            }

            var separatorIndex = trimmed.IndexOf('=', StringComparison.Ordinal);
            if (separatorIndex <= 0)
            {
                continue;
            }

            var name = trimmed[..separatorIndex].Trim();
            var value = trimmed[(separatorIndex + 1)..].Trim().Trim('"');
            if (value.Length == 0
                || Environment.GetEnvironmentVariable(name) is not null)
            {
                continue;
            }

            Environment.SetEnvironmentVariable(name, value);
        }
    }

    // Runs before any test creates a MapperConfiguration, which is when AutoMapper
    // reads the license key from the environment.
    [ModuleInitializer]
    internal static void Load() => Load(AppContext.BaseDirectory);

    private static string? FindFile(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, ".env");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
