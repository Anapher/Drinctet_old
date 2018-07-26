namespace Drinctet.ViewModels.ViewModelBase
{
    internal static class DependencyService
    {
        public static T Get<T>() where T : class => DependencyServiceInitializer.DependencyService.Get<T>();
    }

    public interface IDependencyService
    {
        T Get<T>() where T : class;
    }

    public static class DependencyServiceInitializer
    {
        public static IDependencyService DependencyService { get; set; }
    }
}