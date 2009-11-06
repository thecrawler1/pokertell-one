namespace PokerTell.PokerHandParsers.PokerStars
{
    public class HandHeaderParser : Base.HandHeaderParser
    {
        #region Constants and Fields

        const string PokerStarsHeaderPattern =
            @"PokerStars Game [#](?<GameId>[0-9]+)[:] (Tournament [#](?<TournamentId>[0-9]+)){0,1}.*(Hold'em|Holdem) (No |Pot )*Limit";

        #endregion

        #region Properties

        protected override string HeaderPattern
        {
            get { return PokerStarsHeaderPattern; }
        }

        #endregion
    }
}