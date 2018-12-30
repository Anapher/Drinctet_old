using System;
using Drinctet.Core.Selection;

namespace Drinctet.Core
{
    public interface IGameManager
    {
        DrinctetStatus Status { get; }
        ISelectionAlgorithm Selection { get; }
    }

    public abstract class GameManager : IGameManager
    {
        private static readonly Random Random = new Random();

        protected GameManager(DrinctetStatus status)
        {
            Status = status;

            switch (status.SelectionAlgorithm)
            {
                case SelectionAlgorithm.Benokla:
                    Selection = new BenokolaAlgorithm();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Selection.Initialize(status, Random);
        }

        public DrinctetStatus Status { get; }
        public ISelectionAlgorithm Selection { get; }
    }
}