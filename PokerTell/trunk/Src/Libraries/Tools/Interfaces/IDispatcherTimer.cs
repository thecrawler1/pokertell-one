namespace Tools.Interfaces
{
    using System;
    using System.Windows.Threading;

    public interface IDispatcherTimer
    {
        #region Events

        event EventHandler Tick;

        #endregion

        #region Properties

        Dispatcher Dispatcher { get; }

        TimeSpan Interval { get; set; }

        bool IsEnabled { get; set; }

        object Tag { get; set; }

        #endregion

        #region Public Methods

        void Start();

        void Stop();

        #endregion
    }

    /// <summary>
    /// Adapts the Dispatcher class to implement the ITimer interface.
    /// </summary> 
    public class DispatcherTimerAdapter : DispatcherTimer, IDispatcherTimer
    {
    }

}