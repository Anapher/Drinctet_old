using System.ComponentModel;

namespace Drinctet.ViewModels.ViewModelBase
{
    /// <summary>
    ///     The interface to the view
    /// </summary>
    public static class ViewInterface
    {
        /// <summary>
        ///     Get the current interface set from the view
        /// </summary>
        public static IViewInterface Current { get; private set; }

        /// <summary>
        ///     Initialize the view interface. This method must be called from the view
        /// </summary>
        /// <param name="viewInterface"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Initialize(IViewInterface viewInterface)
        {
            Current = viewInterface;
        }
    }
}