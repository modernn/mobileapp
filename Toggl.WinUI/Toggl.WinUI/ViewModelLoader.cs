using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.WinUI.ViewModels;
using LoginViewModel = Toggl.WinUI.ViewModels.LoginViewModel;

namespace Toggl.WinUI
{
    public class ViewModelLoader : IViewModelLoader
    {
        public UIDependencyContainer _container;
        public ViewModelLoader(UIDependencyContainer container)
        {
            _container = container;
        }

        public TViewModel Load<TViewModel>()
            where TViewModel : IViewModel
            => (TViewModel) CreateInstance<TViewModel>();

        private IViewModel CreateInstance<TViewModel>()
        {
            if (typeof(TViewModel) == typeof(LoginViewModel)) return new LoginViewModel(_container.NavigationService, _container.UserAccessManager, _container.AnalyticsService);
            return null;
        }
    }
}
