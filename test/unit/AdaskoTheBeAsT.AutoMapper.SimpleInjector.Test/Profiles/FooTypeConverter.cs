using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles
{
    internal sealed class FooTypeConverter : ITypeConverter<object, object>
    {
        public object Convert(object source, object destination, ResolutionContext context)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
