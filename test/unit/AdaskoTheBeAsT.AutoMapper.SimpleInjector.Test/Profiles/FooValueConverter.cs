using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles
{
    internal sealed class FooValueConverter : IValueConverter<int, int>
    {
        public int Convert(int sourceMember, ResolutionContext context)
            => sourceMember + 1;
    }
}
