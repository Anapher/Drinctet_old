using System;
using Drinctet.Core;
using Drinctet.Presentation.Screen.Slides;

namespace Drinctet.Presentation.Screen
{
    public class SlidePresenterFactory
    {
        public static ISlidePresenter Create(SlideType slideType, ScreenGameManager gameManager, ICardsProvider cardsProvider, ITextResource textResource)
        {
            var presenter = GetSlidePresenter(slideType);
            presenter.Initialize(gameManager, cardsProvider, textResource);

            return presenter;
        }

        private static ISlidePresenter GetSlidePresenter(SlideType slideType)
        {
            switch (slideType)
            {
                case SlideType.Balancing:
                    break;
                case SlideType.BlackOrRed:
                    break;
                case SlideType.QuizTrust:
                    break;
                case SlideType.Down:
                    return new DownPresenter();
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

            throw new ArgumentException();
        }
    }
}