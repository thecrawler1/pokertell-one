namespace PokerTell.Statistics.ViewModels.Analyzation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using PokerTell.Infrastructure.Interfaces.PokerHand;
    using PokerTell.Statistics.Interfaces;

    using Tools.Interfaces;

    public class RepositoryHandBrowserViewModel : DetailedStatisticsAnalyzerContentViewModel, IRepositoryHandBrowserViewModel
    {
        readonly IRepositoryHandBrowser _repositoryHandBrowser;

        int _currentHandIndex;

        readonly ICollectionValidator _collectionValidator;

        public RepositoryHandBrowserViewModel(IRepositoryHandBrowser repositoryHandBrowser, IHandHistoryViewModel handHistoryViewModel, ICollectionValidator collectionValidator)
        {
            CurrentHandHistory = handHistoryViewModel;
            _repositoryHandBrowser = repositoryHandBrowser;
            _collectionValidator = collectionValidator;
        }

        public IHandHistoryViewModel CurrentHandHistory { get; protected set; }

        public int HandCount { get; protected set; }

        public int CurrentHandIndex
        {
            get { return _currentHandIndex; }
            set
            {
                _currentHandIndex = _collectionValidator.GetValidIndexForCollection(value, HandCount);

                if (_currentHandIndex < HandCount)
                {
                    CurrentHandHistory.UpdateWith(_repositoryHandBrowser.Hand(_currentHandIndex));
                    RaisePropertyChanged(() => CurrentHandIndex);
                }
            }
        }

        public override void Scroll(int change)
        {
           CurrentHandIndex += change;
        }

        public IRepositoryHandBrowserViewModel InitializeWith(IEnumerable<IAnalyzablePokerPlayer> analyzablePokerPlayers)
        {
            var reversedHandIds = analyzablePokerPlayers.Select(p => p.HandId).Reverse();

            _repositoryHandBrowser.InitializeWith(reversedHandIds);

            HandCount = _repositoryHandBrowser.PotentialHandsCount;
            CurrentHandIndex = 0;
            return this;
        }
    }
}