using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test
{
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable CA1812
    public class Source
    {
    }

    public class Dest
    {
    }

    public class Source2
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ConvertedValue { get; set; }
    }

    public class Dest2
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ResolvedValue { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ConvertedValue { get; set; }
    }

    public class Source3
    {
        public int Value { get; set; }
    }

    [AutoMap(typeof(Source3))]
    public class Dest3
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Value { get; set; }
    }

    public class Profile1 : Profile
    {
        public Profile1()
        {
            CreateMap<Source, Dest>();
        }
    }

    public abstract class AbstractProfile : Profile
    {
    }

    internal class Profile2 : Profile
    {
        public Profile2()
        {
            CreateMap<Source2, Dest2>()
                .ForMember(d => d.ResolvedValue, opt => opt.MapFrom<DependencyResolver>())
                .ForMember(d => d.ConvertedValue, opt => opt.ConvertUsing<DependencyValueConverter, int>());
        }
    }

    public class DependencyResolver : IValueResolver<object, object, int>
    {
        private readonly ISomeService _service;

        public DependencyResolver(ISomeService service)
        {
            _service = service;
        }

        public int Resolve(object source, object destination, int destMember, ResolutionContext context)
        {
            return _service.Modify(destMember);
        }
    }

    public interface ISomeService
    {
        int Modify(int value);
    }

    public class MutableService : ISomeService
    {
        public int Value { get; set; }

        public int Modify(int value) => value + Value;
    }

    public class FooService : ISomeService
    {
        private readonly int _value;

        public FooService(int value)
        {
            _value = value;
        }

        public int Modify(int value) => value + _value;
    }

    internal class FooMappingAction : IMappingAction<object, object>
    {
        public void Process(
            object source,
            object destination,
            ResolutionContext context)
        {
            // no op
        }
    }

    internal class FooValueResolver : IValueResolver<object, object, object>
    {
        public object Resolve(object source, object destination, object destMember, ResolutionContext context)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }

    internal class FooMemberValueResolver : IMemberValueResolver<object, object, object, object>
    {
        public object Resolve(object source, object destination, object sourceMember, object destMember, ResolutionContext context)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }

    internal class FooTypeConverter : ITypeConverter<object, object>
    {
        public object Convert(object source, object destination, ResolutionContext context)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }

    internal class FooValueConverter : IValueConverter<int, int>
    {
        public int Convert(int sourceMember, ResolutionContext context)
            => sourceMember + 1;
    }

    internal class DependencyValueConverter : IValueConverter<int, int>
    {
        private readonly ISomeService _service;

        public DependencyValueConverter(ISomeService service) => _service = service;

        public int Convert(int sourceMember, ResolutionContext context)
            => _service.Modify(sourceMember);
    }
#pragma warning restore CA1812
#pragma warning restore SA1202 // Elements should be ordered by access
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore SA1201 // Elements should appear in the correct order
}
