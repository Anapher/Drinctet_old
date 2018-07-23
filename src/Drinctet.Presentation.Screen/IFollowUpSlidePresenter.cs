using Drinctet.Core.Cards.Base;

namespace Drinctet.Presentation.Screen
{
    public interface IFollowUpSlidePresenter : ISlidePresenter
    {
        void InitializeFollowUp(IScreenGameManager gameManager, BaseCard card, ITextResource textResource);
    }
}