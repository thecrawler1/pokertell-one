namespace PokerTell.LiveTracker.PokerRooms
{
    using Infrastructure;

    using Interfaces;

    public class FullTiltPokerInfo : IPokerRoomInfo
    {
        public string Site
        {
            get { return PokerSites.FullTiltPoker; }
        }

        public string TableClass
        {
            get { return "NotImplemented"; }
        }

        public string ProcessName
        {
            get { return "FullTilt"; }
        }

        public string FileExtension
        {
            get { return "txt"; }
        }
    }
}