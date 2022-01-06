namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

public class MutableService : ISomeService
{
    public int Value { get; set; }

    public int Modify(int value) => value + Value;
}
