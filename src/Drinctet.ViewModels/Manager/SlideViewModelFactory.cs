using System;
using System.Collections.Generic;
using System.Text;
using Drinctet.Core;
using Drinctet.ViewModels.Slides;

namespace Drinctet.ViewModels.Manager
{
    public static class SlideViewModelFactory
    {
        public static ISlideViewModel Create(SlideType slideType, ScreenGameManager gameManager, ICardsProvider cardsProvider, ITextResource textResource)
        {
            var presenter = GetSlideViewModel(slideType);
            presenter.Initialize(gameManager, cardsProvider, textResource);

            return presenter;
        }

        private static ISlideViewModel GetSlideViewModel(SlideType slideType)
        {
            switch (slideType)
            {
                case SlideType.Down:
                    return new DownViewModel();
                case SlideType.Drink:
                    return new DrinkViewModel();
                case SlideType.GroupGame:
                    return new GroupGameViewModel();
                case SlideType.NeverEver:
                    return new NeverEverViewModel();
                case SlideType.NoIdeaLoses:
                    return new NoIdeaLosesViewModel();
                case SlideType.Question:
                    return new QuestionViewModel();
                case SlideType.SocialMedia:
                    return new SocialMediaViewModel();
                case SlideType.Task:
                    return new TaskViewModel();
                case SlideType.Virus:
                    return new VirusViewModel();
                case SlideType.WouldYouRather:
                    return new WouldYouRatherViewModel();
                default:
                    throw new ArgumentOutOfRangeException(nameof(slideType), slideType, null);
            }
        }
    }
}
