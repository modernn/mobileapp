using Toggl.Core.UI.ViewModels;

namespace Toggl.Core.UI.Navigation
{
    public interface IViewModelLoader
    {
        public TViewModel Load<TViewModel>() where TViewModel : IViewModel;
    }
}
