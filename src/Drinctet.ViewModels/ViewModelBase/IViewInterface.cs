namespace Drinctet.ViewModels.ViewModelBase
{
    public interface IViewInterface
    {
        void Show<TViewModel>(TViewModel viewModel);
    }
}