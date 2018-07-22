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
                //case SlideType.Balancing:
                //    break;
                //case SlideType.BlackOrRed:
                //    break;
                //case SlideType.QuizTrust:
                //    break;
                case SlideType.Down:
                    return new DownPresenter();
                case SlideType.Drink:
                    return new DrinkPresenter();
                case SlideType.GroupGame:
                    return new GroupGamePresenter();
                case SlideType.NeverEver:
                    return new NeverEverPresenter();
                case SlideType.NoIdeaLoses:
                    return new NoIdeaLosesPresenter();
                case SlideType.Question:
                    return new QuestionPresenter();
                case SlideType.SocialMedia:
                    return new SocialMediaPresenter();
                case SlideType.Task:
                    return new TaskPresenter();
                case SlideType.Virus:
                    return new VirusPresenter();
                case SlideType.WouldYouRather:
                    return new WouldYouRatherPresenter();
                default:
                    throw new ArgumentOutOfRangeException(nameof(slideType), slideType, null);
            }

            throw new ArgumentException();
        }
    }
}