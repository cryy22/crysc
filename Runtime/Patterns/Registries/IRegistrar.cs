namespace Crysc.Patterns.Registries
{
    public interface IRegistrar<out T>
    {
        public T Registrant { get; }
    }
}
