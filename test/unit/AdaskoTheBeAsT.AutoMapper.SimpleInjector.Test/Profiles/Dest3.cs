using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles
{
    [AutoMap(typeof(Source3))]
    public class Dest3
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Value { get; set; }
    }
}
