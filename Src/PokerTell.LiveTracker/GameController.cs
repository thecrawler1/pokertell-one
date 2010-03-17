namespace PokerTell.LiveTracker
{
    using System;
    using System.Collections.Generic;

    using PokerTell.Infrastructure.Interfaces;
    using PokerTell.Infrastructure.Interfaces.PokerHand;
    using PokerTell.Infrastructure.Interfaces.Statistics;
    using PokerTell.LiveTracker.Interfaces;

    using Tools.FunctionalCSharp;
    using Tools.WPF.Interfaces;

    public class GameController : IGameController
    {
        public GameController(
            IGameHistoryViewModel gameHistory, 
            IPokerTableStatisticsViewModel overlayPokerTableStatistics, 
            IPokerTableStatisticsViewModel liveStatsPokerTableStatistics, 
            IConstructor<IPlayerStatistics> playerStatisticsMake, 
            IPlayerStatisticsUpdater playerStatisticsUpdater, 
            ITableOverlayManager tableOverlayManager, 
            IPokerTableStatisticsWindowManager liveStatsWindowManager)
        {
            _gameHistory = gameHistory;
            _overlayPokerTableStatistics = overlayPokerTableStatistics;
            _liveStatsPokerTableStatistics = liveStatsPokerTableStatistics;
            _playerStatisticsMake = playerStatisticsMake;
            _playerStatisticsUpdater = playerStatisticsUpdater;
            _tableOverlayManager = tableOverlayManager;
            _liveStatsWindow = liveStatsWindowManager;

            RegisterEvents();

            PlayerStatistics = new Dictionary<string, IPlayerStatistics>();
        }

        readonly IConstructor<IPlayerStatistics> _playerStatisticsMake;

        readonly IPokerTableStatisticsViewModel _overlayPokerTableStatistics;

        readonly IGameHistoryViewModel _gameHistory;

        readonly IWindowManager _liveStatsWindow;

        readonly IPlayerStatisticsUpdater _playerStatisticsUpdater;

        readonly ITableOverlayManager _tableOverlayManager;

        readonly IPokerTableStatisticsViewModel _liveStatsPokerTableStatistics;

        public IDictionary<string, IPlayerStatistics> PlayerStatistics { get; protected set; }

        public bool IsLaunched { get; protected set; }

        public ILiveTrackerSettingsViewModel LiveTrackerSettings { get; set; }

        public event Action ShuttingDown = delegate { };

        public IGameController NewHand(IConvertedPokerHand convertedPokerHand)
        {
            if (! IsLaunched)
                Launch(convertedPokerHand);

            UpdateGameHistory(convertedPokerHand);

            if (LiveTrackerSettings.ShowTableOverlay)
                _tableOverlayManager.UpdateWith(convertedPokerHand);

            UpdatePlayerStatistics(convertedPokerHand);

            return this;
        }

        void UpdateGameHistory(IConvertedPokerHand convertedPokerHand)
        {
            _gameHistory.AddNewHand(convertedPokerHand);
        }

        void UpdatePlayerStatistics(IConvertedPokerHand convertedPokerHand)
        {
            var playerStatisticsToUpdate = new List<IPlayerStatistics>();
            convertedPokerHand.Players.ForEach(p => {
                if (!PlayerStatistics.ContainsKey(p.Name))
                {
                    var playerStatistics = _playerStatisticsMake.New;
                    playerStatistics.InitializePlayer(p.Name, convertedPokerHand.Site);
                    PlayerStatistics.Add(p.Name, playerStatistics);
                }

                playerStatisticsToUpdate.Add(PlayerStatistics[p.Name]);
            });

            _playerStatisticsUpdater.Update(playerStatisticsToUpdate);
        }

        void Launch(IConvertedPokerHand convertedPokerHand)
        {
            SetupLiveStatsWindow();

            if (LiveTrackerSettings.ShowTableOverlay)
                SetupTableOverlayManager(convertedPokerHand);

            IsLaunched = true;
        }

        void SetupLiveStatsWindow()
        {
            _liveStatsWindow.DataContext = _liveStatsPokerTableStatistics;

            if (LiveTrackerSettings.ShowLiveStatsWindowOnStartup)
                _liveStatsWindow.Show();
        }

        void SetupTableOverlayManager(IConvertedPokerHand convertedPokerHand)
        {
            _tableOverlayManager.InitializeWith(_gameHistory, 
                                                _overlayPokerTableStatistics, 
                                                LiveTrackerSettings.ShowHoleCardsDuration, 
                                                convertedPokerHand);
        }

        void RegisterEvents()
        {
            _playerStatisticsUpdater.FinishedUpdatingPlayerStatistics += stats => {
                _overlayPokerTableStatistics.UpdateWith(stats);
                _liveStatsPokerTableStatistics.UpdateWith(stats);
            };

            _tableOverlayManager.ShowLiveStatsWindowRequested += () => {
                _liveStatsWindow.Show();
                _liveStatsWindow.Activate();
            };

            _tableOverlayManager.TableClosed += () => {
                ShuttingDown();
                _liveStatsWindow.Dispose();
                _tableOverlayManager.Dispose();
            };
        }
    }
}