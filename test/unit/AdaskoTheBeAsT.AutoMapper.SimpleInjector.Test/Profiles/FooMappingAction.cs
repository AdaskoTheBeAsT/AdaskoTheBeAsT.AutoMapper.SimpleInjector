using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles
{
    internal sealed class FooMappingAction : IMappingAction<object, object>
    {
        public void Process(
            object source,
            object destination,
            ResolutionContext context)
        {
            // no op
        }
    }
}
