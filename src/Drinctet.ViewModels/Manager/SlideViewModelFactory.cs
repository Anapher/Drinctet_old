using System;
using System.Collections.Generic;
using System.Text;
using Drinctet.Core;

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
                    break;
                case SlideType.Drink:
                    break;
                case SlideType.GroupGame:
                    break;
                case SlideType.NeverEver:
                    break;
                case SlideType.NoIdeaLoses:
                    break;
                case SlideType.Question:
                    break;
                case SlideType.SocialMedia:
                    break;
                case SlideType.Task:
                    break;
                case SlideType.Virus:
                    break;
                case SlideType.WouldYouRather:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slideType), slideType, null);
            }

            throw new ArgumentOutOfRangeException(nameof(slideType), slideType, null);
        }
    }
}
