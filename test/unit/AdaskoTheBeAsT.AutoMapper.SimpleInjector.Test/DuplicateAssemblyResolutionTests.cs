using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using SimpleInjector;
using Xunit;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test;

public sealed class DuplicateAssemblyResolutionTests
    : IDisposable
{
    private readonly Container _container;

    public DuplicateAssemblyResolutionTests()
    {
        _container = new Container();
        _container.AddAutoMapper(typeof(IMapper), typeof(IMapper));
    }

    [Fact]
    public void ShouldResolveNotificationHandlersOnlyOnce()
    {
        _container.GetCurrentRegistrations()
            .Where(c => c.ServiceType == typeof(IMapper))
            .Should()
            .ContainSingle();
    }

    public void Dispose()
    {
        _container.Dispose();
    }
}
