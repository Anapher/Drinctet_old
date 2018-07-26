using System;
using System.Collections.Generic;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Selection
{
    public interface ISelectionAlgorithm
    {
        void Initialize(DrinctetStatus status, Random random);

        IReadOnlyList<PlayerInfo> SelectPlayers(IReadOnlyList<RequiredGender> requiredGenders, IReadOnlyList<CardTag> tags);
        int SelectRandomNumber(IReadOnlyList<INumber> numbers);
        string SelectRandomString(IReadOnlyList<string> values);
        TCard SelectCard<TCard>(IReadOnlyList<TCard> cards) where TCard : BaseCard;
        int GetRandomNumber(int minValue, int maxValue);
        int GetRandomNumber(int maxValue);
        bool TrueOrFalse(double trueProbability);
        T SelectRandomWeighted<T>(IReadOnlyList<T> items) where T : IWeighted;
        T SelectRandomWeighted<T>(IReadOnlyList<T> items, Func<T, double> getWeight);
        int GetSips(int min);
        int GetWillPowerLevel();
        SlideType SelectNextSlide();
    }
}