using System;
using System.Collections.Generic;
using Drinctet.ViewModels.ViewModelBase;
using Xamarin.Forms;

namespace Drinctet.Mobile
{
    public class MobilePageStore
    {
        private readonly Dictionary<Type, IPageViewModel> _pageViewModels = new Dictionary<Type, IPageViewModel>();
        
        public void RegisterPage<TPage, TViewModel>() where TPage : Page, new()
        {
            _pageViewModels.Add(typeof(TViewModel), new PageViewModel<TPage>());
        }

        public Page GetPage(Type type) => _pageViewModels[type].GetPage();

        private interface IPageViewModel
        {
            Page GetPage();
        }

        private class PageViewModel<TPage> : IPageViewModel where TPage : Page, new()
        {
            public Page GetPage()
            {
                return new TPage();
            }
        }
    }

    public class MobileInterface : IViewInterface
    {
        private readonly MobilePageStore _mobilePageStore;
        private readonly Page _currentPage;

        public MobileInterface(MobilePageStore mobilePageStore, Page currentPage)
        {
            _mobilePageStore = mobilePageStore;
            _currentPage = currentPage;
        }

        public void Show<TViewModel>(TViewModel viewModel)
        {
            var page = _mobilePageStore.GetPage(typeof(TViewModel));
            page.BindingContext = viewModel;

            if (viewModel is IViewModel viewCouple)
                viewCouple.ViewInterface = new MobileInterface(_mobilePageStore, page);

            _currentPage.Navigation.PushAsync(page);
        }
    }
}
